using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Side {
    Left, Right
}

public class WristControl : MonoBehaviour
{
    [SerializeField] Side side = Side.Left;
    [SerializeField] bool return2orig = true;


    float roll = 0;  // 0 - 150
    float pitch = 0;  // 0 - 90
    public float rollSpeed = 1;
    public float pitchSpeed = 1;
    public float rollMin = -60f;
    public float rollMax = 60f;
    public float pitchMin = 0f;
    public float pitchMax = 90f;


    void Update()
    {
        if (return2orig) {
            pitch -= pitchSpeed * 400 * Time.deltaTime;
            pitch = Mathf.Max(pitchMin, pitch);
            roll -= rollSpeed * 400 * Time.deltaTime;
            roll = Mathf.Max(rollMin, roll);
        }
        transform.localRotation = Quaternion.Euler(0, 0, side==Side.Left? roll: -roll) * Quaternion.Euler(pitch, 0, 0);
    }

    // For mouse button input
    public void InputDelta(int btn) {
        if (btn == (side==Side.Left? 1: 0)) {
            pitch += pitchSpeed * 800 * Time.deltaTime;
            pitch = Mathf.Min(pitchMax, pitch);
        }
        if (btn == (side==Side.Left? 0: 1)) {
            roll += rollSpeed * 800 * Time.deltaTime;
            roll = Mathf.Min(rollMax, roll);
        }
    }
}
