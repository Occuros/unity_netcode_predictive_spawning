using Unity.Entities;
using UnityEngine;

public struct CubeSpawner : IComponentData
{
    public Entity Cube;
}

[DisallowMultipleComponent]
public class CubeSpawnerAuthoring : MonoBehaviour
{
    public CubePlayerAuthoring Cube;

    class Baker : Baker<CubeSpawnerAuthoring>
    {
        public override void Bake(CubeSpawnerAuthoring authoring)
        {
            var entity = GetEntity(authoring, TransformUsageFlags.None);
            CubeSpawner component = default(CubeSpawner);
            component.Cube = GetEntity(authoring.Cube, TransformUsageFlags.Dynamic);
            AddComponent(entity, component);
        }
    }
}