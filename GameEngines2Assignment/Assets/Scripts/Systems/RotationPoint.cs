using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

//[Serializable]
public struct RotationPoint : IComponentData
{
    public float3 Value;
}