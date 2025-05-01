using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapManagement : GameManager<SceneSwapManagement>
{
    protected SceneSwapManagement() { } // �ܺο��� �ν��Ͻ� ������ �����ϰ� �̱��� ������ �����ϱ� ���� ��� 

    private static bool _loadFromDoor;

    private GameObject _player;
    private Collider2D _playerCollider;
    private Collider2D _doorCollider;
    private Vector3 _playerSpawnPos;

    private DoorTriggerInteraction.DoorToSpawnAt _doorToSpawnTo; // �����Ϸ��� ��

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerCollider = _player.GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoded;
    }

    public static void SwapSceneFromDoorUse(SceneField myScene, DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt)
    {
        _loadFromDoor = true;
         Instance.StartCoroutine(Instance.FadeOutThenChangeScene(myScene, doorToSpawnAt)); 
    }

    private IEnumerator FadeOutThenChangeScene(SceneField myScene, DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt = DoorTriggerInteraction.DoorToSpawnAt.None)
    {
        // start fading to black
        SceneFadeManagement.Instance.StartFadeOut();

        // keep fading out
        while (SceneFadeManagement.Instance.IsFadingOut)
        {
            yield return null;
        }

        //_doorToSpawnTo = doorToSpawnAt; // ������ �� �Ҵ�
        //SceneManager.LoadScene(myScene);

        string sceneName = myScene.SceneName; // SceneField���� �� �̸� ��������
        SceneManager.LoadScene(sceneName);
    }
 
    // Called whenever a new scene is loade (includin the start of the game)
    private void OnSceneLoded(Scene scene, LoadSceneMode mode)
    {
        SceneFadeManagement.Instance.StartFadeIn();
        if (_loadFromDoor)
        {
            // warp player to correct door location
            FindDoor(_doorToSpawnTo);
            _player.transform.position = _playerSpawnPos;

            _loadFromDoor = false;
        }
    }

    private void FindDoor(DoorTriggerInteraction.DoorToSpawnAt doorSpawnNumber)
    {
        DoorTriggerInteraction[] doors = FindObjectsOfType<DoorTriggerInteraction>(); 

        for (int i = 0; i < doors.Length; i++)
        {
            if (doors[i].CurrentDoorPosition == doorSpawnNumber)
            {
                _doorCollider = doors[i].gameObject.GetComponent<Collider2D>();

                // calculate spwn position
                CalculateSpawnPosition();
                return;
            }
        }
    }

    private void CalculateSpawnPosition()
    {
        float colliderHeight = _playerCollider.bounds.extents.y;
        _playerSpawnPos = _doorCollider.transform.position - new Vector3(0f, colliderHeight, 0f);
    }

    // ���̵� �� �� �� Ÿ��Ÿ�� ���ڱ� �Ҹ��� ���� ���ھ�
}
