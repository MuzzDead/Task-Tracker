console.log('[VideoChat] module loaded');

class VideoChat {
    constructor() {
        this.localStream = new MediaStream();
        this.peers = new Map();
        this.pendingCandidates = new Map();
        this.sendersByUser = new Map();
        this.remoteStreams = new Map();

        this.ICE_CONFIG = {
            iceServers: [
                { urls: 'stun:stun.l.google.com:19302' }
            ]
        };

        this.dotNetHelper = null;
        this.myUserId = null;
        this.isCameraEnabled = false;
        this.isMicEnabled = false;
        this.isInitialized = false;
        this.screenStream = null;
        this.isScreenSharing = false;
        this.originalVideoTrack = null;

        this.initialTracks = {
            video: null,
            audio: null
        };
    }

    async init(dotNetRef) {
        try {
            this.dotNetHelper = dotNetRef;
            await this._requestMediaAccess();

            this.isInitialized = true;
            const mediaState = await this.getMediaState();
            console.log('VideoChat initialized with media state:', mediaState);

            await this.dotNetHelper.invokeMethodAsync('OnInitialized',
                mediaState.IsCameraEnabled,
                mediaState.IsMicEnabled
            );
        } catch (error) {
            console.error('Initialization error:', error);
            throw error;
        }
    }

    async _requestMediaAccess() {
        try {
            const stream = await navigator.mediaDevices.getUserMedia({
                video: true,
                audio: true
            });

            stream.getTracks().forEach(track => {
                this.localStream.addTrack(track);
                if (track.kind === 'video') {
                    this.isCameraEnabled = true;
                    this.initialTracks.video = track;
                }
                if (track.kind === 'audio') {
                    this.isMicEnabled = true;
                    this.initialTracks.audio = track;
                }
            });

        } catch (error) {
            console.warn('Failed to get both media devices:', error.message);

            try {
                const videoStream = await navigator.mediaDevices.getUserMedia({ video: true });
                const videoTrack = videoStream.getVideoTracks()[0];
                this.localStream.addTrack(videoTrack);
                this.isCameraEnabled = true;
                this.initialTracks.video = videoTrack;
            } catch (e) {
                console.warn('No video access:', e.message);
                this.isCameraEnabled = false;
            }

            try {
                const audioStream = await navigator.mediaDevices.getUserMedia({ audio: true });
                const audioTrack = audioStream.getAudioTracks()[0];
                this.localStream.addTrack(audioTrack);
                this.isMicEnabled = true;
                this.initialTracks.audio = audioTrack;
            } catch (e) {
                console.warn('No audio access:', e.message);
                this.isMicEnabled = false;
            }
        }
    }

    async _tryGetVideoSeparately() {
        try {
            const videoStream = await navigator.mediaDevices.getUserMedia({ video: true });
            videoStream.getVideoTracks().forEach(track => {
                this.localStream.addTrack(track);
                this.isCameraEnabled = true;
                console.log('Added video track separately:', track.id);
            });
        } catch (e) {
            console.warn('No video access:', e.message);
        }
    }

    async _tryGetAudioSeparately() {
        try {
            const audioStream = await navigator.mediaDevices.getUserMedia({ audio: true });
            audioStream.getAudioTracks().forEach(track => {
                this.localStream.addTrack(track);
                this.isMicEnabled = true;
                console.log('Added audio track separately:', track.id);
            });
        } catch (e) {
            console.warn('No audio access:', e.message);
        }
    }

