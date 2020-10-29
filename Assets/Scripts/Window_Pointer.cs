using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class PointerTarget
{
    public Transform transform;
    public Sprite sprite;
    public RectTransform pointer;

    public PointerTarget(Transform transform, Sprite sprite)
    {
        this.transform = transform;
        this.sprite = sprite;
    }
}

[System.Serializable]
public class Window_Pointer : MonoBehaviour
{
    [SerializeField] public List<PointerTarget> targets;
    [SerializeField] public Camera uiCamera;

    private Transform player;
    public void SetPlayer(Transform player)
    {
        this.player = player;
    }
    public void SetTargets(List<PointerTarget> targets)
    {
        this.targets = targets;
        if (this.targets == null || this.targets.Count == 0) return;
        var idx = 0;
        while (idx < this.targets.Count)
        {
            PointerTarget target = this.targets[idx];
            GameObject pointer = new GameObject();
            pointer.name = "Pointer";
            Image pointerImage = pointer.AddComponent<Image>();
            pointerImage.sprite = target.sprite;
            pointerImage.transform.localScale = Vector3.one * 0.2f;
            pointer.GetComponent<RectTransform>().SetParent(transform);
            target.pointer = pointer.GetComponent<RectTransform>();

            idx++;
        }
    }

    private void Update()
    {
        if (player == null) return;

        if (targets == null || targets.Count == 0) return;

        for (int idx = 0; idx < targets.Count; idx++)
        {
            PointerTarget target = targets[idx];
            Vector3 toPosition = target.transform.position;
            Vector3 fromPosition = player.position;

            Vector3 dir = (toPosition - fromPosition).normalized;
            Vector3 targetPositionScreenPoint = uiCamera.WorldToScreenPoint(toPosition);

            if (targetPositionScreenPoint.z < 0)
            {
                targetPositionScreenPoint.x = Screen.width - targetPositionScreenPoint.x;
                targetPositionScreenPoint.y = Screen.height - targetPositionScreenPoint.y;
            }

            bool isOffScreen = targetPositionScreenPoint.x <= 0 || targetPositionScreenPoint.x >= Screen.width || targetPositionScreenPoint.y <= 0 || targetPositionScreenPoint.y >= Screen.height;

            if (isOffScreen)
            {
                target.pointer.gameObject.SetActive(true);

                Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
                if (cappedTargetScreenPosition.x <= 20) cappedTargetScreenPosition.x = 20f;
                if (cappedTargetScreenPosition.x >= Screen.width - 20) cappedTargetScreenPosition.x = Screen.width - 20;
                if (cappedTargetScreenPosition.y <= 20) cappedTargetScreenPosition.y = 20f;
                if (cappedTargetScreenPosition.y >= Screen.height - 20) cappedTargetScreenPosition.y = Screen.height - 20;

                target.pointer.position = cappedTargetScreenPosition;
                target.pointer.localPosition = new Vector3(target.pointer.localPosition.x, target.pointer.localPosition.y, 0f);

            }
            else
            {
                target.pointer.gameObject.SetActive(false);
            }
        }
    }
}
