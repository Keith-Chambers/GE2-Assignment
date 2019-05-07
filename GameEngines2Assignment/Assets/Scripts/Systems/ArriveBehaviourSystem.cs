using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

using Unity.Mathematics;
using Unity.Transforms;

public class ArriveBehaviourSystem : JobComponentSystem
{
	public EntityManager entityManager;
	private ArriveForceJob arriveJob;
	public JobHandle jobHandle;

	protected override void OnCreateManager()
    {
        entityManager = World.GetOrCreateManager<EntityManager>();
    }

	[BurstCompile]
	struct ArriveForceJob : IJobProcessComponentData<Translation, RotationEulerXYZ>
	{
		public float deltaTime;
		public float slowingDistance;

		public void Execute( ref Translation position, ref RotationEulerXYZ rot )
		{

			//position.Value.x += 10 * deltaTime;
			//rot.Value.y += 10;

/*
	        Vector3 toTarget = target - transform.position;

	        float distance = toTarget.magnitude;
	        if (distance < 0.1f)
	        {
	            return Vector3.zero;
	        }
	        float ramped = maxSpeed * (distance / slowingDistance);

	        float clamped = Mathf.Min(ramped, maxSpeed);
	        Vector3 desired = clamped * (toTarget / distance);

	        return desired - velocity; */
		}
	}

	/*
		
		private static void CreateArchetypes(EntityManager em)
{
    // ComponentType.Create<> is slightly more efficient than using typeof()
    // em.CreateArchetype(typeof(Position), typeof(Heading), typeof(Health), typeof(MoveSpeed));
    var pos = ComponentType.Create<Position>();
    var heading = ComponentType.Create<Heading>();
    var health = ComponentType.Create<Health>();
    var moveSpeed = ComponentType.Create<MoveSpeed>();
    archetype1 = em.CreateArchetype(pos, heading, health, moveSpeed);
    archetype2 = em.CreateArchetype(pos, heading, moveSpeed);
    // that's exactly how you set your entities archetype, it's like LEGO
}
	*/

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		jobHandle.Complete();

		arriveJob = new ArriveForceJob()
		{
			slowingDistance = 10.0f,
			deltaTime = Time.deltaTime
		};

		jobHandle = arriveJob.ScheduleSingle(this, inputDeps);
		return jobHandle;
	}
}
