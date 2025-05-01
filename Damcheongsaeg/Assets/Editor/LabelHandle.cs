using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DoorTriggerInteraction))]
public class LabelHandle : Editor
{
    private static GUIStyle labelStyle;

    private void OnEnable()
    {
        labelStyle = new GUIStyle();
        labelStyle.normal.textColor = Color.white;
        labelStyle.alignment = TextAnchor.MiddleCenter;
    }

    private void OnSceneGUI() //  Scene 뷰에서 2D 또는 3D 요소를 그리거나 상호 작용할 수 있는 함수
                              // 주로 에디터 환경에서 편의 기능을 추가하거나 게임 오브젝트를 시각화하는 데 사용
                              // 게임 오브젝트가 Scene 뷰에서 선택될 때 호출
    {
        DoorTriggerInteraction door = (DoorTriggerInteraction)target;

        Handles.BeginGUI();
        Handles.Label(door.transform.position + new Vector3(0f, 4f, 0f), door.CurrentDoorPosition.ToString(), labelStyle);
        // 그릴 위치, 내용, 레이블 스타일(?)
        Handles.EndGUI();
    }
}
