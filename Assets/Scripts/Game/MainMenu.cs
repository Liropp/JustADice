using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject levelsMenu;

    private void Start()
    {
        mainMenu.SetActive(true);
        levelsMenu.SetActive(false);
    }

    public void Play()
    {
        mainMenu.SetActive(false);
        levelsMenu.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
