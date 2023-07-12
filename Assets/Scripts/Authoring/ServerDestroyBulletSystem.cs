using System;
using Unity.Burst;
using Unity.Entities;

namespace DefaultNamespace
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ServerDestroyBulletSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buf = new EntityCommandBuffer(state.WorldUpdateAllocator);
            var delta = SystemAPI.Time.DeltaTime;
            foreach (var (bulletComponentDataRef, e) in
                     SystemAPI.Query<RefRW<BulletComponentData>>().WithEntityAccess())
            {
                ref var bullet = ref bulletComponentDataRef.ValueRW;

                bullet.timeToDie -= delta;

                if (bullet.timeToDie <= 0f)
                {
                    buf.DestroyEntity(e);
                }
            }

            buf.Playback(state.EntityManager);
        }
    }
}