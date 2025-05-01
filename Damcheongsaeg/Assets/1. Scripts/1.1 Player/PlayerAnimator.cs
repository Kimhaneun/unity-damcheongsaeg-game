using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private PlayerMovement PlayerMovement;
    private SpriteRenderer SpriteRenderer;
    private Animator Animator;

    [Header("Movement Tilt")]
    [SerializeField] private float _maxTilt;
    [SerializeField] [Range(0, 1)] private float _tiltSpeed;

    [Header("Particle FX")]
    [SerializeField] private GameObject jumpFX;
    [SerializeField] private GameObject landFX;
    private ParticleSystem _jumpParticle;
    private ParticleSystem _landParticle;
    public bool startedJumping { private get; set; }
    public bool justLanded { private get; set; }

    public float currentVelY;

    private void Awake()
    {
        PlayerMovement = GetComponentInParent<PlayerMovement>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();

        _jumpParticle = jumpFX.GetComponent<ParticleSystem>();
        _landParticle = landFX.GetComponent<ParticleSystem>();
    }

    private void LateUpdate()
    {
        float tiltProgress;
        int mult = -1;

        tiltProgress = Mathf.InverseLerp(-PlayerMovement.Data.runMaxSpeed, PlayerMovement.Data.runMaxSpeed, PlayerMovement.RB.velocity.x);
        mult = (PlayerMovement.IsFacingRight) ? -1 : 1;

        float newRot = ((tiltProgress * _maxTilt * 2) - _maxTilt);
        float rot = Mathf.LerpAngle(SpriteRenderer.transform.localRotation.eulerAngles.z * mult, newRot, _tiltSpeed);
        SpriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, rot * mult);

        CheckAnimationState();
    }

    private void CheckAnimationState()
    {
        if (startedJumping)
        {
            Animator.SetTrigger("Jump");
            GameObject obj = Instantiate(jumpFX, transform.position - (Vector3.up * transform.localScale.y / 2), Quaternion.Euler(-90, 0, 0));
            Destroy(obj, 1);
            startedJumping = false;
            return;
        }

        if (justLanded)
        {
            Animator.SetTrigger("Land");
            GameObject obj = Instantiate(landFX, transform.position - (Vector3.up * transform.localScale.y / 1.5f), Quaternion.Euler(-90, 0, 0));
            Destroy(obj, 1);
            justLanded = false;
            return;
        }

        Animator.SetFloat("Vel Y", PlayerMovement.RB.velocity.y);
    }
}
