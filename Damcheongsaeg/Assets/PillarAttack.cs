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
        Animator = GetComponent<Animator>(); // start엣  클립이 잘 잇는지 확인 
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
        if (FidoranceMovement.IsJumping /*떨어지는 중이 아니라면 && 바닥에 있는 상태가 아니라면*/) // 하 공격
        {
            TakeDownAttackBegin();
        }
        //else if (Input.GetKey(KeyCode.W) && !PlayerMovement.IsDashing) // 상 공격
        //{
        //    Attack();
        //    Animator.SetTrigger("isUpAttacking");
        //}
        //else
        //{
        //    Attack();
        //    Animator.SetTrigger("isLRAttacking"); // 좌 우 공격
        //}
        // }
        #endregion
    }

    public void TakeDownAttackBegin()
    {
        // IsTakeDownAttackBegin = true 이면
        Animator.SetTrigger("isTakeDownAttacking");

        // 애니메이션 길이
        float animationLength = AnimationClip.length;

        // 애니메이션 속도 계산
        float animationSpeed = animationLength / FidoranceMovement.Data.jumpTimeToApex;

        // 애니메이션 속도 설정
        Animator.SetFloat("Speed", animationSpeed);
    }

    public void TakeDownAttacking()
    {
        Debug.Log("쾅");


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
    //        Debug.Log("피격!");
    //        iDamageble.Damage(attackDamageAmount);
    //    }
    //}
}
