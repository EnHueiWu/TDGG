using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationController : MonoBehaviour
{
    private GameObject renderModel;
    private Camera renderCamera;
    private readonly string modelName = "Render Model";
    private readonly string cameraName = "Render Dynamic Camera";
    
    private void Start()
    {
        renderModel = transform.Find(modelName).gameObject;
        renderCamera = transform.Find(cameraName).GetComponent<Camera>();
    }

    public void DisplayModel()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float rotationSpeed = 5.0f;

        if (Input.GetMouseButton(0)) renderCamera.transform.RotateAround(renderModel.transform.position, Vector3.up, mouseX * rotationSpeed);
    }
}
