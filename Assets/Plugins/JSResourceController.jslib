mergeInto(LibraryManager.library, {
    StartPoseDetecting: function () {
        new PoseDetector.start();
    },
    StopPoseDetecting: function () {
        new PoseDetector.stop();
    },
    StartAudioRecording: function () {
        new VoiceRecorder.start();
    },
    StopAudioRecording: function () {
        new VoiceRecorder.stop();
    },
});