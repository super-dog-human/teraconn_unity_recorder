using UnityEngine;
using System.Runtime.InteropServices;

public class ControlPanel : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void StartRecording();
    private static extern void StopRecording();
    private static extern void SaveVoice();
    private GraphicSwitcher graphicSwitcher;

    void Start ()
    {
        graphicSwitcher = GameObject.Find("ScriptLoader").GetComponent<GraphicSwitcher>();
    }

    public void ButtonClick ()
    {
        switch (transform.name) {
        case "NextImageButton":
            graphicSwitcher.ChangeNextGraphic();
            break;
        case "PrevImageButton":
            graphicSwitcher.ChangePrevGraphic();
            break;
        case "RecButton":
            StartRecording();
            break;
        case "StopButton":
            StopRecording();
            break;
        case "SaveButton":
            SaveVoice();
            break;
        default:
            break;
        }
    }
}