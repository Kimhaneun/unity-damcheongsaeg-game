using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    #region COMPONENTS
    private PolygonCollider2D PolygonCollider2D;
    private Animator Animator;
    private PlayerMovement PlayerMovement;
    #endregion

    #region STATE PARAMETERS
    private bool _attackRefilling;
    #endregion

    [Header("Player Attack")]
    [SerializeField] private float _attackRefillTime;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _attackDamageAmount;

    private void Awake()
    {
        PolygonCollider2D = GetComponent<PolygonCollider2D>();
        Animator = GetComponent<Animator>();
        PlayerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void Update()
    {
        #region TIMERS
        Animator.SetFloat("attackSpeed", _attackSpeed);
        #endregion

        #region INPUT HANDLER
        if (Input.GetMouseButtonDown(0) && CanAttack())
        {
            if (Input.GetKey(KeyCode.S) && PlayerMovement.LastOnGroundTime != 0.2f && !PlayerMovement.IsDashing) // 하 공격
            {
                Attack();
                Animator.SetTrigger("isDownAttacking");
            }
            else if (Input.GetKey(KeyCode.W) && !PlayerMovement.IsDashing) // 상 공격
            {
                Attack();
                Animator.SetTrigger("isUpAttacking");
            }
            else
            {
                Attack();
                Animator.SetTrigger("isLRAttacking"); // 좌 우 공격
            }
        }
        #endregion
    }

    private void Attack()
    {
        StartCoroutine(nameof(RefillAttack));
    }

    private bool CanAttack()
    {
        if (!_attackRefilling)
            return true;
        else return false;
    }

    public IEnumerator RefillAttack()
    {
        _attackRefilling = true;
        yield return new WaitForSeconds(_attackRefillTime);
        _attackRefilling = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageble iDamageble = collision.gameObject.GetComponent<IDamageble>();

        if (iDamageble != null && collision.gameObject.layer == LayerMask.NameToLayer("ENEMY"))
        {
            Vector2 hitDirection = collision.gameObject.transform.position - transform.position;
            hitDirection.Normalize();
            iDamageble.Damage(_attackDamageAmount, hitDirection);
        }
    }
    // 벽에 맞으면 밀려나게
    // 레이로 벽 감지 
    // 담청색 기린의 스킬 사용
    // 시간이 지나면 지날수록?? 적을 죽일수록?? 능력이 강해지고
    // 능력 사용시 ?? 분간 광폭화 상태
    // 이 후 다시 모아야 하는거지 기린 최고
}
