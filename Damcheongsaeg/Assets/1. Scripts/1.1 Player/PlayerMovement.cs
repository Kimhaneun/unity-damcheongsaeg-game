using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : GameManager<PlayerMovement>
{
    protected PlayerMovement() { } // 외부에서 인스턴스 생성을 제한하고 싱글톤 패턴을 유지하기 위해 사용 

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

    #region CAMERA FOLLOW
    [Header("Camera Follow")]
    [SerializeField] private GameObject CameraFollow;
    private CameraFollowObject CameraFollowObject;
    #endregion

    [SerializeField] float _rayMexDistance = 5f; // 레이 길이

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
            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer)) // OverlapBox가 ground에 겹치는가?
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
            // 잠시 게임 시간을 멈춰 재미를 준다
            Sleep(Data.dashSleepTime);

            // 방향키를 누르지 않는 상태라면 바라보는 방향으로 Dash
            // nameof() 표기법은 해당 코루틴의 이름을 자동으로 가져와 문자열을 직접 입력할 필요가 없음을 의미
            // 코루틴 이름을 하드 코딩하는 대신 철자 오류 가능성을 제거하고 오타나 스펠링 오류를 방지하고 오류 메시지가 있는 경우 이를 개선한다.
            // performsleep 코루틴 안에 duration 시간 동안 기다린 뒤 코루틴을 시작한다.

            //if (_moveInput != Vector2.zero)
            //    _lastDashDir = _moveInput;
            //else
            //    _lastDashDir = IsFacingRight ? Vector2.right : Vector2.left; // 원래의 dash

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
                // 빠른 낙하를 위한 중력값 계산
                SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
                //  빠른 낙하를 해도 최대 낙하 속도를 제한
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFastFallSpeed));
            }
            else if ((_isJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
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
                // 기본 중력값
                SetGravityScale(Data.gravityScale);
            }
        }
        else
        {
            // 돌진 시 중력값 0(초기 돌진 공격 단계가 끝나면 돌아옴)
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
        // nameof() 표기법은 해당 코루틴의 이름을 자동으로 가져와 문자열을 직접 입력할 필요가 없음을 의미
        // 코루틴 이름을 하드 코딩하는 대신 철자 오류 가능성을 제거하고 오타나 스펠링 오류를 방지하고 오류 메시지가 있는 경우 개선.
        // performsleep 코루틴 안에 duration 시간 동안 기다린 뒤 코루틴을 시작함.
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0; // 게임 시간 정지 
        yield return new WaitForSecondsRealtime(duration);  // duration 시간동안 기다려서 
        Time.timeScale = 1; // 시간 원상 복구
    }
    #endregion

    //MOVEMENT METHODS
    #region RUN METHODS
    private void Run(float lerpAmount)
    {
        // 이동 방향과 속도 계산
        float targetSpeed = _moveInput.x * Data.runMaxSpeed;
        // 이동 방향과 속도 보간
        targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float accelRate;

        // 가속 여부에 따라 가속도 값을 가져옵니다(회전 포함).
        // 또는 감속을 시도합니다(정지). 또한 공중에 떠 있는 경우 중력을 적용합니다.
        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        #endregion

        #region Add Bonus Jump Apex Acceleration
        // 최대 점프 높이에 도달하였을 때 가속도와 최대 속도가 증가하여 점프가 좀 더 탄력 있고 반응성이 뛰어나며 자연스럽게 느껴진다
        if ((_isJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }
        #endregion

        #region Conserve Momentum
        // 플레이어가 입력 방향으로 움직이고, maxSpeed보다 빠른 속도로 움직이는 경우 자연스러운 플레이를 위해 플레이어의 속도를 늦추지 않는다.
        if (Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            // 감속이 발생하는 것을 방지합니다. 즉, 현재 운동량을 보존합니다.
            accelRate = 0;
        }
        #endregion

        // 현재 속도와 원하는 속도의 차이를 계산합니다.
        float speedDif = targetSpeed - RB.velocity.x;

        // 이동 값 계산
        float movement = speedDif * accelRate;

        // 값을 vector값으로 변환시켜 더함
        RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

        //AddForce가 하는 것:
        //*RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime * speedDif * accelRate) / RB.mass, RB.velocity.y);
        //*Time.fixedDeltaTime은 기본적으로 Unity 0.02초로, 초당 50개의 FixUpdate() 호출과 같다.
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
        // 한 번의 누름으로 Jump를 여러 번 호출할 수 없도록 제한
        _lastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        #region Perform Jump
        // 떨어질 때 적용되는 중력값을 증가시킴
        // 이것은 우리가 항상 같은 양만큼 점프하는 것처럼 느낄 것임을 의미합니다.
        // (미리 플레이어의 Y 속도를 0으로 설정하는것과 같다)
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

        // 공격 단계 동안 플레이어의 속도를 대시 속도로 유지
        while (Time.time - startTime <= Data.dashAttackTime)
        {
            RB.velocity = dir.normalized * Data.dashSpeed;
            // 다음 프레임까지 루프를 일시 중지하여 업데이트 루프를 생성.
            // 이것은 다중 타이머와 반대되는 보다 깔끔한 구현
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
    //       RaycastHit2D hitData = Physics2D.Raycast(ray.origin, ray.direction, _rayMexDistance); // 위치 방향 정보 거리
    //       Debug.DrawRay(ray.origin, ray.direction * _rayMexDistance, Color.green, 0.3f); // (처음 위치, 방향 * 최대 길이, 화면에 보일 색, 보이는 시간)

    //       if (hitData.collider != null)
    //       {
    //           Vector2 hitPosition = hitData.point; // 레이가 감지된 위치
    //           float hitDistance = hitData.distance; // 원점에서 감지된 위치까지의 거리
    //       }
    //   }
}
