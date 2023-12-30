using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiMap : MonoBehaviour
{
    [SerializeField] private GameObject[] maps;
    [SerializeField] private bool isVersusScene = false;
    private Transform spawn;

    private void Start()
    {
        if (isVersusScene)
        {
            Debug.Log(maps[PlayerPrefs.GetInt("MapIndex")].gameObject.name);
            Instantiate(maps[PlayerPrefs.GetInt("MapIndex")], spawn);
        }
    }

    public void ChooseMap(int index)
    {
        PlayerPrefs.SetInt("MapIndex", index);
    }
}
