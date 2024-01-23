using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FSMGroup : Group
{
    public string Id { get; set; }
    public string PreviousTitle { get; set; }
    private Color _defaultBorderColor;
    private float _defaultBorderWidth;
    
    public FSMGroup(string groupName, Vector2 position)
    {
        Id = Guid.NewGuid().ToString();
        title = groupName;
        PreviousTitle = groupName;
        SetPosition(new Rect(position, Vector2.zero));
        _defaultBorderColor = contentContainer.style.borderBottomColor.value;
        _defaultBorderWidth = contentContainer.style.borderBottomWidth.value;
    }

    public void SetErrorStyle(Color color)
    {
        contentContainer.style.borderBottomColor = color;
        contentContainer.style.borderBottomWidth = 2f;
    }
    
    public void ResetStyle()
    {
        contentContainer.style.borderBottomColor = _defaultBorderColor;
        contentContainer.style.borderBottomWidth = _defaultBorderWidth;
    }
}