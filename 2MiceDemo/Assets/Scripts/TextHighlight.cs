using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextHighlight : MonoBehaviour
{
    public int button = 0;

    public void SetOn(int btn) {
        if (btn != button) return;
        var color = GetComponent<Text>().color;
        color.a = 1;
        GetComponent<Text>().color = color;
    }
    public void SetOff(int btn) {
        if (btn != button) return;
        var color = GetComponent<Text>().color;
        color.a = 0.2f;
        GetComponent<Text>().color = color;
    }
}
