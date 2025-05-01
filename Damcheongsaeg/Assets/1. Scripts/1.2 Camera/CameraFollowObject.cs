using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    #region COMPONENTS
    [SerializeField] private PlayerMovement PlayerMovement;
    #endregion

    [Header("References")]
    [SerializeField] private Transform PlayerTransform;

    [Header("Flip Rotation Stats")]
    [SerializeField] private float _flipYRotationTime;

    public bool IsFacingRight { get; private set; }

    private void Awake()
    {
        PlayerMovement = PlayerMovement.transform.gameObject.GetComponent<PlayerMovement>();
        IsFacingRight = PlayerMovement.IsFacingRight;
    }

    private void Update()
    {
        transform.position = PlayerMovement.transform.position;
    }

    public void CallTurn()
    {
        // 规过 1.
        StartCoroutine(nameof(FlipYLerp));

        // 规过 2.
        // LeanTween.rotateY(gameObject, DetermineEndRotation(), _flipYRotationTime).setEaseInOutSine();
    }

    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotationAmount = DetermineEndRotation();
        float yRotation = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < _flipYRotationTime)
        {
            elapsedTime += Time.deltaTime;

            yRotation = Mathf.Lerp(startRotation, endRotationAmount, (elapsedTime / _flipYRotationTime));
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

            yield return null;
        }
    }

    private float DetermineEndRotation()
    {
        IsFacingRight = !IsFacingRight;
        if (IsFacingRight)
            return 180f;
        else
            return 0f;
    }
}
