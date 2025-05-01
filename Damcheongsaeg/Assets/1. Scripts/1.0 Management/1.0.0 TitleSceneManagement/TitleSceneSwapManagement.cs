using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneSwapManagement : MonoBehaviour
{
    public enum ButtonToSpawnAt
    {
        WestUnderground 
    }

    [Header("Spawn TO")]
    [SerializeField] private ButtonToSpawnAt ButtonToSpawnTo;
    [SerializeField] private SceneField _sceneToLoad;

    [Space(10)]

    [Header("Scene Fade")]
    [SerializeField] private GameObject FadeCanvas;

    public void OnScratchButtonInput()
    {
        // Load new scene
        SwapSceneFromScratchButton(_sceneToLoad, ButtonToSpawnTo);
    }

    private bool _loadFromScratchButton;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoded;
    }

    private void SwapSceneFromScratchButton(SceneField myScene, ButtonToSpawnAt buttonToSpawnAt)
    {
        _loadFromScratchButton = true;
        StartCoroutine(FadeOutThenChangeScene(myScene, buttonToSpawnAt));
    }

    private IEnumerator FadeOutThenChangeScene(SceneField myScene, ButtonToSpawnAt buttonToSpawnAt = ButtonToSpawnAt.WestUnderground)
    {
        // start fading to black
        SceneFadeManagement.Instance.StartFadeOut();

        // keep fading out
        while (SceneFadeManagement.Instance.IsFadingOut)
        {
            yield return null;
        }
        // Gets scene name from SceneField
        string sceneName = myScene.SceneName;
        SceneManager.LoadScene(sceneName);
    }

    // Called whenever a new scene is loade (includin the start of the game)
    private void OnSceneLoded(Scene scene, LoadSceneMode mode)
    {
        SceneFadeManagement.Instance.StartFadeIn();
        if (_loadFromScratchButton)
        {
            _loadFromScratchButton = false;
        }
    }
}
