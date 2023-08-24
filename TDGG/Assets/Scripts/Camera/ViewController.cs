using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ViewController : MonoBehaviour
{
    private Transform basicMap;
    private GameObject scrollContent;
    private readonly string mapName = "Basic Map";
    private readonly string contentName = "ScrollContent";
    public static float rotationSpeed = 5.0f;
    public static float mouseX;
    public static bool isCameraRotating;
    private bool isMouseScrollOnPanel;

    private void Awake()
    {
        basicMap = GameObject.Find(mapName)?.transform;
        scrollContent = GameObject.Find(contentName)?.gameObject;
        if (basicMap == null) Debug.LogWarning("Map not found!");
    }

    private void Start()
    {
        if (scrollContent != null)
        {
            Button[] buttons = scrollContent.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
                button.onClick.AddListener(DeactivatePotentialDrag);
        }
    }

    private void Update()
    {
        mouseX = Input.GetAxis("Mouse X");
        isCameraRotating = Input.GetMouseButton(0) && basicMap != null && !isMouseScrollOnPanel;
        if (isCameraRotating) transform.RotateAround(basicMap.position, Vector3.up, mouseX * rotationSpeed);
    }

    public void DeactivatePotentialDrag() => isMouseScrollOnPanel = false;

    public void InitializePotentialDrag(BaseEventData eventData) => isMouseScrollOnPanel = true;

    public void EndDrag(BaseEventData eventData) => isMouseScrollOnPanel = false;
}
