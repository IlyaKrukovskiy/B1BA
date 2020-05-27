using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class PamsElementData
{
    public string name;

    public GameObject GO;

    public string description;

    public Vector3 position;
    public Vector3 rotation;
    public float2 previewRotation;
    public float cameraDistance;

    public List<LineData> lines;


}
