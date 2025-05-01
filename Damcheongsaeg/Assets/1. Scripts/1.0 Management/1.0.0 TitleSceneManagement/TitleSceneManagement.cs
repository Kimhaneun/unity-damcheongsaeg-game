using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class TitleSceneManagement : MonoBehaviour
{
    public string fileStream = @"C:\Users\ggmuser\Desktop\SaveDamcheongsaegProject\Damcheongsaeg\Assets\11. Texts.txt";

    private void Awake()
    {
        SceneFadeManagement.Instance.StartFadeIn();
        SoundManagement.Instance.PlayBGM("Rain", SoundManagement.Instance.vlumeBGM);
        SoundManagement.Instance.StartBGMVlumeAccel();

        throw new Exception("예외 발생문");
    }

    private void Start()
    {
        if (!File.Exists(fileStream))
        {
            using (StreamWriter sw = File.CreateText(fileStream))
            {
                sw.WriteLine("Hello");
                sw.WriteLine("And");
                sw.WriteLine("Welcome");
            }
        }

        using (StreamReader sr = File.OpenText(fileStream))
        {
            string s;
            while ((s = sr.ReadLine()) != null)
            {
                Debug.Log(s);
            }
        }
    }
}