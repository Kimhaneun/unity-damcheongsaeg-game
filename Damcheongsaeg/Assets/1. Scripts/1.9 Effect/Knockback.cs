using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    #region COMPONENTS
    private Rigidbody2D RB;
    #endregion

    #region STATE PARAMETERS
    public bool IsBegingKnockedBack { get; private set; }
    #endregion

    #region KNOCKBACK 
    [Header("Knockback")]
    [SerializeField] private float _knockbackTime;
    [SerializeField] private float _hitDirectionForce;
    [SerializeField] private float _constForce;
    [SerializeField] private float _inputForce;

    [SerializeField] private AnimationCurve KnockbackForceCurve;
    #endregion

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    public void StartKnockback(Vector2 hitDirection, Vector2 constantForceDirection, float inputDirection)
    {
        StartCoroutine(KnockbackAction(hitDirection, constantForceDirection, inputDirection));
    }

    private IEnumerator KnockbackAction(Vector2 hitDirection, Vector2 constantForceDirection, float inputDirection)
    {
        IsBegingKnockedBack = true;

        Vector2 hitForce;
        Vector2 constantForce;
        Vector2 knockbackForce;
        Vector2 combinedForce;

        float time = 0f;

        constantForce = constantForceDirection * _constForce;

        float elapsedTime = 0f;
        while (elapsedTime < _knockbackTime)
        {
            // Iterate the timer
            elapsedTime += Time.fixedDeltaTime;
            time += Time.fixedDeltaTime;

            // Update hitForce
            hitForce = hitDirection * _hitDirectionForce * KnockbackForceCurve.Evaluate(time);

            // Combine hitForce and constantForce
            knockbackForce = hitForce + constantForce;

            // Comnine knockback forec with input force
            if (inputDirection != 0)
            {
                combinedForce = knockbackForce + new Vector2(inputDirection * _inputForce, 0f);
            }
            else
            {
                combinedForce = knockbackForce;
            }

            // Apply knockback
            RB.velocity = combinedForce;

            yield return new WaitForFixedUpdate();
        }
        IsBegingKnockedBack = false;
    }
}
