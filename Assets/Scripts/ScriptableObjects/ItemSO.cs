using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Bag Item", menuName = "Bag Item")]
public class ItemSO : ScriptableObject
{
    public ItemType type;
    public new string name;
    public string description;
    public Sprite image;
    public int level;
    public int amount;
}