using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace DefaultNamespace
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [BurstCompile]
    public partial struct CubePlayerApplyInputSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick)
            {
                return;
            }

            state.CompleteDependency();


            var speed = SystemAPI.Time.DeltaTime * 0.5f;
            new ApplyInputJob
            {
                deltaTime = SystemAPI.Time.DeltaTime,
                speed = speed,
                tick = SystemAPI.GetSingleton<NetworkTime>().ServerTick,
                isClient = state.WorldUnmanaged.IsClient(),
            }.Run();
        }

        [BurstCompile]
        [WithAll(typeof(Simulate))]
        private partial struct ApplyInputJob : IJobEntity
        {
            public float deltaTime;
            public float speed;
            public NetworkTick tick;
            public bool isClient;

            public void Execute(in CubePlayerInput input, ref LocalTransform transform, ref Gun gun)
            {
                var moveInput = new float2(input.horizontal, input.vertical);
                moveInput = math.normalizesafe(moveInput) * speed;
                transform.Position += new float3(moveInput.x, 0, moveInput.y);
                if (input.shootPressedThisFrame.IsSet)
                {
                    gun.isShooting = true;
                    gun.startShootTick = tick;
                    var coolDownTicks = (uint)(gun.coolDown / deltaTime);
                    var lastShootingTick = tick;
                    lastShootingTick.Add((uint)(gun.ammo - 1) * coolDownTicks);
                    if (isClient)
                    {
                        Debug.Log($"Shoot Up {tick.TickValue} adding {gun.ammo -1} *{coolDownTicks} lt:{lastShootingTick.TickValue}");
                    }
               
                    gun.lastShootingTick = lastShootingTick;
                }

                if (input.shootingReleasedThisFrame.IsSet)
                {
                    if (isClient)
                    {
                        Debug.Log($"Shoot down {tick.TickValue}");
                    }
               

                    gun.isShooting = false;
                }
            }
        }
    }
}