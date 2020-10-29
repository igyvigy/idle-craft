using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Item Slot", menuName = "Item Slot")]
public class ItemSlotSO : ScriptableObject
{
    public ItemSlotPositionType type;
    public int index;
}