using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G379Movement : MonoBehaviour
{
    #region COMPONENTS
    public Rigidbody2D RB { get; private set; }
    #endregion

    #region STATE PARAMETERS
    public bool IsFacingRight { get; private set; }
    #endregion

    [Header("Run")]
    [SerializeField] private float runSpeed;

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

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        IsFacingRight = true;
    }

    private void FixedUpdate()
    {
        if (IsGounded())
        {
            Run();
        }

        if (!IsGounded() || IsRightWalled())
        {
            Turn();
        }
    }

    //MOVEMENT METHODS
    #region RUN METHODS
    private void Run()
    {
        Vector2 moveDirection = IsFacingRight ? Vector2.right : Vector2.left;
        RB.velocity = moveDirection * runSpeed * Time.deltaTime;
    }

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
        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
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
