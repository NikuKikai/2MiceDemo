using System;
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
    public float maxRotateSpeed = 90f;


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
    public void InputButton(int btn) {
        if (btn == (side==Side.Left? 1: 0)) {
            pitch += pitchSpeed * 800 * Time.deltaTime;
            pitch = Mathf.Min(pitchMax, pitch);
        }
        if (btn == (side==Side.Left? 0: 1)) {
            roll += rollSpeed * 800 * Time.deltaTime;
            roll = Mathf.Min(rollMax, roll);
        }
    }
    // For mouse move input
    public void InputMove(Vector3 v) {
        const float e = 1e-4f;
        var p = 90 - pitch + e;
        var x = p * Mathf.Sin(roll / 180 * Mathf.PI);
        var y = p * Mathf.Cos(roll / 180 * Mathf.PI);

        var dx = rollSpeed * v.x * Time.deltaTime * (p/90 + e);
        var dy = pitchSpeed * v.y * Time.deltaTime * (p/90 + e);
        x += dx;
        y += dy;

        var _pitch = Mathf.Sqrt(x * x + y * y);
        var _roll = Mathf.Atan2(x, y) / Mathf.PI * 180;
        _pitch = 90 - _pitch + e;
        Debug.Log(String.Format("{0} {1} ; {2} {3} ; {4} {5}", p, roll, dx, dy, _pitch, _roll));

        _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);
        _roll = Mathf.Clamp(_roll, rollMin, rollMax);

        pitch += Mathf.Clamp(_pitch - pitch, -maxRotateSpeed * Time.deltaTime, maxRotateSpeed * Time.deltaTime);
        roll += Mathf.Clamp(_roll - roll, -maxRotateSpeed * Time.deltaTime, maxRotateSpeed * Time.deltaTime);

        // pitch += pitchSpeed * -v.y * Time.deltaTime;
        // roll += rollSpeed * v.x * Time.deltaTime;
        // pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        // roll = Mathf.Clamp(roll, rollMin, rollMax);
    }
}
