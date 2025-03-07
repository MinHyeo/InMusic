using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Unity.Multiplayer.Center.NetcodeForEntitiesSetup
{
    /// <summary>
    /// Input for the cube to move in the example setup.
    /// </summary>
    public struct CubeInput : IInputComponentData
    {
        /// <summary>
        /// Horizontal movement (X axis).
        /// </summary>
        public int Horizontal;
        
        /// <summary>
        /// Vertical movement (Z axis).
        /// </summary>
        public int Vertical;
    }

    /// <summary>
    /// The authoring component for the CubeInput.
    /// </summary>
    [DisallowMultipleComponent]
    public class CubeInputAuthoring : MonoBehaviour
    {
        class Baking : Baker<CubeInputAuthoring>
        {
            public override void Bake(CubeInputAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<CubeInput>(entity);
            }
        }
    }

    /// <summary>
    /// System in charge of setting the cube input data based on the pressed keys.
    /// Input: whether keys are pressed, <see cref="CubeInput"/> component data.
    /// Output: modified <see cref="CubeInput"/> component data for which the local client is the owner.
    /// </summary>
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial struct SampleCubeInput : ISystem
    {
        /// <summary>
        /// Sets the cube input data based on the pressed keys.
        /// </summary>
        /// <param name="state">Raw entity system state, unused here.</param>
        public void OnUpdate(ref SystemState state)
        {
#if ENABLE_INPUT_SYSTEM
            var left = Keyboard.current.aKey.isPressed;
            var right = Keyboard.current.dKey.isPressed;
            var down = Keyboard.current.sKey.isPressed;
            var up = Keyboard.current.wKey.isPressed;
#else
            var left = UnityEngine.Input.GetKey(KeyCode.A);
            var right = UnityEngine.Input.GetKey(KeyCode.D);
            var down = UnityEngine.Input.GetKey(KeyCode.S);
            var up = UnityEngine.Input.GetKey(KeyCode.W);
#endif

            foreach (var playerInput in SystemAPI.Query<RefRW<CubeInput>>().WithAll<GhostOwnerIsLocal>())
            {
                playerInput.ValueRW = default;
                if (left)
                    playerInput.ValueRW.Horizontal -= 1;
                if (right)
                    playerInput.ValueRW.Horizontal += 1;
                if (down)
                    playerInput.ValueRW.Vertical -= 1;
                if (up)
                    playerInput.ValueRW.Vertical += 1;
            }
        }
    }
    
    /// <summary>
    /// System in charge of moving the cube based on the input for entities with the Simulate flag.
    /// Input: <see cref="CubeInput"/> component data, <see cref="LocalTransform"/> component data.
    /// Output: modified <see cref="LocalTransform"/> component data.
    /// </summary>
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [BurstCompile]
    public partial struct CubeMovementSystem : ISystem
    {
        /// <summary>
        /// Modifies the local transforms of the cubes based on the input.
        /// </summary>
        /// <param name="state">Raw entity system state, unused here.</param>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var speed = SystemAPI.Time.DeltaTime * 4;
            foreach (var (input, trans) in
                     SystemAPI.Query<RefRO<CubeInput>, RefRW<LocalTransform>>()
                         .WithAll<Simulate>())
            {
                var moveInput = new float2(input.ValueRO.Horizontal, input.ValueRO.Vertical);
                moveInput = math.normalizesafe(moveInput) * speed;
                trans.ValueRW.Position += new float3(moveInput.x, 0, moveInput.y);
            }
        }
    }
}