    async toggleMedia(kind, enabled) {
        if (!this.isInitialized) {
            console.warn('VideoChat not initialized');
            return;
        }

        const flagName = kind === 'video' ? 'isCameraEnabled' : 'isMicEnabled';
        if (enabled === this[flagName]) {
            console.log(`${kind} already ${enabled ? 'enabled' : 'disabled'}`);
            return;
        }

        console.log(`Toggling ${kind} to ${enabled}`);

        const activeTracks = this.localStream.getTracks().filter(t => t.kind === kind);

        if (enabled) {
            if (activeTracks.length > 0) {
                activeTracks.forEach(track => {
                    track.enabled = true;
                });
            } else {
                const initialTrack = this.initialTracks[kind];
                if (initialTrack && initialTrack.readyState === 'live') {
                    this.localStream.addTrack(initialTrack);
                    initialTrack.enabled = true;
                } else {
                    console.warn(`No available ${kind} track to enable`);
                    return;
                }
            }

            this[flagName] = true;
        } else {
            activeTracks.forEach(track => {
                track.enabled = false;
            });

            this[flagName] = false;
        }

        this.peers.forEach((pc, userId) => {
            const senders = pc.getSenders().filter(s => s.track && s.track.kind === kind);

            if (senders.length > 0) {
                senders.forEach(sender => {
                    if (sender.track) {
                        sender.track.enabled = enabled;
                    }
                });
            }
        });

        const localEl = document.getElementById('localVideo');
        if (localEl) {
            localEl.srcObject = this.localStream;
        }

        await this.dotNetHelper.invokeMethodAsync('UpdateMediaStatus',
            this.isCameraEnabled,
            this.isMicEnabled
        );
    }

    async createPeerConnection(userId) {
        if (this.peers.has(userId)) {
            console.log(`Peer connection for user ${userId} already exists`);
            return this.peers.get(userId);
        }

        console.log(`Creating peer connection for user: ${userId}`);
        const pc = new RTCPeerConnection(this.ICE_CONFIG);

        this.sendersByUser.set(userId, { audio: [], video: [] });
        this.remoteStreams.set(userId, new MediaStream());

        this.localStream.getTracks().forEach(track => {
            if (track.readyState === 'live') {
                try {
                    const sender = pc.addTrack(track, this.localStream);
                    this._storeSender(userId, sender, track.kind);
                    console.log(`Added ${track.kind} track for user ${userId}`);
                } catch (e) {
                    console.error(`Error adding ${track.kind} track:`, e);
                }
            }
        });

        pc.onicecandidate = (event) => {
            if (event.candidate) {
                this.dotNetHelper.invokeMethodAsync('SendIceCandidate', userId, JSON.stringify(event.candidate))
                    .catch(e => console.error(`ICE candidate send failed:`, e));
            }
        };

        pc.ontrack = (event) => {
            const remoteStream = this.remoteStreams.get(userId);
            if (event.streams && event.streams.length > 0) {
                event.streams[0].getTracks().forEach(track => {
                    if (!remoteStream.getTracks().some(t => t.id === track.id)) {
                        remoteStream.addTrack(track);
                    }
                });
            } else if (event.track) {
                if (!remoteStream.getTracks().some(t => t.id === event.track.id)) {
                    remoteStream.addTrack(event.track);
                }
            }

            this._handleTrack(userId, remoteStream);
        };

        this.peers.set(userId, pc);
        return pc;
    }

    async _createDisabledSender(pc, userId, kind) {
        try {
            console.log(`Creating disabled ${kind} sender for user ${userId}`);

            const tempStream = await navigator.mediaDevices.getUserMedia(
                kind === 'video' ? { video: true } : { audio: true }
            );
            const tempTrack = tempStream.getTracks()[0];

            const sender = pc.addTrack(tempTrack, this.localStream);

            await sender.replaceTrack(null);
            tempTrack.stop();

            this._storeSender(userId, sender, kind);
            console.log(`Created disabled ${kind} sender for user ${userId}`);
        } catch (e) {
            console.warn(`Could not create disabled ${kind} sender for user ${userId}:`, e);
        }
    }

