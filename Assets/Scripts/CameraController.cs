using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    GameObject cameraTarget;
    public float rotateSpeed;
    float rotate;

    public bool useSmoothing = true;
    public float smoothing;
    [Range(0.1f, 15)] public float height;
    public float distance;
    public float armDistance;
    public float cameraBubbleRadius = 0.2f;
    public float actualDistance;
    public float zoomSpeed = 0.1f;
    public Vector3 offset;

    //////
    float distanceMin = .5f;
    float distanceMax = 40f;
    float yMinLimit = -20f;
    float yMaxLimit = 80f;
    float xPanSpeed = 2f;
    float yPanSpeed = 2f;
    float x = 0.0f;
    float y = 0.0f;

    bool isUsingOrbit = false;

    //////
    public LayerMask groundLayer;
    [SerializeField] public GameObject injectedTarget;
    public void SetTarget(GameObject target)
    {
        cameraTarget = target;
    }
    GamepadInputManager inputManager;
    private bool pointerStartedOnUI;
    void Start()
    {
        inputManager = TagResolver.i.inputManager;
        if (injectedTarget != null)
        {
            SetTarget(injectedTarget.gameObject);
        }
        armDistance = distance;
    }

    void Update()
    {
        isUsingOrbit = true;
#if (!UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
        isUsingOrbit = Input.GetMouseButton(1);
#endif

        if (isUsingOrbit)
        {
#if (!UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
            Cursor.lockState = CursorLockMode.Confined;
#else 
            {
                if (Input.GetMouseButtonDown(0) && Utils.IsPointerOverUI()) isUsingOrbit = false;
            }
#endif
            Cursor.visible = false;
        }
        else
        {
#if (!UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
            Cursor.lockState = CursorLockMode.Confined;
#endif
            Cursor.visible = true;
        }
    }

    void LateUpdate()
    {
        if (isUsingOrbit)
        {
            if (Mathf.Abs(inputManager.ThirdPersonLookValue.x) > 0) x += (inputManager.ThirdPersonLookValue.x) * xPanSpeed;
            if (Mathf.Abs(inputManager.ThirdPersonLookValue.y) > 0)
            {
                y -= inputManager.ThirdPersonLookValue.y * yPanSpeed;
                y = ClampAngle(y, yMinLimit, yMaxLimit);
            }
        }
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        distance = Mathf.Clamp(distance - inputManager.ZoomValue * 0.1f, distanceMin, distanceMax);

        Vector3 direction = (transform.position - cameraTarget.transform.position).normalized;

        RaycastHit hit;
        if (Physics.Raycast(cameraTarget.transform.position, direction, out hit, distance))
        {
            actualDistance = hit.distance * (1 - cameraBubbleRadius);
            Vector3 camPosition = new Vector3(hit.point.x + hit.normal.x * cameraBubbleRadius, transform.position.y, hit.point.z + hit.normal.z * cameraBubbleRadius);
        }
        else
        {
            actualDistance = distanceMax;
        }

        var _finalDistance = Mathf.Min(actualDistance, distance);
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -_finalDistance);
        Vector3 position = rotation * negDistance + cameraTarget.transform.position;

        transform.rotation = rotation;
        transform.position = position;

    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}