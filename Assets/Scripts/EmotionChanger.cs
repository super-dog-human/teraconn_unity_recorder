using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionChanger : MonoBehaviour {
    Animator animator;

    public void ChangeTo (string emotion) {
        animator.SetBool(EmotionType(emotion), true);
    }

    void Start () {
        animator = gameObject.GetComponent<Animator>();
    }

    string EmotionType (string emotion) {
        switch(emotion) {
            case "smile1":
                return "emo1";
            case "smile2":
                return "emo2";
            case "anger1":
                return "emo3";
            case "anger2":
                return "emo4";
            case "anger3":
                return "emo5";
            case "sad1":
                return "emo6";
            case "sad2":
                return "emo7";
            case "surprise":
                return "emo8";
            default:
                return "emo1";
        }
    }
}
