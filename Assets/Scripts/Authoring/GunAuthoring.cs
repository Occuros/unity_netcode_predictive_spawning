using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace DefaultNamespace
{

    [GhostComponent]
    public struct Gun : IComponentData
    {
        [GhostField]
        public float coolDown;
        public Entity bulletPrefab;
        [GhostField]
        public bool isShooting;
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
                    coolDown = 0,
                    isShooting = false
                };
                AddComponent(entity, gun);
            }
        }
    }
}

