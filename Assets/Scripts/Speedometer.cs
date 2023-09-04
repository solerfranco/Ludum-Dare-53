using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    private const float MAX_SPEED_ANGLE = -6;
    private const float ZERO_SPEED_ANGLE = 190;

    [SerializeField]
    private Transform _needleTransform;

    private float _speedMax = 340f;
    public float Speed;

    private void Update()
    {
        if (Speed > _speedMax) Speed = _speedMax;
        _needleTransform.eulerAngles = new Vector3(0, 0, GetSpeedRotation());
    }

    private float GetSpeedRotation()
    {
        float totalAngleSize = ZERO_SPEED_ANGLE - MAX_SPEED_ANGLE;

        float speedNormalized = Speed / _speedMax;

        return ZERO_SPEED_ANGLE - speedNormalized * totalAngleSize;
    }
}
