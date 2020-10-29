using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PlayerToEntityConversion : MonoBehaviour, IConvertGameObjectToEntity
{
    public float healthValue = 1f;

    public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
    {
        manager.AddComponent(entity, typeof(PlayerTag));

        HealthComponent health = new HealthComponent { Value = healthValue };
        manager.AddComponentData(entity, health);
        manager.AddComponentData(entity, new Translation() { Value = this.transform.position });
    }
}