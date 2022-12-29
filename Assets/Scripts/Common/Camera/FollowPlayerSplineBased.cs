using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerSplineBased : MonoBehaviour
{
    public bool excludeX = false;
    public bool excludeY = false;
    public bool lookAt = false;
    public bool inverseTransform = true;
    public float cameraChangeLerpFactor = 0.2f;
    // Connections
    public Transform target;
    // State variables
    Vector3 targetLocalSpaceOffset;
    Vector3 targetLocalSpaceLookDirection;
    float currentOffsetMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 startOffsetVector = transform.position - target.position;
        targetLocalSpaceOffset = target.InverseTransformVector(startOffsetVector);
        targetLocalSpaceLookDirection = target.InverseTransformDirection(transform.forward);
        currentOffsetMultiplier = 1.0f;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        Vector3 targetWorldSpaceOffset = target.TransformVector(targetLocalSpaceOffset);
        Vector3 targetFollowPosition = target.position + currentOffsetMultiplier * targetWorldSpaceOffset;
        Vector3 lookDirection = target.TransformDirection(targetLocalSpaceLookDirection);

        transform.position = targetFollowPosition;
       // transform.position = Vector3.Lerp(transform.position, targetPosition, cameraChangeLerpFactor * Time.deltaTime);
        if(lookAt)
            transform.LookAt(transform.position + lookDirection);
    }

    public void SetOffsetMultiplier(float multiplier)
    {
        currentOffsetMultiplier = multiplier;
    }

    Vector3 ExcludeDirection(Vector3 vector, Vector3 direction)
    {
        vector -=  Vector3.Dot(vector, direction) * direction;
        return vector;
    }


}