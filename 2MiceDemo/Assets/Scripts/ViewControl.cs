using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManyMouseUnity;
using System;

public enum InputSource {
    mouse0, mouse1, joystickL, joystickR
}

public class ViewControl : MonoBehaviour
{
    public InputSource inputSource = InputSource.mouse0;
    public  float sensitivity = 1f;
    public Boolean dontAffectShoulder = true;

    ManyMouse mouse;
    float xRotation = 0f;
    float yRotation = 0f;


    private void OnEnable()
    {
        int numMice = ManyMouseWrapper.MouseCount;
        ManyMouseWrapper.OnInitialized += InitManyMouse;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (inputSource == InputSource.joystickR) {
            var h = Input.GetAxis("JoyRH") * 10000 * Time.deltaTime;
            var v = -Input.GetAxis("JoyRV") * 10000 * Time.deltaTime;
            UpdateDelta(new Vector2(h, v));
        }
    }

    private void InitManyMouse()
    {
        if (inputSource == InputSource.mouse0 || inputSource == InputSource.mouse1){
            var mouseId = inputSource == InputSource.mouse0? 0: 1;
            Debug.Assert(ManyMouseWrapper.MouseCount > mouseId);
            mouse = ManyMouseWrapper.GetMouseByID(mouseId);
            mouse.OnMouseDeltaChanged += UpdateDelta;
        }
    }
    private void UpdateDelta(Vector2 delta)
    {
        delta = delta * sensitivity * 0.01f; // * Time.deltaTime;

        xRotation -= delta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += delta.x;
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // Make shoulders not follow view
        if (dontAffectShoulder) {
            var shoulders = GetComponentsInChildren<Shoulder>();
            foreach (var shoulder in shoulders) {
                shoulder.ApplyDelta(-delta);
            }
        }
    }
}

