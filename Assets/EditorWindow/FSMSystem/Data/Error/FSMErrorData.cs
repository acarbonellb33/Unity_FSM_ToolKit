using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMErrorData
{
    public Color Color { get; set; }

    public FSMErrorData()
    {
        GenerateRandomColor();
    }

    private void GenerateRandomColor()
    {
        Color = new Color32(
            (byte) Random.Range(65, 256),
            (byte) Random.Range(50, 176),
            (byte) Random.Range(50, 176),
            255
        );
    }
}