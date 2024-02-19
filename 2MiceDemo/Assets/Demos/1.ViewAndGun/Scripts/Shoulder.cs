using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManyMouseUnity;

public class Shoulder : MonoBehaviour
{
    public int mouseId = 1;
    public  float mouseSensitivity = 1f;

    ManyMouse mouse;
    float xRotation = 0f;
    float yRotation = 0f;

    private void OnEnable()
    {
        int numMice = ManyMouseWrapper.MouseCount;
        ManyMouseWrapper.OnInitialized += InitManyMouse;
    }

    void Update()
    {
    }

    private void InitManyMouse()
    {
        Debug.Assert(ManyMouseWrapper.MouseCount > mouseId);
        mouse = ManyMouseWrapper.GetMouseByID(mouseId);
        mouse.OnMouseDeltaChanged += OnMouseDeltaChanged;
        mouse.OnMouseButtonDown += OnMouseButtonDown;
    }

    public void ApplyDelta(Vector2 delta)
    {
        xRotation -= delta.y;
        xRotation = Mathf.Clamp(xRotation, -15f, 30f);

        yRotation += delta.x;
        yRotation = Mathf.Clamp(yRotation, -40f, 40f);
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    private void OnMouseDeltaChanged(Vector2 Delta)
    {
        Vector2 delta = mouse.Delta * mouseSensitivity * 0.01f; // Time.deltaTime;
        ApplyDelta(delta);
    }

    private void OnMouseButtonDown(int button)
    {
        if (button == 0) {
            GetComponentInChildren<Gun>().Fire();
        }
    }
}
