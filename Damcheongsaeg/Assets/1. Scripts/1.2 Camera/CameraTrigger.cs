using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

#if UNITY_EDITOR
using UnityEditor;

public class CameraTrigger : MonoBehaviour
{
    public CustomInspectorObjects customInspectorObjects;

    #region COMPONENTS
    private Collider2D Collider2D;
    #endregion

    private void Awake()
    {
        Collider2D = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // pan the camera
            // Move the camera when the player reaches the point
            CameraManagement.Instance.PanCameraOnContanct(customInspectorObjects.panDistance,
                customInspectorObjects.panTime, customInspectorObjects.PanDirection, false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 exitDirection = (collision.transform.position - Collider2D.bounds.center).normalized;

            if (customInspectorObjects.swapCameras && customInspectorObjects.cameraOnLeft != null && customInspectorObjects.cameraOnRight != null)
            {
                // swap cameras
                CameraManagement.Instance.SwapCamera(customInspectorObjects.cameraOnLeft, customInspectorObjects.cameraOnRight, exitDirection);
            }

            if (customInspectorObjects.panCamerOnContact)
            {
                // pan the camera
                CameraManagement.Instance.PanCameraOnContanct(customInspectorObjects.panDistance,
                    customInspectorObjects.panTime, customInspectorObjects.PanDirection, true);
            }
        }
    }
}

[System.Serializable]
public class CustomInspectorObjects // 이 클래스는 인스펙터(Inspector) 창에서 해당 변수들을 편집할 수 있도록 하는 데 사용
{
    public bool swapCameras;
    public bool panCamerOnContact;

    [HideInInspector] public CinemachineVirtualCamera cameraOnLeft;
    [HideInInspector] public CinemachineVirtualCamera cameraOnRight;

    [HideInInspector] public PanDirection PanDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime = 0.35f;
}

public enum PanDirection // enum: 열거형
{
    // Camera movement direction
    Up,
    Down,
    Left,
    Right
}

[CustomEditor(typeof(CameraTrigger))]

public class MyScriptEditor : Editor
{
    CameraTrigger CameraTrigger;

    private void OnEnable()
    {
        CameraTrigger = (CameraTrigger)target;
    }

    public override void OnInspectorGUI() //Inspector 창에 커스텀 UI 요소를 그리고 필드 및 속성을 편집하는 데 사용
    {
        DrawDefaultInspector();

        if (CameraTrigger.customInspectorObjects.swapCameras) 
        {
            CameraTrigger.customInspectorObjects.cameraOnLeft = EditorGUILayout.ObjectField("Camera On Left", CameraTrigger.customInspectorObjects.cameraOnLeft,
                typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;

            CameraTrigger.customInspectorObjects.cameraOnRight = EditorGUILayout.ObjectField("Camera On Right", CameraTrigger.customInspectorObjects.cameraOnRight,
                typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
        }

        if (CameraTrigger.customInspectorObjects.panCamerOnContact)
        {
            CameraTrigger.customInspectorObjects.PanDirection = (PanDirection)EditorGUILayout.EnumPopup("Camera Pan Direction",
                CameraTrigger.customInspectorObjects.PanDirection);

            CameraTrigger.customInspectorObjects.panDistance = EditorGUILayout.FloatField("Pan Distance", CameraTrigger.customInspectorObjects.panDistance);
            CameraTrigger.customInspectorObjects.panTime = EditorGUILayout.FloatField("Pan Time", CameraTrigger.customInspectorObjects.panTime);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(CameraTrigger);
        }
    }
}
#endif
