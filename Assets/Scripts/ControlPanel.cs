using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;

public class ControlPanel : MonoBehaviour {
    GraphicSwitcher graphicSwitcher;
    LessonRecorder lessonRecorder;
    PoseUpdater    poseUpdater;
    EmotionChanger emotionChanger;

    Image recordingIcon;
    GameObject nextGraphicButton;
    GameObject prevGraphicButton;
    GameObject recButton;
    GameObject stopButton;
    GameObject resumeButton;
    GameObject saveButton;

    void Start () {
        GameObject scriptLoader = GameObject.Find("ScriptLoader");
        graphicSwitcher = scriptLoader.GetComponent<GraphicSwitcher>();
        lessonRecorder  = scriptLoader.GetComponent<LessonRecorder>();

        GameObject kaoru = GameObject.Find("Kaoru");
        poseUpdater    = kaoru.GetComponent<PoseUpdater>();
        emotionChanger = kaoru.GetComponent<EmotionChanger>();

        recordingIcon = GameObject.Find("RecordingIcon").GetComponent<Image>();
        ColorAlphaToZero(recordingIcon);

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

        resumeButton = GameObject.Find("ResumeButton");
        resumeButton.GetComponent<Button>().onClick.AddListener(ResumeRecording);
        resumeButton.SetActive(false);

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
        EventTrigger stepLeftEventTrigger = GameObject.Find("StepLeftButton")
            .GetComponent<Button>().gameObject.AddComponent<EventTrigger>();
        EventTrigger stepRightEventTrigger = GameObject.Find("StepRightButton")
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
        EventTrigger.Entry moveLeftEntry = new EventTrigger.Entry();
        moveLeftEntry.eventID = EventTriggerType.PointerDown;
        moveLeftEntry.callback.AddListener((_) => {
            poseUpdater.SwicthMovingBackAndForward("startMovingLeft");
        });
        EventTrigger.Entry moveRightEntry = new EventTrigger.Entry();
        moveRightEntry.eventID = EventTriggerType.PointerDown;
        moveRightEntry.callback.AddListener((_) => {
            poseUpdater.SwicthMovingBackAndForward("startMovingRight");
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
        stepLeftEventTrigger.triggers.Add(moveLeftEntry);
        stepLeftEventTrigger.triggers.Add(stopEntry);
        stepRightEventTrigger.triggers.Add(moveRightEntry);
        stepRightEventTrigger.triggers.Add(stopEntry);
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.Z)) {
            SwitchPrevGraphic();
        } else if (Input.GetKeyDown(KeyCode.C)) {
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

        if (!Debug.isDebugBuild) lessonRecorder.StartRecording();

        ColorAlphaToMax(recordingIcon);
        stopButton.SetActive(true);
    }

    void StopRecording () {
        stopButton.SetActive(false);

        if (!Debug.isDebugBuild) lessonRecorder.StopRecording();

        ColorAlphaToZero(recordingIcon);

        resumeButton.SetActive(true);
        saveButton.SetActive(true);
    }

    void ResumeRecording () {
        StartRecording();
        resumeButton.SetActive(false);
        saveButton.SetActive(false);
    }

    void SaveRecord () {
        // show indicator
        lessonRecorder.Save();
    }

    void ColorAlphaToZero (Image image) {
        Color color = image.color;
        color.a = 0;
        image.color = color;
    }

    void ColorAlphaToMax (Image image) {
        Color color = image.color;
        color.a = 255;
        image.color = color;
    }
}