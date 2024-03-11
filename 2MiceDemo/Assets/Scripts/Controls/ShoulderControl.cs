using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoulderControl : MonoBehaviour
{
    [SerializeField] Side side = Side.Left;
    [SerializeField]  float mouseSensitivity = 1f;

    float xRotation = 0f;
    float yRotation = 0f;
    [SerializeField] float xRotMin = -25;
    [SerializeField] float xRotMax = 40;
    [SerializeField] float yRotMin = -30;
    [SerializeField] float yRotMax = 30;


    public void ApplyDelta(Vector2 delta)
    {
        xRotation += delta.x;
        if (side == Side.Right)
            xRotation = Mathf.Clamp(xRotation, xRotMin, xRotMax);
        else
            xRotation = Mathf.Clamp(xRotation, -xRotMax, -xRotMin);

        yRotation += delta.y;
        yRotation = Mathf.Clamp(yRotation, yRotMin, yRotMax);
        transform.localRotation = Quaternion.Euler(-yRotation, xRotation, 0f);
    }

    public void InputXYZDelta_Mouse(Vector3 v)
    {
        ApplyDelta(new Vector2(v.x, v.y) * 0.002f);
    }

}
