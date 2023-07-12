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
            state.CompleteDependency();
            var speed = SystemAPI.Time.DeltaTime * 0.5f;
            new ApplyInputJob()
            {
                speed = speed,
            }.Run();
        }

        [BurstCompile]
        [WithAll(typeof(Simulate))]
        private partial struct ApplyInputJob : IJobEntity
        {
            public float speed;
            public void Execute(in CubePlayerInput input, ref LocalTransform transform, ref Gun gun)
            {
                var moveInput = new float2(input.horizontal, input.vertical);
                moveInput = math.normalizesafe(moveInput) * speed;
                transform.Position += new float3(moveInput.x, 0, moveInput.y);
                if (input.shootPressedThisFrame.IsSet)
                {
                    gun.isShooting = true;
                }

                if (input.shootingReleasedThisFrame.IsSet)
                {
                    gun.isShooting = false;
                }
            }
        }

    }
}