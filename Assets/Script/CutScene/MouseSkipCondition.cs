using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSkipCondition : MonoBehaviour, ISkipCondition
{
    public bool ShouldSkip()
    {
        return Input.GetKey(KeyCode.Mouse0);
    }
}

