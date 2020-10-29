using System.Collections.Generic;
using UnityEngine;

public class Window_HealthBar : MonoBehaviour
{
    private Dictionary<Health, HealthBar> healthBars = new Dictionary<Health, HealthBar>();

    public void SubscribeOnHealth(Health health)
    {
        health.OnHealthAdded += AddHealthBar;
        health.OnHealthRemoved += RemoveHealthBar;
    }

    private void AddHealthBar(Health health)
    {
        if (!healthBars.ContainsKey(health))
        {
            var healthBar = Instantiate(GameAssets.i.pfHealthBar, transform);
            healthBars.Add(health, healthBar);
            healthBar.SetHealth(health);
        }
    }

    private void RemoveHealthBar(Health health)
    {
        if (healthBars.ContainsKey(health))
        {
            Destroy(healthBars[health].gameObject);
            healthBars.Remove(health);
        }
    }
}
