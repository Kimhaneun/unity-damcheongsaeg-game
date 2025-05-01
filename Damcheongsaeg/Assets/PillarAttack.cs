using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarAttack : MonoBehaviour
{
    #region COMPONENTS
    public BoxCollider2D BoxCollider2D { get; private set; }
    public Animator Animator { get; private set; }
    public FidoranceMovement FidoranceMovement { get; private set; }
    #endregion

    #region STATE PARAMETERS
    private bool _attackRefilling;

    public bool IsTakeDownAttackBegin { get; set; } //
    public AnimationClip AnimationClip; //

    [SerializeField] private float attackSpeed;
    #endregion

    private void Awake()
    {
        BoxCollider2D = GetComponent<BoxCollider2D>();
        Animator = GetComponent<Animator>(); // start��  Ŭ���� �� �մ��� Ȯ�� 
        FidoranceMovement = GetComponentInParent<FidoranceMovement>();
    }


    private void Update()
    {
        #region TIMERS
        Animator.SetFloat("attackSpeed", attackSpeed);
        #endregion

        #region INPUT HANDLER
        //if (Input.GetMouseButtonDown(0) && CanAttack())
        //{
        if (FidoranceMovement.IsJumping /*�������� ���� �ƴ϶�� && �ٴڿ� �ִ� ���°� �ƴ϶��*/) // �� ����
        {
            TakeDownAttackBegin();
        }
        //else if (Input.GetKey(KeyCode.W) && !PlayerMovement.IsDashing) // �� ����
        //{
        //    Attack();
        //    Animator.SetTrigger("isUpAttacking");
        //}
        //else
        //{
        //    Attack();
        //    Animator.SetTrigger("isLRAttacking"); // �� �� ����
        //}
        // }
        #endregion
    }

    public void TakeDownAttackBegin()
    {
        // IsTakeDownAttackBegin = true �̸�
        Animator.SetTrigger("isTakeDownAttacking");

        // �ִϸ��̼� ����
        float animationLength = AnimationClip.length;

        // �ִϸ��̼� �ӵ� ���
        float animationSpeed = animationLength / FidoranceMovement.Data.jumpTimeToApex;

        // �ִϸ��̼� �ӵ� ����
        Animator.SetFloat("Speed", animationSpeed);
    }

    public void TakeDownAttacking()
    {
        Debug.Log("��");


    }

    //private void Attack()
    //{
    //    StartCoroutine(nameof(RefillAttack));
    //}

    //private bool CanAttack()
    //{
    //    if (!_attackRefilling)
    //        return true;

    //    else return false;
    //}

    //public IEnumerator RefillAttack()
    //{
    //    _attackRefilling = true;
    //    yield return new WaitForSeconds(attackRefillTime);
    //    _attackRefilling = false;
    //}


    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    IDamageble iDamageble = collision.gameObject.GetComponent<IDamageble>();

    //    if (iDamageble != null && collision.gameObject.layer == LayerMask.NameToLayer("PLAYER"))
    //    {
    //        Debug.Log("�ǰ�!");
    //        iDamageble.Damage(attackDamageAmount);
    //    }
    //}
}
