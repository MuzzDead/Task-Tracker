console.log('[VideoChat] module loaded');

const { HubConnectionBuilder, HubConnectionState } = window.signalR || {};
if (!HubConnectionBuilder) {
    console.error('[VideoChat] SignalR client not found!');
}

class VideoChat {
    constructor() {
        this.connection = null;
        this.localStream = new MediaStream();
        this.peers = new Map();
        this.pendingCandidates = new Map();
        this.sendersByUser = new Map();

        this.ICE_CONFIG = { iceServers: [{ urls: 'stun:stun.l.google.com:19302' }] };

        this.dotNetHelper = null;
        this.boardId = null;
        this.myUserId = null;

        this.isCameraEnabled = false;
        this.isMicEnabled = false;
    }

    log(...args) {
        console.log('[VideoChat]', ...args);
    }

    warn(...args) {
        console.warn('[VideoChat]', ...args);
    }

    error(...args) {
        console.error('[VideoChat]', ...args);
    }

    async init(dotNetRef, hubUrl, boardId) {
        this.dotNetHelper = dotNetRef;
        this.boardId = boardId;

        this.connection = new HubConnectionBuilder()
            .withUrl(`${hubUrl}?boardId=${boardId}`)
            .withAutomaticReconnect()
            .build();

        this.registerHandlers();

        this.log('starting SignalR connection...');
        await this.connection.start();
        this.log('SignalR connected');

        this.myUserId = this.connection.connectionId;

        await this.connection.invoke('JoinConference', this.boardId, 'Current User');
        this.log('joined conference');

        const localEl = document.getElementById('localVideo');
        if (localEl) localEl.srcObject = this.localStream;

        await this._notifyInitialized();
    }

    async _notifyInitialized() {
        if (this.dotNetHelper) {
            try {
                await this.dotNetHelper.invokeMethodAsync(
                    'OnConnectionInitialized',
                    this.isCameraEnabled,
                    this.isMicEnabled
                );
            } catch (e) {
                this.warn('OnConnectionInitialized error', e);
            }
        }
    }

    async toggleMedia(kind, enabled) {
        const constraints = kind === 'video' ? { video: true } : { audio: true };
        const flagName = kind === 'video' ? 'isCameraEnabled' : 'isMicEnabled';

        this[flagName] = !!enabled;

        if (this[flagName]) {
            try {
                const stream = await navigator.mediaDevices.getUserMedia(constraints);
                const track = stream.getTracks()[0];
                if (!track) throw new Error(`No ${kind} track`);

                this.localStream.addTrack(track);

                this.peers.forEach((pc, userId) => {
                    try {
                        const sender = pc.addTrack(track, this.localStream);
                        this._storeSender(userId, sender, kind);
                    } catch (e) {
                        this.warn(`addTrack ${kind} error for`, userId, e);
                    }
                });

                this.log(`${kind} ON`);
            } catch (err) {
                this.error(`${kind} ON error:`, err);
                if (this.dotNetHelper) {
                    try {
                        await this.dotNetHelper.invokeMethodAsync(
                            'OnDeviceError',
                            kind,
                            err.name || err.message
                        );
                    } catch { }
                }
                this[flagName] = false;
            }
        } else {
            const toStop = this.localStream.getTracks().filter(t => t.kind === kind);
            for (const t of toStop) {
                try { t.stop(); } catch { }
                this.localStream.removeTrack(t);
            }
            this.peers.forEach((pc, userId) => {
                this._removeSendersOfKind(pc, userId, kind);
            });
            this.log(`${kind} OFF`);
        }

        const localEl = document.getElementById('localVideo');
        if (localEl) localEl.srcObject = this.localStream;

        await this._updateMediaStatus();
    }

    toggleCamera(on) { return this.toggleMedia('video', on); }
    toggleMicrophone(on) { return this.toggleMedia('audio', on); }

    async _updateMediaStatus() {
        if (this.connection?.state === HubConnectionState.Connected) {
            try {
                await this.connection.invoke(
                    'UpdateMediaStatus',
                    this.boardId,
                    this.isCameraEnabled,
                    this.isMicEnabled
                );
            } catch (e) {
                this.error('updateMediaStatus error:', e);
            }
        }
    }

    _storeSender(userId, sender, kind) {
        if (!this.sendersByUser.has(userId))
            this.sendersByUser.set(userId, { audio: [], video: [] });
        this.sendersByUser.get(userId)[kind].push(sender);
    }

    _removeSendersOfKind(pc, userId, kind) {
        const obj = this.sendersByUser.get(userId);
        if (!obj) return;
        for (const sender of obj[kind] || []) {
            try { pc.removeTrack(sender); } catch { }
        }
        obj[kind] = [];
    }

    async leaveConference() {
        if (this.connection?.state === HubConnectionState.Connected) {
            try {
                await this.connection.invoke('LeaveConference', this.boardId);
                await this.connection.stop();
            } catch (e) {
                this.warn('leaveConference error:', e);
            }
        }
        this.cleanup();
    }

    cleanup() {
        this.localStream?.getTracks().forEach(t => { try { t.stop(); } catch { } });
        this.localStream = new MediaStream();

        this.peers.forEach(pc => { try { pc.close(); } catch { } });
        this.peers.clear();

        this.pendingCandidates.clear();
        this.sendersByUser.clear();
        this.connection = null;
    }

