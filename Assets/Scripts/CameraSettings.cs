using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSettings : MonoBehaviour
{
    [System.Serializable]
    public enum Mode
    {
        First, Third
    }
    Camera currentCamera;
    public static Camera CurrentCamera
    {
        get
        {
            return instance.currentCamera;
        }
    }
    public static Mode CameraMode
    {
        get
        {
            return instance.mode;
        }
    }
    public static bool isFirstPerson
    {
        get
        {
            return instance.mode == Mode.First;
        }
    }
    public static bool isThirdPerson
    {
        get
        {
            return instance.mode == Mode.Third;
        }
    }
    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;
    Mode mode;
    static CameraSettings instance;
    GamepadInputManager inputManager;
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }
    void Start()
    {
        inputManager = TagResolver.i.inputManager;
        SetMode(mode);
    }

    bool pressetToggleKey = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F) || inputManager.StartButtonValue)
        {
            if (!pressetToggleKey)
            {
                pressetToggleKey = true;
            }
        }
        else
        {
            if (pressetToggleKey)
            {
                pressetToggleKey = false;
                ToggleCamera();
            }
        }
    }

    private void ToggleCamera()
    {
        if (mode == Mode.First)
        {
            mode = Mode.Third;
            SetMode(mode);
        }
        else if (mode == Mode.Third)
        {
            mode = Mode.First;
            SetMode(mode);
        }
    }

    private void SetMode(Mode mode)
    {
        if (mode == Mode.Third)
        {
            firstPersonCamera.gameObject.SetActive(false);
            thirdPersonCamera.gameObject.SetActive(true);
#if (!UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
            Cursor.lockState = CursorLockMode.Confined;
#endif
            Cursor.visible = true;
            currentCamera = thirdPersonCamera;
        }
        else if (mode == Mode.First)
        {
            firstPersonCamera.gameObject.SetActive(true);
            thirdPersonCamera.gameObject.SetActive(false);
#if (!UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
            Cursor.lockState = CursorLockMode.Locked;
#endif
            Cursor.visible = false;
            currentCamera = firstPersonCamera;
        }
    }
}
