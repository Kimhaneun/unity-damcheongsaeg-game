using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStop : MonoBehaviour
{
    public float sleepTime;

    public void Sleep()
    {
        float duration = sleepTime;
        // �̰� �ǵ� ������ �� �ְ� �ؾ���
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0; // ���� �ð� ���� 
        yield return new WaitForSecondsRealtime(duration);  // duration �ð����� ��ٷ��� 
        Time.timeScale = 1; // �ð� ���� ����
    }
}
