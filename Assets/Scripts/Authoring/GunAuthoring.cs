using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace DefaultNamespace
{
    [GhostComponent]
    public struct Gun : IComponentData
    {
        [GhostField] public NetworkTick startShootTick;
        [GhostField] public bool isShooting;

        public Entity bulletPrefab;
    }

    public class GunAuthoring : MonoBehaviour
    {
        public GameObject bulletPrefab;

        internal class GunAuthoringBaker : Baker<GunAuthoring>
        {
            public override void Bake(GunAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                var gun = new Gun()
                {
                    bulletPrefab = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic),
                    startShootTick = new NetworkTick(1),
                    isShooting = false
                };
                AddComponent(entity, gun);
            }
        }
    }
}