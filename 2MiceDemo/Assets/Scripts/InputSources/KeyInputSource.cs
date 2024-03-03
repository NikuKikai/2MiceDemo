using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeyInputSource : MonoBehaviour
{
    [SerializeField] UnityEvent<KeyCode> outputKey;
    [SerializeField] UnityEvent<Vector3> outputAxis;

    [SerializeField] KeyCode[] keys = {KeyCode.Space};


    void Update()
    {
        foreach (var key in keys) {
            if (Input.GetKey(key)) {
                outputKey.Invoke(key);
            }
        }

        outputAxis.Invoke(new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        ));
    }
}
