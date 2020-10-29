using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThirdPersonSelect : MonoBehaviour
{
    Player player;
    public float selectionDistance = 200f;
    public float rotationSpeed = 4f;
    SelectionController selectionController;
    Inventory inventory;
    PlayerMovement playerMovement;
    AIController playerAi;
    Vector3 pointerDownPoint;
    public LayerMask selectionLayerMask;

    void Start()
    {
        selectionController = TagResolver.i.selectionController;
        inventory = TagResolver.i.inventory;
        player = TagResolver.i.player;
        playerMovement = player.GetComponent<PlayerMovement>();
        playerAi = player.GetComponent<AIController>();
    }

    void Update()
    {
        float pointerDelta = math.min(Screen.height, Screen.width) / 500;
        if (CameraSettings.isThirdPerson)
        {
            if (Input.GetMouseButtonDown(0))
            {
                pointerDownPoint = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0))
            {
                bool pointerReleasedNotTooFarAway = (math.abs(Vector3.Distance(Input.mousePosition, pointerDownPoint)) <= pointerDelta);
                if (pointerReleasedNotTooFarAway)
                {
                    if (Utils.IsPointerOverUI())
                    {
                        return;
                    }
                    RaycastHit hitInfo;
                    if (Physics.Raycast(CameraSettings.CurrentCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, 100, selectionLayerMask))
                    {
                        Vector3 pointInTargetBlock;
                        pointInTargetBlock = hitInfo.point + transform.forward * .01f;

                        if (playerAi.isSelecting)
                        {
                            if (Vector3.Distance(pointInTargetBlock, transform.position) > selectionDistance) return;
                            int4 chunkPos = Utils.ChunkPosbyPosition(pointInTargetBlock);
                            Chunk chunk = WorldSettings.Chunks[chunkPos];
                            var coord = Utils.CoordByPosition(pointInTargetBlock);
                            int index = Utils.to1D(coord);
                            var block = chunk.blocks[index];
                            var worldBlockPos = Utils.WorldBlockPosition(coord, chunkPos);
                            if (chunk.CanDestroyBlockAt(index))
                            {
                                player.aIController.AddOrRemoveBlock(worldBlockPos, block);
                            }
                        }
                        else if (playerAi.isFindingPath)
                        {
                            var cpCoord = Utils.ChunkPosAndCoordForPosition(pointInTargetBlock);
                            int index = Utils.to1D(cpCoord.Item2);
                            var block = WorldSettings.Chunks[cpCoord.Item1].blocks[index];
                            pointInTargetBlock = hitInfo.point - transform.forward * .01f;// move outsibe the block
                            player.aIController.HandlePathFindingInput(Utils.CentrifyPosition(pointInTargetBlock), block);
                        }
                        else if (playerAi.isMoving)
                        {
                            pointInTargetBlock = hitInfo.point + transform.forward * .01f;
                            playerMovement.MoveToPoint(pointInTargetBlock);
                        }
                    }
                    else
                    {
                        Debug.Log("Click on something");
                    }
                }
                else
                {
                    Debug.Log("delta " + pointerDelta + " dist " + math.abs(Vector3.Distance(Input.mousePosition, pointerDownPoint)));
                }
            }

        }
    }

}
