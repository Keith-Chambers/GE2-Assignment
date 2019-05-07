using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using UnityEngine.Rendering;
using System.Collections;

public class GameManager : MonoBehaviour
{

	[Header("Spacecraft Settings")]
	public Material enemyMaterial;
	public Mesh enemyMesh;

	public Material defenderMaterial;
	public Mesh defenderMesh;

	public int enemyRows;
	public int enemyCols;
	public float enemyHeightRange; 

	public int defenderRows;
	public int defenderCols;
	public float defenderHeightRange;

	[Header("Asteroid Belt")]

	public float asteroidBeltHeight;
	public int numberAsteroids;
	public float outerSpread;
	public Material asteroidMaterial;
	public Mesh asteroidMesh;


	private float battleBaseHeight = 150.0f;

	private static EntityManager entityManager;
    private static RenderMesh renderMesh;
    private static EntityArchetype asteroidArchetype;
    private static EntityArchetype seekTargetArchetype;
    private static Vector3 enemySeekPos;

	void Start()
	{

		Unity.Mathematics.Random random = new Unity.Mathematics.Random(12345);

		RenderMesh enemyRenderMesh = new RenderMesh
		{
			mesh = enemyMesh,
			material = enemyMaterial,
			subMesh = 0,
			castShadows = ShadowCastingMode.Off,
			receiveShadows = false
		};

		RenderMesh defenderRenderMesh = new RenderMesh
		{
			mesh = defenderMesh,
			material = defenderMaterial,
			subMesh = 0,
			castShadows = ShadowCastingMode.Off,
			receiveShadows = false
		};

		RenderMesh asteroidRenderMesh = new RenderMesh
		{
			mesh = asteroidMesh,
			material = asteroidMaterial,
			subMesh = 0,
			castShadows = ShadowCastingMode.Off,
			receiveShadows = false
		};

		entityManager = World.Active.GetOrCreateManager<EntityManager>();

	    seekTargetArchetype = entityManager.CreateArchetype(
        	typeof(Translation),
            typeof(Rotation),
            typeof(Scale),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(BoidState),
            typeof(Velocity)
	    );

	    asteroidArchetype = entityManager.CreateArchetype(
        	typeof(Translation),
            typeof(Rotation),
            typeof(Scale),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(RotationPoint),
            typeof(OrbitSpeed),
            typeof(OrbitRotation),
            typeof(OrbitRadius)
	    );

	    NativeArray<Entity> asteroidEntityArray = new NativeArray<Entity>(numberAsteroids, Allocator.Temp);
	    entityManager.CreateEntity(asteroidArchetype, asteroidEntityArray);

	    for(int i = 0; i < numberAsteroids; i++)
	    {
    		// Calculate a position around the planet
    		// Use distance and angle
	    	float angle = random.NextFloat(0.0f, 360.0f);
	    	float radius = random.NextFloat(400.0f, 400.0f + outerSpread);
	    	float height = random.NextFloat(-(asteroidBeltHeight / 2), asteroidBeltHeight / 2);

    		float3 pos = new float3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
    		pos.y = height;

	    	entityManager.SetComponentData(asteroidEntityArray[i], new Translation {Value = pos});
			entityManager.SetSharedComponentData<RenderMesh>(asteroidEntityArray[i], asteroidRenderMesh);
			entityManager.SetComponentData(asteroidEntityArray[i], new Rotation {Value = Quaternion.identity});
			entityManager.SetComponentData(asteroidEntityArray[i], new Scale {Value = random.NextFloat(1.0f, 5.0f)} ); 
			entityManager.SetComponentData(asteroidEntityArray[i], new LocalToWorld {Value = float4x4.identity} ); 
			entityManager.SetComponentData(asteroidEntityArray[i], new RotationPoint {Value = new float3(0.0f, 0.0f, 0.0f)} ); 
			entityManager.SetComponentData(asteroidEntityArray[i], new OrbitSpeed { Value = 10.0f }); 
			entityManager.SetComponentData(asteroidEntityArray[i], new OrbitRotation {Value = angle} ); 
			entityManager.SetComponentData(asteroidEntityArray[i], new OrbitRadius {Value = radius} ); 
	    }


	    NativeArray<Entity> enemyEntityArray = new NativeArray<Entity>(enemyRows * enemyCols, Allocator.Temp);
	    entityManager.CreateEntity(seekTargetArchetype, enemyEntityArray);

	    // Spawn enemies
	    for(int cols = 0; cols < enemyCols; cols++)
	    {
	    	for(int rows = 0; rows < enemyRows; rows++)
	    	{
	    		float heightOffset = random.NextFloat(-(enemyHeightRange / 2), enemyHeightRange / 2);

		    	float3 pos = new float3(50 * cols, battleBaseHeight + heightOffset + 200, 1650.0f + rows * 30.0f);
		    	float3 target = new float3(50 * cols, battleBaseHeight + heightOffset, 1150.0f + rows * 30.0f);

				entityManager.SetComponentData(enemyEntityArray[cols * enemyRows + rows], new Translation {Value = pos});
				entityManager.SetSharedComponentData<RenderMesh>(enemyEntityArray[cols * enemyRows + rows], enemyRenderMesh);
				entityManager.SetComponentData(enemyEntityArray[cols * enemyRows + rows], new Rotation {Value = Quaternion.identity});
				entityManager.SetComponentData(enemyEntityArray[cols * enemyRows + rows], new Scale {Value = 1.0f} ); 
				entityManager.SetComponentData(enemyEntityArray[cols * enemyRows + rows], new LocalToWorld {Value = float4x4.identity} ); 
				entityManager.SetComponentData(enemyEntityArray[cols * enemyRows + rows], new Velocity {Value = new float3(0.0f, 0.0f, 0.0f)} );

				BoidState boidState = new BoidState(); 
				boidState.state = State.Arriving;
				boidState.target = target;

				entityManager.SetComponentData(enemyEntityArray[cols * enemyRows + rows], boidState);
	    	}
	    } 

	    enemyEntityArray.Dispose();

	    NativeArray<Entity> defenderEntityArray = new NativeArray<Entity>(defenderRows * defenderCols, Allocator.Temp);
	    entityManager.CreateEntity(seekTargetArchetype, defenderEntityArray);

	    // Spawn Defenders
	    for(int cols = 0; cols < defenderCols; cols++)
	    {
    		for(int rows = 0; rows < defenderRows; rows++)
    		{
				float heightOffset = random.NextFloat(-(defenderHeightRange / 2), defenderHeightRange / 2);

				float3 pos = new float3(50 * cols, battleBaseHeight + heightOffset, 450.0f + rows * 30.0f);
		    	float3 target = new float3(50 * cols, battleBaseHeight + heightOffset, 850.0f + rows * 30.0f);

				entityManager.SetComponentData(defenderEntityArray[cols * defenderRows + rows], new Translation {Value = pos});
				entityManager.SetSharedComponentData<RenderMesh>(defenderEntityArray[cols * defenderRows + rows], defenderRenderMesh);
				entityManager.SetComponentData(defenderEntityArray[cols * defenderRows + rows], new Rotation {Value = Quaternion.identity});
				entityManager.SetComponentData(defenderEntityArray[cols * defenderRows + rows], new Scale {Value = 1.0f} ); 
				entityManager.SetComponentData(defenderEntityArray[cols * defenderRows + rows], new LocalToWorld {Value = float4x4.identity} ); 
				entityManager.SetComponentData(defenderEntityArray[cols * defenderRows + rows], new Velocity {Value = new float3(0.0f, 0.0f, 0.0f)} );

				BoidState boidState = new BoidState(); 
				boidState.state = State.Arriving;
				boidState.target = target;

				entityManager.SetComponentData(defenderEntityArray[cols * defenderRows + rows], boidState);
    		}	    	
	    } 

	    defenderEntityArray.Dispose();
	}

