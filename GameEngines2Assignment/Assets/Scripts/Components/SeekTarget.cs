using UnityEngine;
using Unity.Entities;

//[Serializable]
public struct SeekTarget : IComponentData
{
    public Vector3 target;
}