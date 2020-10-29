using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public static DamagePopup Create(Vector3 position, float damage, bool isCriticalHit, Color? color)
    {
        Window_Pointer pointerUI = TagResolver.i.gameManager.pointerUI;
        Transform damagePopupTransform = Instantiate(GameAssets.i.pfDamagePopup, position, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup((int)damage, isCriticalHit, color);
        damagePopup.transform.SetParent(pointerUI.transform);
        damagePopup.transform.LookAt(CameraSettings.CurrentCamera.transform);
        damagePopup.transform.Rotate(0, 180, 0);
        damagePopup.transform.position = position + (CameraSettings.CurrentCamera.transform.position - position).normalized * 0.73f;
        damagePopup.isCritical = isCriticalHit;
        return damagePopup;
    }

    private const float DISAPPEAR_TIMER_MAX = 1f;

    private static VertexSortingOrder sortingOrder;

    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;
    private Color color;
    private bool isCritical;
    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    float GetFontSize()
    {
        float dist = Vector3.Distance(transform.position, CameraSettings.CurrentCamera.transform.position) / 6;
        return isCritical ? 2 * dist : dist;
    }

    public void Setup(int damageAmount, bool isCriticalHit, Color? color)
    {
        if (isCriticalHit)
        {
            textMesh.SetText(damageAmount.ToString() + "!");
            // textMesh.fontSize = 10;
            textColor = Helpers.GetColorFromString("F55442");
        }
        else
        {
            textMesh.SetText(damageAmount.ToString());
            // textMesh.fontSize = 5;
            textColor = Helpers.GetColorFromString("f5bc42");
        }
        textMesh.color = textColor;
        if (color != null) textMesh.color = color.Value;
        disappearTimer = DISAPPEAR_TIMER_MAX;

        sortingOrder++;
        textMesh.geometrySortingOrder = sortingOrder;

        moveVector = new Vector3(UnityEngine.Random.Range(-1f, 1f), 2);
    }

    private void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector.y -= 6f * Time.deltaTime;
        textMesh.fontSize = GetFontSize();
        disappearTimer -= Time.deltaTime;

        if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f)
        {
            // first half of popup lifetime
            float increaseScaleAmount = 1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            // second half
            float increaseScaleAmount = 1f;
            transform.localScale -= Vector3.one * increaseScaleAmount * Time.deltaTime;
        }

        if (disappearTimer < 0)
        {
            // start disappearing
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }

}

