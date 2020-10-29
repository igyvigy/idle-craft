using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets i
    {
        get
        {
            return Singleton<GameAssets>.Instance;
        }
    }
    public Material mBlue;
    public Material mRed;
    public Material mDirt;
    public Material mBlockUI;
    public Material mBlockIUSelected;
    public RectTransform pfDamagePopup;
    public RectTransform pfDebugLabel;
    public HealthBar pfHealthBar;
    public Transform pfChunk;
    public Transform pfWall;
    public Transform pfRespawn;
    public Transform pfBlockUI;
    public Transform pfPickupItem;
}
