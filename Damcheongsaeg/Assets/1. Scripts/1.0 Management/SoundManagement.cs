using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]            // 인스펙터창에 노출시킴
public class Sound
{
    public string soundName;     // 곡 이름
    public AudioClip clip;       // 곡
}

public class SoundManagement : GameManager<SoundManagement>
{
    protected SoundManagement() { } // 외부에서 인스턴스 생성을 제한하고 싱글톤 패턴을 유지하기 위해 사용

    public Sound[] bgmSounds;           // BGM 오디오 클립들
    public Sound[] effectSounds;        // 효과음 오디오 클립들

    // AudioSource: 재생기
    public AudioSource audioSourceBgmPlayers;           // BGM 재생기 (동시에 여러개 재생 X)
    public AudioSource[] audioSourceEffectsPlayers;     // 효과음 재생기

    public string[] playSoundName;                      // 재생 중인 효과음 사운드 이름 

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

    public void PlaySE(string name, float volume)                                                     // name: 곡 이름
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            // 만약 효과음 배열에서 똑같은 이름의 곡이 있는지 찾아 검사하고
            if (name == effectSounds[i].soundName)
            {
                for (int j = 0; j < audioSourceEffectsPlayers.Length; j++)
                {
                    if (!audioSourceEffectsPlayers[j].isPlaying)                        // 재생중이지 않은 재생기를 찾으면
                    {
                        audioSourceEffectsPlayers[j].clip = effectSounds[i].clip;       // clip: 오디오 클립
                        audioSourceEffectsPlayers[j].volume = volume;                   // 검토 바람 볼륨 값으로 재생하길 바람
                        audioSourceEffectsPlayers[j].Play();                            //오디오 재생
                        playSoundName[j] = effectSounds[i].soundName;
                        return;
                    }
                }
                return;
            }
        }
        Debug.Log(name + "사운드가 사운드 메니저에 등록되지 않음");
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
        Debug.Log(name + "사운드가 사운드 매니저에 등록되지 않음");
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

    public void StopAllEffectsSound()               //모든 효과음 끄기
    {
        for (int i = 0; i < audioSourceEffectsPlayers.Length; i++)
        {
            audioSourceEffectsPlayers[i].Stop();
        }
    }

    public void StopEffectsSound(string name)       //특정 효과음 끄기
    {
        for (int i = 0; i < audioSourceEffectsPlayers.Length; i++)
        {
            if (playSoundName[i] == name)
            {
                audioSourceEffectsPlayers[i].Stop();
                break;
            }
        }
        Debug.Log("재생중인 " + name + "사운드가 없음");
    }
}
