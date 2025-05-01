using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]            // �ν�����â�� �����Ŵ
public class Sound
{
    public string soundName;     // �� �̸�
    public AudioClip clip;       // ��
}

public class SoundManagement : GameManager<SoundManagement>
{
    protected SoundManagement() { } // �ܺο��� �ν��Ͻ� ������ �����ϰ� �̱��� ������ �����ϱ� ���� ���

    public Sound[] bgmSounds;           // BGM ����� Ŭ����
    public Sound[] effectSounds;        // ȿ���� ����� Ŭ����

    // AudioSource: �����
    public AudioSource audioSourceBgmPlayers;           // BGM ����� (���ÿ� ������ ��� X)
    public AudioSource[] audioSourceEffectsPlayers;     // ȿ���� �����

    public string[] playSoundName;                      // ��� ���� ȿ���� ���� �̸� 

    [Space(5f)]

    [Header("VlumeBGM")]
    public float MaxVlumeBGM;
    public float MinVlumeBGM;

    [Range(0f, 1f), SerializeField] public float vlumeBGM;
    [Range(0f, 1f), SerializeField] public float vlumeSpeedBGM;

    public bool IsVlumeAccelBGM { get; private set; }
    public bool IsBGMVlumeDeccel { get; private set; }

    private void Start()
    {
        playSoundName = new string[audioSourceEffectsPlayers.Length];
    }

    private void Update()
    {
        if (IsVlumeAccelBGM) 
        {
            if (vlumeBGM < MaxVlumeBGM)
            {
                vlumeBGM += Time.deltaTime * vlumeSpeedBGM;
                audioSourceBgmPlayers.volume = vlumeBGM;
            }
            else
            {
                IsVlumeAccelBGM = false;
            }
        }

        if (IsBGMVlumeDeccel) 
        {
            if (vlumeBGM > MinVlumeBGM)
            {
                vlumeBGM -= Time.deltaTime * vlumeSpeedBGM;
                audioSourceBgmPlayers.volume = vlumeBGM;
            }
            else
            {
                IsBGMVlumeDeccel = false;
            }
        }
    }

    public void PlaySE(string name, float volume)                                                     // name: �� �̸�
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            // ���� ȿ���� �迭���� �Ȱ��� �̸��� ���� �ִ��� ã�� �˻��ϰ�
            if (name == effectSounds[i].soundName)
            {
                for (int j = 0; j < audioSourceEffectsPlayers.Length; j++)
                {
                    if (!audioSourceEffectsPlayers[j].isPlaying)                        // ��������� ���� ����⸦ ã����
                    {
                        audioSourceEffectsPlayers[j].clip = effectSounds[i].clip;       // clip: ����� Ŭ��
                        audioSourceEffectsPlayers[j].volume = volume;                   // ���� �ٶ� ���� ������ ����ϱ� �ٶ�
                        audioSourceEffectsPlayers[j].Play();                            //����� ���
                        playSoundName[j] = effectSounds[i].soundName;
                        return;
                    }
                }
                return;
            }
        }
        Debug.Log(name + "���尡 ���� �޴����� ��ϵ��� ����");
    }

    public void PlayBGM(string name, float volume)
    {
        for (int i = 0; i < bgmSounds.Length; i++)
        {
            if (name == bgmSounds[i].soundName)
            {
                audioSourceBgmPlayers.clip = bgmSounds[i].clip;
                audioSourceBgmPlayers.volume = volume;
                audioSourceBgmPlayers.Play();
                return;
            }
        }
        Debug.Log(name + "���尡 ���� �Ŵ����� ��ϵ��� ����");
    }

    public void StartBGMVlumeDeccel()
    {
        IsBGMVlumeDeccel = true;
        PlayBGM("Rain", vlumeBGM);
    }

    public void StartBGMVlumeAccel()
    {
        IsVlumeAccelBGM = true;
        PlayBGM("Rain", vlumeBGM);
    }

    public void StopAllEffectsSound()               //��� ȿ���� ����
    {
        for (int i = 0; i < audioSourceEffectsPlayers.Length; i++)
        {
            audioSourceEffectsPlayers[i].Stop();
        }
    }

    public void StopEffectsSound(string name)       //Ư�� ȿ���� ����
    {
        for (int i = 0; i < audioSourceEffectsPlayers.Length; i++)
        {
            if (playSoundName[i] == name)
            {
                audioSourceEffectsPlayers[i].Stop();
                break;
            }
        }
        Debug.Log("������� " + name + "���尡 ����");
    }
}
