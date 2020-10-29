using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;

public class ChunkToEntityConversion : MonoBehaviour, IConvertGameObjectToEntity
{
    Chunk chunk;

    void Start()
    {
        chunk = GetComponent<Chunk>();
    }
    public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
    {

        // NativeArray<Entity> blockEntities = new NativeArray<Entity>(Chunk.BlocksCount, Allocator.Temp);

        // EntityArchetype entityArchetype = manager.CreateArchetype(typeof(BlockPosComponent), typeof(BlockChunkComponent));

        // manager.CreateEntity(entityArchetype, blockEntities);

        // for (var i = 0; i < blockEntities.Length; i++)
        // {
        //     var _entity = blockEntities[i];
        //     manager.SetComponentData(_entity, new BlockChunkComponent { Chunk = chunkPos });
        //     manager.SetComponentData(_entity, new BlockPosComponent { Pos = chunk.blocks[i] });
        // }

        // manager.AddComponentData(entity, new ChunkComponent { blocks = blockEntities });
        // manager.AddComponentData(entity, new Translation() { Value = this.transform.position });
    }
}