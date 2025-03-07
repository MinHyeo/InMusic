using Unity.Entities;
using Unity.Rendering;
using Unity.NetCode;

using MaterialPropertyBaseColor = Unity.Rendering.URPMaterialPropertyBaseColor;

namespace Unity.Multiplayer.Center.NetcodeForEntitiesSetup
{
    /// <summary>Denotes that a ghost will be set to the debug color specified in <see cref="NetworkIdDebugColorUtility"/>.</summary>
    public struct SetPlayerToDebugColor : IComponentData
    {
    }

    [UnityEngine.DisallowMultipleComponent]
    public class SetPlayerToDebugColorAuthoring : UnityEngine.MonoBehaviour
    {
        class SetPlayerToDebugColorBaker : Baker<SetPlayerToDebugColorAuthoring>
        {
            public override void Bake(SetPlayerToDebugColorAuthoring authoring)
            {
                SetPlayerToDebugColor component = default(SetPlayerToDebugColor);
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, component);
                AddComponent(entity, new MaterialPropertyBaseColor {Value = 1});
            }
        }
    }

    /// <summary>
    ///     Every NetworkId has its own unique Debug color. This system sets it for all ghosts with a <see cref="SetPlayerToDebugColor"/>.
    /// </summary>
    [AlwaysSynchronizeSystem]
    [RequireMatchingQueriesForUpdate]
    [WorldSystemFilter(WorldSystemFilterFlags.Presentation)]
    public partial class SetPlayerToDebugColorSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var (colorRef, ghostOwner) in SystemAPI.Query<RefRW<MaterialPropertyBaseColor>, RefRO<GhostOwner>>().WithChangeFilter<MaterialMeshInfo, GhostOwner>())
            {
                colorRef.ValueRW.Value = NetworkIdDebugColorUtility.Get(ghostOwner.ValueRO.NetworkId);
            }
        }
    }
}

