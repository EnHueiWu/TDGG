using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    private LevelBasic levelBasic;
    private GameObject upgradeCanvas, upgradePanel, upgradedArtillery;
    public GameObject advancedArtillery, poisonArtillery, iceArtillery;
    private Button selfUpgradeButton, specialUpgradeButton, advancedUpgradeButton, poisonUpgradeButton, iceUpgradeButton;
    private readonly float offset = 0.098f;
    private readonly string artilleryLayer = "Artillery";
    public int hierarchyLevel;

    private void Awake()
    {
        levelBasic = FindObjectOfType<LevelBasic>();
        upgradeCanvas = GameObject.Find("UpgradeCanvas");
        upgradePanel = upgradeCanvas.transform.Find("UpgradePanel").gameObject;
        selfUpgradeButton = upgradePanel.transform.Find("SelfUpgrade").gameObject.GetComponent<Button>();
        specialUpgradeButton = upgradePanel.transform.Find("SpecialUpgrade").gameObject.GetComponent<Button>();
        advancedUpgradeButton = upgradePanel.transform.Find("AdvancedUpgrade").gameObject.GetComponent<Button>();
        poisonUpgradeButton = upgradePanel.transform.Find("PoisonUpgrade").gameObject.GetComponent<Button>();
        iceUpgradeButton = upgradePanel.transform.Find("IceUpgrade").gameObject.GetComponent<Button>();
    }

    private void Start()
    {
        upgradePanel.SetActive(false);
        advancedUpgradeButton.gameObject.SetActive(false);
        poisonUpgradeButton.gameObject.SetActive(false);
        iceUpgradeButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && (!EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject == null))
        {
            upgradePanel.SetActive(false);
            advancedUpgradeButton.gameObject.SetActive(false);
            poisonUpgradeButton.gameObject.SetActive(false);
            iceUpgradeButton.gameObject.SetActive(false);
            RemoveAllButtonsListeners(selfUpgradeButton, specialUpgradeButton, advancedUpgradeButton, poisonUpgradeButton, iceUpgradeButton);
        }
    }

    private void OnMouseUp()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            RemoveAllButtonsListeners(selfUpgradeButton, specialUpgradeButton, advancedUpgradeButton, poisonUpgradeButton, iceUpgradeButton);

            if (gameObject.layer == LayerMask.NameToLayer(artilleryLayer) && IsNotCapsuleCollider())
            {
                Vector2 ViewportPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
                upgradePanel.GetComponent<RectTransform>().position = ViewportPosition;
                upgradePanel.SetActive(true);
                upgradedArtillery = gameObject;
                
                selfUpgradeButton.onClick.AddListener(() => SelfUpgrade(upgradedArtillery, selfUpgradeButton));
                specialUpgradeButton.onClick.AddListener(SpecialUpgrade);
                advancedUpgradeButton.onClick.AddListener(() => Upgrade(upgradedArtillery, advancedArtillery));
                poisonUpgradeButton.onClick.AddListener(() => Upgrade(upgradedArtillery, poisonArtillery));
                iceUpgradeButton.onClick.AddListener(() => Upgrade(upgradedArtillery, iceArtillery));
            }
        }
    }

    private bool IsNotCapsuleCollider()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out RaycastHit hit) && hit.collider is not CapsuleCollider;
    }

    private void SelfUpgrade(GameObject artillery, Button upgradeButton)
    {
        int price = artillery.GetComponent<ArtilleryManager>().price;

        if (levelBasic.coin >= price && hierarchyLevel < 4)
        {
            upgradeButton.interactable = true;
            GameObject instantiatedArtillery = Instantiate(artillery, artillery.transform.position + new Vector3(0, offset, 0), Quaternion.identity);
            instantiatedArtillery.transform.SetParent(artillery.transform);
            instantiatedArtillery.GetComponent<UpgradeManager>().hierarchyLevel = AccessHierarchyLevel(instantiatedArtillery.transform);
            levelBasic.coin -= price;
            levelBasic.UpdateCoinWallet();
        }

        upgradePanel.SetActive(false);
        RemoveAllButtonsListeners(selfUpgradeButton, specialUpgradeButton, advancedUpgradeButton, poisonUpgradeButton, iceUpgradeButton);
    }

    private void SpecialUpgrade()
    {
        advancedUpgradeButton.gameObject.SetActive(true);
        poisonUpgradeButton.gameObject.SetActive(true);
        iceUpgradeButton.gameObject.SetActive(true);
    }

    private void Upgrade(GameObject artillery, GameObject upgradePrefab)
    {
        int price = upgradePrefab.GetComponent<ArtilleryManager>().price;

        if (levelBasic.coin >= price && hierarchyLevel < 1)
        {
            Instantiate(upgradePrefab, artillery.transform.position, artillery.transform.rotation);
            Destroy(artillery);
            levelBasic.coin -= price;
            levelBasic.UpdateCoinWallet();
        }

        upgradePanel.SetActive(false);
        RemoveAllButtonsListeners(selfUpgradeButton, specialUpgradeButton, advancedUpgradeButton, poisonUpgradeButton, iceUpgradeButton);
    }

    private int AccessHierarchyLevel(Transform artilleryTransform)
    {
        int level = 1;
        Transform upperLavelTransform = artilleryTransform;

        while (upperLavelTransform.parent != null)
        {
            level++;
            upperLavelTransform = upperLavelTransform.parent;
        }

        return level;
    }

    private void RemoveAllButtonsListeners(Button selfUpgrade, Button specialUpgrade, Button advancedUpgrade, Button poisonUpgrade, Button iceUpgrade)
    {
        selfUpgrade.onClick.RemoveAllListeners();
        specialUpgrade.onClick.RemoveAllListeners();
        advancedUpgrade.onClick.RemoveAllListeners();
        poisonUpgrade.onClick.RemoveAllListeners();
        iceUpgrade.onClick.RemoveAllListeners();
    }
}
