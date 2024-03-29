﻿using Unity.Entities;
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

[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial struct SampleCubeInput : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var left = UnityEngine.Input.GetKey(KeyCode.A);
        var right = UnityEngine.Input.GetKey(KeyCode.D);
        var down = UnityEngine.Input.GetKey(KeyCode.S);
        var up = UnityEngine.Input.GetKey(KeyCode.W);
        var shoot = UnityEngine.Input.GetMouseButtonDown(0);
        var stopShoot = UnityEngine.Input.GetMouseButtonUp(0);

        foreach (var playerInput in SystemAPI.Query<RefRW<CubePlayerInput>>().WithAll<GhostOwnerIsLocal>())
        {
            playerInput.ValueRW = default;
            if (left)
                playerInput.ValueRW.horizontal -= 1;
            if (right)
                playerInput.ValueRW.horizontal += 1;
            if (down)
                playerInput.ValueRW.vertical -= 1;
            if (up)
                playerInput.ValueRW.vertical += 1;
            if (shoot)
            {
                playerInput.ValueRW.shootPressedThisFrame.Set();
            }
            if (stopShoot)
            {
                playerInput.ValueRW.shootingReleasedThisFrame.Set();
            }
        }
    }
}