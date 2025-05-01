using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPC : MonoBehaviour, IInteractable // abstract: 공개 추상 클래스
{
    [SerializeField] private SpriteRenderer _interactSprite; // 대화 활성화 표시 이미지
    private Transform _playerTransform;

    #region STATE PARAMETERS
    public GameObject Player { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool CanInteract { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    #endregion

    #region STATE CONSTANT
    private const float INTERACT_DISTANCE = 5F;
    #endregion

    private void Awake()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        #region INPUT HANDLER
        if (Input.GetKeyDown(KeyCode.F) && CanWithinInteractDistance())
        {
            // interact
            OnInteractInput();
        }
        #endregion

        #region INTERACT CHECKS
        if (_interactSprite.gameObject.activeSelf && !CanWithinInteractDistance())
        {
            // 상호 작용 불가
            // turn off the sprite
            _interactSprite.gameObject.SetActive(false);
        }
        else if (!_interactSprite.gameObject.activeSelf && CanWithinInteractDistance())
        {
            // 만약 범위 안에 들어와 있다면
            // 상호 작용 가능
            // turn off the sprite
            _interactSprite.gameObject.SetActive(true);
        }
        #endregion
    }

    public abstract void OnInteractInput();
    
    #region CHECK METHODS
    private bool CanWithinInteractDistance()
    {
        if (Vector2.Distance(_playerTransform.position, transform.position) < INTERACT_DISTANCE) // 가로로 직선 필요의 따라 원형이나 바꿔야 할 듯
            return true;
        else
            return false;
    }
    #endregion
}
