using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapManagement : GameManager<SceneSwapManagement>
{
    protected SceneSwapManagement() { } // 외부에서 인스턴스 생성을 제한하고 싱글톤 패턴을 유지하기 위해 사용 

    private static bool _loadFromDoor;

    private GameObject _player;
    private Collider2D _playerCollider;
    private Collider2D _doorCollider;
    private Vector3 _playerSpawnPos;

    private DoorTriggerInteraction.DoorToSpawnAt _doorToSpawnTo; // 생성하려는 문

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

        //_doorToSpawnTo = doorToSpawnAt; // 생성한 문 할당
        //SceneManager.LoadScene(myScene);

        string sceneName = myScene.SceneName; // SceneField에서 씬 이름 가져오기
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

    // 패이드 인 될 때 타닥타닥 발자국 소리가 나면 좋겠어
}
