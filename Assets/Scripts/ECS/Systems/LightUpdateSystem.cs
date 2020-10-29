using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using System.Collections.Generic;

// public class LightUpdateSystem : ComponentSystem
// {
//     protected override void OnUpdate()
//     {
//         Entities.WithAll<PlayerTag>().ForEach((ref Translation pos) =>
//         {
//             int4 playerCP = Utils.ChunkPosbyPosition(pos.Value);
//             if (!math.Equals(playerCP, LightData.LastUpdatedCP))
//             {
//                 int4 cp = playerCP;
//                 LightData.SetLastUpdatedCP(cp);
//                 int4 bcp = new int4(cp.x, cp.y, cp.z - 1, cp.w);
//                 int4 lcp = new int4(cp.x - 1, cp.y, cp.z, cp.w);
//                 int4 fcp = new int4(cp.x, cp.y, cp.z + 1, cp.w);
//                 int4 rcp = new int4(cp.x + 1, cp.y, cp.z, cp.w);

//                 var bjob = new CalculateLightJob() { cp = bcp };
//                 JobHandle bhandle = bjob.Schedule();
//                 var ljob = new CalculateLightJob() { cp = lcp };
//                 JobHandle lhandle = ljob.Schedule();
//                 var job = new CalculateLightJob() { cp = cp };
//                 JobHandle handle = job.Schedule();
//                 var fjob = new CalculateLightJob() { cp = fcp };
//                 JobHandle fhandle = fjob.Schedule();
//                 var rjob = new CalculateLightJob() { cp = rcp };
//                 JobHandle rhandle = rjob.Schedule();
//             }

//             if (LightData.RebuildCP != null)
//             {
//                 int4 cp = LightData.RebuildCP.Value;
//                 LightData.SetRebuildCP(null);
//                 int4 bcp = new int4(cp.x, cp.y, cp.z - 1, cp.w);
//                 int4 lcp = new int4(cp.x - 1, cp.y, cp.z, cp.w);
//                 int4 fcp = new int4(cp.x, cp.y, cp.z + 1, cp.w);
//                 int4 rcp = new int4(cp.x + 1, cp.y, cp.z, cp.w);

//                 var bjob = new CalculateLightJob() { cp = bcp };
//                 JobHandle bhandle = bjob.Schedule();
//                 var ljob = new CalculateLightJob() { cp = lcp };
//                 JobHandle lhandle = ljob.Schedule();
//                 var job = new CalculateLightJob() { cp = cp };
//                 JobHandle handle = job.Schedule();
//                 var fjob = new CalculateLightJob() { cp = fcp };
//                 JobHandle fhandle = fjob.Schedule();
//                 var rjob = new CalculateLightJob() { cp = rcp };
//                 JobHandle rhandle = rjob.Schedule();
//             }
//         });
//     }

//     struct CalculateLightJob : IJob
//     {
//         [ReadOnly] public int4 cp;
//         public void Execute()
//         {
//             CalculateLight(cp);
//         }

//         private void CalculateLight(int4 cp, bool fillNeighbours = false)
//         {
//             // Debug.Log("CALCULATE LIGHTS " + cp + ", fillNeighbours " + fillNeighbours);
//             float?[,,] data = new float?[Chunk.Width, Chunk.Height, Chunk.Width];

//             for (int y = Chunk.Height - 1; y >= 0; y--) // from top to bottom
//             {
//                 for (int x = 0; x < Chunk.Width - 1; x++)
//                 {
//                     for (int z = 0; z < Chunk.Width - 1; z++)
//                     {
//                         Flow(cp, ref data, x, y, z, true, true);
//                     }
//                 }
//                 for (int x = Chunk.Width - 1; x >= 0; x--)
//                 {
//                     for (int z = Chunk.Width - 1; z >= 0; z--)
//                     {
//                         Flow(cp, ref data, x, y, z, true, false);
//                     }
//                 }
//             }
//             for (int y = 0; y < Chunk.Height - 1; y++) // from bottom to top
//             {
//                 for (int x = 0; x < Chunk.Width - 1; x++)
//                 {
//                     for (int z = 0; z < Chunk.Width - 1; z++)
//                     {
//                         Flow(cp, ref data, x, y, z, false, true);
//                     }
//                 }
//                 for (int x = Chunk.Width - 1; x >= 0; x--)
//                 {
//                     for (int z = Chunk.Width - 1; z >= 0; z--)
//                     {
//                         Flow(cp, ref data, x, y, z, false, false);
//                     }
//                 }
//             }
//             LightData.Data[cp] = data;
//         }

