using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoulderControl : MonoBehaviour
{
    [SerializeField]  float mouseSensitivity = 1f;

    float xRotation = 0f;
    float yRotation = 0f;


    public void ApplyDelta(Vector2 delta)
    {
        xRotation -= delta.y;
        xRotation = Mathf.Clamp(xRotation, -15f, 30f);

        yRotation += delta.x;
        yRotation = Mathf.Clamp(yRotation, -40f, 40f);
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    public void InputXYZDelta_Mouse(Vector3 v)
    {
        ApplyDelta(new Vector2(v.x, v.y) * 0.002f);
        Debug.Log(v);
    }

    public void InputButton(int btn)
    {
        if (btn == 0) {
            GetComponentInChildren<Gun>().Fire();
        }
    }

}
