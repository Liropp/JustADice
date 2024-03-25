using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject levelsMenu;
    [SerializeField] private GameObject versusMenu;
    [SerializeField] private GameObject soloMenu;
    [SerializeField] private GameObject creditMenu;

    private void Start()
    {
        if (PlayerPrefs.GetInt("Story", 0) == 1)
        {
            Story();
        }
        else
        {
            mainMenu.SetActive(true);
            levelsMenu.SetActive(false);
            versusMenu.SetActive(false);
            soloMenu.SetActive(false);
            creditMenu.SetActive(false);
        }
    }

    public void Story()
    {
        mainMenu.SetActive(false);
        versusMenu.SetActive(false);
        soloMenu.SetActive(false);
        creditMenu.SetActive(false);
        levelsMenu.SetActive(true);
    }

    public void Versus()
    {
        PlayerPrefs.SetInt("Story", 0);

        mainMenu.SetActive(false);
        levelsMenu.SetActive(false);
        soloMenu.SetActive(false);
        creditMenu.SetActive(false);
        versusMenu.SetActive(true);
    }

    public void Solo()
    {
        PlayerPrefs.SetInt("Story", 0);

        mainMenu.SetActive(false);
        levelsMenu.SetActive(false);
        versusMenu.SetActive(false);
        creditMenu.SetActive(false);
        soloMenu.SetActive(true);
    }

    public void Credits()
    {
        mainMenu.SetActive(false);
        versusMenu.SetActive(false);
        soloMenu.SetActive(false);
        levelsMenu.SetActive(false);
        creditMenu.SetActive(true);
    }

    public void Quit()
    {
        PlayerPrefs.SetInt("Story", 0);

        Application.Quit();
    }
}