//         private void Flow(int4 cp, ref float?[,,] data, int x, int y, int z, bool down, bool forward)
//         {
//             if (!WorldSettings.Chunks.ContainsKey(cp)) return;
//             int3 block = WorldSettings.Chunks[cp].blocks[Utils.to1D(x, y, z)];
//             if (block.x != 0) return;

//             int3 id = new int3(x, y, z);
//             if (y == Chunk.Height - 1) // start layer
//             {
//                 if (block.x == 0)
//                 {
//                     data[x, y, z] = 1f;
//                 }
//             }
//             else
//             { // lower levels
//                 if (block.x != 0) return;
//                 if (data[x, y, z] != null) return;
//                 float? lightValue = GetLightForBlock(cp, data, id, down, forward);
//                 ////
//                 float intencity = 1.0f;
//                 if (y > Settings.LightUpperBound)
//                 {
//                     intencity = 1.001f;
//                 }
//                 else if (y < Settings.LightLowerBound)
//                 {
//                     intencity = 0;
//                 }
//                 else
//                 {
//                     float range = Settings.LightUpperBound - Settings.LightLowerBound;
//                     intencity = Mathf.Abs(y - Settings.LightLowerBound) / range;
//                 }
//                 lightValue *= intencity;
//                 /////
//                 data[x, y, z] = lightValue;
//             }
//         }

//         private float? GetLightForBlock(int4 cp, float?[,,] data, int3 id, bool down, bool forward)
//         {
//             // if (outsideLights[id.x, id.y, id.z] != null)
//             // {
//             //     return outsideLights[id.x, id.y, id.z];
//             // }
//             Debug.LogFormat("cp {0} id {1} down {2} forward {3}", cp, id, down, forward);
//             var feeds = new List<float?>();

//             // front
//             if (id.z != 0)
//             {
//                 var fL = data[id.x, id.y, id.z - 1];
//                 var fB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x, id.y, id.z - 1)];
//                 if (fB.x == 0 && fL != null) feeds.Add(fL.Value);
//             }
//             // left
//             if (id.x != 0)
//             {
//                 var lL = data[id.x - 1, id.y, id.z];
//                 var lB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x - 1, id.y, id.z)];
//                 if (lB.x == 0 && lL != null) feeds.Add(lL.Value);
//             }
//             // back
//             if (id.z != Chunk.Width - 1)
//             {
//                 var bL = data[id.x, id.y, id.z + 1];
//                 var bB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x, id.y, id.z + 1)];
//                 if (bB.x == 0 && bL != null) feeds.Add(bL.Value);
//             }
//             // right
//             if (id.x != Chunk.Width - 1)
//             {
//                 var rB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x + 1, id.y, id.z)];
//                 var rL = data[id.x + 1, id.y, id.z];
//                 if (rB.x == 0 && rL != null) feeds.Add(rL.Value);
//             }
//             if (down)
//             {
//                 var uB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x, id.y + 1, id.z)];

//                 // up
//                 if (id.y != Chunk.Height - 1 && uB.x == 0)
//                 {
//                     var uL = data[id.x, id.y + 1, id.z];
//                     if (uL != null) feeds.Add(uL.Value);
//                 }
//                 //up left

//                 if (id.x != 0)
//                 {
//                     var lB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x - 1, id.y, id.z)];
//                     var ulB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x - 1, id.y + 1, id.z)];
//                     var ulL = data[id.x - 1, id.y + 1, id.z];
//                     if (uB.x == 0 || lB.x == 0)
//                     {
//                         if (ulL != null && ulB.x == 0) feeds.Add(ulL.Value);
//                     }
//                 }
//                 //up front

//                 if (id.z != 0)
//                 {
//                     var fB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x, id.y, id.z - 1)];
//                     var ufB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x, id.y + 1, id.z - 1)];
//                     var ufL = data[id.x, id.y + 1, id.z - 1];
//                     if (uB.x == 0 || fB.x == 0)
//                     {
//                         if (ufL != null && ufB.x == 0) feeds.Add(ufL.Value);
//                     }
//                 }
//                 //up right

//                 if (id.x != Chunk.Width - 1)
//                 {
//                     var rB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x + 1, id.y, id.z)];
//                     var urB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x + 1, id.y + 1, id.z)];
//                     var urL = data[id.x + 1, id.y + 1, id.z];
//                     if (uB.x == 0 || rB.x == 0)
//                     {
//                         if (urL != null && urB.x == 0) feeds.Add(urL.Value);
//                     }
//                 }
//                 //up back

