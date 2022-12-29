using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinWheel : MonoBehaviour
{
    public float startAngle, finishAngle;
    public int sectionCount;
    public float spinSpeed;
    public float[] ranges;

    public Transform spinWheelArrowOrigin;

    bool spinning;
    float direction;
    float progress;

    // Start is called before the first frame update
    void Start()
    {
        spinning = true;
        direction = 1;
        spinWheelArrowOrigin.transform.Rotate(transform.forward * startAngle);
    }

    // Update is called once per frame
    void Update()
    {
        if (spinning)
        {
            RotateArrow();
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (spinning == true)
            {
                spinning = false;
                Vector3 eulerAngles = spinWheelArrowOrigin.eulerAngles;
                progress = (eulerAngles.z > 180) ? eulerAngles.z - 360 : eulerAngles.z;
                progress = (progress - startAngle) / (finishAngle - startAngle);
                float sectionAngle = 1f / sectionCount;
                for (int i = 1; i <= sectionCount; i++)
                {
                    if (progress > 1.0f - sectionAngle * i)
                    {
                        Debug.Log("Stopped at section " + i);
                        break;
                    }
                }
            }

        }
    }

    private void RotateArrow()
    {
        spinWheelArrowOrigin.Rotate(Vector3.forward * direction * spinSpeed * Time.deltaTime);
        Vector3 eulerAngles = spinWheelArrowOrigin.eulerAngles;
        eulerAngles.z = (eulerAngles.z > 180) ? eulerAngles.z - 360 : eulerAngles.z;
        if (direction == 1)
        {
            if (eulerAngles.z > finishAngle)
            {
                direction = -1;
            }
        }
        else if (direction == -1)
        {
            if (eulerAngles.z < startAngle)
            {
                direction = 1;
            }
        }
    }
}
