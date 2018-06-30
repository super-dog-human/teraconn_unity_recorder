var isRecording = false;
const buffers = [];

/*
    navigator.getUserMedia =
    navigator.getUserMedia ||
    navigator.webkitGetUserMedia ||
    navigator.mozGetUserMedia ||
    navigator.msGetUserMedia;
*/
navigator.mediaDevices.getUserMedia(
    { audio: true, video: false },
    function(stream) {
        var context = new AudioContext();
        var source = context.createMediaStreamSource(stream)
        var processor = context.createScriptProcessor(1024, 1, 1);

        source.connect(processor);
        processor.connect(context.destination);
        processor.onaudioprocess = function(e){
            console.log(new Date().getTime());
            console.log(context.currentTime);

            var input = event.inputBuffer.getChannelData(0);
            var sum = 0.0;
            for (i = 0; i < input.length; ++i) {
                sum += input[i] * input[i];
            }
            console.log(Math.sqrt(sum / input.length));

            var output = event.outputBuffer.getChannelData(0);
            output = input;
        };
    }
);

function startVoiceRecording() {
    isRecording = true;
}

function hasIntervalElapsed() {
    return false;
}
function stopVoiceRecording() {
    isRecording = false;
    console.log("stopVoiceRecording");
}
function postVoice() {
    console.log("save voice.");
}