	void Update()
	{

	}

/*
	void OnDisable()
	{
		moveHandle.Complete();
		transforms.Dispose();
	}

	void Update()
	{
		arriveHandle.Complete();

		arriveJob = new ArriveBehaviour()
		{
			target = enemySeekPos
		};

		arriveHandle = arriveJob.Schedule(transforms);

		JobHandle.ScheduleBatchedJobs();
	}

	void AddBoids(int amount)
	{
		moveHandle.Complete();

		transforms.capacity = transforms.length + amount;

		for(int i = 0; i < amount; i++)
		{
			Vector3 pos = new Vector3(20 * i, 0, 1000);
			Quaternion rot = Quaternion.Euler(0f, 108f, 0f);

			var obj = Instantiate(prefab, pos, rot) as GameObject;
			transforms.Add(obj.transform);
		}
	}

	*/
}

/*
BarrierSystem renamed to EntityCommandBufferSystem
Subtractive renamed to Exclude
[RequireSubtractiveComponent] renamed to [ExcludeComponent]
*/


/*
	void Update()
	{
		moveHandle.Complete();

		moveJob = new LinearForwardMovement()
		{
			speed = 10,
			deltaTime = Time.deltaTime
		};

		moveHandle = moveJob.Schedule(transforms);

		JobHandle.ScheduleBatchedJobs();
	}
*/

/*
	void ECSAddBoids()
	{
		Entity entity = entityManager.CreateEntity(moveArchetype);
        entityManager.SetComponentData(entity, new MoveBehaviour { speed = 5 });
	}
*/