    async createOffer(userId) {
        console.log(`Creating offer for user: ${userId}`);

        let pc = this.peers.get(userId);
        if (!pc) {
            console.log(`No existing peer connection for ${userId}, creating new one`);
            pc = await this.createPeerConnection(userId);
        }

        const senders = pc.getSenders();
        if (senders.length === 0) {
            console.error(`[${userId}] No senders found - recreating peer connection`);
            this.removePeer(userId);
            pc = await this.createPeerConnection(userId);

            if (pc.getSenders().length === 0) {
                console.error(`[${userId}] Still no senders after recreation - check media tracks`);
                return;
            }
        }

        try {
            console.log(`[${userId}] Creating offer with ${senders.length} senders`);
            const offer = await pc.createOffer({
                offerToReceiveAudio: true,
                offerToReceiveVideo: true,
                voiceActivityDetection: true
            });

            console.log(`[${userId}] Offer SDP created:`, {
                hasVideo: offer.sdp.includes('m=video'),
                hasAudio: offer.sdp.includes('m=audio'),
                sdpLength: offer.sdp.length
            });

            if (!offer.sdp.includes('m=video') && !offer.sdp.includes('m=audio')) {
                console.error(`[${userId}] SDP doesn't contain media sections!`);
                console.log(`[${userId}] Full SDP:`, offer.sdp);
            }

            await pc.setLocalDescription(offer);
            console.log(`[${userId}] Local description set`);

            await this.dotNetHelper.invokeMethodAsync('SendOffer', userId, JSON.stringify(offer));
            console.log(`[${userId}] Offer sent via SignalR`);
        } catch (error) {
            console.error(`[${userId}] Create offer error:`, error);
        }
    }

    async handleOffer(userId, offer) {
        console.log(`Handling offer from user: ${userId}`);

        try {
            let pc = this.peers.get(userId);
            if (!pc) {
                console.log(`Creating peer connection for incoming offer from ${userId}`);
                pc = await this.createPeerConnection(userId);
            }

            const offerDescription = JSON.parse(offer);
            console.log(`[${userId}] Received offer SDP:`, {
                hasVideo: offerDescription.sdp.includes('m=video'),
                hasAudio: offerDescription.sdp.includes('m=audio')
            });

            await pc.setRemoteDescription(offerDescription);
            console.log(`[${userId}] Remote description set from offer`);

            await this._flushCandidates(userId, pc);

            const answer = await pc.createAnswer({
                offerToReceiveAudio: true,
                offerToReceiveVideo: true
            });

            console.log(`[${userId}] Answer SDP:`, {
                hasVideo: answer.sdp.includes('m=video'),
                hasAudio: answer.sdp.includes('m=audio')
            });

            await pc.setLocalDescription(answer);
            console.log(`[${userId}] Local description set from answer`);

            await this.dotNetHelper.invokeMethodAsync('SendAnswer', userId, JSON.stringify(answer));
            console.log(`[${userId}] Answer sent via SignalR`);
        } catch (error) {
            console.error(`[${userId}] Handle offer error:`, error);
        }
    }

    async handleAnswer(userId, answer) {
        console.log(`Handling answer from user: ${userId}`);

        const pc = this.peers.get(userId);
        if (!pc) {
            console.error(`[${userId}] No peer connection found for answer`);
            return;
        }

        try {
            const answerDescription = JSON.parse(answer);
            await pc.setRemoteDescription(answerDescription);
            console.log(`[${userId}] Remote description set from answer`);

            await this._flushCandidates(userId, pc);
        } catch (error) {
            console.error(`[${userId}] Handle answer error:`, error);
        }
    }

    async handleIceCandidate(userId, candidate) {
        console.log(`Handling ICE candidate from user: ${userId}`);

        const pc = this.peers.get(userId);
        if (!pc) {
            console.warn(`[${userId}] No peer connection for ICE candidate`);
            return;
        }

        if (!pc.remoteDescription || !pc.remoteDescription.type) {
            console.log(`[${userId}] Queueing ICE candidate - no remote description yet`);
            if (!this.pendingCandidates.has(userId)) this.pendingCandidates.set(userId, []);
            this.pendingCandidates.get(userId).push(JSON.parse(candidate));
            return;
        }

        try {
            await pc.addIceCandidate(JSON.parse(candidate));
            console.log(`[${userId}] ICE candidate added successfully`);
        } catch (error) {
            console.error(`[${userId}] addIceCandidate error:`, error);
        }
    }

