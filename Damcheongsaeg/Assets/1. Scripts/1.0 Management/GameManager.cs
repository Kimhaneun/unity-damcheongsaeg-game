using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager<T> : MonoBehaviour where T : MonoBehaviour // 이제 이건 게임 매니저가 아니라 인스턴스라고 불린다.
{
    private static bool _quitingApplication = false; // m_어플리케이션이 종료 되었는지
    private static object _lock = new object(); // m_동시에 접근할 때 한 스레드만 작동하기 위한 변수
    private static T _instance; // 인스턴스가 하나만 존재하기 위한 변수

    public static T Instance
    {
        get
        {
            if (_quitingApplication) // 만약 어플리케이션이 종료 중 이라면 
            {
                return null;
            }

            lock (_lock) // lock: 소속된 코드가 실행이 완료 되기 전까지 호출 해도 막음
            {
                if (_instance == null) // 만약 인스턴스가 없으면
                {
                    _instance = (T)FindObjectOfType(typeof(T)); // 기존 인스턴스를 찾음

                    if (_instance == null) // 그래도 없으면
                    {
                        // 싱글톤을 연결할 새 obj를 만듦
                        var singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";

                        DontDestroyOnLoad(singletonObject); // 씬이 바뀌어도 파괴되지 않게 
                    }
                    DontDestroyOnLoad(_instance);
                }
                return _instance; // 이미 인스턴스가 있거나 새로 만들면 인스턴스를 반환
            }
        }
    }

    private void OnApplicationQuit() // 게임 종료 시
    {
        _quitingApplication = true;
    }

    private void OnDestroy()
    {
        _quitingApplication = true;
    }

    private void Awake()
    {
        Application.targetFrameRate = 60; // 프레임 수 고정해줌
                                          // 고정 안해주면 한 프레임 30 ?
                                          // 몰라 적게 나와서 60으로 고정해줘야 돼 모바일은 -은총 선배
    }
}
