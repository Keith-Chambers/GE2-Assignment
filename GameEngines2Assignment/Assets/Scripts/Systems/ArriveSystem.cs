using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using UnityEngine.Rendering;
using Unity.Burst;

// public class CustomBarrier : EntityCommandBufferSystem {}

public class ArriveSystem : JobComponentSystem
{
	public EntityManager entityManager;
	private ArriveForceJob arriveJob;
	public JobHandle jobHandle;
	// public CustomBarrier endFrameBarrier;

	// [Inject] EndFrameBarrier endFrameBarrier; // Injection Deprecated

	protected override void OnCreateManager()
    {
        entityManager = World.GetOrCreateManager<EntityManager>();
        // endFrameBarrier = World.GetOrCreateManager<CustomBarrier>();

        //EntityQuery m_Group = GetEntityQuery(typeof(Rotation), ComponentType.ReadOnly<Velocity>());
    }

	[BurstCompile]
	struct ArriveForceJob : IJobProcessComponentDataWithEntity<Translation, Rotation, Velocity, BoidState>
	{
		public float deltaTime;
		public float slowingDistance;
		public float maxSpeed;
		
		// [ReadOnly] // This doesn't work either for some reason..
		// public EntityCommandBuffer.Concurrent cmdBuffer; // Causes issues with leakages 

		public void Execute(Entity entity, int i, ref Translation position, ref Rotation rot, ref Velocity velocity, ref BoidState boidState )
		{

			// cmdBuffer.SetComponent(entity, new Translation { Value = new float3(0.0f, 100.0f, 800.0f) }); // Test to see this works

	        switch(boidState.state)
	        {
	        	case State.Arriving: 
	        		handleArriveState(ref position, ref rot, ref velocity, ref boidState);
	        		break;
	        	case State.Chasing:
	        		break;
	        	case State.KeepingFormation:
	        		handleKeepingFormation(ref position, ref rot, ref velocity, ref boidState);
	        		break;
	        	default:
	        		//Debug.Log("Error: Invalid boid state. System cannot operate on it");
	        		break;
	        }
		}

		bool float3Equals(float3 f1, float3 f2, float precision)
		{
			return 	(f1.x >= (f2.x - precision) && f1.x <= (f2.x + precision)) && 
					(f1.y >= (f2.y - precision) && f1.y <= (f2.y + precision)) &&
					(f1.z >= (f2.z - precision) && f1.z <= (f2.z + precision));
		}

		void handleKeepingFormation( ref Translation position, ref Rotation rot, ref Velocity velocity, ref BoidState boidState )
		{
			// Get the origion pos
			// Apply the offset to get the target position
			// Apply an arrive


		}

		void handleArriveState( ref Translation position, ref Rotation rot, ref Velocity velocity, ref BoidState boidState )
		{
			float3 toTarget = boidState.target - position.Value;

	        Vector3 temp = (Vector3) toTarget;
	        float distance = temp.magnitude;

	        if (distance < 0.1f)
	            return;

	        float ramped = maxSpeed * (distance / slowingDistance);

	        float clamped = Mathf.Min(ramped, maxSpeed);
	        float3 desired = clamped * (toTarget / distance);

	        rot.Value = Quaternion.LookRotation( (desired - velocity.Value) , Vector3.up);
	        // TODO: Figure out how banking will work

	        position.Value += (desired - velocity.Value) * deltaTime; // Update position

	        if(float3Equals(position.Value, boidState.target, 0.1f))
	        {
	        	return;
	        	// position.Value = new float3(50.0f, 0.0f, 650.0f);
	        }
		}
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		jobHandle.Complete();

		arriveJob = new ArriveForceJob()
		{
			slowingDistance = 10.0f
			, deltaTime = Time.deltaTime
			, maxSpeed = 25.0f
			// , cmdBuffer = endFrameBarrier.CreateCommandBuffer().ToConcurrent()
		};

		return arriveJob.Schedule(this, inputDeps);
	}
}
