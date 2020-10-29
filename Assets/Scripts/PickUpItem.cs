using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public static PickUpItem SpawnPickUpItem(Item item, Vector3 position)
    {
        Transform transform = Instantiate(GameAssets.i.pfPickupItem, position, Quaternion.identity);
        PickUpItem pickUpItem = transform.GetComponent<PickUpItem>();
        pickUpItem.SetItem(item);
        return pickUpItem;
    }
    private Item item;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        transform.LookAt(CameraSettings.CurrentCamera.transform);
    }

    public void SetItem(Item item)
    {
        this.item = item;
        spriteRenderer.sprite = Item.GetSprite(item.type);
    }
}
