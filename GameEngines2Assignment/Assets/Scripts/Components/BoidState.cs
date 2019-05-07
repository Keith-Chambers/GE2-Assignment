using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;


public enum State 
{
	Idle,
	Defending,
	Chasing,
	KeepingFormation,
	Arriving
}

public struct BoidState : IComponentData
{

	public State state;

	// Arrive, Chase?
	public float3 target;

	// Formation, Chase?
	public Entity leader;
	public float3 formationOffset;
}