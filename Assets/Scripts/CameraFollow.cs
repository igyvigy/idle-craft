using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField]
    public Transform target;

    public float smoothSpeed = 0.125f;
    public float cameraAdjustSpeed = 0.125f;
    public Vector3 offset;
    public Vector3 rotation;

    private float X;
    private float Y;

    public float Sensitivity;
    private GamepadInputManager inputManager;
    void Start()
    {
        inputManager = TagResolver.i.inputManager;
        Vector3 euler = transform.rotation.eulerAngles;
        X = euler.x;
        Y = euler.y;
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            target = TagResolver.i.player.transform;
        }
        // Vector3 euler = transform.rotation.eulerAngles;
        // X = euler.x;
        // Y = euler.y;
        float cameraFrontBack = inputManager.CameraFrontBackValue;
        float cameraUpDown = inputManager.CameraUpDownValue;
        if (cameraFrontBack != 0 || cameraUpDown != 0)
        {
            offset += new Vector3(0, cameraUpDown, cameraFrontBack) * Time.deltaTime * cameraAdjustSpeed;
        }
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        transform.LookAt(target);

        const float MIN_X = 0.0f;
        const float MAX_X = 360.0f;
        const float MIN_Y = -90.0f;
        const float MAX_Y = 90.0f;

        X += Input.GetAxis("Mouse X") * (Sensitivity * Time.deltaTime);
        if (X < MIN_X) X += MAX_X;
        else if (X > MAX_X) X -= MAX_X;
        Y -= Input.GetAxis("Mouse Y") * (Sensitivity * Time.deltaTime);
        if (Y < MIN_Y) Y = MIN_Y;
        else if (Y > MAX_Y) Y = MAX_Y;

        transform.rotation = Quaternion.Euler(Y, X, 0.0f);
    }

}