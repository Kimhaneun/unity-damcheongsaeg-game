using UnityEngine;

[CreateAssetMenu(menuName = "Movement Data")]
public class MovementData : ScriptableObject
{
	[Header("Gravity")]
	[HideInInspector] public float gravityStrength; 
	[HideInInspector] public float gravityScale;
										 
	[Space(5)]
	public float fallGravityMult;
	[Tooltip("최대 낙하 속도")]
	public float maxFallSpeed;
	[Space(5)]
	public float fastFallGravityMult;
	[Tooltip("최대 빠른 낙하 속도")] // 빠른 낙하: 낙하 중 아래 키 입력시 중력 증가
	public float maxFastFallSpeed; 
	
	[Space(20)]

	[Header("Run")]
	[Tooltip("최대 이동 속도")]
	public float runMaxSpeed;
	public float runAcceleration; // 최대 이동 속도까지 도달하기 위해 가속하는 속도(0으로 설정 가능)
	[HideInInspector] public float runAccelAmount; // 가속하는 힘
	public float runDecceleration; // 정지하기 위한 감속하는 속도(0으로 설정 가능)
	[HideInInspector] public float runDeccelAmount; // 감속하는 힘
	[Space(5)]
	[Range(0f, 1)] public float accelInAir; 
	[Range(0f, 1)] public float deccelInAir;
	[Space(5)]
	public bool doConserveMomentum = true;

	[Space(20)]

	[Header("Jump")]
	public float jumpHeight; // 최대 점프 높이
	public float jumpTimeToApex; // 점프 높이까지 도달하는 시간
	[HideInInspector] public float jumpForce; // 점프하는 힘
	[HideInInspector] public float horizontalJumpForce; // new! x방향으로 점프하는 힘

	[Header("Both Jumps")]
	[Range(0f, 1)] public float jumpHangGravityMult; // 최대 점프 높이에 도달하며 중력 크기 감소
	public float jumpHangTimeThreshold; 
	[Space(0.5f)]
	public float jumpHangAccelerationMult; 
	public float jumpHangMaxSpeedMult; 				

	[Space(20)]

    [Header("Assists")]
	[Range(0.01f, 0.5f)] public float coyoteTime; // 플랫폼에서 떨어진 후에도 점프할 수 있는 유예 시간
	[Range(0.01f, 0.5f)] public float jumpInputBufferTime; //Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.

	[Space(20)]

	[Header("Dash")]
	public int dashAmount;
	public float dashSpeed;
	public float dashSleepTime; // 플레이어가 땅에 닿는 순간/벽 점프를 하려는 경우 버튼을 정확하게 누르기 어려울 수 있으므로
								// 땅에 닿지 않았을 때 조금 있다가 점프하도록 하는 변수
	[Space(5)]
	public float dashAttackTime;
	[Space(5)]
	public float dashEndTime; // 대시 상태에서 원래 상태로 돌아오는데 걸리는 시간
	public Vector2 dashEndSpeed; // 플레이어의 속도를 늦춰, 대시의 반응성 증가
	[Range(0f, 1f)] public float dashEndRunLerp; // 대시 하는 동안 속도 감소
	[Space(5)]
	public float dashRefillTime;
	[Space(5)]
	[Range(0.01f, 0.5f)] public float dashInputBufferTime;

	// 인스펙터가 업데이트될 때 호출되는 Unity 콜백
	private void OnValidate()
    {
		//Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
		gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
		
		//Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
		gravityScale = gravityStrength / Physics2D.gravity.y;

		//Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
		runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
		runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

		//Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
		jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

		horizontalJumpForce = jumpForce / 2; // new! 값은 임시로 설정

		#region Variable Ranges
		runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
		runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
		#endregion
	}
}