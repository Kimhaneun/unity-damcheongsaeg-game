using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShakeManagement : GameManager<CameraShakeManagement>
{
    protected CameraShakeManagement() { }

    [SerializeField] private float _globalShakeForce = 1f;

    public void CameraShake(CinemachineImpulseSource impulseSource)
    {
        impulseSource.GenerateImpulseWithForce(_globalShakeForce);
    }
}
