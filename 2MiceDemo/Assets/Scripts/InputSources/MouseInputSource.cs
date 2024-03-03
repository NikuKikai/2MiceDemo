using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ManyMouseUnity;


public class MouseInputSource : MonoBehaviour
{
    [SerializeField] UnityEvent<Vector3> outputXYZ;
    [SerializeField] UnityEvent<int> outputButton;
    [SerializeField] int mouseId = 0;


    ManyMouse mouse;


    private void OnEnable()
    {
        Debug.Log(ManyMouseWrapper.MouseCount);
        ManyMouseWrapper.OnInitialized += InitManyMouse;
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
    }

    void OnMouseDeltaChanged(Vector2 v)
    {
        outputXYZ.Invoke(new Vector3(v.x, v.y));
    }

    void OnMouseButtonDown(int button)
    {
        outputButton.Invoke(button);
    }
}
