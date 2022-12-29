using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public bool excludeX = false; // Use this if you don't want camera to follow in horizontal
    public bool excludeY = false; // Use this if you don't want camera to follow in vertical
    public float cameraChangeLerpFactor = 0.2f; // Use this to smooth the camera motion
    // Connections
    public Transform target;
    // State variables
    Vector3 defaultOffset; // Starting offset of the camera to the target
    float currentOffsetMultiplier; // Modify this for zoom in / out effects

    // Start is called before the first frame update
    void Start()
    {
        InitState();
    }

    private void InitState()
    {
        defaultOffset = transform.position - target.position;
        currentOffsetMultiplier = 1.0f;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        Vector3 targetFollowPosition = target.position + currentOffsetMultiplier * defaultOffset;

        if (excludeX)
        {
            targetFollowPosition.x = transform.position.x;
        }

        if (excludeY)
            targetFollowPosition.y = transform.position.y;

        transform.position = Vector3.Lerp(transform.position, targetFollowPosition, cameraChangeLerpFactor * Time.deltaTime);
    }

    public void SetOffsetMultiplier(float multiplier)
    {
        currentOffsetMultiplier = multiplier;
    }
}