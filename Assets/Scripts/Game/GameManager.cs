using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Pawns")] [SerializeField] List<GameObject> pawns = new List<GameObject>();

    //Event
    [HideInInspector] public UnityEvent pawnPlayed;

    [Header("Turn Setup")]
    [SerializeField] private Text turnText;
    [SerializeField] private int curIndexTurn = 0;
    private bool isPlayerTurn = false;
    private bool isEnemyTurn = false;

    [SerializeField] private GameObject winUI;
    private Text winTxt;
    private GameObject player;

    private void Start()
    {
        pawnPlayed = new UnityEvent();
        pawnPlayed.AddListener(Played);
        winTxt = winUI.GetComponent<Text>();
        winUI.SetActive(false);
    }

    private void Update()
    {
        // travel the list of pawns
        for (int i = 0; i < pawns.Count; i++)
        {
            // try to find the current pawn turn
            if (i == curIndexTurn && pawns[i].gameObject != null)
            {
                //Debug.Log(pawns[i].gameObject.name);

                // the current pawn is the player ?
                if (pawns[i].CompareTag("Player") && !isPlayerTurn)
                {
                    // yes

                    //Debug.Log("player turn");
                    player = pawns[i].gameObject;
                    turnText.text = "Turn: Player";

                    pawns[i].GetComponent<PlayerController>().SetcanMove(true);
                    isPlayerTurn = true;
                }
                else if (pawns[i].CompareTag("Enemy") && !isEnemyTurn)
                {
                    // no, it's bot turn

                    //Debug.Log("enemy" + i + " turn");
                    //Debug.Log(pawns[i].gameObject.name);
                    turnText.text = "Turn: Enemy n°"+i;
                    isPlayerTurn = false;
                    pawns[i].GetComponent<EnemyBase>().SetcanMove(true);
                    isEnemyTurn = true;
                }
            }
            else if(pawns[i].gameObject == null)
            {
                // if a pawn is killed, we remove him from the list to avoid error.

                pawns.Remove(pawns[i].gameObject);
                isEnemyTurn = false;
            }
        }

        if(!player.activeSelf)
        {
            AIWin();
        }
        else if(pawns.Count <= 1)
        {
            PlayerWin();
        }
    }

    /// <summary>
    /// current pawn has played ?
    /// </summary>
    public void Played()
    {
        //Debug.Log("played");
        isEnemyTurn = false;
        curIndexTurn++;

        // if curIndexTurn is at the end of the list, return to 0
        if(curIndexTurn >= pawns.Count)
        {
            curIndexTurn = 0;
        }
    }

    /// <summary>
    /// Player win the level, AI loose
    /// </summary>
    private void PlayerWin()
    {
        winUI.SetActive(true);
        winTxt.text = "Player win!";
    }

    /// <summary>
    /// AI win the level, you loose
    /// </summary>
    private void AIWin()
    {
        winUI.SetActive(true);
        winTxt.text = "AI win.";
    }

    /// <summary>
    /// relance le niveau à zéro
    /// </summary>
    public void Retry()
    {
        Debug.Log("Reload: "+SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
