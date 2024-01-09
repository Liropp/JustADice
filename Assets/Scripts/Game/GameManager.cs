using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Multi")]
    public bool isMulti = false;
    [SerializeField] private GameObject[] playerUI;
    private GameObject currPawnPlaying;

    [Header("Pawns")]
    [SerializeField] List<GameObject> pawns = new List<GameObject>();
    [SerializeField] private GameObject player;
    private int malusCount = 0;

    //Event
    [HideInInspector] public UnityEvent pawnPlayed;

    [Header("Turn Setup")]
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private int curIndexTurn = 0;
    private bool isPlayerTurn = false;
    private bool isEnemyTurn = false;
    private int turnCount = 0;

    [Header("Win")]
    [SerializeField] private GameObject endUI;
    [SerializeField] private GameObject winBtn;
    [SerializeField] private TextMeshProUGUI endTxt;

    [Header("Solo")]
    public bool isSolo = false;
    public bool canRevive = false;
    private int currAdCount = 0;
    private int adMaxPerTurn = 1;

    private void Start()
    {
        pawnPlayed = new UnityEvent();
        pawnPlayed.AddListener(Played);
        endUI.SetActive(false);

        malusCount = 0;
        if (!isMulti && !isSolo)
        {
            PlayerMalus();
        }
    }

    private void Update()
    {
        Turns();

        if (!canRevive)
        {
            if (!isMulti)
            {
                if (!player.activeSelf)
                {
                    AIWin();
                }
                else if (pawns.Count <= 1)
                {
                    PlayerWin();
                }
            }
            else
            {
                if (pawns.Count <= 1)
                {
                    ThisPlayerWin();
                }
            }
        }
        else
        {
            Revive();
        }
    }

    /// <summary>
    /// the player lose few spells at start, because of AIs malus
    /// </summary>
    private void PlayerMalus()
    {
        for (int i = 0; i < pawns.Count; i++)
        {
            if (pawns[i].CompareTag("Enemy"))
            {
                malusCount += pawns[i].GetComponent<EnemyStats>().spl_disableAmount;
            }
        }

        Debug.Log(malusCount);
        player.GetComponent<PlayerAttack>().UnableRandomSpells(malusCount);
    }

    private void Turns()
    {
        if (!isMulti)
        {
            //Debug.Log("Not mulitplayer");

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
                        turnText.text = "Turn: Player";

                        pawns[i].GetComponent<PlayerController>().SetcanMove(true);

                        #region manage turns count
                        turnCount++;

                        if (turnCount > 1)
                        {
                            foreach (var spell in pawns[i].GetComponent<PlayerAttack>().GetSpells())
                            {
                                if (!spell.canUseSpell && spell.curUseSpellCooldown > 0)
                                {
                                    spell.curUseSpellCooldown--;
                                    //Debug.Log(spell.name + " " + spell.curUseSpellCooldown);
                                }
                            }
                        }
                        #endregion

                        isPlayerTurn = true;
                    }
                    else if (pawns[i].CompareTag("Enemy") && !isEnemyTurn)
                    {
                        // no, it's bot turn

                        //Debug.Log("enemy" + i + " turn");
                        //Debug.Log(pawns[i].gameObject.name);
                        turnText.text = "Turn: Enemy n°" + i;
                        isPlayerTurn = false;
                        pawns[i].GetComponent<EnemyBase>().SetcanMove(true);
                        isEnemyTurn = true;
                    }
                }
                else if (pawns[i].gameObject == null)
                {
                    // if a pawn is killed, we remove him from the list to avoid error.
                    pawns.Remove(pawns[i].gameObject);
                    isEnemyTurn = false;

                    if (curIndexTurn >= pawns.Count)
                    {
                        curIndexTurn = 0;
                    }
                }
            }
        }
        else
        {
            //Debug.Log("Mulitplayer 1 vs 1");

            // travel the list of pawns
            for (int i = 0; i < pawns.Count; i++)
            {
                // try to find the current pawn turn
                if (i == curIndexTurn && pawns[i].gameObject != null)
                {
                    //Debug.Log(pawns[i].gameObject.name);
                    //Debug.Log("curIndexTurn : " + i);

                    // the current pawn is the player ?
                    if (pawns[i].CompareTag("Player") && !isPlayerTurn)
                    {
                        //Debug.Log("turn : " + pawns[i].gameObject.name);

                        turnText.text = "Turn: " + pawns[i].gameObject.name;
                        currPawnPlaying = pawns[i].gameObject;

                        pawns[i].GetComponent<PlayerController>().SetcanMove(true);

                        #region manage turns count
                        turnCount++;
                        #endregion

                        for (int j = 0; j < playerUI.Length; j++)
                        {
                            if(j == curIndexTurn)
                            {
                                playerUI[j].SetActive(true);
                            }
                            else
                            {
                                playerUI[j].SetActive(false);
                            }
                        }

                        isPlayerTurn = true;
                    }
                }
                
                if (pawns[i].gameObject == null || pawns[i].gameObject.activeInHierarchy == false)
                {
                    // if a pawn is killed, we remove him from the list to avoid error.
                    pawns.Remove(pawns[i].gameObject);
                    isPlayerTurn = false;

                    if (curIndexTurn >= pawns.Count)
                    {
                        curIndexTurn = 0;
                    }
                }
            }
        }
    }

    /// <summary>
    /// current pawn has played ?
    /// </summary>
    public void Played()
    {
        if (!isMulti)
        {
            //Debug.Log("played");
            isEnemyTurn = false;
            curIndexTurn++;
            FindObjectOfType<AudioManager>().Play("NextTurn");

            // if curIndexTurn is at the end of the list, return to 0
            if (curIndexTurn >= pawns.Count)
            {
                curIndexTurn = 0;
            }
        }
        else
        {
            //Debug.Log("played");
            isPlayerTurn = false;
            curIndexTurn++;
            FindObjectOfType<AudioManager>().Play("NextTurn");

            // if curIndexTurn is at the end of the list, return to 0
            if (curIndexTurn >= pawns.Count)
            {
                curIndexTurn = 0;
            }
        }
    }

    /// <summary>
    /// Player win the level, AI loose
    /// </summary>
    private void PlayerWin()
    {
        if (!endUI.activeInHierarchy)
        {
            FindObjectOfType<AudioManager>().Play("Win");
            endUI.SetActive(true);
        }

        if (!isSolo)
        {
            winBtn.GetComponent<Button>().interactable = true;
        }
        else
        {
            winBtn.GetComponent<Button>().interactable = false;
        }
        endTxt.text = "Player win!";

        // this make the player return right at the levels menu, and not to the main menu, before.
        PlayerPrefs.SetInt("Story", 1);
    }

    /// <summary>
    /// AI win the level, you loose
    /// </summary>
    private void AIWin()
    {
        if (!endUI.activeInHierarchy)
        {
            FindObjectOfType<AudioManager>().Play("Lose");
            currAdCount++;

            if(currAdCount <= adMaxPerTurn)
            FindObjectOfType<AdsInitializer>().OnInitializationComplete();

            endUI.SetActive(true);
        }

        if(!isSolo)
        winBtn.GetComponent<Button>().interactable = false;

        endTxt.text = "AI win.";
    }

    /// <summary>
    /// A specific Player win the game
    /// </summary>
    private void ThisPlayerWin()
    {
        if (!endUI.activeInHierarchy)
        {
            FindObjectOfType<AudioManager>().Play("Win");
            endUI.SetActive(true);
        }

        winBtn.GetComponent<Button>().interactable = true;
        endTxt.text = pawns[0].gameObject.name + " win!";

        PlayerPrefs.SetInt("Story", 0);
    }

    /// <summary>
    /// Player can revive and continue playing / increasing his score
    /// </summary>
    private void Revive()
    {
        player.GetComponent<PlayerHP>().Heal(25);
        player.SetActive(true);
        curIndexTurn = 0;
        endUI.SetActive(false);
        canRevive = false;
    }

    public int GetTurnCount()
    {
        return turnCount;
    }

    public GameObject GetCurrPawnPlaying()
    {
        return currPawnPlaying;
    }

    public void NewPawn(GameObject pawn)
    {
        pawns.Add(pawn);
    }
}
