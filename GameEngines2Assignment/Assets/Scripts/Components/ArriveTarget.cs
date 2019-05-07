using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

// [Serializable]
public struct ArriveTarget : IComponentData
{
    public float3 Value;
}