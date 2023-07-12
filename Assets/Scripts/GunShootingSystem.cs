using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace DefaultNamespace
{
    [BurstCompile]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct GunShootingSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick) return;
            
            state.CompleteDependency();
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            new GunShootingJob()
            {
                ecb = ecb,
                deltaTime = SystemAPI.Time.DeltaTime,
                transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
            }.Run();
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        private partial struct GunShootingJob : IJobEntity
        {
            public float deltaTime;
            public EntityCommandBuffer ecb;
            public ComponentLookup<LocalTransform> transformLookup;
            
            public void Execute(ref Gun gun, in LocalToWorld ltw)
            {
                gun.coolDown = math.max(gun.coolDown - deltaTime, 0);
                if (gun.isShooting && gun.coolDown <= 0)
                {
                    gun.coolDown = 0.3f;
                    var transform = transformLookup[gun.bulletPrefab];
                    var bulletEntity = ecb.Instantiate(gun.bulletPrefab);
                    transform.Position = ltw.Position;
                    transform.Rotation = ltw.Rotation;
                    ecb.SetComponent(bulletEntity, transform);
                    ecb.SetComponent(bulletEntity, new PhysicsVelocity()
                    {
                        Angular = float3.zero,
                        Linear = ltw.Forward * 10,
                    });
                }
            }
        }


    }
}