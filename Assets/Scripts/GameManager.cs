using UnityEngine;
using Unity.Entities;

public class GameManager : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    public Window_Pointer pointerUI;
    private Inventory inventory;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        throw new System.NotImplementedException();
    }
    void Awake()
    {
        inventory = TagResolver.i.inventory;
        PlayerData pd = LoadManager.Load();
        if (pd != null)
        {
            Player player = TagResolver.i.player;
            player.transform.position = LoadManager.GetPlayerPosition();
            player.transform.rotation = Quaternion.Euler(0, pd.playerRotationY, 0);
        }
    }
}
