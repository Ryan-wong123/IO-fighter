using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class atkBtnScript : MonoBehaviour
{

    //Cross-Script Variable, is that a thing? Tells other scripts if this (AtkBtn) is currently pressed.
    public bool isPressed = false;

    public void OnPointerDown()
    {
        isPressed = true;
    }

    public void OnPointerUp()
    {
        isPressed = false;
    }
}
