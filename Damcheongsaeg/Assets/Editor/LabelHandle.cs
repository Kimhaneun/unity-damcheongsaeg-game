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

    private void OnSceneGUI() //  Scene �信�� 2D �Ǵ� 3D ��Ҹ� �׸��ų� ��ȣ �ۿ��� �� �ִ� �Լ�
                              // �ַ� ������ ȯ�濡�� ���� ����� �߰��ϰų� ���� ������Ʈ�� �ð�ȭ�ϴ� �� ���
                              // ���� ������Ʈ�� Scene �信�� ���õ� �� ȣ��
    {
        DoorTriggerInteraction door = (DoorTriggerInteraction)target;

        Handles.BeginGUI();
        Handles.Label(door.transform.position + new Vector3(0f, 4f, 0f), door.CurrentDoorPosition.ToString(), labelStyle);
        // �׸� ��ġ, ����, ���̺� ��Ÿ��(?)
        Handles.EndGUI();
    }
}
