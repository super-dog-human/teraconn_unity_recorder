mergeInto(LibraryManager.library, {
    StartPoseDetecting: function () {
        new PoseDetector.start();
    },
    StartAudioRecording: function () {
        startVoiceRecording();
    },
    StopPoseDetecting: function () {
        new PoseDetector.stop();
    },
    StopAudioRecording: function () {
        stopVoiceRecording();
    },
    PostVoiceRecord: function () {
        postVoice();
    },
});