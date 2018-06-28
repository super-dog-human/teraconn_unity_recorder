using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicSwitcher : MonoBehaviour {
    LessonRecorder lessonRecorder;
    List<Sprite> graphics;
    List<string> graphicIds;
    SpriteRenderer spriteRenderer;
    int currentIndex;

    void Start () {
        LessonMaterial lessonMaterial = GameObject.Find("ScriptLoader").GetComponent<LessonMaterial>();
        lessonRecorder = GameObject.Find("ScriptLoader").GetComponent<LessonRecorder>();
        graphics       = lessonMaterial.graphics;
        graphicIds     = lessonMaterial.graphicIds;
        spriteRenderer = GameObject.Find("Graphic").GetComponent<SpriteRenderer>();
        currentIndex   = -1;
    }

    public void ChangeNextGraphic () {
        if (!CanChangeGraphic()) return;
        if (graphics.Count <= currentIndex + 1) return;

        currentIndex += 1;
        ChangeGraphic();
        RecordChangingGraphic(currentIndex - 1);
    }

    public void ChangePrevGraphic () {
        if (!CanChangeGraphic()) return;
        if (currentIndex < 0) return;

        currentIndex -= 1;
        ChangeGraphic();
        RecordChangingGraphic(currentIndex + 1);
    }

    bool CanChangeGraphic () {
        if (graphics.Count == 0) return false;
        return true;
    }

    void ChangeGraphic () {
        Sprite sprite = (currentIndex > -1) ? graphics[currentIndex] : null;
        spriteRenderer.sprite = sprite;
    }

    void RecordChangingGraphic (int oldGraphicIndex) {
        string changedGraphicId = (currentIndex    > -1) ? graphicIds[currentIndex]    : null;
        string oldGraphicId     = (oldGraphicIndex > -1) ? graphicIds[oldGraphicIndex] : null;

        lessonRecorder.RecordGraphicSwitching(changedGraphicId, oldGraphicId);
    }
}
