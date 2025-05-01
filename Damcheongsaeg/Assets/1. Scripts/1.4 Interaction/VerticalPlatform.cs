using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlatform : MonoBehaviour
{
    #region COMPONENTS
    public PlatformEffector2D PlatformEffector2D { get; private set; }
    #endregion

    #region STATE CONSTANT
    private const float PRESS_TIME = 0f;
    #endregion

    [SerializeField] private float _holdTime;

    private void Awake()
    {
        PlatformEffector2D = GetComponent<PlatformEffector2D>();
    }

    private void Update()
    {
        #region INPUT HANDLER
        if (Input.GetKey(KeyCode.S))
        {
            _holdTime += Time.deltaTime;

            if (_holdTime >= PRESS_TIME)
            {
                PlatformDownOpen();
            }
        }
        else
        {
            _holdTime = 0f; // S 키를 뗐을 때 타이머 초기화
            PlatformUpOpen();
        }
        #endregion
    }

    private void PlatformDownOpen()
    {
        PlatformEffector2D.rotationalOffset = 180f;
    }

    private void PlatformUpOpen()
    {
        PlatformEffector2D.rotationalOffset = 0f;
    }
}
