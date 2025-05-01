using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : GameManager<PlayerMovement>
{
    protected PlayerMovement() { } // �ܺο��� �ν��Ͻ� ������ �����ϰ� �̱��� ������ �����ϱ� ���� ��� 

    public MovementData Data;

    #region COMPONENTS
    public Rigidbody2D RB { get; private set; }
    public PlayerAnimator PlayerAnimator { get; private set; }
    private Knockback Knockback;
    #endregion

    #region STATE PARAMETERS
    public bool IsFacingRight { get; private set; }
    public bool IsDashing { get; private set; }

    // Timers
    public float LastOnGroundTime { get; private set; }
    private float _lastOnWallTime;
    private float _lastOnWallRightTime;
    private float _lastOnWallLeftTime;

    // Jump
    private bool _isJumping;
    private bool _isJumpFalling;

    // Dash
    private int _dashesLeft;
    private bool _dashRefilling;
    private Vector2 _lastDashDir;
    private bool _isDashAttacking;

    // Camera
    private float _fallSpeedYDampingChangeThreshold;
    #endregion

    #region INPUT PARAMETERS
    public Vector2 _moveInput;

    private float _lastPressedJumpTime;
    private float _lastPressedDashTime;
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

    #region CAMERA FOLLOW
    [Header("Camera Follow")]
    [SerializeField] private GameObject CameraFollow;
    private CameraFollowObject CameraFollowObject;
    #endregion

    [SerializeField] float _rayMexDistance = 5f; // ���� ����

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        PlayerAnimator = GetComponentInChildren<PlayerAnimator>();
        CameraFollowObject = CameraFollow.GetComponent<CameraFollowObject>();
        Knockback = GetComponent<Knockback>();

        _fallSpeedYDampingChangeThreshold = CameraManagement.Instance._fallSpeedYDampingChangeThreshold;
    }

    private void Start()
    {
        SetGravityScale(Data.gravityScale);
        IsFacingRight = true;
    }

    private void FixedUpdate()
    {
        // Handle Run
        if (!IsDashing && !Knockback.IsBegingKnockedBack)
        {
            Run(1);
        }
        else if (_isDashAttacking && !Knockback.IsBegingKnockedBack)
        {
            Run(Data.dashEndRunLerp);
        }
    }

    private void Update()
    {
        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;
        _lastOnWallTime -= Time.deltaTime;
        _lastOnWallRightTime -= Time.deltaTime;
        _lastOnWallLeftTime -= Time.deltaTime;

        _lastPressedJumpTime -= Time.deltaTime;
        _lastPressedDashTime -= Time.deltaTime;
        #endregion

        #region INPUT HANDLER
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");

        if (_moveInput.x != 0)
            CheckDirectionToFace(_moveInput.x > 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpInput();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            OnDashInput();
        }
        #endregion

        #region COLLISION CHECKS
        if (!IsDashing && !_isJumping)
        {
            // Ground Check
            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer)) // OverlapBox�� ground�� ��ġ�°�?
            {
                if (LastOnGroundTime < -0.1f)
                    PlayerAnimator.startedJumping = true;
                LastOnGroundTime = Data.coyoteTime;
            }

            // Right Wall Check
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
                    || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)))
                _lastOnWallRightTime = Data.coyoteTime;

            // Left Wall Check
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)
                || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)))
                _lastOnWallLeftTime = Data.coyoteTime;

            _lastOnWallTime = Mathf.Max(_lastOnWallLeftTime, _lastOnWallRightTime);
        }
        #endregion

        #region JUMP CHECKS
        if (_isJumping && RB.velocity.y < 0)
        {
            _isJumping = false;

            _isJumpFalling = true;
        }

        if (LastOnGroundTime > 0 && !_isJumping)
        {
            _isJumpFalling = false;
        }

        if (!IsDashing)
        {
            // Jump
            if (CanJump() && _lastPressedJumpTime > 0 && !Knockback.IsBegingKnockedBack)
            {
                _isJumping = true;
                _isJumpFalling = false;
                Jump();

                PlayerAnimator.startedJumping = true;
            }
        }
        #endregion

        #region DASH CHECKS
        if (CanDash() && _lastPressedDashTime > 0 && !Knockback.IsBegingKnockedBack)
        {
            // ��� ���� �ð��� ���� ��̸� �ش�
            Sleep(Data.dashSleepTime);

            // ����Ű�� ������ �ʴ� ���¶�� �ٶ󺸴� �������� Dash
            // nameof() ǥ����� �ش� �ڷ�ƾ�� �̸��� �ڵ����� ������ ���ڿ��� ���� �Է��� �ʿ䰡 ������ �ǹ�
            // �ڷ�ƾ �̸��� �ϵ� �ڵ��ϴ� ��� ö�� ���� ���ɼ��� �����ϰ� ��Ÿ�� ���縵 ������ �����ϰ� ���� �޽����� �ִ� ��� �̸� �����Ѵ�.
            // performsleep �ڷ�ƾ �ȿ� duration �ð� ���� ��ٸ� �� �ڷ�ƾ�� �����Ѵ�.

            //if (_moveInput != Vector2.zero)
            //    _lastDashDir = _moveInput;
            //else
            //    _lastDashDir = IsFacingRight ? Vector2.right : Vector2.left; // ������ dash

            _lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;

            IsDashing = true;
            _isJumping = false;

            StartCoroutine(nameof(StartDash), _lastDashDir);
        }
        #endregion

        #region GRAVITY
        if (!_isDashAttacking)
        {
            if (RB.velocity.y < 0 && _moveInput.y < 0)
            {
                // ���� ���ϸ� ���� �߷°� ���
                SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
                //  ���� ���ϸ� �ص� �ִ� ���� �ӵ��� ����
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFastFallSpeed));
            }
            else if ((_isJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
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
            // Camera
            if (RB.velocity.y < _fallSpeedYDampingChangeThreshold && !CameraManagement.Instance.IsLerpingYDamping && !CameraManagement.Instance.LerpedFromPlayerFalling)
            {
                CameraManagement.Instance.LerpYDamping(true);
            }
            if (RB.velocity.y >= 0f && !CameraManagement.Instance.IsLerpingYDamping && CameraManagement.Instance.LerpedFromPlayerFalling)
            {
                CameraManagement.Instance.LerpedFromPlayerFalling = false;

                CameraManagement.Instance.LerpYDamping(false);
            }
            else
            {
                // �⺻ �߷°�
                SetGravityScale(Data.gravityScale);
            }
        }
        else
        {
            // ���� �� �߷°� 0(�ʱ� ���� ���� �ܰ谡 ������ ���ƿ�)
            SetGravityScale(0);
        }
        #endregion
    }

    #region INPUT CALLBACKS
    public void OnJumpInput()
    {
        _lastPressedJumpTime = Data.jumpInputBufferTime;
    }

    public void OnDashInput()
    {
        _lastPressedDashTime = Data.dashInputBufferTime;
    }
    #endregion

    #region GENERAL METHODS
    public void SetGravityScale(float scale)
    {
        RB.gravityScale = scale;
    }

    private void Sleep(float duration)
    {
        // nameof() ǥ����� �ش� �ڷ�ƾ�� �̸��� �ڵ����� ������ ���ڿ��� ���� �Է��� �ʿ䰡 ������ �ǹ�
        // �ڷ�ƾ �̸��� �ϵ� �ڵ��ϴ� ��� ö�� ���� ���ɼ��� �����ϰ� ��Ÿ�� ���縵 ������ �����ϰ� ���� �޽����� �ִ� ��� ����.
        // performsleep �ڷ�ƾ �ȿ� duration �ð� ���� ��ٸ� �� �ڷ�ƾ�� ������.
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0; // ���� �ð� ���� 
        yield return new WaitForSecondsRealtime(duration);  // duration �ð����� ��ٷ��� 
        Time.timeScale = 1; // �ð� ���� ����
    }
    #endregion

    //MOVEMENT METHODS
    #region RUN METHODS
    private void Run(float lerpAmount)
    {
        // �̵� ����� �ӵ� ���
        float targetSpeed = _moveInput.x * Data.runMaxSpeed;
        // �̵� ����� �ӵ� ����
        targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float accelRate;

        // ���� ���ο� ���� ���ӵ� ���� �����ɴϴ�(ȸ�� ����).
        // �Ǵ� ������ �õ��մϴ�(����). ���� ���߿� �� �ִ� ��� �߷��� �����մϴ�.
        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        #endregion

        #region Add Bonus Jump Apex Acceleration
        // �ִ� ���� ���̿� �����Ͽ��� �� ���ӵ��� �ִ� �ӵ��� �����Ͽ� ������ �� �� ź�� �ְ� �������� �پ�� �ڿ������� ��������
        if ((_isJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }
        #endregion

        #region Conserve Momentum
        // �÷��̾ �Է� �������� �����̰�, maxSpeed���� ���� �ӵ��� �����̴� ��� �ڿ������� �÷��̸� ���� �÷��̾��� �ӵ��� ������ �ʴ´�.
        if (Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            // ������ �߻��ϴ� ���� �����մϴ�. ��, ���� ����� �����մϴ�.
            accelRate = 0;
        }
        #endregion

        // ���� �ӵ��� ���ϴ� �ӵ��� ���̸� ����մϴ�.
        float speedDif = targetSpeed - RB.velocity.x;

        // �̵� �� ���
        float movement = speedDif * accelRate;

        // ���� vector������ ��ȯ���� ����
        RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

        //AddForce�� �ϴ� ��:
        //*RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime * speedDif * accelRate) / RB.mass, RB.velocity.y);
        //*Time.fixedDeltaTime�� �⺻������ Unity 0.02�ʷ�, �ʴ� 50���� FixUpdate() ȣ��� ����.
    }

    private void Turn()
    {
        // Vector3 scale = transform.localScale;
        // scale.x *= -1;
        // transform.localScale = scale;

        if (IsFacingRight)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);

            IsFacingRight = !IsFacingRight;

            CameraFollowObject.CallTurn();
        }

        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);

            IsFacingRight = !IsFacingRight;

            CameraFollowObject.CallTurn();
        }
    }
    #endregion

    #region JUMP METHODS
    private void Jump()
    {
        // �� ���� �������� Jump�� ���� �� ȣ���� �� ������ ����
        _lastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        #region Perform Jump
        // ������ �� ����Ǵ� �߷°��� ������Ŵ
        // �̰��� �츮�� �׻� ���� �縸ŭ �����ϴ� ��ó�� ���� ������ �ǹ��մϴ�.
        // (�̸� �÷��̾��� Y �ӵ��� 0���� �����ϴ°Ͱ� ����)
        float force = Data.jumpForce;
        if (RB.velocity.y < 0)
            force -= RB.velocity.y;

        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        #endregion
    }
    #endregion

    #region DASH METHODS
    //Dash Coroutine
    private IEnumerator StartDash(Vector2 dir)
    {
        LastOnGroundTime = 0;
        _lastPressedDashTime = 0;

        float startTime = Time.time;

        _dashesLeft--;
        _isDashAttacking = true;

        SetGravityScale(0);

        // ���� �ܰ� ���� �÷��̾��� �ӵ��� ��� �ӵ��� ����
        while (Time.time - startTime <= Data.dashAttackTime)
        {
            RB.velocity = dir.normalized * Data.dashSpeed;
            // ���� �����ӱ��� ������ �Ͻ� �����Ͽ� ������Ʈ ������ ����.
            // �̰��� ���� Ÿ�̸ӿ� �ݴ�Ǵ� ���� ����� ����
            yield return null;
        }

        startTime = Time.time;

        _isDashAttacking = false;

        SetGravityScale(Data.gravityScale);
        RB.velocity = Data.dashEndSpeed * dir.normalized;

        while (Time.time - startTime <= Data.dashEndTime)
        {
            yield return null;
        }

        //Dash over
        IsDashing = false;
    }

    private IEnumerator RefillDash()
    {
        _dashRefilling = true;
        yield return new WaitForSeconds(Data.dashRefillTime);
        _dashRefilling = false;
        _dashesLeft = Mathf.Min(Data.dashAmount, _dashesLeft + 1);
    }
    #endregion

    #region CHECK METHODS
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }

    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !_isJumping;
    }

    private bool CanDash()
    {
        if (!IsDashing && _dashesLeft < Data.dashAmount && LastOnGroundTime > 0 && !_dashRefilling)
        {
            StartCoroutine(nameof(RefillDash), 1);
        }

        return _dashesLeft > 0;
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

    // ---------------------------------------------------------------------


    //void OnFireRay()
    //   {
    //       Ray ray = new Ray(transform.position, _moveDir);
    //       RaycastHit2D hitData = Physics2D.Raycast(ray.origin, ray.direction, _rayMexDistance); // ��ġ ���� ���� �Ÿ�
    //       Debug.DrawRay(ray.origin, ray.direction * _rayMexDistance, Color.green, 0.3f); // (ó�� ��ġ, ���� * �ִ� ����, ȭ�鿡 ���� ��, ���̴� �ð�)

    //       if (hitData.collider != null)
    //       {
    //           Vector2 hitPosition = hitData.point; // ���̰� ������ ��ġ
    //           float hitDistance = hitData.distance; // �������� ������ ��ġ������ �Ÿ�
    //       }
    //   }
}
