mergeInto(LibraryManager.library, {
    StartRecording: function () {
        new PoseDetector.start();
    },
    StopRecording: function () {
        new PoseDetector.stop();
    },
    SaveVoice: function () {
        new PoseDetector.saveVoice();
    },
});