using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewControl : MonoBehaviour
{
    [SerializeField] float sensitivity = 1f;
    [SerializeField] bool dontAffectShoulder = true;

    float xRotation = 0f;
    float yRotation = 0f;

    // For joystick/face
    public void InputXYX(Vector3 v)
    {
        ApplyDelta(new Vector2(v.x, v.y) * 10000 * Time.deltaTime);
    }

    // For mouse
    public void InputXYZDelta(Vector3 v)
    {
        ApplyDelta(new Vector2(v.x, v.y));
    }


    void ApplyDelta(Vector2 delta)
    {
        delta = delta * sensitivity * 0.01f; // * Time.deltaTime;

        xRotation -= delta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += delta.x;
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // Make shoulders not follow view
        if (dontAffectShoulder) {
            var shoulders = GetComponentsInChildren<ShoulderControl>();
            foreach (var shoulder in shoulders) {
                shoulder.ApplyDelta(-delta);
            }
        }
    }
}
