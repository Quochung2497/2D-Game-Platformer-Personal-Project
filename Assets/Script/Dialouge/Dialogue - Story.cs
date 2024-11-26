using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [TextArea(3, 10)]
    public string name;
    public string[] sentences;
}
[System.Serializable]
public class StoryElement
{
    [TextArea(5, 15)]
    public string[] sentences;
}
