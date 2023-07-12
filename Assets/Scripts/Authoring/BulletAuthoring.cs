using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    public class BulletAuthoring : MonoBehaviour
    {
        public class BulletAuthoringBaker : Baker<BulletAuthoring>
        {
            public override void Bake(BulletAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BulletComponentData()
                {
                    timeToDie = 3f
                });
            }
        }
    }

    public struct BulletComponentData : IComponentData
    {
        public float timeToDie;
    }
}