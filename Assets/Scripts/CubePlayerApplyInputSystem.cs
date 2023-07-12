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
            var speed = SystemAPI.Time.DeltaTime * 4;
            foreach (var (input, trans, gun) in SystemAPI.Query<
                         RefRO<CubePlayerInput>, 
                         RefRW<LocalTransform>,
                        RefRW<Gun>
                     >().WithAll<Simulate>())
            {
                var moveInput = new float2(input.ValueRO.horizontal, input.ValueRO.vertical);
                moveInput = math.normalizesafe(moveInput) * speed;
                trans.ValueRW.Position += new float3(moveInput.x, 0, moveInput.y);
                if (input.ValueRO.shootPressedThisFrame.IsSet)
                {
                    gun.ValueRW.isShooting = true;
                }

                if (input.ValueRO.shootingReleasedThisFrame.IsSet)
                {
                    gun.ValueRW.isShooting = false;
                }
            }
        }
    }
}