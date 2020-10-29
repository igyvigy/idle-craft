using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class BlockUI : MonoBehaviour
{
    public static BlockUI Create(int3 blockWorldPos, int3 block, string text, bool isSelected = false)
    {
        Vector3 pos = Utils.CenterOfBlockWithWorldPos(blockWorldPos);
        Transform tr = Instantiate(GameAssets.i.pfBlockUI, pos, Quaternion.identity);
        BlockUI blockUI = tr.GetComponent<BlockUI>();
        blockUI.Setup(pos, block, text, isSelected);
        return blockUI;
    }
    private TextMeshProUGUI textLabel;

    public Vector3 pos;
    private int3 block;
    private string text;
    private float disappearTimer;
    private Color textColor;
    private Vector3 position;
    public bool isSelected = false;
    void Awake()
    {
        textLabel = transform.Find("Canvas").Find("Text").GetComponent<TextMeshProUGUI>();
    }
    public void Setup(Vector3 pos, int3 block, string text, bool isSelected = false)
    {
        this.pos = pos;
        this.block = block;
        this.text = text;
        this.isSelected = isSelected;
        textLabel.color = Colors.green;
        textLabel.SetText(text);
        transform.position = pos;
        textLabel.transform.position = CameraSettings.CurrentCamera.WorldToScreenPoint(pos);
    }

    public void ToggleSelection()
    {
        isSelected = !isSelected;
    }
    bool preSelected = false;
    void LateUpdate()
    {
        textLabel.transform.position = CameraSettings.CurrentCamera.WorldToScreenPoint(pos);
        if (preSelected != isSelected)
        {
            preSelected = isSelected;
            GetComponent<MeshRenderer>().material = isSelected ? GameAssets.i.mBlockIUSelected : GameAssets.i.mBlockUI;
        }
    }
}
