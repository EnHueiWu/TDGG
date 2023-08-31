using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public GameObject manualPanel, settingPanel;
    public Button manualButton, settingButton;
    private Vector3 originalManualButtonPosition, originalSettingButtonPosition;
    private bool isManualButtonActive = false;
    private bool isSettingButtonActive = false;

    public GameObject normalArtilleryPanel, advancedArtilleryPanel, poisonArtilleryPanel, iceArtilleryPanel, normalWallPanel, poisonWallPanel, burnWallPanel;
    public Button normalArtilleryButton, advancedArtilleryButton, poisonArtilleryButton, iceArtilleryButton, normalWallButton, poisonWallButton, burnWallButton;

    private void Start()
    {
        manualPanel.SetActive(false);
        settingPanel.SetActive(false);
        originalManualButtonPosition = manualButton.transform.parent.transform.position;
        originalSettingButtonPosition = settingButton.transform.parent.transform.position;

        manualButton.onClick.AddListener(MoveManualButton);
        settingButton.onClick.AddListener(MoveSettingButton);

        normalArtilleryButton.onClick.AddListener(() => SetActivePanel(normalArtilleryPanel));
        advancedArtilleryButton.onClick.AddListener(() => SetActivePanel(advancedArtilleryPanel));
        poisonArtilleryButton.onClick.AddListener(() => SetActivePanel(poisonArtilleryPanel));
        iceArtilleryButton.onClick.AddListener(() => SetActivePanel(iceArtilleryPanel));
        normalWallButton.onClick.AddListener(() => SetActivePanel(normalWallPanel));
        poisonWallButton.onClick.AddListener(() => SetActivePanel(poisonWallPanel));
        burnWallButton.onClick.AddListener(() => SetActivePanel(burnWallPanel));

        SetActivePanel(normalArtilleryPanel);
    }

    private void MoveManualButton()
    {
        if (isManualButtonActive && !isSettingButtonActive)
            return;

        manualButton.transform.parent.transform.position = originalManualButtonPosition + new Vector3(50, 0, 0);
        settingButton.transform.parent.transform.position = originalSettingButtonPosition;

        isManualButtonActive = true;
        isSettingButtonActive = false;
}

private void MoveSettingButton()
    {
        if (!isManualButtonActive && isSettingButtonActive)
            return;

        manualButton.transform.parent.transform.position = originalManualButtonPosition;
        settingButton.transform.parent.transform.position = originalSettingButtonPosition + new Vector3(50, 0, 0);

        isManualButtonActive = false;
        isSettingButtonActive = true;
    }

    private void SetActivePanel(GameObject panelToActivate)
    {
        normalArtilleryPanel.SetActive(panelToActivate == normalArtilleryPanel);
        advancedArtilleryPanel.SetActive(panelToActivate == advancedArtilleryPanel);
        poisonArtilleryPanel.SetActive(panelToActivate == poisonArtilleryPanel);
        iceArtilleryPanel.SetActive(panelToActivate == iceArtilleryPanel);
        normalWallPanel.SetActive(panelToActivate == normalWallPanel);
        poisonWallPanel.SetActive(panelToActivate == poisonWallPanel);
        burnWallPanel.SetActive(panelToActivate == burnWallPanel);
    }

    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
