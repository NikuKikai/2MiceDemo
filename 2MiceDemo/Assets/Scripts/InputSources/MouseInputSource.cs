using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ManyMouseUnity;


public class MouseInputSource : MonoBehaviour
{
    [SerializeField] int mouseId = 0;
    [SerializeField] UnityEvent<Vector3> outputXYZ;
    [SerializeField] UnityEvent<int> outputButtonDown;
    [SerializeField] UnityEvent<int> outputButtonHold;
    [SerializeField] UnityEvent<int> outputButtonUp;


    ManyMouse mouse;


    private void OnEnable()
    {
        Debug.Log(ManyMouseWrapper.MouseCount);
        ManyMouseWrapper.OnInitialized += InitManyMouse;
    }

    void Start() { Cursor.lockState = CursorLockMode.Locked; }

    void Update() {
        if (mouse == null) return;
        if (mouse.Button1) outputButtonHold.Invoke(0);
        if (mouse.Button2) outputButtonHold.Invoke(1);
        if (mouse.Button3) outputButtonHold.Invoke(2);
    }

    void InitManyMouse()
    {
        if (ManyMouseWrapper.MouseCount <= mouseId) {
            Debug.LogWarning("! Mouse not found for id = " + mouseId);
            return;
        }
        mouse = ManyMouseWrapper.GetMouseByID(mouseId);
        mouse.OnMouseDeltaChanged += OnMouseDeltaChanged;
        mouse.OnMouseButtonDown += OnMouseButtonDown;
        mouse.OnMouseButtonUp += OnMouseButtonUp;
    }

    void OnMouseDeltaChanged(Vector2 v)
    {
        outputXYZ.Invoke(new Vector3(v.x, v.y));
    }

    void OnMouseButtonDown(int button)
    {
        outputButtonDown.Invoke(button);
    }

    void OnMouseButtonUp(int button)
    {
        outputButtonUp.Invoke(button);
    }
}
