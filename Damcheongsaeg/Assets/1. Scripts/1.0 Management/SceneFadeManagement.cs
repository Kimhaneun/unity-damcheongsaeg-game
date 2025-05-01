using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeManagement : GameManager<SceneFadeManagement>
{
    protected SceneFadeManagement() { } // �ܺο��� �ν��Ͻ� ������ �����ϰ� �̱��� ������ �����ϱ� ���� ���

    [SerializeField] private Image _fadeOutImage;
    [Range(0.1f, 10f), SerializeField] private float _fadeOutSpeed;
    [Range(0.1f, 10f), SerializeField] private float _fadeInSpeed;

    [SerializeField] private Color FadeOutStartColor;

    #region STATE PARAMETERS
    public bool IsFadingOut { get; private set; }
    public bool IsFadingIn { get; private set; }
    #endregion

    private void Awake()
    {
        // FadeOutStartColor.a = 0f;
    }

    private void Update()
    {
        if (IsFadingOut) 
        {
            _fadeOutImage.enabled = true;
            if (_fadeOutImage.color.a < 1f) 
            {
                FadeOutStartColor.a += Time.deltaTime * _fadeOutSpeed;
                _fadeOutImage.color = FadeOutStartColor; 
            }
            else
            {
                IsFadingOut = false; 
            }
        }

        if (IsFadingIn)
        {
            _fadeOutImage.enabled = true;
            if (_fadeOutImage.color.a > 0f) 
            {
                FadeOutStartColor.a -= Time.deltaTime * _fadeInSpeed;
                _fadeOutImage.color = FadeOutStartColor; 
            }
            else
            {
                IsFadingIn = false;
                _fadeOutImage.enabled = false;
            }
        }
    }

    public void StartFadeOut()
    {
        _fadeOutImage.color = FadeOutStartColor;
        IsFadingOut = true;
    }

    public void StartFadeIn()
    {
        if (_fadeOutImage.color.a >= 1f) 
        {
            _fadeOutImage.color = FadeOutStartColor;
            IsFadingIn = true;
        }
    }
}

