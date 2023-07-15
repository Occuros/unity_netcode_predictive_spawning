using Systems;
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
            state.RequireForUpdate<NetworkId>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick)
            {
                return;
            }

            state.CompleteDependency();
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            new GunShootingJob
            {
                ecb = ecb,
                deltaTime = SystemAPI.Time.DeltaTime,
                transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
                tick = SystemAPI.GetSingleton<NetworkTime>().ServerTick,
                isClient = state.WorldUnmanaged.IsClient()
            }.Run();
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        [WithAll(typeof(Simulate))]
        private partial struct GunShootingJob : IJobEntity
        {
            public float deltaTime;
            public EntityCommandBuffer ecb;
            public ComponentLookup<LocalTransform> transformLookup;
            public NetworkTick tick;
            public bool isClient;


            public void Execute(ref Gun gun, in LocalToWorld ltw)
            {
                if (gun.isShooting)
                {
                    const int fireRate = 3000;

                    var predictor = new ShootingPredictor(gun.startShootTick, tick, fireRate, deltaTime);

                    foreach (var shot in predictor)
                    {
                        if (isClient)
                        {
                            Debug.Log($"Client: {tick.TickValue}");
                        }
                        else
                        {
                            Debug.Log($"Server: {tick.TickValue}");
                        }

                        var transform = transformLookup[gun.bulletPrefab];
                        transform.Rotation = ltw.Rotation;


                        const int bulletVelocity = 10;

                        transform.Position = ltw.Position + bulletVelocity * (transform.Forward() * shot.timePassed);

                        var physicsVelocity = new PhysicsVelocity
                        {
                            Angular = float3.zero,
                            Linear = ltw.Forward * bulletVelocity,
                        };


                        var bulletEntity = ecb.Instantiate(gun.bulletPrefab);
                        ecb.SetComponent(bulletEntity, transform);
                        ecb.SetComponent(bulletEntity, physicsVelocity);
                    }
                }
            }
        }
    }
}