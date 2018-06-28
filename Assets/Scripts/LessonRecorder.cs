using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class LessonRecorder : MonoBehaviour {
    [DllImport("__Internal")]
    static extern void StartPoseDetecting();
    [DllImport("__Internal")]
    static extern void StartAudioRecording();
    [DllImport("__Internal")]
    static extern void StopPoseDetecting();
    [DllImport("__Internal")]
    static extern void StopAudioRecording();
    [DllImport("__Internal")]
    static extern void PostVoiceRecord();

    bool isRecording;
    List<PoseRecord>    poseRecords    = new List<PoseRecord>();
    List<GraphicRecord> graphicRecords = new List<GraphicRecord>();
    float recordStartMsec;
    float elapsedTimeMsec = 0.0f;

    public void StartRecording () {
        isRecording = true;
        recordStartMsec = Time.realtimeSinceStartup;

        StartPoseDetecting();
        StartAudioRecording();
    }

    public void StopRecording () {
        isRecording = false;
        elapsedTimeMsec = Time.realtimeSinceStartup - recordStartMsec;

        StopPoseDetecting();
        StopAudioRecording();
    }

    public void RecordPose (PoseRecord record) {
        if (!isRecording) return;

        string currentTime = CurrentTimeMsec().ToString();
        record.time = currentTime;
        poseRecords.Add(record);
    }

    public void RecordGraphicSwitching (string showGraphicId, string hideGraphicId) {
        if (!isRecording) return;

        string currentTime = CurrentTimeMsec().ToString();
        if (showGraphicId != null) {
            GraphicRecord record = new GraphicRecord(showGraphicId, "show");
            record.time = currentTime;
            graphicRecords.Add(record);
        }

        if (hideGraphicId != null) {
            GraphicRecord record = new GraphicRecord(hideGraphicId, "hide");
            record.time = currentTime;
            graphicRecords.Add(record);
        }
    }

    public void Save () {
        PostRecord();
        PostVoiceRecord();
    }

    float CurrentTimeMsec () {
        float currentTimeMsec = Time.realtimeSinceStartup - recordStartMsec;
        if (elapsedTimeMsec > 0) currentTimeMsec += elapsedTimeMsec;
        return currentTimeMsec;
    }

    void PostRecord () {
        Record requestRecord = new Record(poseRecords, graphicRecords);
        string jsonString = JsonUtility.ToJson(requestRecord);

        HTTPClient httpClient = GameObject.Find("ScriptLoader").GetComponent<HTTPClient>();
        httpClient.postJson(jsonString);
    }
}

[System.Serializable]
public class Record {
    public List<PoseRecord>    poses;
    public List<GraphicRecord> graphics;

    public Record(List<PoseRecord> poses, List<GraphicRecord> graphics) {
        this.poses    = poses;
        this.graphics = graphics;
    }
}

[System.Serializable]
public class PoseRecord {
    public string time;
    public Vector3 body;
    public Vector3 lookAt;
    public Vector3 leftElbow;
    public Vector3 rightElbow;
    public Vector3 leftHand;
    public Vector3 rightHand;
}

[System.Serializable]
public class GraphicRecord {
    public string time;
    public string id;
    public string action;

    public GraphicRecord(string id, string action) {
        this.id     = id;
        this.action = action;
    }
}