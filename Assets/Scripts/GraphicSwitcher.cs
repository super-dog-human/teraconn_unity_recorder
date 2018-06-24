using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicSwitcher : MonoBehaviour {
    private LessonMaterial lessonMaterial;
    private List<Sprite> graphics;
    private SpriteRenderer spriteRenderer;
    private int currentIndex;

    void Start ()
    {
        lessonMaterial = GameObject.Find("ScriptLoader").GetComponent<LessonMaterial>();
        graphics       = lessonMaterial.graphics;
        spriteRenderer = GameObject.Find("Graphic").GetComponent<SpriteRenderer>();
        currentIndex = -1;
    }

    public void ChangeNextGraphic ()
    {
        if (!CanChangeGraphic()) return;
        if (graphics.Count <= currentIndex + 1) return;

        currentIndex += 1;
        ChangeGraphic();
    }

    public void ChangePrevGraphic ()
    {
        if (!CanChangeGraphic()) return;
        if (currentIndex < 0) return;

        currentIndex -= 1;

        if (currentIndex == -1) {
            spriteRenderer.sprite = null;
            return;
        }

        ChangeGraphic();
    }

    private void ChangeGraphic ()
    {
        Sprite sprite = graphics[currentIndex];
        spriteRenderer.sprite = sprite;
    }

    private bool CanChangeGraphic ()
    {
        if (!lessonMaterial.isLoadCompleted) return false;
        if (graphics.Count == 0) return false;
        return true;
    }
}
