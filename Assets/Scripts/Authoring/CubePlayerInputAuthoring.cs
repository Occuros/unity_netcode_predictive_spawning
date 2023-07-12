using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
public struct CubePlayerInput : IInputComponentData
{
    public int horizontal;
    public int vertical;
    public InputEvent shootPressedThisFrame;
    public InputEvent shootingReleasedThisFrame;
}

[DisallowMultipleComponent]
public class CubePlayerInputAuthoring : MonoBehaviour
{
    class Baking : Unity.Entities.Baker<CubePlayerInputAuthoring>
    {
        public override void Bake(CubePlayerInputAuthoring authoring)
        {
            var entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent<CubePlayerInput>(entity);
        }
    }
}