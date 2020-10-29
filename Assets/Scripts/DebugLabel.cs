using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugLabel : MonoBehaviour
{
    public static DebugLabel Create(Vector3 position, string text)
    {
        Window_Pointer pointerUI = TagResolver.i.gameManager.pointerUI;
        Transform damagePopupTransform = Instantiate(GameAssets.i.pfDebugLabel, position, Quaternion.identity);
        DebugLabel damagePopup = damagePopupTransform.GetComponent<DebugLabel>();
        damagePopup.SetText(text);
        damagePopup.transform.SetParent(pointerUI.transform);

        return damagePopup;
    }

    private static VertexSortingOrder sortingOrder;

    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    private bool isCritical;
    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    private void SetText(string text)
    {
        textMesh.SetText(text);
    }

}

