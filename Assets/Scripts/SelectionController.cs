using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

public class SelectionController : MonoBehaviour
{
    struct DebugLabel
    {
        public Vector3 position;
        public string text;
        public DebugLabel(Vector3 position, string text) { this.position = position; this.text = text; }
    }
    [SerializeField] Transform pfSelection;
    [SerializeField] Window_HealthBar healthBarUI;
    public delegate void SelectionControllerDidDestroyBlock();
    public GameObject selection { get; private set; }
    private int3? currentPos;
    private int3? selectedPos;
    int3? block;
    Health blockHealth;
    DateTime? startedAt;
    DebugLabel? debugLabel;

    bool _isDestroyingBlck = false;

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (debugLabel != null)
        {
            UnityEditor.Handles.Label(debugLabel.Value.position, debugLabel.Value.text);
        }
#endif
    }

    private Vector3 GetPosition(int3 pos)
    {
        return new Vector3(pos.x + 0.5f, pos.y + 0.5f, pos.z + 0.5f);
    }
    public void ShowHealthForBlock(int3 block, int3 pos)
    {
        if (blockHealth == null)
        {
            blockHealth = new Health();
            healthBarUI.SubscribeOnHealth(blockHealth);
        }
        blockHealth.SetMaxHealth(Block.GetMaxHealth(block));
        blockHealth.SetCurrentHealth(Block.GetMaxHealth(block));
        blockHealth.SetLevel(block.y);
        blockHealth.Show(GetPosition(pos));
    }
    public void SelectBlockAt(int3 block, int3 pos)
    {
        if (selection != null)
        {
            selection.transform.position = GetPosition(pos);
        }
        else
        {
            CreateNewSelectionObject(block, GetPosition(pos));
        }

        this.selectedPos = pos;
        this.block = block;

        ShowHealthForBlock(block, pos);
    }
    private void CreateNewSelectionObject(int3 block, Vector3 position)
    {
        selection = Instantiate(pfSelection, position, Quaternion.identity).gameObject;
        selection.transform.SetParent(gameObject.transform);
        selection.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
    }

    private void Reset()
    {
        if (blockHealth != null)
        {
            blockHealth.Hide();
        }
        block = null;
        currentPos = null;
        selectedPos = null;
        startedAt = null;
        _isDestroyingBlck = false;
    }

    public void Deselect()
    {
        Destroy(selection);
        selection = null;
        Reset();
    }

    public bool isDestroyingBlock()
    {
        return _isDestroyingBlck;
    }

    void StartDestroyingSelectedBlock(SelectionControllerDidDestroyBlock onDestroy)
    {
        if (selectedPos == null || block == null || blockHealth == null)
        {
            Reset();
            return;
        }
        else if (currentPos == null)
        {
            currentPos = selectedPos;
            startedAt = DateTime.Now;
            blockHealth.SetCurrentHealth(Block.GetMaxHealth(block.Value));
            StartCoroutine(DestroyBlock(onDestroy));
        }
        else if (currentPos.Equals(selectedPos))
        {
            return;
        }
        else
        {
            currentPos = selectedPos;
            startedAt = DateTime.Now;
            blockHealth.SetCurrentHealth(Block.GetMaxHealth(block.Value));
            StartCoroutine(DestroyBlock(onDestroy));
        }
    }

    float damage = 1f;
    float attackSpeed = 0.1f; // in seconds
    float critChance = 30f; // in percent
    float critPower = 3f; // times stronger
    IEnumerator DestroyBlock(SelectionControllerDidDestroyBlock onDestroy)
    {
        if (blockHealth == null)
        {
            Reset();
            yield break;
        }
        while (blockHealth.currentHealth > 0)
        {
            _isDestroyingBlck = true;
            if (currentPos == null || selectedPos == null || block == null || blockHealth == null || !currentPos.Equals(selectedPos))
            {
                Reset();
                yield break;
            }
            bool crit = UnityEngine.Random.Range(0f, 100f) > (100 - critChance);
            float dmg = crit ? damage * critPower : damage;
            DamagePopup.Create(selection.transform.position, dmg, crit, null);
            blockHealth.ModifyHealth(-dmg);
            yield return new WaitForSeconds(attackSpeed);
        }
        if (onDestroy != null && block != null && currentPos != null && selectedPos != null && currentPos.Equals(selectedPos)) onDestroy.Invoke();
        _isDestroyingBlck = false;
        Reset();
    }

    public void ModifyHealth(int3 worldPos, int3 block, int3 health)
    {
        if (this.selectedPos == null || this.block == null) return;
        if (!this.selectedPos.Equals(worldPos)) return;
        blockHealth.SetMaxHealth(health.y);
        blockHealth.SetCurrentHealth(health.x);
        blockHealth.ModifyHealth(health.z);
    }
}
