using System;
using UnityEngine;

public interface IHealthOwner
{
    float GetMaxHealth();
    int GetLevel();
}

public class Health
{
    public event Action<Health> OnHealthAdded = delegate { };
    public event Action<Health> OnHealthRemoved = delegate { };
    public event Action<Health> OnReset = delegate { };
    public event Action<float> OnHealthPercentChaged = delegate { };
    public event Action<int> OnLevelChaged = delegate { };
    public float currentHealth { get; private set; }
    public float maxHealth { get; private set; }
    public int level;
    public Vector3? position;
    public Transform target;

    public Health()
    {

    }

    public void Show(Vector3? position = null, Transform target = null)
    {
        this.position = position;
        this.target = target;
        OnHealthAdded(this);
    }

    public void Hide()
    {
        OnHealthRemoved(this);
    }


    public void SetCurrentHealth(float value)
    {
        currentHealth = value;
        ModifyHealth(0);
    }

    public void SetMaxHealth(float value)
    {
        maxHealth = value;
        ModifyHealth(0);
    }

    public void SetLevel(int value)
    {
        level = value;
        OnLevelChaged(level);
    }

    public void ModifyHealth(float amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        if (currentHealth == 0)
        {
            OnHealthRemoved(this);
        }
        else if (currentHealth > 0)
        {
            OnHealthAdded(this);
        }
        float healthPercent = currentHealth / maxHealth;
        OnHealthPercentChaged(healthPercent);
    }
}


