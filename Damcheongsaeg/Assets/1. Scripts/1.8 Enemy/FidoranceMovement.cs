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
    public bool IsRefilling { get; private set; } // � ���°� ��� ���� ���� �ð� ��� �����ְ� �ϱ� ���� ����� �����̶� �ٸ� �װ� �ƴ� ��¼�� ��� ������
    public bool IsTakeDownAttacking { get; private set; } //

    // Timers
    public float IsJumpingTime { get; private set; }

    #endregion

    #region CHECK PARAMETERS
    // Inspector settings
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [Tooltip("groundCheck�� ũ��� Player�� ũ�⺸�� �ణ ���� ���� ����.")]
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
        // ���� �Ʒ� ������ �����ϴٸ�
        // �Ʒ� ���� �̺�Ʈ�� ��ٸ������� �ɾ�α�
        // ������ noting �ִٰ� �ٸ� ���·� ����
        #region JUMP CHECKS
        if (IsJumping && RB.velocity.y < 0)
        {
            IsJumping = false; // �������� �� �̶��
        }
        #endregion

        #region GRAVITY
        if (IsJumping && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
        {
            SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
        }
        else if (RB.velocity.y < 0)
        {
            // ���Ͻ� �߷°� ���� 
            SetGravityScale(Data.gravityScale * Data.fallGravityMult);
            // ���� �� �ִ� ���� �ӵ��� ����
            RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
        }
        else
        {
            // �⺻ �߷°�
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
        IsJumping = true; // ������ ĵ ������ �־�� �ϴµ� player���� �ٶ� 
                          // ê ����Ƽ ��� ���� -------------------------------------------------

        PillarAttack.IsTakeDownAttackBegin = true; //

        #region Perform Jump
        // ������ �� ����Ǵ� �߷°��� ������Ŵ
        // �̰��� �츮�� �׻� ���� �縸ŭ �����ϴ� ��ó�� ���� ������ �ǹ��մϴ�.
        // (�̸� �÷��̾��� Y �ӵ��� 0���� �����ϴ°Ͱ� ����)
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
        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer)) // OverlapBox�� ground�� ��ġ�°�?
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