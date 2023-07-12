using Unity.Entities;
using UnityEngine;

public struct CubePlayer : IComponentData
{
}

[DisallowMultipleComponent]
public class CubePlayerAuthoring : MonoBehaviour
{
    class Baker : Baker<CubePlayerAuthoring>
    {
        public override void Bake(CubePlayerAuthoring authoring)
        {
            var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            CubePlayer component = default(CubePlayer);
            AddComponent(entity, component);
        }
    }
}