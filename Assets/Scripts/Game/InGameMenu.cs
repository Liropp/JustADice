using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private GameObject inGameMenu;
    [SerializeField] private GameObject inGameUI;

    private void Start()
    {
        QuitSettings();
    }

    public void Settings()
    {
        inGameMenu.SetActive(true);
        inGameUI.SetActive(false);
    }

    public void QuitSettings()
    {
        inGameMenu.SetActive(false);
        inGameUI.SetActive(true);

        FindObjectOfType<AudioManager>().UpdateVolume();
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// relance le niveau en cours à zéro
    /// </summary>
    public void ReloadLevel()
    {
        //Debug.Log("Reload: " + SceneManager.GetActiveScene().name);

        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index);
    }

    public void PlayerWon()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetString("lvl" + index, "done");
        //Debug.Log("lvl" + index + "is done");

        SceneManager.LoadScene(0);
    }

    public void PlayerLose()
    {
        ReloadLevel();
    }
}
