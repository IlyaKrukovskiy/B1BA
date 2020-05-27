using Unity.Mathematics;

[System.Serializable]
public struct CameraZoomingData
{
    public float sensitivity;
    public float2 coefficientRange;
    public float currentCoefficient;
}
