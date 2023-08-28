using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverAnimation : MonoBehaviour
{
    private static GameObject hoveredInterlocking;
    public Renderer interlockingRenderer;
    private LayerMask raycastLayers;
    private Color originalColor;
    private static Color hoverColor;
    private bool isMouseEnter = false;
    private static bool isAnyButtonSelected;
    private readonly string gridLayer = "Grid";
    private readonly string artilleryLayer = "Artillery";
    private readonly string breakableWallLayer = "Breakable Wall";
    private readonly string unbreakableWallLayer = "Unbreakable Wall";
    private static readonly string modelName = "Render Model";

    public static void AccessSelectedButtonColor(BuildingManager buildingManager)
    {
        if (buildingManager != null)
        {
            hoveredInterlocking = buildingManager.gameObject.transform.Find(modelName).gameObject;
            hoverColor = buildingManager.gameObject.transform.Find(modelName).GetComponent<Renderer>().material.color;
            isAnyButtonSelected = buildingManager.isButtonSelected;
        }

        else
        {
            isAnyButtonSelected = false;
        }
    }

    private void Start()
    {
        interlockingRenderer = GetComponent<Renderer>();
        raycastLayers = LayerMask.GetMask(artilleryLayer);
        originalColor = interlockingRenderer.material.color;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~raycastLayers))
        {
            if (hit.collider.gameObject == gameObject && !isMouseEnter)
            {
                OnRaycastEnter(hit);
            }

            else if (hit.collider.gameObject != gameObject && isMouseEnter)
            {
                OnRaycastExit();
            }
        }

        else if (isMouseEnter)
        {
            OnRaycastExit();
        }
    }

    private void OnRaycastEnter(RaycastHit hit)
    {
        if (isAnyButtonSelected)
        {
            isMouseEnter = true;

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer(unbreakableWallLayer) && hoveredInterlocking.layer == LayerMask.NameToLayer(artilleryLayer))
            {
                interlockingRenderer.material.color = hoverColor;
            }

            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer(gridLayer) && hoveredInterlocking.layer == LayerMask.NameToLayer(breakableWallLayer))
            {
                interlockingRenderer.material.color = hoverColor;
            }
        }
    }

    private void OnRaycastExit()
    {
        isMouseEnter = false;
        interlockingRenderer.material.color = originalColor;
    }
}