    registerHandlers() {
        this.connection.on('UserJoined', async (userId, userName) => {
            this.log('UserJoined:', userId, userName);

            await this._createPeer(userId);

            this.log('Creating offer for', userId);
            await this._createOffer(userId);

            if (this.dotNetHelper) {
                try { await this.dotNetHelper.invokeMethodAsync('OnUserJoined', userId, userName); } catch { }
            }
        });

        this.connection.on('UserLeft', userId => {
            this.log('UserLeft:', userId);
            this._closePeer(userId);
            if (this.dotNetHelper) {
                try { this.dotNetHelper.invokeMethodAsync('OnUserLeft', userId); } catch { }
            }
        });

        this.connection.on('ReceiveOffer', (userId, offerJson) => {
            this.log('ReceiveOffer from', userId);
            this._handleOffer(userId, JSON.parse(offerJson));
        });

        this.connection.on('ReceiveAnswer', (userId, answerJson) => {
            this.log('ReceiveAnswer from', userId);
            this._handleAnswer(userId, JSON.parse(answerJson));
        });

        this.connection.on('ReceiveIceCandidate', (userId, candidateJson) => {
            this.log('ReceiveIceCandidate from', userId);
            this._handleIceCandidate(userId, JSON.parse(candidateJson));
        });

        this.connection.on('UserMediaStatusChanged', (userId, cam, mic) => {
            if (this.dotNetHelper) {
                try { this.dotNetHelper.invokeMethodAsync('OnMediaStatusChanged', userId, cam, mic); } catch { }
            }
        });
    }

    async _createPeer(userId) {
        if (this.peers.has(userId)) return this.peers.get(userId);

        const pc = new RTCPeerConnection(this.ICE_CONFIG);
        this.peers.set(userId, pc);
        this.sendersByUser.set(userId, { audio: [], video: [] });

        this.localStream.getTracks().forEach(track => {
            try {
                const sender = pc.addTrack(track, this.localStream);
                this._storeSender(userId, sender, track.kind);
            } catch (e) {
                this.warn('addTrack failed on createPeer', userId, e);
            }
        });

        pc.onicecandidate = event => {
            if (event.candidate && this.connection?.state === HubConnectionState.Connected) {
                this.connection.invoke(
                    'SendIceCandidate',
                    this.boardId,
                    userId,
                    JSON.stringify(event.candidate)
                ).catch(e => this.warn('SendIceCandidate error:', e));
            }
        };

        pc.ontrack = event => this._handleTrack(userId, event);

        return pc;
    }

    async _createOffer(userId) {
        const pc = this.peers.get(userId);
        if (!pc) return;

        const offer = await pc.createOffer({ offerToReceiveAudio: true, offerToReceiveVideo: true });
        await pc.setLocalDescription(offer);

        if (this.connection?.state === HubConnectionState.Connected) {
            await this.connection.invoke('SendOffer', this.boardId, userId, JSON.stringify(offer));
        }
    }

    _closePeer(userId) {
        const pc = this.peers.get(userId);
        if (pc) { try { pc.close(); } catch { } }
        this.peers.delete(userId);
        this.pendingCandidates.delete(userId);
        this.sendersByUser.delete(userId);

        const el = document.getElementById(`remoteVideo_${userId}`);
        if (el?.parentNode) el.parentNode.removeChild(el);
    }

    async _handleOffer(userId, offer) {
        const pc = await this._createPeer(userId);
        await pc.setRemoteDescription(offer);

        await this._flushCandidates(userId, pc);

        const answer = await pc.createAnswer();
        await pc.setLocalDescription(answer);

        if (this.connection?.state === HubConnectionState.Connected) {
            await this.connection.invoke('SendAnswer', this.boardId, userId, JSON.stringify(answer));
        }
    }

    async _handleAnswer(userId, answer) {
        const pc = this.peers.get(userId);
        if (!pc) return;

        await pc.setRemoteDescription(answer);
        await this._flushCandidates(userId, pc);
    }

    async _handleIceCandidate(userId, candidate) {
        const pc = this.peers.get(userId);
        if (!pc || !pc.remoteDescription?.type) {
            if (!this.pendingCandidates.has(userId)) this.pendingCandidates.set(userId, []);
            this.pendingCandidates.get(userId).push(candidate);
            return;
        }
        await pc.addIceCandidate(candidate).catch(e => this.error('addIceCandidate error:', e));
    }

    async _flushCandidates(userId, pc) {
        const list = this.pendingCandidates.get(userId) || [];
        for (const c of list) {
            try { await pc.addIceCandidate(c); } catch (e) { this.warn('flush candidate error', e); }
        }
        this.pendingCandidates.set(userId, []);
    }

    _handleTrack(userId, event) {
        this.log('ontrack for', userId, event.track.kind);
        let remoteEl = document.getElementById(`remoteVideo_${userId}`);
        if (!remoteEl) {
            this.warn(`Remote video element for ${userId} not found!`);
            return;
        }

        if (event.streams?.[0]) {
            this.log(`Attaching stream to remoteVideo_${userId}`);
            remoteEl.srcObject = event.streams[0];
        } else {
            this.warn('No streams in ontrack event for', userId);
        }
    }
}

let videoChat = new VideoChat();

export function initVideo(dotNetRef, hubUrl, id) { return videoChat.init(dotNetRef, hubUrl, id); }
export function connectLocalVideo() {
    const el = document.getElementById('localVideo');
    if (el) el.srcObject = videoChat.localStream;
}
export function toggleCamera(on) { return videoChat.toggleCamera(on); }
export function toggleMicrophone(on) { return videoChat.toggleMicrophone(on); }
export function leaveConference() { return videoChat.leaveConference(); }
