using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Image foregroundImage;

    [SerializeField]
    private TextMeshProUGUI levelLabel;

    [SerializeField]
    private float updateSpeedSeconds = 0.5f;
    [SerializeField]
    private float positionOffset = 1f;

    private Health health;

    private SelectionController sc;
    void Start()
    {
        sc = TagResolver.i.selectionController;
    }

    public void SetHealth(Health health)
    {
        this.health = health;
        this.health.OnHealthPercentChaged += HandleHealthPercentChanged;
        this.health.OnLevelChaged += SetLevel;
    }

    public void SetLevel(int level)
    {
        levelLabel.SetText(level.ToString());
    }
    private void HandleHealthPercentChanged(float healthPercent)
    {
        StartCoroutine(ChangeToPct(healthPercent));
    }

    private IEnumerator ChangeToPct(float pct)
    {
        float preChangePct = foregroundImage.fillAmount;
        float elapsed = 0f;

        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            foregroundImage.fillAmount = Mathf.Lerp(preChangePct, pct, elapsed / updateSpeedSeconds);
            yield return null;
        }

        foregroundImage.fillAmount = pct;
    }

    private void LateUpdate()
    {
        Vector3 pos = Vector3.zero;
        if (sc.selection != null) pos = sc.selection.transform.position;
        if (health.position != null) pos = health.position.Value;
        if (health.target != null) pos = health.target.position;
        transform.position = CameraSettings.CurrentCamera.WorldToScreenPoint(pos);
        // transform.position = CameraSettings.CurrentCameratransform.InverseTransformPoint(sc.selection.transform.position);
        // transform.LookAt(CameraSettings.CurrentCameratransform);
        // transform.Rotate(0, 180, 0);
    }

    private void OnDestroy()
    {
        health.OnHealthPercentChaged -= HandleHealthPercentChanged;
    }
}
