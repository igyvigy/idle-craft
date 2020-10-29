using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class LightData : MonoBehaviour
{
    [HideInInspector] public int4 lastUpdatedCP;
    public static int4 LastUpdatedCP
    {
        get
        {
            return instance.lastUpdatedCP;
        }
    }
    public static void SetLastUpdatedCP(int4 cp)
    {
        instance.lastUpdatedCP = cp;
    }
    [HideInInspector] public int4? rebuildCP;
    public static int4? RebuildCP
    {
        get
        {
            return instance.rebuildCP;
        }
    }
    public static void SetRebuildCP(int4? cp)
    {
        instance.rebuildCP = cp;
    }
    [HideInInspector] public Dictionary<int4, float?[,,]> data = new Dictionary<int4, float?[,,]>();
    public static Dictionary<int4, float?[,,]> Data
    {
        get
        {
            return instance.data;
        }
    }

    static LightData instance;
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }


}