//                 if (id.z != Chunk.Width - 1)
//                 {
//                     var bB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x, id.y, id.z + 1)];
//                     var ubB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x, id.y + 1, id.z + 1)];
//                     var ubL = data[id.x, id.y + 1, id.z + 1];
//                     if (uB.x == 0 || bB.x == 0)
//                     {
//                         if (ubL != null && ubB.x == 0) feeds.Add(ubL.Value);
//                     }
//                 }
//             }
//             else
//             {
//                 var dB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x, id.y - 1, id.z)];
//                 // down
//                 if (id.y != 0 && dB.x == 0)
//                 {
//                     var dL = data[id.x, id.y - 1, id.z];
//                     if (dL != null) feeds.Add(dL.Value);
//                 }
//                 //up left

//                 if (id.x != 0)
//                 {
//                     var lB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x - 1, id.y, id.z)];
//                     var dlL = data[id.x - 1, id.y - 1, id.z];
//                     var dlB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x - 1, id.y - 1, id.z)];
//                     if (dB.x == 0 || lB.x == 0)
//                     {
//                         if (dlL != null && dlB.x == 0) feeds.Add(dlL.Value);
//                     }
//                 }
//                 //up front

//                 if (id.z != 0)
//                 {
//                     var dfB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x, id.y - 1, id.z - 1)];
//                     var fB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x, id.y, id.z - 1)];
//                     var dfL = data[id.x, id.y - 1, id.z - 1];
//                     if (dB.x == 0 || fB.x == 0)
//                     {
//                         if (dfL != null && dfB.x == 0) feeds.Add(dfL.Value);
//                     }
//                 }
//                 //up right


//                 if (id.x != Chunk.Width - 1)
//                 {
//                     var drB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x + 1, id.y - 1, id.z)];
//                     var rB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x + 1, id.y, id.z)];
//                     var drL = data[id.x + 1, id.y - 1, id.z];
//                     if (dB.x == 0 || rB.x == 0)
//                     {
//                         if (drL != null && drB.x == 0) feeds.Add(drL.Value);
//                     }
//                 }
//                 //up back

//                 if (id.z != Chunk.Width - 1)
//                 {
//                     var dbB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x, id.y - 1, id.z + 1)];
//                     var bB = WorldSettings.Chunks[cp].blocks[Utils.to1D(id.x, id.y, id.z + 1)];
//                     var dbL = data[id.x, id.y - 1, id.z + 1];
//                     if (dB.x == 0 || bB.x == 0)
//                     {
//                         if (dbL != null && dbB.x == 0) feeds.Add(dbL.Value);
//                     }
//                 }
//             }
//             int airCount = GetNearAirBlocksCount(cp, id);
//             float? lightValue = GetBlockLightValue(feeds, airCount);
//             return lightValue;
//         }

//         private int GetNearAirBlocksCount(int4 cp, int3 id)
//         {
//             var count = 0;
//             if (Utils.GetLeftBlock(id, cp).x == 0) count++;
//             if (Utils.GetRightBlock(id, cp).x == 0) count++;
//             if (Utils.GetFrontBlock(id, cp).x == 0) count++;
//             if (Utils.GetBackBlock(id, cp).x == 0) count++;
//             if (Utils.GetTopBlock(id, cp).x == 0) count++;
//             if (Utils.GetBottomBlock(id, cp).x == 0) count++;
//             return count;
//         }
//         private float? GetBlockLightValue(List<float?> feeders, int airCount)
//         {
//             float? max = null;
//             float? feedersSum = null;
//             if (feeders.Count > 0)
//             {
//                 foreach (float? feed in feeders)
//                 {
//                     if (feed == null) continue;
//                     if (max == null) max = 0;
//                     if (feed > max) max = feed;
//                     if (feedersSum == null) feedersSum = 0;
//                     feedersSum += feed;
//                 }
//                 // feedersAverage /= feeders.Count;
//             }
//             if (max == null)
//             {
//                 return max;
//             }
//             else
//             {
//                 switch (airCount)
//                 {
//                     case 6: return max;
//                     case 5: return max * 0.95f;
//                     case 4: return max * 0.8f;
//                     case 3: return max * 0.7f;
//                     case 2: return max * 0.6f;
//                     case 1: return max * 0.5f;
//                     default: return 0f;
//                 }
//             }
//         }
//     }
// }