using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;                                                       //GA ve Elephant SDK'yı yükleyince kodları yorumlardan çıkarmak gerekiyor.

public class GameManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private sceneTypes gameType;
    [SerializeField] private GameData data;
    #endregion

    private void Start()
    {
        switch (gameType)
        {
            case sceneTypes.SplashScreen:
                StartCoroutine(OpeningRoutine());
                break;
            case sceneTypes.Level:
                StartGame();
                break;
        }
    }

    private void OnEnable()
    {
        EventManager.checkingSceneType += ReturnSceneType;
        EventManager.winGame += WinGame;
        EventManager.loseGame += FailGame;
    }

    private void OnDisable()
    {
        EventManager.checkingSceneType -= ReturnSceneType;
        EventManager.winGame -= WinGame;
        EventManager.loseGame -= FailGame;
    }

    IEnumerator OpeningRoutine()
    {
        yield return new WaitForSeconds(2f);
        SaveManager.LoadData(data);
        EventManager.loadOpeningScene?.Invoke();
    }

    private void StartGame()
    {
#if !UNITY_EDITOR
        ElephantManager.instance.SendPlayerStartsPlayLevel(data.levelTextValue);      
#endif

        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "level", data.levelTextValue);
    }

    private void WinGame()
    {
#if !UNITY_EDITOR
        ElephantManager.instance.SendPlayerCompletesLevel(data.levelTextValue);
#endif
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "level", data.levelTextValue);
        EventManager.loadNextScene?.Invoke();
    }

    private void FailGame()
    {
#if !UNITY_EDITOR
        ElephantManager.instance.SendPlayerFailsLevel(data.levelTextValue);
#endif
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "level", data.levelTextValue);
        EventManager.loadSameScene?.Invoke();
    }

    private bool ReturnSceneType()
    {
        bool loadingScene = false;
        if (gameType == sceneTypes.SplashScreen)
        {
            loadingScene = true;
        }
        return loadingScene;
    }
}