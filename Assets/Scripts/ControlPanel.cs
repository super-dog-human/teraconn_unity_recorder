using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;

public class ControlPanel : MonoBehaviour {
    GraphicSwitcher graphicSwitcher;
    LessonRecorder lessonRecorder;
    PoseUpdater    poseUpdater;
    EmotionChanger emotionChanger;

    Text recordingText;
    GameObject nextGraphicButton;
    GameObject prevGraphicButton;
    GameObject recButton;
    GameObject stopButton;
    GameObject saveButton;

    void Start () {
        GameObject scriptLoader = GameObject.Find("ScriptLoader");
        graphicSwitcher = scriptLoader.GetComponent<GraphicSwitcher>();
        lessonRecorder  = scriptLoader.GetComponent<LessonRecorder>();

        GameObject kaoru = GameObject.Find("Kaoru");
        poseUpdater    = kaoru.GetComponent<PoseUpdater>();
        emotionChanger = kaoru.GetComponent<EmotionChanger>();

        recordingText = GameObject.Find("RecordingText").GetComponent<Text>();

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

        GameObject.Find("FullScreenButton").GetComponent<Button>().onClick.AddListener(() => {
            Screen.fullScreen = !Screen.fullScreen;
        });

        scriptLoader.GetComponent<LessonMaterial>().OnLoadCompleted.Subscribe (_ => {
            nextGraphicButton.SetActive(true);
            prevGraphicButton.SetActive(true);

            TouchDetector touchDetector = scriptLoader.GetComponent<TouchDetector>();
            touchDetector.initButtons();
        });

        GameObject.Find("SmileButton").GetComponent<Button>().onClick.AddListener(() => {
            SwitchFacialExpression("smile1");
        });

        GameObject.Find("SadButton").GetComponent<Button>().onClick.AddListener(() => {
            SwitchFacialExpression("sad2");
        });

        GameObject.Find("AngerButton").GetComponent<Button>().onClick.AddListener(() => {
            SwitchFacialExpression("anger3");
        });

        EventTrigger stepForwardEventTrigger = GameObject.Find("StepForwardButton")
            .GetComponent<Button>().gameObject.AddComponent<EventTrigger>();
        EventTrigger stepBackEventTrigger    = GameObject.Find("StepBackButton")
            .GetComponent<Button>().gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry moveForwardEntry = new EventTrigger.Entry();
        moveForwardEntry.eventID = EventTriggerType.PointerDown;
        moveForwardEntry.callback.AddListener((_) => {
            poseUpdater.SwicthMovingBackAndForward("startMovingForward");
        });
        EventTrigger.Entry moveBackEntry = new EventTrigger.Entry();
        moveBackEntry.eventID = EventTriggerType.PointerDown;
        moveBackEntry.callback.AddListener((_) => {
            poseUpdater.SwicthMovingBackAndForward("startMovingBack");
        });
        EventTrigger.Entry stopEntry = new EventTrigger.Entry();
        stopEntry.eventID = EventTriggerType.PointerUp;
        stopEntry.callback.AddListener((_) => {
            poseUpdater.SwicthMovingBackAndForward("stopMoving");
        });

        stepForwardEventTrigger.triggers.Add(moveForwardEntry);
        stepForwardEventTrigger.triggers.Add(stopEntry);
        stepBackEventTrigger.triggers.Add(moveBackEntry);
        stepBackEventTrigger.triggers.Add(stopEntry);
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            SwitchPrevGraphic();
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            SwitchNextGraphic();
        }
    }

    void SwitchNextGraphic () {
        graphicSwitcher.ChangeNextGraphic();
    }

    void SwitchPrevGraphic () {
        graphicSwitcher.ChangePrevGraphic();
    }

    void SwitchFacialExpression (string expressionName) {
        emotionChanger.ChangeTo(expressionName);
        lessonRecorder.RecordFacialExpression(expressionName);
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