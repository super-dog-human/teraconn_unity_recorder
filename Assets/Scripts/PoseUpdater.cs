using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoseUpdater : MonoBehaviour {
    const int   idlingAnimationId = 19;
    const float cameraZIndex      = 15.0f;
    const float accuracyThreshold = 0.3f;
    Animator animator;
    PoseVector currentPoseVector;
    LessonRecorder lessonRecorder;
    PoseRecord poseRecord;
    bool isMoveForward;
    bool isMoveBack;

    void Start () {
        animator = GetComponent<Animator>();
        animator.SetInteger("animation", idlingAnimationId);

        lessonRecorder = GameObject.Find("ScriptLoader").GetComponent<LessonRecorder>();
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            isMoveForward = false;
            isMoveBack    = true;
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            isMoveForward = true;
            isMoveBack    = false;
        } else if (Input.GetKeyUp(KeyCode.UpArrow)) {
            isMoveForward = false;
            isMoveBack    = false;
        } else if (Input.GetKeyUp(KeyCode.DownArrow)) {
            isMoveForward = false;
            isMoveBack    = false;
        }
    }

    void FixedUpdate () {
        Vector3 target = transform.position;

        if (isMoveForward && transform.position.z <= 10.0f) {
            target +=  Vector3.forward * 1.0f;
        } else if (isMoveBack && transform.position.z >= 0.5f) {
            target +=  Vector3.back * 1.0f;
        } else {
            return;
        }

        Vector3 updatedPosition = Vector3.MoveTowards(transform.position, target, 0.2f);
        transform.position = updatedPosition;

        PoseRecord coreBodyPoseRecord = new PoseRecord();
        coreBodyPoseRecord.coreBody = updatedPosition;
        lessonRecorder.RecordPose(coreBodyPoseRecord);
    }

    void OnAnimatorIK (int layerIndex) {
        if (currentPoseVector == null) return;

        poseRecord = new PoseRecord();

        UpdateCoreBodyPosition();
        UpdateHeadPosition();
        UpdateArmsPosition();

        lessonRecorder.RecordPose(poseRecord);
    }

    public void SwicthMovingBackAndForward(string movingType) {
        switch(movingType) {
            case "startMovingForward":
                isMoveForward = true;
                isMoveBack    = false;
                break;
            case "startMovingBack":
                isMoveForward = false;
                isMoveBack    = true;
                break;
            case "stopMoving":
                isMoveForward = false;
                isMoveBack    = false;
                break;
            default:
                break;
        }
    }

    void UpdatePosition(string jsonString) {
        Pose pose = JsonUtility.FromJson<Pose>(jsonString);
        currentPoseVector = new PoseVector(pose);
    }

    void UpdateCoreBodyPosition () {
        Vector3 rightShoulderPosition = currentPoseVector.rightShoulder.position;
        rightShoulderPosition.z = cameraZIndex;
        Vector3 worldRightShoulderPosition = Camera.main.ScreenToWorldPoint(rightShoulderPosition);

        Vector3 leftShoulderPosition = currentPoseVector.leftShoulder.position;
        leftShoulderPosition.z = cameraZIndex;
        Vector3 worldLeftShoulderPosition = Camera.main.ScreenToWorldPoint(leftShoulderPosition);

        Vector3 position = transform.position;
        position.x = (worldRightShoulderPosition.x + worldLeftShoulderPosition.x) / 2 ;
        Vector3 bodyPosition = Vector3.Lerp(transform.position, position, Time.deltaTime * 2);

        if (transform.position == bodyPosition) return;

        poseRecord.coreBody = bodyPosition;
        transform.position = bodyPosition;
    }

    void UpdateHeadPosition () {
        if (!IsEyesAndNoseScoreGood(currentPoseVector)) return;
        if (!IsLeftOrRightEarScoreGood(currentPoseVector)) return;

        float leftLength = (currentPoseVector.leftEar.position - currentPoseVector.leftEye.position).magnitude;
        float rightLength = (currentPoseVector.rightEar.position - currentPoseVector.rightEye.position).magnitude;

        bool isLookRight = false;
        if (currentPoseVector.leftEar.score >= accuracyThreshold && currentPoseVector.rightEar.score < 0.1) {
            isLookRight = true;
        } else if (leftLength > rightLength) {
            isLookRight = true;
        }
        float xRatio = isLookRight ? -(leftLength / rightLength) : rightLength / leftLength;

        Vector3 leftEyePosition  = animator.GetBoneTransform(HumanBodyBones.LeftEye).position;
        Vector3 rightEyePosition = animator.GetBoneTransform(HumanBodyBones.RightEye).position;
        float eyeLength = (leftEyePosition - rightEyePosition).magnitude;

        animator.SetLookAtWeight(1, 0.1f, 1f, 0.3f, 1f);
        Vector3 lookAtPosition = (leftEyePosition + rightEyePosition) / 2;
        if (Mathf.Abs(xRatio) >= 1.3f ) {
            lookAtPosition.x += eyeLength * xRatio;
        }

        poseRecord.lookAt = lookAtPosition;
        animator.SetLookAtPosition(lookAtPosition);
    }

    bool IsEyesAndNoseScoreGood (PoseVector poseVector) {
        bool isGoodScore = true;
        string[] faceKeypoints = { "nose", "leftEye", "rightEye" };
        foreach (FieldInfo field in poseVector.GetType().GetFields())
        {
            if (!(faceKeypoints.Contains(field.Name))) continue;

            Keypoint keypoint = (Keypoint)field.GetValue(poseVector);
            if (keypoint.score < accuracyThreshold)
            {
                isGoodScore = false;
                break;
            }
        };
        return isGoodScore;
    }

    bool IsLeftOrRightEarScoreGood (PoseVector poseVector) {
        if (poseVector.leftEar.score >= accuracyThreshold)  { return true; }
        if (poseVector.rightEar.score >= accuracyThreshold) { return true; }
        return false;
    }

    void UpdateArmsPosition () {
        // swap the left and right
        SetWorldElbowPosition(currentPoseVector.leftElbow, AvatarIKHint.RightElbow);
        SetWorldHandPosition(currentPoseVector.leftElbow, currentPoseVector.leftWrist, AvatarIKGoal.RightHand);

        SetWorldElbowPosition(currentPoseVector.rightElbow, AvatarIKHint.LeftElbow);
        SetWorldHandPosition(currentPoseVector.rightElbow, currentPoseVector.rightWrist, AvatarIKGoal.LeftHand);
    }

    void SetWorldElbowPosition (PartVector elbowPart, AvatarIKHint avatarPart) {
        if (elbowPart.score < accuracyThreshold) return;

        Vector3 position = elbowPart.position;
        position.z = cameraZIndex;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
        worldPosition.z = animator.GetIKHintPosition(avatarPart).z;

        animator.SetIKHintPositionWeight(avatarPart, 1);
        animator.SetIKHintPosition(avatarPart, worldPosition);

        if (avatarPart == AvatarIKHint.LeftElbow) {
            poseRecord.leftElbow  = worldPosition;
        } else {
            poseRecord.rightElbow = worldPosition;
        }
    }

    void SetWorldHandPosition (PartVector elbowPart, PartVector wristPart, AvatarIKGoal avatarPart) {
        if (wristPart.score < accuracyThreshold) return;

        Vector3 position = HandPosition(elbowPart.position, wristPart.position);
        position.z = cameraZIndex;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
        worldPosition.z = animator.GetIKPosition(avatarPart).z;

        animator.SetIKPositionWeight(avatarPart, 1);
        animator.SetIKPosition(avatarPart, worldPosition);

        if(avatarPart == AvatarIKGoal.LeftHand) {
            poseRecord.leftHand  = worldPosition;
        } else {
            poseRecord.rightHand = worldPosition;
        }
    }

    Vector3 HandPosition (Vector3 elbowPosition, Vector3 wristPosition) {
        float armLength = (wristPosition - elbowPosition).magnitude;
        Vector3 handVector = (wristPosition - elbowPosition).normalized * armLength * 0.5f;
        return wristPosition + handVector;
    }

    public class PoseVector {
        public PartVector nose { get; set; }
        public PartVector leftEye { get; set; }
        public PartVector rightEye { get; set; }
        public PartVector leftEar { get; set; }
        public PartVector rightEar { get; set; }
        public PartVector leftShoulder { get; set; }
        public PartVector rightShoulder { get; set; }
        public PartVector leftElbow { get; set; }
        public PartVector rightElbow { get; set; }
        public PartVector leftWrist { get; set; }
        public PartVector rightWrist { get; set; }
        public PartVector leftHip { get; set; }
        public PartVector rightHip { get; set; }

        public PoseVector(Pose pose) {
            nose          = new PartVector(pose.nose);
            leftEye       = new PartVector(pose.leftEye);
            rightEye      = new PartVector(pose.rightEye);
            leftEar       = new PartVector(pose.leftEar);
            rightEar      = new PartVector(pose.rightEar);
            leftShoulder  = new PartVector(pose.leftShoulder);
            rightShoulder = new PartVector(pose.rightShoulder);
            leftElbow     = new PartVector(pose.leftElbow);
            rightElbow    = new PartVector(pose.rightElbow);
            leftWrist     = new PartVector(pose.leftWrist);
            rightWrist    = new PartVector(pose.rightWrist);
            leftHip       = new PartVector(pose.leftHip);
            rightHip      = new PartVector(pose.rightHip);
        }
    }

    public class PartVector {
        public float score { get; set; }
        public Vector3 position { get; set; }

        const int poseCanvasWidth  = 640;
        const int poseCanvasHeight = 480;

        public PartVector(Keypoint keypoint)
        {
            float multiplier = Screen.width / poseCanvasWidth;
            score = keypoint.score;

            Vector3 originPose = new Vector3(Screen.width, poseCanvasHeight * multiplier, 0);
            Vector3 partPose   = new Vector3(keypoint.x * multiplier, keypoint.y * multiplier, 0);
            position = originPose - partPose;
        }
    }

    [System.Serializable]
    public class CanvasSize {
        public int width;
        public int height;
    }

    [System.Serializable]
    public class Pose {
        public float score;
        public Keypoint nose;
        public Keypoint leftEye;
        public Keypoint rightEye;
        public Keypoint leftEar;
        public Keypoint rightEar;
        public Keypoint leftShoulder;
        public Keypoint rightShoulder;
        public Keypoint leftElbow;
        public Keypoint rightElbow;
        public Keypoint leftWrist;
        public Keypoint rightWrist;
        public Keypoint leftHip;
        public Keypoint rightHip;
        public Keypoint leftKnee;
        public Keypoint rightKnee;
        public Keypoint leftAnkle;
        public Keypoint rightAnkle;
    }

    [System.Serializable]
    public class Keypoint {
        public float score;
        public float x;
        public float y;
    }
}