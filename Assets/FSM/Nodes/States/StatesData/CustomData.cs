using OpenCover.Framework.Model;
using UnityEngine;

public class CustomData : StateScriptData
{
    public GameObject selectedGameObject;
    public Component selectedComponent;
    public string selectedFunction;
    public CustomData()
    {
        SetStateName("Custom");
    }
}
