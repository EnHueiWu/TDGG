using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    private GameObject upgradeCanvas, upgradePanel;
    public GameObject advancedArtillery, poisonArtillery, iceArtillery;
    private Button selfUpgradeButton, specialUpgradeButton, advancedUpgradeButton, poisonUpgradeButton, iceUpgradeButton;
    private readonly float offset = 0.098f;
    private readonly string artilleryLayer = "Artillery";

    private void Awake()
    {
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
        selfUpgradeButton.onClick.AddListener(SelfUpgrade);
        specialUpgradeButton.onClick.AddListener(SpecialUpgrade);
        advancedUpgradeButton.onClick.AddListener(() => Upgrade(advancedArtillery));
        poisonUpgradeButton.onClick.AddListener(() => Upgrade(poisonArtillery));
        iceUpgradeButton.onClick.AddListener(() => Upgrade(iceArtillery));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && (!EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject == null)) upgradePanel.SetActive(false);
    }

    private void OnMouseUp()
    {
        if (gameObject.layer == LayerMask.NameToLayer(artilleryLayer) && IsNotCapsuleCollider())
        {
            Vector2 ViewportPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
            upgradePanel.GetComponent<RectTransform>().position = ViewportPosition;
            upgradePanel.SetActive(true);
        }
    }

    private bool IsNotCapsuleCollider()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out RaycastHit hit) && hit.collider is not CapsuleCollider;
    }

    private void SelfUpgrade()
    {
        GameObject instantiatedArtillery = Instantiate(gameObject, transform.position + new Vector3(0, offset, 0), Quaternion.identity);
        instantiatedArtillery.transform.SetParent(transform);
        upgradePanel.SetActive(false);
    }

    private void SpecialUpgrade()
    {
        advancedUpgradeButton.gameObject.SetActive(true);
        poisonUpgradeButton.gameObject.SetActive(true);
        iceUpgradeButton.gameObject.SetActive(true);
    }

    private void Upgrade(GameObject upgradePrefab)
    {
        Instantiate(upgradePrefab, transform.position, transform.rotation);
        Destroy(gameObject);
        upgradePanel.SetActive(false);
    }
}
