using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggerInteraction : TriggerInteractionBase
{
    public enum DoorToSpawnAt
    {
        // ����ϴ� �� ��ŭ �ۼ�
        None,
        Test,
        Room
    }

    [Header("Spawn TO")]
    [SerializeField] private DoorToSpawnAt DoorToSpawnTo; // ��� ������ ������
    [SerializeField] private SceneField _sceneToLoad;

    [Space(10f)]
    [Header("This Door")]
    public DoorToSpawnAt CurrentDoorPosition;

    public override void OnInteractInput()
    {
        // Load new scene
        SceneSwapManagement.SwapSceneFromDoorUse(_sceneToLoad, DoorToSpawnTo);
    }
}
