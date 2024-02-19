using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManyMouseUnity;
using System;

public class ViewControl : MonoBehaviour
{
    public int mouseId = 0;
    public  float mouseSensitivity = 1f;
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

    private void InitManyMouse()
    {
        Debug.Assert(ManyMouseWrapper.MouseCount > mouseId);
        mouse = ManyMouseWrapper.GetMouseByID(mouseId);
        mouse.OnMouseDeltaChanged += UpdateDelta;
    }
    private void UpdateDelta(Vector2 delta)
    {
        delta = delta * mouseSensitivity * 0.01f; // * Time.deltaTime;

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

