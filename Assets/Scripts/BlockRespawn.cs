using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class BlockRespawn : MonoBehaviour
{
    public static BlockRespawn Create(Vector3 position, DateTime deadline)
    {
        Transform blockRespawnTransform = Instantiate(GameAssets.i.pfRespawn, position, Quaternion.identity);
        BlockRespawn blockRespawn = blockRespawnTransform.GetComponent<BlockRespawn>();
        blockRespawn.Setup(position, deadline);
        return blockRespawn;
    }
    public TextMeshPro textMesh;
    public DateTime start;
    public DateTime deadline;
    public TimeSpan originalInterval;
    private float disappearTimer;
    private Color textColor;
    private Vector3 position;

    private string MakeTimeString()
    {
        TimeSpan t = deadline - DateTime.Now;
        if (t.Days > 0) return t.ToString(@"d\.hh\:mm\:ss\.f");
        else if (t.Hours > 0) return t.ToString(@"hh\:mm\:ss\.f");
        else if (t.Minutes > 0) return t.ToString(@"mm\:ss\.f");
        else return t.ToString(@"ss\.f");
    }

    public float SecondsTillDeadline
    {
        get
        {
            return (float)(deadline - DateTime.Now).TotalSeconds;
        }
    }
    void Awake()
    {
        textMesh = transform.Find("Text").GetComponent<TextMeshPro>();
    }
    public void Setup(Vector3 position, DateTime deadline)
    {
        this.start = DateTime.Now;
        this.deadline = deadline;
        this.originalInterval = deadline - start;
        this.position = position;

        textMesh.color = Colors.green;
        textMesh.SetText(MakeTimeString());
        textMesh.transform.LookAt(CameraSettings.CurrentCamera.transform);
    }

    void LateUpdate()
    {
        if (SecondsTillDeadline > originalInterval.TotalSeconds * .6f)
        {
            textMesh.color = Colors.green;
        }
        else if (SecondsTillDeadline > originalInterval.TotalSeconds * .2f)
        {
            textMesh.color = Colors.orange;
        }
        else
        {
            textMesh.color = Colors.red;
        }
        textMesh.SetText(MakeTimeString());
        textMesh.transform.LookAt(CameraSettings.CurrentCamera.transform);
        // if (SecondsTillDeadline < 0)
        // {
        //     Destroy(gameObject);
        // }
    }
}
