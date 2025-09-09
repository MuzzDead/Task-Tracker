console.log('[VideoChat] module loaded');

const { HubConnectionBuilder, HubConnectionState } = window.signalR;
if (!HubConnectionBuilder) {
    console.error('[VideoChat] SignalR client not found! ' +
        'Додайте <script src="_content/Microsoft.AspNetCore.SignalR.Client.js"></script>');
}

let connection = null;
let localStream = null;
const peers = new Map();

const ICE_CONFIG = { iceServers: [{ urls: 'stun:stun.l.google.com:19302' }] };
const MEDIA_CONSTRAINTS = { video: true, audio: true };

let dotNetHelper = null;
let boardId = null;
let isCameraEnabled = false;
let isMicEnabled = false;

export async function initVideo(dotNetRef, hubUrl, id) {
    dotNetHelper = dotNetRef;
    boardId = id;

    connection = new HubConnectionBuilder()
        .withUrl(`${hubUrl}?boardId=${boardId}`)
        .withAutomaticReconnect()
        .build();

    registerHandlers();

    console.log('[VideoChat] starting SignalR connection...');
    await connection.start();
    console.log('[VideoChat] SignalR connected');

    await connection.invoke('JoinConference', boardId, 'Current User');
    console.log('[VideoChat] joined conference');

    await setupLocalStream();
    console.log('[VideoChat] local stream ready');

    await dotNetHelper.invokeMethodAsync(
        'OnConnectionInitialized',
        isCameraEnabled,
        isMicEnabled
    );
}

async function setupLocalStream() {
    localStream = await navigator.mediaDevices.getUserMedia(MEDIA_CONSTRAINTS);

    localStream.getVideoTracks().forEach(track => (track.enabled = false));
    localStream.getAudioTracks().forEach(track => (track.enabled = false));

    const el = document.getElementById('localVideo');
    if (el) {
        el.srcObject = localStream;
        console.log('[VideoChat] localVideo attached (cam off, mic off)');
    } else {
        console.warn('[VideoChat] #localVideo not found at setupLocalStream');
    }
}

export async function connectLocalVideo() {
    if (!localStream) return;
    const el = document.getElementById('localVideo');
    if (el && !el.srcObject) {
        el.srcObject = localStream;
        console.log('[VideoChat] connectLocalVideo: attached');
    }
}

export async function toggleCamera(on) {
    isCameraEnabled = on;
    if (localStream) {
        localStream.getVideoTracks().forEach(t => (t.enabled = isCameraEnabled));
    }
    console.log('[VideoChat] camera toggled:', isCameraEnabled);
    await updateMediaStatus();
}

export async function toggleMicrophone(on) {
    isMicEnabled = on;
    if (localStream) {
        localStream.getAudioTracks().forEach(t => (t.enabled = isMicEnabled));
    }
    console.log('[VideoChat] mic toggled:', isMicEnabled);
    await updateMediaStatus();
}

async function updateMediaStatus() {
    if (connection && connection.state === HubConnectionState.Connected) {
        try {
            await connection.invoke(
                'UpdateMediaStatus',
                boardId,
                isCameraEnabled,
                isMicEnabled
            );
        } catch (e) {
            console.error('[VideoChat] updateMediaStatus error:', e);
        }
    }
}

export async function leaveConference() {
    if (connection && connection.state === HubConnectionState.Connected) {
        await connection.invoke('LeaveConference', boardId);
        await connection.stop();
    }
    cleanup();
}

function cleanup() {
    if (localStream) {
        localStream.getTracks().forEach(t => t.stop());
        localStream = null;
    }
    peers.forEach(pc => pc.close());
    peers.clear();
    connection = null;
}

function registerHandlers() {
    connection.on('UserJoined', async (userId, userName) => {
        console.log('[VideoChat] UserJoined:', userId);
        await createPeer(userId);
        await dotNetHelper.invokeMethodAsync('OnUserJoined', userId, userName);
    });

    connection.on('UserLeft', async userId => {
        console.log('[VideoChat] UserLeft:', userId);
        closePeer(userId);
        await dotNetHelper.invokeMethodAsync('OnUserLeft', userId);
    });

    connection.on('ReceiveOffer', (userId, offer) => {
        console.log('[VideoChat] ReceiveOffer from', userId);
        handleOffer(userId, JSON.parse(offer));
    });

    connection.on('ReceiveAnswer', (userId, answer) => {
        console.log('[VideoChat] ReceiveAnswer from', userId);
        handleAnswer(userId, JSON.parse(answer));
    });

    connection.on('ReceiveIceCandidate', (userId, candidate) => {
        console.log('[VideoChat] ReceiveIceCandidate from', userId);
        handleIceCandidate(userId, JSON.parse(candidate));
    });

    connection.on('UserMediaStatusChanged', (userId, cam, mic) => {
        dotNetHelper.invokeMethodAsync('OnMediaStatusChanged', userId, cam, mic);
    });
}

async function createPeer(userId) {
    if (peers.has(userId)) {
        return peers.get(userId);
    }

    const pc = new RTCPeerConnection(ICE_CONFIG);
    peers.set(userId, pc);

    if (localStream) {
        localStream.getTracks().forEach(track => pc.addTrack(track, localStream));
    }

    pc.onicecandidate = event => {
        if (event.candidate && connection.state === HubConnectionState.Connected) {
            connection.invoke(
                'SendIceCandidate',
                boardId,
                userId,
                JSON.stringify(event.candidate)
            );
        }
    };

    pc.ontrack = async event => {
        const selector = `#remoteVideo_${userId}`;
        const remoteEl = await waitForElement(selector, 10000);
        if (remoteEl) {
            remoteEl.srcObject = event.streams[0];
            console.log('[VideoChat] remoteVideo attached for', userId);
        } else {
            console.warn('[VideoChat] cannot find remoteVideo element for', userId);
        }
    };

    const offer = await pc.createOffer();
    await pc.setLocalDescription(offer);
    if (connection.state === HubConnectionState.Connected) {
        await connection.invoke(
            'SendOffer',
            boardId,
            userId,
            JSON.stringify(offer)
        );
    }

    return pc;
}

async function handleOffer(userId, offer) {
    const pc = await createPeer(userId);
    await pc.setRemoteDescription(offer);
    const answer = await pc.createAnswer();
    await pc.setLocalDescription(answer);
    if (connection.state === HubConnectionState.Connected) {
        await connection.invoke(
            'SendAnswer',
            boardId,
            userId,
            JSON.stringify(answer)
        );
    }
}

async function handleAnswer(userId, answer) {
    const pc = peers.get(userId);
    if (pc) {
        await pc.setRemoteDescription(answer);
    }
}

async function handleIceCandidate(userId, candidate) {
    const pc = peers.get(userId);
    if (pc) {
        await pc.addIceCandidate(candidate);
    }
}

function waitForElement(selector, timeout = 5000, interval = 200) {
    return new Promise(resolve => {
        const endTime = Date.now() + timeout;
        (function check() {
            const el = document.querySelector(selector);
            if (el) return resolve(el);
            if (Date.now() < endTime) setTimeout(check, interval);
            else resolve(null);
        })();
    });
}
