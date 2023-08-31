using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    private LevelBasic levelBasic;
    private GameObject model, instantiateModel;
    private Button instantiateButton;
    private LayerMask raycastLayers;
    private int price;
    private readonly float offset = 0.098f;
    private readonly float instantiateOffset = 0.252f;
    private readonly string modelName = "Render Model";
    private readonly string unTag = "Untagged";
    private readonly string gridTag = "Grid";
    private readonly string baseTag = "Base";
    private string containerName;
    public bool isButtonSelected = false;

    private void Awake()
    {
        levelBasic = FindObjectOfType<LevelBasic>();
        instantiateModel = transform.Find(modelName).gameObject;
        containerName = gameObject.name;
    }

    private void Start()
    {
        SetupButton("Normal Artillery", "Artillery Instantiation", 50);
        SetupButton("Normal Breakable", "Normal Breakable Instantiation", 30);
        SetupButton("Poison Breakable", "Poison Breakable Instantiation", 80);
        SetupButton("Burn Breakable", "Burn Breakable Instantiation", 100);
        raycastLayers = LayerMask.GetMask("Artillery");
    }

    private void Update()
    {
        if (levelBasic.coin >= price)
        {
            instantiateButton.interactable = true;

            if (isButtonSelected && Input.GetMouseButtonDown(0))
            {
                InstantiateArtilleryAndBreakable();
                isButtonSelected = false;
                HoverAnimation.AccessSelectedButtonColor(null);
            }
        }

        else instantiateButton.interactable = false;
    }

    private void SetupButton(string targetContainerName, string buttonName, int buttonPrice)
    {
        if (containerName == targetContainerName)
        {
            instantiateButton = GameObject.Find(buttonName).GetComponent<Button>();
            price = buttonPrice;
        }
    }

    private void InstantiateArtilleryAndBreakable()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~raycastLayers))
        {
            if (containerName == "Normal Artillery" && hit.transform.gameObject.CompareTag(baseTag))
            {
                Vector3 instantiatePosition = hit.transform.position + Vector3.up * instantiateOffset;
                Vector3 targetPosition = hit.transform.position + Vector3.up * offset;
                model = Instantiate(instantiateModel, instantiatePosition, Quaternion.identity);
                StartCoroutine(levelBasic.Build(model, instantiatePosition, targetPosition));
                hit.transform.gameObject.tag = unTag;
                levelBasic.coin -= price;
                levelBasic.UpdateCoinWallet();
                isButtonSelected = false;
            }

            else if (containerName.Contains("Breakable") && hit.transform.gameObject.CompareTag(gridTag) && !hit.transform.gameObject.GetComponent<HoverAnimation>().isBulit)
            {
                Vector3 instantiatePosition = hit.transform.position;
                Vector3 targetPosition = hit.transform.position + Vector3.up * offset;
                model = Instantiate(instantiateModel, instantiatePosition, Quaternion.identity);
                StartCoroutine(levelBasic.Build(model, instantiatePosition, targetPosition));
                hit.transform.gameObject.GetComponent<HoverAnimation>().isBulit = true;
                model.GetComponent<WallManager>().builtGrid = hit.transform.gameObject;
                levelBasic.coin -= price;
                levelBasic.UpdateCoinWallet();
                isButtonSelected = false;
            }
        }

        else isButtonSelected = false;
    }

    public void SelectAndUpdateSelected(BaseEventData eventData)
    {
        isButtonSelected = true;
        HoverAnimation.AccessSelectedButtonColor(this);
    }
}
