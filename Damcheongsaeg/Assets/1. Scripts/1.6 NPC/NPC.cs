using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPC : MonoBehaviour, IInteractable // abstract: ���� �߻� Ŭ����
{
    [SerializeField] private SpriteRenderer _interactSprite; // ��ȭ Ȱ��ȭ ǥ�� �̹���
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
            // ��ȣ �ۿ� �Ұ�
            // turn off the sprite
            _interactSprite.gameObject.SetActive(false);
        }
        else if (!_interactSprite.gameObject.activeSelf && CanWithinInteractDistance())
        {
            // ���� ���� �ȿ� ���� �ִٸ�
            // ��ȣ �ۿ� ����
            // turn off the sprite
            _interactSprite.gameObject.SetActive(true);
        }
        #endregion
    }

    public abstract void OnInteractInput();
    
    #region CHECK METHODS
    private bool CanWithinInteractDistance()
    {
        if (Vector2.Distance(_playerTransform.position, transform.position) < INTERACT_DISTANCE) // ���η� ���� �ʿ��� ���� �����̳� �ٲ�� �� ��
            return true;
        else
            return false;
    }
    #endregion
}
