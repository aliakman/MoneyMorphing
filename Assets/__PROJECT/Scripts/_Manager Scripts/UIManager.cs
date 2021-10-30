using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Variables
    [Header("GameData")]
    [SerializeField] private GameData gameData;
    [Header("Panels")]
    [SerializeField] private GameObject splashPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject failPanel;
    [SerializeField] private GameObject tutorialPanel;
    [Header("In Game Panel")]
    [SerializeField] private inGamePanelTypes inGamePanelType;
    [SerializeField] private GameObject levelBarPanel;
    [SerializeField] private GameObject progressBarPanel;
    [SerializeField] private Text levelBarLevelText;
    [SerializeField] private Text progressBarCurrentLevelText;
    [SerializeField] private Text progressBarNextLevelText;
    [SerializeField] private Image progressBarFillImage;
    [Header("Tutorial")]
    [SerializeField] private List<TutorialPanel> tutorialPanels;
    [SerializeField] private float tutorialTime;
    [SerializeField] private tutorialTypes tutorialTypes;
    private IEnumerator tutorialCloseRoutine;
    private bool tutorialEnable;
    private bool buttonClicked;
    #endregion

    private void OnEnable()
    {
        EventManager.showWinPanel += ShowWinPanel;
        EventManager.showFailPanel += ShowFailPanel;
        EventManager.changeProgressBarFillAmount += ChangeProgressBarFillAmount;
    }

    private void OnDisable()
    {
        EventManager.showWinPanel -= ShowWinPanel;
        EventManager.showFailPanel -= ShowFailPanel;
        EventManager.changeProgressBarFillAmount -= ChangeProgressBarFillAmount;
    }

    private void Start()
    {
        ArrangeFirstAppearance();
    }

    private void Update()
    {
        if(tutorialEnable) TutorialCloseFunction();
    }

    private void ArrangeFirstAppearance()
    {
        CloseAllPanels();
        bool loadingScene = EventManager.checkingSceneType.Invoke();
        if (loadingScene)
        {
            splashPanel.SetActive(true);
        }
        else if (!loadingScene)
        {
            gamePanel.SetActive(true);
            if (inGamePanelType == inGamePanelTypes.LevelBar)
            {
                levelBarPanel.SetActive(true);
                levelBarLevelText.text = "LEVEL " + gameData.levelTextValue;
            }

            else if (inGamePanelType == inGamePanelTypes.ProgressBar)
            {
                progressBarPanel.SetActive(true);
                progressBarCurrentLevelText.text = gameData.levelTextValue.ToString();
                progressBarNextLevelText.text = (gameData.levelTextValue + 1).ToString();
                progressBarFillImage.fillAmount = 0f;
            }

            if (gameData.levelTextValue <= gameData.tutorialLevelCount)
            {
                foreach (var item in tutorialPanels)
                {
                    if (item.name == tutorialTypes)
                    {
                        item.panel.SetActive(true);
                        tutorialPanel.SetActive(true);
                        tutorialCloseRoutine = TutorialPanelCloseRoutine(tutorialTime);
                        StartCoroutine(tutorialCloseRoutine);
                        tutorialEnable = true;
                        break;
                    }
                }
            }
        }
    }

    private void TutorialCloseFunction()
    {
        if (!tutorialPanel.activeInHierarchy) return;
        if (Input.GetMouseButtonDown(0))
        {
            tutorialPanel.SetActive(false);
            tutorialEnable = false;
        }
    }

    private void CloseAllPanels()
    {
        splashPanel.SetActive(false);
        gamePanel.SetActive(false);
        winPanel.SetActive(false);
        failPanel.SetActive(false);
        tutorialPanel.SetActive(false);
        foreach (var item in tutorialPanels)
        {
            item.panel.SetActive(false);
        }
    }

    IEnumerator TutorialPanelCloseRoutine(float closeTime)
    {
        yield return new WaitForSeconds(closeTime);
        tutorialPanel.SetActive(false);
        tutorialEnable = false;
    }

    private void ShowWinPanel()
    {
        EventManager.invokeHaptic?.Invoke(vibrationTypes.Success);

        CloseAllPanels();
        winPanel.SetActive(true);
    }

    private void ShowFailPanel()
    {
        EventManager.invokeHaptic?.Invoke(vibrationTypes.Failure);

        CloseAllPanels();
        failPanel.SetActive(true);
    }

    public void NextButtonClicked()
    {
        EventManager.invokeHaptic?.Invoke(vibrationTypes.Selection);

        if (buttonClicked) return;
        buttonClicked = true;
        EventManager.winGame.Invoke();
    }

    public void RetryButtonClicked()
    {
        EventManager.invokeHaptic?.Invoke(vibrationTypes.Selection);

        if (buttonClicked) return;
        buttonClicked = true;
        EventManager.loseGame.Invoke();
    }

    private void ChangeProgressBarFillAmount(float value)
    {
        if (inGamePanelType != inGamePanelTypes.ProgressBar) return;
        progressBarFillImage.fillAmount = value;
    }
}
