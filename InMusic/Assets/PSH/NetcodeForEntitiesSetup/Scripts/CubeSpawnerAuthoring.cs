using Unity.Entities;
using UnityEngine;

namespace Unity.Multiplayer.Center.NetcodeForEntitiesSetup
{
    /// <summary>
    /// Component data that identifies a cube spawner and gives access to the cube prefab.
    /// </summary>
    public struct CubeSpawner : IComponentData
    {
        /// <summary>
        /// The Cube prefab converted to an entity.
        /// </summary>
        public Entity Cube;
    }

    /// <summary>
    /// Baker that transforms our cube prefab into an entity and creates a spawner entity.
    /// </summary>
    [DisallowMultipleComponent]
    public class CubeSpawnerAuthoring : MonoBehaviour
    {
        /// <summary>
        /// The cube prefab to spawn.
        /// </summary>
        public GameObject Cube;

        class SpawnerBaker : Baker<CubeSpawnerAuthoring>
        {
            public override void Bake(CubeSpawnerAuthoring authoring)
            {
                CubeSpawner component = default(CubeSpawner);
                component.Cube = GetEntity(authoring.Cube, TransformUsageFlags.Dynamic);
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, component);
            }
        }
    }
}
