using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class AsteroidSystem : JobComponentSystem
{
	public EntityManager entityManager;
	private RotateAsteroidsJob rotateAsteroidJob;
	public JobHandle jobHandle;

	protected override void OnCreateManager()
    {
        entityManager = World.GetOrCreateManager<EntityManager>();
    }

	[BurstCompile]
	struct RotateAsteroidsJob : IJobProcessComponentData<Translation, RotationPoint, OrbitSpeed, OrbitRadius, OrbitRotation>
	{
		public float deltaTime;

		public void Execute( ref Translation position, ref RotationPoint rotPoint, ref OrbitSpeed orbitSpeed, ref OrbitRadius orbitRadius, ref OrbitRotation orbitRotation )
		{

			orbitRotation.Value += orbitSpeed.Value * deltaTime * 0.002f;
			/*
			float currentY = position.Value.y;
			position.Value = new float3(Mathf.Cos(orbitRotation.Value), 0, Mathf.Sin(orbitRotation.Value)) * orbitRadius.Value;
			position.Value.y = currentY; */

			position.Value.x = Mathf.Cos(orbitRotation.Value) * orbitRadius.Value;
			position.Value.z = Mathf.Sin(orbitRotation.Value) * orbitRadius.Value;
		}
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		jobHandle.Complete();

		rotateAsteroidJob = new RotateAsteroidsJob()
		{
			deltaTime = Time.deltaTime
		};

		return rotateAsteroidJob.Schedule(this, inputDeps);
	}
}
