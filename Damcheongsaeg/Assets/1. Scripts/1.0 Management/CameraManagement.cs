using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManagement : GameManager<CameraManagement>
{
    protected CameraManagement() { } // 외부에서 인스턴스 생성을 제한하고 싱글톤 패턴을 유지하기 위해 사용

    [SerializeField] private CinemachineVirtualCamera[] _allVirtualCameras;

    [Header("Controls for lerping the Y Damping during player jump/fall")]
    [SerializeField] private float _fallPanAmount; // 낙하 중 카메라의 수직 이동을 감쇠하는 양
    [SerializeField] private float _fallYPanTime; // 감쇠 지속 시간, 값은 Y 댐핑이 얼마 동안 변경될지 제어
    public float _fallSpeedYDampingChangeThreshold; // 낙하 속도가 일정 임계값을 넘으면 Y 댐핑을 변경하는 데 사용되는 임계값
                                                    // 낙하 속도가 언제 Y 댐핑을 변경할지 결정
    [Space(5)]
    [SerializeField] private float _normYPanAmount; // 일반적인(낙하 중이 아닐 때) Y 댐핑 값
                                                    // 낙하 동작 중이 아닐 때 카메라 동작을 정의

    public bool IsLerpingYDamping { get; private set; } // 댐핑(Damping) 값을 변경하는 동작 중인지 여부
    public bool LerpedFromPlayerFalling { get; set; } // 플레이어의 낙하 동작에서 댐핑 값을 변경했는지 여부
                                                      // 낙하 동작 중인 플레이어에게 영향을 주었음을 나타냄

    private CinemachineVirtualCamera _currentCamera; // 현재 활성화된 CinemachineVirtualCamera 오브젝트를 나타내는 변수
    private CinemachineFramingTransposer _framingTransposer; // 현재 카메라의 프레임 구성을 나타내는 변수
                                                             // 프레임 구성은 카메라 시야와 추적을 조절하기 위해 사용

    private Vector2 _startingTrackedObjectOffset;

    private void Awake()
    {
#if UNITY_EDITOR
        UnityEditorInitialize();
#endif
        for (int i = 0; i < _allVirtualCameras.Length; i++)
        {
            if (_allVirtualCameras[i].enabled)
            {
                // Set the current active camera
                _currentCamera = _allVirtualCameras[i];

                // Set the framing transposer
                _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }

        // set the YDamping amount so it' based on the inspector value
        _normYPanAmount = _framingTransposer.m_YDamping;

        // set the starting position of the tracked object offset
        _startingTrackedObjectOffset = _framingTransposer.m_TrackedObjectOffset;
    }

#if UNITY_EDITOR
    private void UnityEditorInitialize()
    {
        for (int i = 0; i < _allVirtualCameras.Length; i++)
        {
            if (_allVirtualCameras[i].enabled)
            {
                // Set the current active camera
                _currentCamera = _allVirtualCameras[i];

                // Set the framing transposer
                _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }

        // set the YDamping amount so it's based on the inspector value
        _normYPanAmount = _framingTransposer.m_YDamping;

        // set the starting position of the tracked object offset
        _startingTrackedObjectOffset = _framingTransposer.m_TrackedObjectOffset;
    }
#endif

    #region LERP THE Y DAMPING
    public void LerpYDamping(bool isPlayerFalling)
    {
        StartCoroutine(nameof(LerpYAction), isPlayerFalling);
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true; // Y 댐핑 값을 변경 중

        // grab the starting damping amount 
        float startDampAmount = _framingTransposer.m_YDamping;
        float endDampAmount = 0f;

        // grab the end damping amount 
        if (isPlayerFalling)
        {
            endDampAmount = _fallPanAmount;
            LerpedFromPlayerFalling = true;
        }
        else
        {
            endDampAmount = _normYPanAmount;
        }

        // lerp the pan amount
        float elapsedTime = 0f;
        while (elapsedTime < _fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime / _fallYPanTime));
            _framingTransposer.m_YDamping = lerpedPanAmount;

            yield return null;
        }
        IsLerpingYDamping = false; // 변경 끝
    }
    #endregion

    #region PAN CAMERA
    public void PanCameraOnContanct(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }

    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    // 이동할 거리, 이동하는데 걸리는 시간, 이동 방향, 시작 위치로 돌아갈까?
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        // handle pan from trigger
        if (!panToStartingPos) // 카메라가 이동하는 경우
        {
            // set the direction and distance
            switch (panDirection) // 이동 방향 설정
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case PanDirection.Left:
                    endPos = Vector2.right;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.left;
                    break;
                default:
                    break;
            }
            endPos *= panDistance; // endPos를 panDistance만큼 확장

            startingPos = _startingTrackedObjectOffset;

            endPos += startingPos;
        }

        // handle the pan back to starting position
        else // 시작 위치로 돌아가는 경우
        {
            startingPos = _framingTransposer.m_TrackedObjectOffset;
            endPos = _startingTrackedObjectOffset;
        }

        // handle the actual panning of the camera
        float elapsedTime = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;

            Vector3 panLerp = Vector3.Lerp(startingPos, endPos, (elapsedTime / panTime));
            _framingTransposer.m_TrackedObjectOffset = panLerp;

            yield return null;
        }
    }
    #endregion

    #region SWAP CAMERAS
    public void SwapCamera(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, Vector2 triggerExitDirection) // 좌 우 설정(위 아래도 가능)
    {
        // if the current camera is the camera on the left and our trigger exit direction was on the right
        // 현재 카메라가 왼쪽에 있고 출구의 방향이 오른쪽에 있는 경우
        if (_currentCamera == cameraFromLeft && triggerExitDirection.x > 0f)
        {
            // activate the new camera (키메라의 우선 순위를 번경하여 동일하게 작업할 수 있다)
            cameraFromRight.enabled = true;

            // deactivate the old camera
            cameraFromLeft.enabled = false;

            // set the new camera as the current camera
            _currentCamera = cameraFromRight;

            // update our composer variable
            _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        // if the current camera is the camera on the cright and our trigger hit direction was on the left
        else if (_currentCamera == cameraFromRight && triggerExitDirection.x < 0f)
        {
            // activate the new camera (키메라의 우선 순위를 번경하여 동일하게 작업할 수 있다)
            cameraFromLeft.enabled = true;

            // deactivate the old camera
            cameraFromRight.enabled = false;

            // set the new camera as the current camera
            _currentCamera = cameraFromLeft;

            // update our composer variable
            _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }
    #endregion
}