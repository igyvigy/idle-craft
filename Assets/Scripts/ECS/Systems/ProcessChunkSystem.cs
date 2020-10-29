using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

// public class ProcessChunkSystem : SystemBase
// {
//     // [BurstCompile]

//     protected override void OnUpdate()
//     {
//         // List<BlockChunkComponent> chunks = new List<BlockChunkComponent>();
//         // EntityManager.GetAllUniqueSharedComponentData<BlockChunkComponent>(chunks);
//         // foreach (BlockChunkComponent chunk in chunks)
//         // {
//         //     int4 chunkPos = chunk.Chunk;
//         //     var blocks = WorldSettings.Chunks[chunkPos].blocks;

//         //     Entities.WithSharedComponentFilter(chunk)
//         //         .ForEach((ref BlockTypeComponent blockType) => { blockType.Value = 1; })
//         //         .ScheduleParallel();
//         // }

//     }

// }