using Unity.Entities;
using UnityEngine;
    
namespace Unity.Multiplayer.Center.NetcodeForEntitiesSetup
{
    /// <summary>
    /// Flag component to mark an entity as a cube.
    /// </summary>
    public struct Cube : IComponentData
    {
    }

    /// <summary>
    /// The authoring component for the Cube.
    /// </summary>
    [DisallowMultipleComponent]
    public class CubeAuthoring : MonoBehaviour
    {
        class CubeBaker : Baker<CubeAuthoring>
        {
            public override void Bake(CubeAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<Cube>(entity);
            }
        }
    }
}
