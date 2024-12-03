using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStickSkipCondition : MonoBehaviour, ISkipCondition
{
    public bool ShouldSkip()
    {
        return Input.GetKey(KeyCode.JoystickButton0);
    }
}
