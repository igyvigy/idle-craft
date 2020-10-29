using System;
using UnityEngine;

[Serializable]
public struct PlayerStats
{
    public int level;
    public float destroyBlockAttackSpeed;
    public float destroyBlockDamage;
    public float destroyBlockCritMultiplier;
    public float destroyBlockAttackCritChance;
    public float destroyBlockRange;
    public static PlayerStats MakeDefault()
    {
        var ps = new PlayerStats();
        ps.level = 1;
        ps.destroyBlockAttackSpeed = 0.1f;
        ps.destroyBlockDamage = 1f;
        ps.destroyBlockCritMultiplier = 4f;
        ps.destroyBlockAttackCritChance = 30f;
        ps.destroyBlockRange = 0.5f;
        return ps;
    }
}

public class StatsController : MonoBehaviour
{
    PlayerStats playerStats = PlayerStats.MakeDefault();

    public PlayerStats GetStats()
    {
        return playerStats;
    }
}