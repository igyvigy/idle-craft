using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 1;

    public Transform playerBody;

    private float xRotation = 0f;
    private GamepadInputManager inputManager;
    [SerializeField] private Transform bag;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = TagResolver.i.inputManager;
        xRotation = transform.localEulerAngles.x;
#if (!UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Locked;
#endif

        Cursor.visible = false;


        mouseSensitivity = 180;

        if (Application.isEditor)
            mouseSensitivity = 180;
    }

    float mx;

    // Update is called once per frame
    void Update()
    {
        if (bag.gameObject.activeInHierarchy) return;

        float mouseX = inputManager.LookValue.x * mouseSensitivity * Time.deltaTime;
        float mouseY = inputManager.LookValue.y * mouseSensitivity * Time.deltaTime;

        if (Mathf.Abs(mouseX) > 20 || Mathf.Abs(mouseY) > 20)
            return;

        //camera's x rotation (look up and down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        //mx = Input.GetAxis("Mouse X");

        //player body's y rotation (turn left and right)
        playerBody.Rotate(Vector3.up * mouseX);

        //playerBody.GetComponent<Rigidbody>().rotation *= Quaternion.Euler(0, mouseX, 0);
        //playerBody.GetComponent<Rigidbody>().MoveRotation *= Quaternion.Euler(0, mouseX, 0);

        //transform.localRotation = Quaternion.Euler(transform.rotation.eulerAngles.x + mouseY,0, 0);
        //playerBody.rotation = Quaternion.Euler(0,playerBody.rotation.eulerAngles.y + mouseX,0);
    }


    private void FixedUpdate()
    {
        //float mouseX = mx * mouseSensitivity * Time.fixedDeltaTime;

        // playerBody.GetComponent<Rigidbody>().MoveRotation(playerBody.GetComponent<Rigidbody>().rotation *= Quaternion.Euler(0, mouseX, 0));
    }

}
