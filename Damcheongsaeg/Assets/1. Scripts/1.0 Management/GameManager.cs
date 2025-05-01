using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager<T> : MonoBehaviour where T : MonoBehaviour // ���� �̰� ���� �Ŵ����� �ƴ϶� �ν��Ͻ���� �Ҹ���.
{
    private static bool _quitingApplication = false; // m_���ø����̼��� ���� �Ǿ�����
    private static object _lock = new object(); // m_���ÿ� ������ �� �� �����常 �۵��ϱ� ���� ����
    private static T _instance; // �ν��Ͻ��� �ϳ��� �����ϱ� ���� ����

    public static T Instance
    {
        get
        {
            if (_quitingApplication) // ���� ���ø����̼��� ���� �� �̶�� 
            {
                return null;
            }

            lock (_lock) // lock: �Ҽӵ� �ڵ尡 ������ �Ϸ� �Ǳ� ������ ȣ�� �ص� ����
            {
                if (_instance == null) // ���� �ν��Ͻ��� ������
                {
                    _instance = (T)FindObjectOfType(typeof(T)); // ���� �ν��Ͻ��� ã��

                    if (_instance == null) // �׷��� ������
                    {
                        // �̱����� ������ �� obj�� ����
                        var singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";

                        DontDestroyOnLoad(singletonObject); // ���� �ٲ� �ı����� �ʰ� 
                    }
                    DontDestroyOnLoad(_instance);
                }
                return _instance; // �̹� �ν��Ͻ��� �ְų� ���� ����� �ν��Ͻ��� ��ȯ
            }
        }
    }

    private void OnApplicationQuit() // ���� ���� ��
    {
        _quitingApplication = true;
    }

    private void OnDestroy()
    {
        _quitingApplication = true;
    }

    private void Awake()
    {
        Application.targetFrameRate = 60; // ������ �� ��������
                                          // ���� �����ָ� �� ������ 30 ?
                                          // ���� ���� ���ͼ� 60���� ��������� �� ������� -���� ����
    }
}
