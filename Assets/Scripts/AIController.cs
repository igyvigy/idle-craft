using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public enum AIStateMode
{
    Idle, Ready, MoveToTarget, DestroyBlock
}

[Serializable]
public struct AIState
{
    public AIStateMode mode;
    public static AIState MakeDefault()
    {
        var aiState = new AIState();
        aiState.mode = AIStateMode.Idle;
        return aiState;
    }
}

[Serializable]
public class AIController : MonoBehaviour
{
    [Header("dev controls")]
    public bool isSelecting;
    public bool isMoving;
    public bool isFindingPath;
    public bool clearAll;

    public bool isOnManualPathFinding = false;
    public List<BlockUI> blocksUI = new List<BlockUI>();
    public PathFinding currentPathFinding;
    public BlockDestroying currentBlockDestroying;
    bool didStartPathFinding = false;

    [Header("other")]

    public AIState aiState = AIState.MakeDefault();

    SelectionController selectionController;
    PlayerMovement playerMovement;
    StatsController statsController;

    public bool isDestroyingBlock { get { return aiState.mode == AIStateMode.DestroyBlock; } }
    public bool isIdleing { get { return aiState.mode == AIStateMode.Idle; } }
    public bool isReady { get { return aiState.mode == AIStateMode.Ready; } }
    public bool isMovingToTarget { get { return aiState.mode == AIStateMode.MoveToTarget; } }
    public int3x3? current = null;
    private Inventory inventory;
    void Start()
    {
        selectionController = TagResolver.i.selectionController;
        playerMovement = GetComponent<PlayerMovement>();
        statsController = GetComponent<StatsController>();
        inventory = TagResolver.i.inventory;
        TimeTickSystem.OnTick += TimeTickSystem_OnTick;
    }

    void OnGUI()
    {

        if (currentPathFinding != null && currentPathFinding.pathNodes.Count > 0)
        {
            Rect screenRect = new Rect(Screen.width - 160, 25, 150, 25 * (currentPathFinding.pathNodes.Count + 1));
            GUILayout.BeginArea(screenRect);
            foreach (var node in currentPathFinding.pathNodes)
            {
                if (GUILayout.RepeatButton(String.Format("{0} {1} {2}", node.startValue, node.value, node.endValue)))
                {
                    BlockUI bUI = blocksUI.Find(bui => bui.pos == node.pos);

                    bUI.ToggleSelection();
                }
            }
            clearAll = GUILayout.Toggle(clearAll, " Clear All");
            GUILayout.EndArea();
        }
    }

