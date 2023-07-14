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
                
                if (gun.ammo == 0)
                {
                    gun.reloadTimeLeft = math.max(gun.reloadTimeLeft - deltaTime, 0.0f);
                }
                
                if (gun.reloadTimeLeft == 0)
                {
                    gun.ammo = gun.maxAmmo;
                    gun.reloadTimeLeft = gun.reloadTime;
                }
                if (isClient)
                {
                    Debug.Log($"is shooting {tick.TickValue} {gun.isShooting}");
                }
                
                if (!gun.isShooting) return;
                // if (isClient)
                // {
                //     Debug.Log($"Could be shooting {tick.TickValue}");
                // }

                var cooldownTicks = (int)(gun.coolDown / deltaTime);

                var ticksElapsed = tick.TicksSince(gun.startShootTick);

                var shootTick = ticksElapsed % cooldownTicks;

                var isEarlyEnough = tick.TicksSince(gun.lastShootingTick) <= 0;

                var canShoot = shootTick == 0 || tick == gun.startShootTick;

                if (!canShoot || !isEarlyEnough || gun.ammo <= 0)
                {
                    // if (isClient)
                    // {
                    //     Debug.Log($"Client Not shooting: {tick.TickValue} c:{cooldownTicks} te:{ticksElapsed} lt:{gun.lastShootingTick.TickValue} - {canShoot} s:{gun.startShootTick.TickValue} ts:{tick.TicksSince(gun.lastShootingTick)} | {shootTick} | {gun.ammo}");
                    //
                    // }
 
                    return;
                }

                //
                // if (isClient)
                // {
                //     Debug.Log($"Client shot: {tick.TickValue} c:{cooldownTicks} te:{ticksElapsed} lt:{gun.lastShootingTick.TickValue}");
                // }
                // else
                // {
                //     Debug.Log($"Server shot: {tick.TickValue}");
                // }

                gun.ammo--;
                var transform = transformLookup[gun.bulletPrefab];
                var bulletEntity = ecb.Instantiate(gun.bulletPrefab);
                transform.Position = ltw.Position;
                transform.Rotation = ltw.Rotation;
                ecb.SetComponent(bulletEntity, transform);
                ecb.SetComponent(bulletEntity, new PhysicsVelocity
                {
                    Angular = float3.zero,
                    Linear = ltw.Forward * 10,
                });
            }
        }
    }
}