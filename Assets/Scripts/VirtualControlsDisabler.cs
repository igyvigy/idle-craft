using UnityEngine;
using System.Collections;
public class VirtualControlsDisabler : MonoBehaviour
{
    private bool controllerConnected = false;

    IEnumerator CheckForControllers()
    {
        while (true)
        {
            var controllers = Input.GetJoystickNames();
            if (!controllerConnected && controllers.Length > 0)
            {
                controllerConnected = true;
                for (int i = 0; i < controllers.Length; i++)
                {
                    Debug.Log("Connected controller!\nlist of controllers: " + controllers[i]);
                }
            }
            else if (controllerConnected && controllers.Length == 0)
            {
                controllerConnected = false;
                Debug.Log("No controllers");
            }
            yield return new WaitForSeconds(1f);
        }
    }

    void Awake()
    {
        StartCoroutine(CheckForControllers());
    }

    protected void OnEnable()
    {
#if (!UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
        gameObject.SetActive(false);
#endif
    }

    void FixedUpdate()
    {
        gameObject.SetActive(!controllerConnected);
    }
}

