using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FidoranceMovement : MonoBehaviour
{
    public MovementData Data;

    #region COMPONENTS
    public Rigidbody2D RB { get; private set; }

    public PillarAttack PillarAttack { get; private set; } //
    #endregion

    [Header("References")]
    [SerializeField] private Transform PlayerTransform;

    #region STATE PARAMETERS
    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsRefilling { get; private set; } // 어떤 상태건 상관 없이 공격 시간 잠시 멈춰주게 하기 위해 만든거 낫띵이랄 다름 그거 아님 어쩌면 없어도 될지도
    public bool IsTakeDownAttacking { get; private set; } //

    // Timers
    public float IsJumpingTime { get; private set; }

    #endregion

    #region CHECK PARAMETERS
    // Inspector settings
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [Tooltip("groundCheck의 크기는 Player의 크기보다 약간 작은 것이 좋다.")]
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [Space(5)]
    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    #endregion


    public bool canStatecahnge; // test;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        IsFacingRight = true;
    }

    private void Update()
    {
        // 만약 아래 공격이 가능하다면
        // 아래 공격 이벤트로 기다리느ㄴ거 걸어두기
        // 끝나면 noting 있다가 다른 상태로 전이
        #region JUMP CHECKS
        if (IsJumping && RB.velocity.y < 0)
        {
            IsJumping = false; // 떨어지는 중 이라면
        }
        #endregion

        #region GRAVITY
        if (IsJumping && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
        {
            SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
        }
        else if (RB.velocity.y < 0)
        {
            // 낙하시 중력값 증가 
            SetGravityScale(Data.gravityScale * Data.fallGravityMult);
            // 낙하 시 최대 낙하 속도를 제한
            RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
        }
        else
        {
            // 기본 중력값
            SetGravityScale(Data.gravityScale);
        }
        #endregion
    }

    IEnumerator StateSet()
    {
        if (canStatecahnge)
        {
            Jump();
            yield return null;
        }
    }

    #region GENERAL METHODS
    public void SetGravityScale(float scale)
    {
        RB.gravityScale = scale;
    }
    #endregion

    #region FACING
    private void Turn()
    {
        if (IsFacingRight)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);

            IsFacingRight = !IsFacingRight;
        }

        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);

            IsFacingRight = !IsFacingRight;
        }
    }
    #endregion

    #region JUMP METHODS
    private void Jump()
    {
        IsJumping = true; // 원래는 캔 점프에 넣어야 하는데 player참고 바람 
                          // 챗 지피티 기록 보기 -------------------------------------------------

        PillarAttack.IsTakeDownAttackBegin = true; //

        #region Perform Jump
        // 떨어질 때 적용되는 중력값을 증가시킴
        // 이것은 우리가 항상 같은 양만큼 점프하는 것처럼 느낄 것임을 의미합니다.
        // (미리 플레이어의 Y 속도를 0으로 설정하는것과 같다)
        float force = Data.jumpForce;
        float horizontalForce = Data.horizontalJumpForce;
        float direction = PlayerTransform.position.x < transform.position.x ? -1 : 1;
        if (RB.velocity.y < 0)
            force -= RB.velocity.y;

        RB.AddForce(new Vector2(horizontalForce * direction, force), ForceMode2D.Impulse);
        #endregion
    }
    #endregion

   

    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
    }
    #endregion

    #region COLLISION CHECKS
    private bool IsGounded()
    {
        // Ground Check
        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer)) // OverlapBox가 ground에 겹치는가?
            return true;
        else
            return false;
    }

    private bool IsLeftWalled()
    {
        // Left Wall Check
        if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)
            || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)))
            return true;
        else
            return false;
    }

    private bool IsRightWalled()
    {
        // Right Wall Check
        if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
                || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)))
            return true;
        else
            return false;
    }
    #endregion

   
}