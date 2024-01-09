using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreTxt;

    private void Update()
    {
        highScoreTxt.text = "High score : " + PlayerPrefs.GetInt("BestScore").ToString();
    }
}