    async _flushCandidates(userId, pc) {
        const pendingList = this.pendingCandidates.get(userId) || [];
        if (pendingList.length === 0) return;

        console.log(`[${userId}] Flushing ${pendingList.length} pending ICE candidates`);

        for (let i = 0; i < pendingList.length; i++) {
            const candidate = pendingList[i];
            try {
                await pc.addIceCandidate(candidate);
                console.log(`[${userId}] Flushed ICE candidate ${i + 1}/${pendingList.length}`);
            } catch (e) {
                console.error(`[${userId}] Failed to flush ICE candidate ${i + 1}:`, e);
            }
        }

        this.pendingCandidates.set(userId, []);
        console.log(`[${userId}] All pending ICE candidates processed`);
    }

    _handleTrack(userId, stream) {
        const remoteEl = document.getElementById('remoteVideo_' + userId);
        if (!remoteEl) {
            console.error(`[${userId}] Remote video element not found: remoteVideo_${userId}`);
            return;
        }

        console.log(`[${userId}] Setting remote stream with ${stream.getTracks().length} tracks`);

        if (remoteEl.srcObject !== stream) {
            remoteEl.srcObject = stream;
            console.log(`[${userId}] Remote stream assigned to video element`);
        }

        const playPromise = remoteEl.play();
        if (playPromise !== undefined) {
            playPromise
                .then(() => console.log(`[${userId}] Remote video playing`))
                .catch(error => {
                    if (error.name !== 'AbortError') {
                        console.error(`[${userId}] Video play error:`, error);
                    }
                });
        }
    }

    removePeer(userId) {
        console.log(`Removing peer: ${userId}`);

        const pc = this.peers.get(userId);
        if (pc) {
            try {
                pc.close();
                console.log(`[${userId}] Peer connection closed`);
            } catch (e) {
                console.error(`[${userId}] Error closing peer connection:`, e);
            }
        }

        this.peers.delete(userId);
        this.pendingCandidates.delete(userId);
        this.sendersByUser.delete(userId);
        this.remoteStreams.delete(userId);

        const el = document.getElementById('remoteVideo_' + userId);
        if (el && el.parentNode) {
            el.parentNode.removeChild(el);
            console.log(`[${userId}] Remote video element removed`);
        }
    }

    _storeSender(userId, sender, kind) {
        if (!this.sendersByUser.has(userId)) {
            this.sendersByUser.set(userId, { audio: [], video: [] });
        }
        this.sendersByUser.get(userId)[kind].push(sender);
    }

    cleanup() {
        console.log('Starting VideoChat cleanup');

        if (this.localStream) {
            const tracks = this.localStream.getTracks();
            console.log(`Stopping ${tracks.length} local tracks`);

            tracks.forEach(track => {
                try {
                    track.stop();
                    this.localStream.removeTrack(track);
                    console.log(`Stopped ${track.kind} track: ${track.id}`);
                } catch (e) {
                    console.error('Error stopping track:', e);
                }
            });
        }

        this.peers.forEach((pc, userId) => {
            try {
                pc.close();
                console.log(`Closed peer connection for user: ${userId}`);
            } catch (e) {
                console.error(`Error closing peer connection for ${userId}:`, e);
            }
        });

        this.peers.clear();
        this.pendingCandidates.clear();
        this.sendersByUser.clear();
        this.remoteStreams.clear();
        this.isInitialized = false;

        console.log('VideoChat cleanup completed');
    }

    getMediaState() {
        return {
            IsCameraEnabled: this.isCameraEnabled,
            IsMicEnabled: this.isMicEnabled
        };
    }

    connectLocalVideo() {
        const localEl = document.getElementById('localVideo');
        if (localEl && this.localStream) {
            localEl.srcObject = this.localStream;
            localEl.play()
                .then(() => console.log('Local video connected and playing'))
                .catch(e => console.log('Local video play error:', e));
        } else {
            console.warn('Cannot connect local video:', {
                element: !!localEl,
                stream: !!this.localStream,
                tracks: this.localStream?.getTracks().length || 0
            });
        }
    }

