using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class LessonRecorder : MonoBehaviour {
    [DllImport("__Internal")]
    static extern void StartPoseDetecting();
    [DllImport("__Internal")]
    static extern void StopPoseDetecting();
    [DllImport("__Internal")]
    static extern void StartAudioRecording();
    [DllImport("__Internal")]
    static extern void StopAudioRecording();

    const string apiURL = "https://api.teraconnect.org";
    bool isRecording;
    float recordStartSec;
    float elapsedTimeSec = 0.0f;
    List<TimelineRecord> timelineRecords = new List<TimelineRecord>();
    List<PoseRecord> poseRecords         = new List<PoseRecord>();

    public void StartRecording () {
        isRecording = true;
        recordStartSec = Time.realtimeSinceStartup;

        StartPoseDetecting();
        StartAudioRecording();
    }

    public void StopRecording () {
        isRecording = false;
        elapsedTimeSec = Time.realtimeSinceStartup - recordStartSec;

        StopPoseDetecting();
        StopAudioRecording();
    }

    public void RecordPose (PoseRecord record) {
        if (!isRecording) return;

        record.timeSec = CurrentTimeSec();
        poseRecords.Add(record);
    }

    public void RecordGraphicSwitching (string showGraphicId, string hideGraphicId) {
        if (!isRecording) return;

        List<GraphicRecord> graphicRecords = new List<GraphicRecord>();
        if (showGraphicId != null) {
            GraphicRecord record = new GraphicRecord(showGraphicId, "show");
            graphicRecords.Add(record);
        }

        if (hideGraphicId != null) {
            GraphicRecord record = new GraphicRecord(hideGraphicId, "hide");
            graphicRecords.Add(record);
        }

        TimelineRecord timeline = timelineRecords.Find(t => t.timeSec == CurrentTimeSec());
        if (timeline == null) {
            timeline = new TimelineRecord();
            timeline.timeSec = CurrentTimeSec();
            timelineRecords.Add(timeline);
        };
        timeline.graphic = graphicRecords;
    }

    public void RecordFacialExpression(string expressionName) {
        float currentTime = CurrentTimeSec();

        TimelineRecord timeline = timelineRecords.Find(t => t.timeSec == currentTime);
        if (timeline == null) {
            timeline = new TimelineRecord();
            timeline.timeSec = currentTime;
            timelineRecords.Add(timeline);
        };

        SpecialActionRecord action = new SpecialActionRecord(null, expressionName);
        timeline.spAction = action;
    }

    public void RecordSpeech(string jsonString) {
        SpeechHistory history   = JsonUtility.FromJson<SpeechHistory>(jsonString);
        TextRecord textRecord   = new TextRecord(history.durationSec);
        VoiceRecord voiceRecord = new VoiceRecord(history.durationSec, history.fileID);
        TimelineRecord timeline = timelineRecords.Find(t => t.timeSec == history.timeSec);

        if (timeline == null) {
            timeline = new TimelineRecord();
            timeline.timeSec = history.timeSec;
            timeline.text    = textRecord;
            timeline.voice   = voiceRecord;
            timelineRecords.Add(timeline);
            return;
        }

        if (timeline.text == null) {
            timeline.text = textRecord;
        }

        if (timeline.voice == null) {
            timeline.voice = voiceRecord;
        } else {
            Utilities.MergeValues(timeline.voice, voiceRecord);
        }
    }

    public void Save () {
        if (timelineRecords.Count() == 0) {
            return;
        }

        int incompletedVoiceCount = timelineRecords.Count(timeline => timeline.voice != null && timeline.voice.fileID == null);
        if (incompletedVoiceCount > 0) {
            StartCoroutine("RetrySaveAfterSeconds", 1);
            return;
        }

        PostRecord();
    }

    IEnumerator RetrySaveAfterSeconds (int seconds) {
        yield return new WaitForSeconds(seconds);
        Save();
    }

    float CurrentTimeSec () {
        float currentTimeSec = Time.realtimeSinceStartup - recordStartSec;
        if (elapsedTimeSec > 0) currentTimeSec += elapsedTimeSec;
        return currentTimeSec;
    }

    void PostRecord () {
        // sort timelineRecords by time
        LessonRecord record = new LessonRecord();
        record.durationSec  = elapsedTimeSec;
        record.timelines    = timelineRecords;
        record.poses        = poseRecords;
        record.published    = System.DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
        record.updated      = System.DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");

        string jsonString = JsonUtility.ToJson(record);
        string lessonId = Application.absoluteURL.Split("?"[0])[1];
        string url = string.Format("{0}/lessons/{1}/materials", apiURL, lessonId);
        HTTPClient httpClient = GameObject.Find("ScriptLoader").GetComponent<HTTPClient>();
        httpClient.postJson(url, jsonString);
    }

    void SaveCompleted () {
        // dismiss loading indicator
        //
    }
}

[System.Serializable]
public class LessonRecord {
    public float                durationSec;
    public List<TimelineRecord> timelines;
    public List<PoseRecord>     poses;
    public string               published;
    public string               updated;
}

[System.Serializable]
public class TimelineRecord {
    public float               timeSec;
    public TextRecord          text;
    public VoiceRecord         voice;
    public List<GraphicRecord> graphic;
    public SpecialActionRecord spAction;
}

[System.Serializable]
public class PoseRecord {
    public float   timeSec;
    public Vector3 leftHand;
    public Vector3 rightHand;
    public Vector3 leftElbow;
    public Vector3 rightElbow;
    public Vector3 lookAt;
    public Vector3 coreBody;
}

[System.Serializable]
public class GraphicRecord {
    public string graphicID;
    public string action;

    public GraphicRecord (string graphicID, string action) {
        this.graphicID = graphicID;
        this.action    = action;
    }
}

[System.Serializable]
public class SpecialActionRecord {
    public string action;
    public string facialExpression;

    public SpecialActionRecord (string action, string facialExpression) {
        this.action           = action;
        this.facialExpression = facialExpression;
    }
}

[System.Serializable]
public class SpeechHistory {
    public int    index;
    public float  timeSec;
    public float  durationSec;
    public string fileID;
}

[System.Serializable]
public class TextRecord {
    public float durationSec;

    public TextRecord (float durationSec) {
        this.durationSec = durationSec;
    }
}

[System.Serializable]
public class VoiceRecord {
    public float  durationSec;
    public string fileID;

    public VoiceRecord (float durationSec, string fileID) {
        this.durationSec = durationSec;
        this.fileID      = fileID;
    }
}