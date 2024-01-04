using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelsMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject levelsMenu;
    [SerializeField] private List<GameObject> buttons = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            int num = i + 1;
            //Debug.Log("lvl" + num + "is done ?");
            if (PlayerPrefs.GetString("lvl" + num) == "done")
            {
                buttons[i].GetComponent<Image>().color = Color.green;
                buttons[i].GetComponent<Button>().interactable = true;
                if(i + 1 <= buttons.Count-1)
                {
                    buttons[i + 1].GetComponent<Button>().interactable = true;
                }
            }
            else
            {
                buttons[i].GetComponent<Image>().color = Color.white;
            }
        }
    }

    public void Back()
    {
        mainMenu.SetActive(true);
        levelsMenu.SetActive(false);
    }

    public void LoadLevel(int lvl_Index)
    {
        SceneManager.LoadScene(lvl_Index);
    }
}
