using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggerInteraction : TriggerInteractionBase
{
    public enum DoorToSpawnAt
    {
        // 사용하는 문 만큼 작성
        None,
        Test,
        Room
    }

    [Header("Spawn TO")]
    [SerializeField] private DoorToSpawnAt DoorToSpawnTo; // 어느 문인지 보여줌
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
