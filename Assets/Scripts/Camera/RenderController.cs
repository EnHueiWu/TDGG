using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderController : MonoBehaviour
{
    private GameObject renderModel;
    public Button instantiateButton;
    private Camera renderCamera;
    private readonly string modelName = "Render Model";
    private readonly string cameraName = "Render Camera";

    private void Start()
    {
        renderModel = transform.Find(modelName).gameObject;
        renderCamera = transform.Find(cameraName).GetComponent<Camera>();
    }

    private void Update()
    {
        if (ViewController.canCameraRotating) renderCamera.transform.RotateAround(renderModel.transform.position, Vector3.up, ViewController.mouseX * ViewController.rotationSpeed);
    }
}
