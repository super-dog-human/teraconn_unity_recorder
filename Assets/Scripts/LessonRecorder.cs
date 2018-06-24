using System;
using UnityEngine;

public class LessonRecorder : MonoBehaviour
{
    private float recordStartMsec;
    private bool isRecording;

    void Start ()
    {
    }

    void Update ()
    {
        // OpenCVを導入後はここでRecord*メソッドを呼ぶ
    }

    void StartRecording ()
    {
//		startUnixTime = currentime;
        recordStartMsec = Time.realtimeSinceStartup;
        isRecording = true;
    }

    void StopRecording ()
    {
//		startUnixTime = null;
        isRecording = false;
    }

    void PostRecordToServer ()
    {

    }

    private void RecordPose (string part, Vector3 worldPosition)
    {
        if (!isRecording) return;

        ElapsedTimeMsec();
    }

    private void RecordShowImage (int imageId, string action)
    {
        if (!isRecording) return;

    }

    private float ElapsedTimeMsec ()
    {
        return recordStartMsec - Time.realtimeSinceStartup;
    }
}
