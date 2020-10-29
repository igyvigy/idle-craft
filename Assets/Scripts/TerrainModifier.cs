using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;

public class TerrainModifier : MonoBehaviour
{


    private GamepadInputManager inputManager;
    private SelectionController selectionController;
    private Inventory inventory;
    public LayerMask groundLayer;
    private Player player;
    float maxDist = 4;
    void Start()
    {
        inputManager = TagResolver.i.inputManager;
        selectionController = TagResolver.i.selectionController;
        inventory = TagResolver.i.inventory;
        player = TagResolver.i.player;
    }

    private bool lastBuildValue;
    private bool lastDestroyValue;


    void Update()
    {
        bool buildValue = inputManager.BuildValue;
        bool destroyValue = inputManager.DestroyValue;
 
        if (inventory.isInventoryUIVisible) return;

        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, maxDist, groundLayer))
        {
            if (destroyValue && destroyValue == lastDestroyValue)
            {
                return;
            }
            if (buildValue && buildValue == lastBuildValue)
            {
                return;
            }
            Vector3 pointInTargetBlock;

            //destroy
            if (destroyValue)
                pointInTargetBlock = hitInfo.point + transform.forward * .01f;
            else if (buildValue || Input.GetKey(KeyCode.LeftShift))
                pointInTargetBlock = hitInfo.point - transform.forward * .01f;
            else
            {
                pointInTargetBlock = hitInfo.point + transform.forward * .01f;
            }

            int4 chunkPos = Utils.ChunkPosbyPosition(pointInTargetBlock);

            Chunk chunk = WorldSettings.Chunks[chunkPos];

            var coord = Utils.CoordByPosition(pointInTargetBlock);
            int index = Utils.to1D(coord);
            var block = chunk.blocks[index];
            var blockPos = Utils.WorldBlockPosition(coord, chunkPos);

            selectionController.SelectBlockAt(block, blockPos);

            if (destroyValue)
            {
                if (chunk.CanDestroyBlockAt(index))
                {
                    player.aIController.AddOrRemoveBlock(blockPos, block);
                }
            }
            else if (buildValue)
            {
                if (inventory.HasSelectedItem() && !CheckIfPlayerIsOnIndex(coord))
                {
                    Item selectedItem = inventory.GetSelectedItem();
                    Block? newBlock = Item.GetWoldBLock(selectedItem.type, selectedItem.level);
                    if (newBlock != null && chunk.BuildBlock(newBlock.Value, index))
                    {
                        inventory.ReduceSelectedBlockAmount();
                    }
                }
            }
        }
        else
        {
            selectionController.Deselect();
        }

        lastBuildValue = buildValue;
        lastDestroyValue = destroyValue;
    }

    bool CheckIfPlayerIsOnIndex(int3 id)
    {
        int3 cameraPositionCoord = Utils.CoordByPosition(transform.position);
        return cameraPositionCoord.Equals(id) ||
        new Index3D(cameraPositionCoord.x, cameraPositionCoord.y - 1, cameraPositionCoord.z).Equals(id);
    }

}

