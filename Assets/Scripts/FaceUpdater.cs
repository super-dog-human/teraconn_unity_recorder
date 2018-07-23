using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceUpdater : MonoBehaviour {
    /*
    int blendShapeCount;
    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;
    float blendOne = 0f;
    float blendTwo = 0f;
    float blendSpeed = 1f;
    bool blendOneFinished = false;

    void Awake () {
        skinnedMeshRenderer = GameObject.Find("Mesh/Kaoru_skin").GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = skinnedMeshRenderer.sharedMesh;
    }

    void Start () {
        Debug.Log("count: " + skinnedMesh.blendShapeCount);
        blendShapeCount = skinnedMesh.blendShapeCount;
    }

    void Update () {
        if (blendShapeCount > 2) {
            if (blendOne < 100f) {
                skinnedMeshRenderer.SetBlendShapeWeight (0, blendOne);
                blendOne += blendSpeed;
            } else {
                blendOneFinished = true;
            }

            if (blendOneFinished == true && blendTwo < 100f) {
                skinnedMeshRenderer.SetBlendShapeWeight (1, blendTwo);
                blendTwo += blendSpeed;
            }
        }
    }
*/
}
