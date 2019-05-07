using UnityEngine;
using Unity.Entities;

//[Serializable]
public struct ArriveBehaviour : IComponentData
{
    public Vector3 target;
}