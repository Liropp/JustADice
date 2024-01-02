using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiManager : MonoBehaviour
{
    private GameObject _currPawnPlaying;
    [SerializeField] private PlayerGFX[] PlayerGFXs;
    [SerializeField] private GameObject readyCanvas;
    [SerializeField] private Image readyImage;
    [SerializeField] private TextMeshProUGUI readyTxt;
    [SerializeField] private GameObject readyBtn;
    private int _turnCount;

    private void Update()
    {
        diffGFX();
        TurnReadyCanvas();
    }

    private void diffGFX()
    {
        if(FindObjectOfType<GameManager>().GetCurrPawnPlaying() != null)
        _currPawnPlaying = FindObjectOfType<GameManager>().GetCurrPawnPlaying();

        if(_currPawnPlaying != null)
        {
            foreach (var PlayerGFX in PlayerGFXs)
            {
                if (PlayerGFX.playerName == _currPawnPlaying.name)
                {
                    foreach (var GFX in PlayerGFX.GFXs)
                    {
                        GFX.enabled = true;
                    }
                    PlayerGFX.notMyTurnGFX.SetActive(false);
                }
                else
                {
                    foreach (var GFX in PlayerGFX.GFXs)
                    {
                        GFX.enabled = false;
                    }
                    PlayerGFX.notMyTurnGFX.SetActive(true);
                }
            }
        }
    }

    private void TurnReadyCanvas()
    {
        int i = FindObjectOfType<GameManager>().GetTurnCount();

        if (i > _turnCount)
        {
            StartCoroutine(ShowCanvas());

            readyImage.color = new Color(readyImage.color.r, readyImage.color.g, readyImage.color.b, 0);
            readyTxt.color = new Color(readyTxt.color.r, readyTxt.color.g, readyTxt.color.b, 0);
            readyBtn.SetActive(false);

            if (_currPawnPlaying != null)
            {
                readyTxt.text = _currPawnPlaying.name;
            }
            readyCanvas.SetActive(true);

            _turnCount++;
        }
    }

    private IEnumerator ShowCanvas()
    {
        yield return new WaitForSeconds(.4f);

        readyImage.color = new Color(readyImage.color.r, readyImage.color.g, readyImage.color.b, 1);
        readyTxt.color = new Color(readyTxt.color.r, readyTxt.color.g, readyTxt.color.b, 1);
        readyBtn.SetActive(true);

        //Debug.Log("new turn");

        yield break;
    }

    public void PlayerReady()
    {
        readyCanvas.SetActive(false);
    }
}
