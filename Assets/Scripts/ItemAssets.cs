using UnityEngine;
public class ItemAssets : MonoBehaviour
{
    public static ItemAssets i { get; private set; }

    private void Awake()
    {
        i = this;
    }
    public Sprite grassBlock;
    public Sprite dirtBlock;
    public Sprite stoneBlock;
    public Sprite trunkBlock;
    public Sprite leafBlock;
}
