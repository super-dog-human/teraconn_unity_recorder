using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ControlPanel : MonoBehaviour {
    GraphicSwitcher graphicSwitcher;
    LessonRecorder lessonRecorder;
    Text recordingText;
    GameObject nextGraphicButton;
    GameObject prevGraphicButton;
    GameObject recButton;
    GameObject stopButton;
    GameObject saveButton;

    void Start () {
        graphicSwitcher = GameObject.Find("ScriptLoader").GetComponent<GraphicSwitcher>();
        lessonRecorder  = GameObject.Find("ScriptLoader").GetComponent<LessonRecorder>();
        recordingText   = GameObject.Find("RecordingText").GetComponent<Text>();

        nextGraphicButton = GameObject.Find("NextGraphicButton");
        nextGraphicButton.GetComponent<Button>().onClick.AddListener(SwitchNextGraphic);
        nextGraphicButton.SetActive(false);

        prevGraphicButton = GameObject.Find("PrevGraphicButton");
        prevGraphicButton.GetComponent<Button>().onClick.AddListener(SwitchPrevGraphic);
        prevGraphicButton.SetActive(false);

        recButton = GameObject.Find("RecButton");
        recButton.GetComponent<Button>().onClick.AddListener(StartRecording);

        stopButton = GameObject.Find("StopButton");
        stopButton.GetComponent<Button>().onClick.AddListener(StopRecording);
        stopButton.SetActive(false);

        saveButton = GameObject.Find("SaveButton");
        saveButton.GetComponent<Button>().onClick.AddListener(SaveRecord);
        saveButton.SetActive(false);

        GameObject.Find("FullScreenButton").GetComponent<Button>().onClick.AddListener(SwitchFullScreen);

        LessonMaterial lessonMaterial = GameObject.Find("ScriptLoader").GetComponent<LessonMaterial>();
        lessonMaterial.OnLoadCompleted.Subscribe (_ => {
            nextGraphicButton.SetActive(true);
            prevGraphicButton.SetActive(true);
        });
    }

    void SwitchNextGraphic () {
        graphicSwitcher.ChangeNextGraphic();
    }

    void SwitchPrevGraphic () {
        graphicSwitcher.ChangePrevGraphic();
    }

    void StartRecording () {
        recButton.SetActive(false);
        saveButton.SetActive(false);

        lessonRecorder.StartRecording();

        ColorAlphaToMax(recordingText);
        stopButton.SetActive(true);
    }

    void StopRecording () {
        stopButton.SetActive(false);

        lessonRecorder.StopRecording();

        ColorAlphaToZero(recordingText);
        recButton.SetActive(true);
        saveButton.SetActive(true);
    }

    void SaveRecord () {
        lessonRecorder.Save();
    }

    void SwitchFullScreen () {
        Screen.fullScreen = !Screen.fullScreen;
    }

    void ColorAlphaToZero (Text text) {
        Color color = text.color;
        color.a = 0;
        text.color = color;
    }

    void ColorAlphaToMax (Text text) {
        Color color = text.color;
        color.a = 255;
        text.color = color;
    }
}