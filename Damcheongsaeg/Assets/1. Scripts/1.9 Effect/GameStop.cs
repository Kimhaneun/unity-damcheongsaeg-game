using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStop : MonoBehaviour
{
    public float sleepTime;

    public void Sleep()
    {
        float duration = sleepTime;
        // 이게 되도 움직일 수 있게 해야해
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0; // 게임 시간 정지 
        yield return new WaitForSecondsRealtime(duration);  // duration 시간동안 기다려서 
        Time.timeScale = 1; // 시간 원상 복구
    }
}
