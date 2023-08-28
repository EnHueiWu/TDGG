using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewController : MonoBehaviour
{
    private Transform basicMap;
    private readonly string mapName = "Basic Map";
    public static float rotationSpeed = 5.0f;
    public static float mouseX;
    public static bool canCameraRotating;

    private void Awake()
    {
        basicMap = GameObject.Find(mapName)?.transform;
        if (basicMap == null) Debug.LogWarning("Map not found!");
    }

    private void Update()
    {
        mouseX = Input.GetAxis("Mouse X");
        canCameraRotating = Input.GetMouseButton(0) && basicMap != null;
        if (canCameraRotating) transform.RotateAround(basicMap.position, Vector3.up, mouseX * rotationSpeed);
    }
}
