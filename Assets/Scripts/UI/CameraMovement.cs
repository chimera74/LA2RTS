using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float defaultHeight;
    public float defaultXOffset;
    public float defaultZOffset;

    public float zoomSpd = 2.0f;
    public float xSpeed = 240.0f;
    public float ySpeed = 123.0f;

    public float mouseSensitivity = 1.0f;
    private Vector3 lastPosition;

    public void Start()
    {

    }

    public void LateUpdate()
    {
        ProcessKeyControls();
        ProcessMouseControls();
    }

    private void ProcessMouseControls()
    {
        if (Input.GetMouseButtonDown(2))
        {
            lastPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastPosition;
            transform.Translate(delta.x * mouseSensitivity, 0, delta.y * mouseSensitivity, Space.World);
            lastPosition = Input.mousePosition;
        }
    }

    private void ProcessKeyControls()
    {
        float dx = -Input.GetAxis("Horizontal") * xSpeed * 0.02f;
        float dz = -Input.GetAxis("Vertical") * ySpeed * 0.02f;

        float dy = Input.GetAxis("Mouse ScrollWheel") * zoomSpd;

        //print(Input.GetAxis("Mouse ScrollWheel"));

        Vector3 position = transform.position;
        position.x += dx;
        position.z += dz;
        position.y += dy;

        transform.position = position;
    }

    public void LookAt(Vector3 lookPos)
    {
        Vector3 newPosition = lookPos;
        newPosition.y += defaultHeight;
        newPosition.x += defaultXOffset;
        newPosition.z += defaultZOffset;

        transform.position = newPosition;
        transform.LookAt(lookPos);
    }
}