    async startScreenShare() {
        try {
            this.screenStream = await navigator.mediaDevices.getDisplayMedia({
                video: true,
                audio: true
            });

            const videoTrack = this.localStream.getVideoTracks()[0];
            if (videoTrack) {
                this.originalVideoTrack = videoTrack;
            }

            if (this.screenStream.getVideoTracks().length > 0) {
                const screenVideoTrack = this.screenStream.getVideoTracks()[0];

                screenVideoTrack.onended = () => {
                    console.log('Screen sharing stopped by user');
                    this.stopScreenShare();
                };

                this._replaceVideoTrack(screenVideoTrack);
            }

            this.isScreenSharing = true;
            console.log('Screen sharing started');

        } catch (error) {
            console.error('Error starting screen share:', error);
            throw error;
        }
    }
    async stopScreenShare() {
        if (!this.isScreenSharing) return;

        if (this.originalVideoTrack) {
            this._replaceVideoTrack(this.originalVideoTrack);
        } else {
            try {
                const newStream = await navigator.mediaDevices.getUserMedia({ video: true });
                const newVideoTrack = newStream.getVideoTracks()[0];
                this._replaceVideoTrack(newVideoTrack);
                newStream.getTracks().forEach(track => track.stop());
            } catch (error) {
                console.error('Error getting camera track:', error);
            }
        }

        if (this.screenStream) {
            this.screenStream.getTracks().forEach(track => track.stop());
            this.screenStream = null;
        }

        this.isScreenSharing = false;
        this.originalVideoTrack = null;
        console.log('Screen sharing stopped');

        if (this.dotNetHelper) {
            this.dotNetHelper.invokeMethodAsync('OnScreenShareStopped');
        }
    }

    _replaceVideoTrack(newTrack) {
        const oldVideoTrack = this.localStream.getVideoTracks()[0];
        if (oldVideoTrack) {
            this.localStream.removeTrack(oldVideoTrack);
        }
        this.localStream.addTrack(newTrack);

        this.peers.forEach((pc, userId) => {
            const videoSender = pc.getSenders().find(sender =>
                sender.track && sender.track.kind === 'video'
            );

            if (videoSender) {
                videoSender.replaceTrack(newTrack)
                    .catch(error => console.error('Error replacing track:', error));
            }
        });

        const localVideo = document.getElementById('localVideo');
        if (localVideo) {
            localVideo.srcObject = this.localStream;
        }
    }

    async startScreenShareAsAdditional() {
        const screenStream = await navigator.mediaDevices.getDisplayMedia({
            video: { cursor: "always" },
            audio: true
        });

        this.peers.forEach((pc, userId) => {
            screenStream.getTracks().forEach(track => {
                pc.addTrack(track, screenStream);
            });
        });

        this.screenStream = screenStream;
    }
}

let videoChat = new VideoChat();

export function initVideo(dotNetRef) { return videoChat.init(dotNetRef); }
export function toggleCamera(on) { return videoChat.toggleMedia('video', on); }
export function toggleMicrophone(on) { return videoChat.toggleMedia('audio', on); }
export function cleanup() { return videoChat.cleanup(); }
export function createPeerConnection(userId) { return videoChat.createPeerConnection(userId); }
export function createOffer(userId) { return videoChat.createOffer(userId); }
export function handleOffer(userId, offer) { return videoChat.handleOffer(userId, offer); }
export function handleAnswer(userId, answer) { return videoChat.handleAnswer(userId, answer); }
export function handleIceCandidate(userId, candidate) { return videoChat.handleIceCandidate(userId, candidate); }
export function removePeer(userId) { return videoChat.removePeer(userId); }
export function getMediaState() { return videoChat.getMediaState(); }
export function connectLocalVideo() {
    const localEl = document.getElementById('localVideo');
    if (localEl && videoChat.localStream) {
        localEl.srcObject = videoChat.localStream;
        localEl.play()
            .then(() => console.log('Local video connected via export function'))
            .catch(e => console.log('Local video play error via export:', e));
    } else {
        console.warn('Cannot connect local video via export:', {
            element: !!localEl,
            stream: !!videoChat.localStream
        });
    }
}

export function startScreenShare() { return videoChat.startScreenShare(); }
export function stopScreenShare() { return videoChat.stopScreenShare(); }