    public void HandlePathFindingInput(Vector3 blockCenter, int3 block)
    {
        if (!didStartPathFinding)
        {
            didStartPathFinding = true;
            Vector3 pos = Utils.CentrifyPosition(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z));
            currentPathFinding = new PathFinding(pos, blockCenter);
            if (isOnManualPathFinding)
            {
                var moves = currentPathFinding.OpenPossibleNewMoves(pos);
                RemoveFromBlocksUIIfExists(moves);
                foreach (var node in moves)
                {
                    var bUI = BlockUI.Create(Utils.WorldBlockPosForCenter(node.pos), int3.zero, String.Format("{0}\n{1}\n{2}", node.startValue, node.value, node.endValue));
                    blocksUI.Add(bUI);
                }
                var sbUI = BlockUI.Create(Utils.WorldBlockPosForCenter(pos), int3.zero, "Start");
                sbUI.ToggleSelection();
                blocksUI.Add(sbUI);
                var fbUI = BlockUI.Create(Utils.WorldBlockPosForCenter(blockCenter), int3.zero, "Finish");
                fbUI.ToggleSelection();
                blocksUI.Add(fbUI);
            }
            else
            {
                StartCoroutine(currentPathFinding.BuildPathToTarget(() =>
                {
                    if (currentPathFinding.isClosed)
                    {
                        foreach (var bbbUI in blocksUI) GameObject.Destroy(bbbUI.gameObject);
                        foreach (var node in currentPathFinding.pathNodes)
                        {
                            var bbUI = BlockUI.Create(Utils.WorldBlockPosForCenter(node.pos), int3.zero, String.Format("{0}\n{1}\n{2}", node.startValue, node.value, node.endValue));
                            blocksUI.Add(bbUI);
                        }
                        var sbUI = BlockUI.Create(Utils.WorldBlockPosForCenter(currentPathFinding.startPos), int3.zero, "Start");
                        sbUI.ToggleSelection();
                        blocksUI.Add(sbUI);
                        var fbUI = BlockUI.Create(Utils.WorldBlockPosForCenter(currentPathFinding.endPos), int3.zero, "Finish");
                        fbUI.ToggleSelection();
                        blocksUI.Add(fbUI);
                    }
                    else
                    {
                        var poppedBottom = currentBlockDestroying.PopBottomBlock();
                        selectionController.Deselect();
                        aiState.mode = AIStateMode.Idle;
                        currentBlockDestroying.UpdateDestroyBlockQueueUI();
                    }
                }));

            }
        }
        else if (blocksUI.Find(b => b.pos.Equals(blockCenter)) != null)
        {

            var n = currentPathFinding.SuggestNextNode();
            var bUI = blocksUI.Find(b => b.pos.Equals(n.pos));
            bUI.ToggleSelection();
            var moves = currentPathFinding.OpenPossibleNewMoves(bUI.pos, n);

            RemoveFromBlocksUIIfExists(moves);
            foreach (var node in moves)
            {
                var bbUI = BlockUI.Create(Utils.WorldBlockPosForCenter(node.pos), int3.zero, String.Format("{0}\n{1}\n{2}", node.startValue, node.value, node.endValue));
                blocksUI.Add(bbUI);
            }
            if (currentPathFinding.CheckIfEnd(n))
            {
                foreach (var bbbUI in blocksUI) GameObject.Destroy(bbbUI.gameObject);
                foreach (var node in currentPathFinding.pathNodes)
                {
                    var bbUI = BlockUI.Create(Utils.WorldBlockPosForCenter(node.pos), int3.zero, String.Format("{0}\n{1}\n{2}", node.startValue, node.value, node.endValue));
                    blocksUI.Add(bbUI);
                }
                var sbUI = BlockUI.Create(Utils.WorldBlockPosForCenter(currentPathFinding.startPos), int3.zero, "Start");
                sbUI.ToggleSelection();
                blocksUI.Add(sbUI);
                var fbUI = BlockUI.Create(Utils.WorldBlockPosForCenter(currentPathFinding.endPos), int3.zero, "Finish");
                fbUI.ToggleSelection();
                blocksUI.Add(fbUI);
            }
        }
        else
        {
            didStartPathFinding = false;
            foreach (var bUI in blocksUI) GameObject.Destroy(bUI.gameObject);
            blocksUI = new List<BlockUI>();
        }

    }

    void RemoveFromBlocksUIIfExists(List<PathNode> nodes)
    {
        var toRemove = new List<BlockUI>();
        foreach (var _bUI in blocksUI)
            foreach (var node in nodes)
                if (_bUI.pos.Equals(node.pos)) toRemove.Add(_bUI);
        foreach (var __bUI in toRemove)
        {
            blocksUI.Remove(__bUI);
            GameObject.Destroy(__bUI.gameObject);
        }
    }

    void RemoveFromBlocksUIIfExists(PathNode node)
    {
        var toRemove = new List<BlockUI>();
        foreach (var _bUI in blocksUI)
            if (_bUI.pos.Equals(node.pos)) toRemove.Add(_bUI);
        foreach (var __bUI in toRemove)
        {
            blocksUI.Remove(__bUI);
            GameObject.Destroy(__bUI.gameObject);
        }
    }

    void ModifyBlockHealth(int3x3 was, int index, int3 newLifetime)
    {
        currentBlockDestroying.blocksToDestroy[index] = new int3x3(was.c0, was.c1, newLifetime);
        selectionController.ModifyHealth(was.c0, was.c1, newLifetime);
    }

    void Update()
    {
        if (clearAll)
        {
            clearAll = false;
            currentBlockDestroying.ClearQueue();
            aiState.mode = AIStateMode.Idle;
            currentPathFinding.Clear();
            didStartPathFinding = false;
            foreach (var bUI in blocksUI) GameObject.Destroy(bUI.gameObject);
            blocksUI = new List<BlockUI>();
        }
        if (!currentBlockDestroying.hasBlocksToDestroy)
        {
            selectionController.Deselect();
        }
        if (currentBlockDestroying.hasBlocksToDestroy && isIdleing)
        {
            int3x3 data = currentBlockDestroying.nextBlockData.Value;
            int3 block = currentBlockDestroying.nextBlock.Value;
            int3 blockHealth = currentBlockDestroying.nextBlockHealth.Value;
            int3 blockWorldPos = currentBlockDestroying.nextBlockWorldPos.Value;
            Vector3 blockCenter = Utils.CenterOfBlockWithWorldPos(blockWorldPos);
            if (Utils.IsTargetReachable(transform.position, blockCenter))
            {
                // start destroying
                int maxHp = Mathf.RoundToInt(Block.GetMaxHealth(block));
                ModifyBlockHealth(data, 0, new int3(maxHp, maxHp, 0));
                int3 coord = Utils.CoordByPosition(blockCenter);
                selectionController.SelectBlockAt(block, blockWorldPos);
                StartCoroutine(DestroyBlock(() =>
                {
                    int4 chunkPos = Utils.ChunkPosbyPosition(blockCenter);
                    Chunk chunk = WorldSettings.Chunks[chunkPos];
                    int index = Utils.to1D(coord);
                    if (chunk.CanDestroyBlockAt(index))
                    {
                        Block b = chunk.DestroyBlockAt(index);
                        foreach (Item i in b.DropItems())
                        {
                            inventory.Add(i);
                        }
                    };
                }));
            }
            else
            {
                //move closer
                aiState.mode = AIStateMode.MoveToTarget;
                Vector3? closestPathPos = null;
                var availablePathNodes = PathFinding.ClosestPathBlockTowardsPos(blockCenter, transform.position);
                if (availablePathNodes.Count > 0) closestPathPos = availablePathNodes[0].pos;
                if (closestPathPos != null)
                {
                    var pathFinding = new PathFinding(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), closestPathPos.Value);
                    StartCoroutine(pathFinding.BuildPathToTarget(() =>
                    {
                        if (pathFinding.isClosed)
                        {
                            aiState.mode = AIStateMode.MoveToTarget;
                            playerMovement.ReachToPoint(closestPathPos.Value, pathFinding);
                        }
                        else
                        {
                            var poppedBottom = currentBlockDestroying.PopBottomBlock();
                            selectionController.Deselect();
                            aiState.mode = AIStateMode.Idle;
                            currentBlockDestroying.UpdateDestroyBlockQueueUI();
                        }
                    }));

                }
                else
                {
                    var poppedBottom = currentBlockDestroying.PopBottomBlock();
                    selectionController.Deselect();
                    aiState.mode = AIStateMode.Idle;
                    currentBlockDestroying.UpdateDestroyBlockQueueUI();
                }
            }
        };
    }

    private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        if (aiState.mode == AIStateMode.DestroyBlock &&
            currentBlockDestroying.nextBlockHealth != null &&
            current != null &&
            currentBlockDestroying.nextBlockHealth.Value.x > 0 &&
            currentBlockDestroying.nextBlockWorldPos.Equals(current.Value.c0))
        {
            int3x3 data = currentBlockDestroying.nextBlockData.Value;
            int3 block = currentBlockDestroying.nextBlock.Value;
            int3 blockHealth = currentBlockDestroying.nextBlockHealth.Value;
            int3 blockWorldPos = currentBlockDestroying.nextBlockWorldPos.Value;
            var regen = Block.GetRegen(block);
            if (regen > 0)
            {
                DamagePopup.Create(Utils.CenterOfBlockWithWorldPos(blockWorldPos), regen, false, Colors.green);
                ModifyBlockHealth(data, 0, new int3(blockHealth.x + Mathf.RoundToInt(regen), blockHealth.y, Mathf.RoundToInt(regen)));
            }
        }
    }

    IEnumerator DestroyBlock(IThirdPersonSelectCallback onDestroy)
    {
        aiState.mode = AIStateMode.DestroyBlock;
        while (currentBlockDestroying.hasBlocksToDestroy)
        {
            if (currentBlockDestroying.nextBlockData != null && current == null) current = new int3x3(currentBlockDestroying.nextBlockWorldPos.Value, currentBlockDestroying.nextBlock.Value, currentBlockDestroying.nextBlockHealth.Value);

            if (currentBlockDestroying.nextBlockHealth != null &&
            current != null &&
            currentBlockDestroying.nextBlockHealth.Value.x <= 0 &&
            currentBlockDestroying.nextBlockWorldPos.Equals(current.Value.c0))
            {
                // block is dead
                var poppedBottom = currentBlockDestroying.PopBottomBlock();
                selectionController.Deselect();
                onDestroy.Invoke();
                aiState.mode = AIStateMode.Idle;
                currentBlockDestroying.UpdateDestroyBlockQueueUI();
                yield break;
            }
            else if (currentBlockDestroying.nextBlockHealth != null &&
            current != null &&
            currentBlockDestroying.nextBlockHealth.Value.x > 0 &&
            currentBlockDestroying.nextBlockWorldPos.Equals(current.Value.c0))
            {
                // block has health. Hit and wait
                int3x3 data = currentBlockDestroying.nextBlockData.Value;
                current = new int3x3(currentBlockDestroying.nextBlockWorldPos.Value, currentBlockDestroying.nextBlock.Value, currentBlockDestroying.nextBlockHealth.Value);
                int3 block = currentBlockDestroying.nextBlock.Value;
                int3 blockWorldPos = currentBlockDestroying.nextBlockWorldPos.Value;
                int3 blockHealth = currentBlockDestroying.nextBlockHealth.Value;

                bool crit = UnityEngine.Random.Range(0f, 100f) > (100 - statsController.GetStats().destroyBlockAttackCritChance);
                float dmg = crit ? statsController.GetStats().destroyBlockDamage * statsController.GetStats().destroyBlockCritMultiplier : statsController.GetStats().destroyBlockDamage;
                DamagePopup.Create(Utils.CenterOfBlockWithWorldPos(blockWorldPos), dmg, crit, null);
                ModifyBlockHealth(data, 0, new int3(blockHealth.x - Mathf.RoundToInt(dmg), blockHealth.y, -Mathf.RoundToInt(dmg)));

                yield return new WaitForSeconds(statsController.GetStats().destroyBlockAttackSpeed);

            }
            else if (currentBlockDestroying.nextBlockHealth != null &&
            current != null &&
            currentBlockDestroying.nextBlockHealth.Value.x > 0 &&
            !currentBlockDestroying.nextBlockWorldPos.Equals(current.Value.c0))
            {
                //current block changed
                current = new int3x3(currentBlockDestroying.nextBlockWorldPos.Value, currentBlockDestroying.nextBlock.Value, currentBlockDestroying.nextBlockHealth.Value);
                aiState.mode = AIStateMode.Idle;
 
                yield break;
            }
            else
            {

                aiState.mode = AIStateMode.Idle;
                if (current != null)
                {
                    Debug.Log("else current: " + current.Value.c0 + " " + current.Value.c1 + " " + current.Value.c2);
                }
                yield break;
            }

        }
        aiState.mode = AIStateMode.Idle;

    }

    public void AddOrRemoveBlock(int3 blockWorldpos, int3 block)
    {
        currentBlockDestroying.AddOrRemoveBlock(blockWorldpos, block);
    }
}