using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour                                       //Yükleme işlemlerinin çalışması için Level'ların isimlerinin şu şekilde olması gerek; "Level 1"
{
    #region Variables
    [SerializeField] private GameData data;
    [SerializeField] private int levelCount;
    #endregion

    private void OnEnable()
    {
        EventManager.loadOpeningScene += LoadOpeningLevel;
        EventManager.loadNextScene += LoadNextLevel;
        EventManager.loadSameScene += LoadSameLevel;
    }

    private void OnDisable()
    {
        EventManager.loadOpeningScene -= LoadOpeningLevel;
        EventManager.loadNextScene -= LoadNextLevel;
        EventManager.loadSameScene -= LoadSameLevel;
    }

    void LoadOpeningLevel()
    {
        if (data.levelValue > levelCount)
        {
            data.levelValue = 1;
            SaveManager.SaveData(data);
            SceneManager.LoadScene("Level " + data.levelValue);
        }
        else
        {
            SceneManager.LoadScene("Level " + data.levelValue);
        }
    }

    void LoadNextLevel()
    {
        data.levelValue++;
        data.levelTextValue++;
        if (data.levelValue > levelCount)
        {
            data.levelValue = 1;
            SaveManager.SaveData(data);
            SceneManager.LoadScene("Level " + data.levelValue);
        }
        else
        {
            SaveManager.SaveData(data);
            SceneManager.LoadScene("Level " + data.levelValue);
        }
    }

    void LoadSameLevel()
    {
        SceneManager.LoadScene("Level " + data.levelValue);
    }
}