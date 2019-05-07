using UnityEngine;
using Unity.Entities;

//[Serializable]
public struct SeekTarget_ : IComponentData
{
    public Vector3 target;
}