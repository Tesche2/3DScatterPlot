using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed = 5.0f, rotationSensitivity = 0.1f, rayCastDistance = 20f;
    [SerializeField]
    private float minMoveSpeed = 1f, maxMoveSpeed = 50f;
    private Vector3 movementDirection;
    private Vector2 rotationDirection;

    [SerializeField]
    private GameObject menu;

    [SerializeField]
    private InputActionReference move, rotate, interact, changeSpeed, openMenu;

    private int layer3Mask;
    RaycastHit rayHit;
    DataCube hitDataCube;
    GameObject hitDataCubeObj;

    private void OnEnable()
    {
        // Enable controllers and lock mouse cursor
        changeSpeed.action.performed += ChangeMoveSpeed;
        openMenu.action.performed += OpenMenu;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        // Disable controllers
        changeSpeed.action.performed -= ChangeMoveSpeed;
        openMenu.action.performed -= OpenMenu;
    }

    // Start is called before the first frame update
    void Start()
    {
        layer3Mask = 1 << 3;

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Define camera movement based on inputs
        movementDirection = move.action.ReadValue<Vector3>();
        rotationDirection += rotate.action.ReadValue<Vector2>() * rotationSensitivity;
        rotationDirection.y = Mathf.Clamp(rotationDirection.y, -80f, 80f);
        Move();

        // Define ray cast limits
        Vector3 rayCastOrigin = transform.position;
        Vector3 rayCastDirection = transform.forward;

        // Handle ray cast hits
        if (Physics.Raycast(rayCastOrigin, rayCastDirection, out rayHit, rayCastDistance, layer3Mask))
        {
            // Disable hit object upon hitting a new one, this is in case two different objects are hit between 2 frames
            if (hitDataCubeObj != null && hitDataCubeObj != rayHit.collider.gameObject)
            {
                hitDataCube.enabled = false;
            }

            // Get GameObject component and enable its script
            hitDataCubeObj = rayHit.collider.gameObject;
            hitDataCube = rayHit.collider.gameObject.GetComponent<DataCube>();
            hitDataCube.enabled = true;
        }
        else if(hitDataCube)
        {
            // Disable current cube script once nothing is being hit
            hitDataCube.enabled = false;
        }
    }

    private void Move()
    {
        // Set position and rotation based on previously calculated data
        transform.SetPositionAndRotation(

            transform.position +
            moveSpeed * Time.deltaTime *
            (movementDirection.x * transform.right +
             movementDirection.y * transform.up +
             movementDirection.z * transform.forward),

            Quaternion.Euler(-rotationDirection.y,
                                rotationDirection.x,
                                0));
    }
    private void ChangeMoveSpeed(InputAction.CallbackContext obj)
    {
        // ob.ReadValue<float>() returns -/+ 120, depending on scrolling direction
        moveSpeed += obj.ReadValue<float>() / 360;
        moveSpeed = Mathf.Clamp(moveSpeed, minMoveSpeed, maxMoveSpeed);
    }

    private void OpenMenu(InputAction.CallbackContext obj)
    {
        // Enable mouse and give controller to menu
        Cursor.lockState = CursorLockMode.None;

        menu.SetActive(true);
        gameObject.GetComponent<CameraController>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Change position of axes, based on camera location
        PlotRegion plotRegion = GameObject.Find("PlotRegion").GetComponent<PlotRegion>();

        switch (other.name)
        {
            case "xNzNQuad":
                plotRegion.MoveAxes(new Vector3(-1, 0, -1));
                break;
            case "xNzPQuad":
                plotRegion.MoveAxes(new Vector3(-1, 0, 1));
                break;
            case "xPzNQuad":
                plotRegion.MoveAxes(new Vector3(1, 0, -1));
                break;
            case "xPzPQuad":
                plotRegion.MoveAxes(new Vector3(1, 0, 1));
                break;
            default:
                break;
        }
    }
}
