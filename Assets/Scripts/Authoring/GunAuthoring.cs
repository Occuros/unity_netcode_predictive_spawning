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
        [GhostField] public NetworkTick lastShootingTick;

        public float coolDown;

        [GhostField]
        public float remainingCoolDown;
        [GhostField]
        public short ammo;
        public short maxAmmo;

        public float reloadTime;
        public float reloadTimeLeft;

        public Entity bulletPrefab;
    }

    [GhostComponent]
    public struct MagazineBullet : IBufferElementData
    {
        [GhostField]
        public Entity value;
    }

    public class GunAuthoring : MonoBehaviour
    {
        public short maxAmmo;
        public float reloadTime;
        public float coolDown;
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
                    isShooting = false,
                    coolDown = authoring.coolDown,
                    reloadTime = authoring.reloadTime,
                    maxAmmo = authoring.maxAmmo,
                };
                AddComponent(entity, gun);
                AddBuffer<MagazineBullet>(entity);
            }
        }
    }
}