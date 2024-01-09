using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class InfinityManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject knightPrefab;
    [SerializeField] private GameObject princePrefab;
    [SerializeField] private GameObject kingPrefab;
    [SerializeField] private GameObject dragonPrefab;
    [Header("Other")]
    private GameManager gm;
    [SerializeReference] private GameObject firstAI;
    [SerializeReference] private Transform _target;
    [SerializeReference] private Transform _camDir;
    [SerializeField] private int respawnMaxCooldown = 2;
    [SerializeField] private int maxPawns = 4;
    private int respawnCooldown = 0;
    private List<GameObject> pawnsList = new List<GameObject>();
    [Header("Scoring")]
    private int score = 0;
    [SerializeReference] private TextMeshProUGUI scoreTxtInGame;
    [SerializeReference] private TextMeshProUGUI scoreTxt;
    [SerializeReference] private TextMeshProUGUI bestScoreTxt;
    [SerializeReference] private GameObject endUI;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        respawnCooldown = gm.GetTurnCount() + respawnMaxCooldown;
        pawnsList.Add(firstAI);
    }

    private void Update()
    {
        Debug.Log("TurnCount : " + gm.GetTurnCount());

        Collider[] hitForward = Physics.OverlapSphere(transform.position, 0.35f);
        if(hitForward.Length > 0)
        {
            Debug.Log("I COLLIDE WITH : " + hitForward[0].gameObject.name);
        }
        else
        {
            if (gm.GetTurnCount() >= 300)
            {
                //Debug.Log("spawn dragons");

                if (gm.GetTurnCount() >= respawnCooldown && pawnsList.Count < maxPawns - 1 || pawnsList.Count == 1)
                {
                    //Debug.Log("New Pawn Spawned");

                    GameObject _pawn = Instantiate(dragonPrefab, transform.position, Quaternion.identity);
                    _pawn.GetComponent<EnemyBase>().target = _target;
                    _pawn.GetComponent<EnemyStats>().cameraDir = _camDir;
                    gm.NewPawn(_pawn);
                    pawnsList.Add(_pawn);

                    respawnCooldown = gm.GetTurnCount() + respawnMaxCooldown;
                }
            }
            else if (gm.GetTurnCount() > 200)
            {
                //Debug.Log("spawn kings");

                if (gm.GetTurnCount() >= respawnCooldown && pawnsList.Count < maxPawns - 1 || pawnsList.Count == 1)
                {
                    //Debug.Log("New Pawn Spawned");

                    GameObject _pawn = Instantiate(kingPrefab, transform.position, Quaternion.identity);
                    _pawn.GetComponent<EnemyBase>().target = _target;
                    _pawn.GetComponent<EnemyStats>().cameraDir = _camDir;
                    gm.NewPawn(_pawn);
                    pawnsList.Add(_pawn);

                    respawnCooldown = gm.GetTurnCount() + respawnMaxCooldown;
                }
            }
            else if (gm.GetTurnCount() > 100)
            {
                //Debug.Log("spawn princes");

                if (gm.GetTurnCount() >= respawnCooldown && pawnsList.Count < maxPawns - 1 || pawnsList.Count == 1)
                {
                    //Debug.Log("New Pawn Spawned");

                    GameObject _pawn = Instantiate(princePrefab, transform.position, Quaternion.identity);
                    _pawn.GetComponent<EnemyBase>().target = _target;
                    _pawn.GetComponent<EnemyStats>().cameraDir = _camDir;
                    gm.NewPawn(_pawn);
                    pawnsList.Add(_pawn);

                    respawnCooldown = gm.GetTurnCount() + respawnMaxCooldown;
                }
            }
            else
            {
                //Debug.Log("pawnsList.Count " + pawnsList.Count);

                if (gm.GetTurnCount() >= respawnCooldown && pawnsList.Count < maxPawns - 1 || pawnsList.Count == 1)
                {
                    //Debug.Log("New Pawn Spawned");

                    score += 50;
                    GameObject _pawn = Instantiate(knightPrefab, transform.position, Quaternion.identity);
                    _pawn.GetComponent<EnemyBase>().target = _target;
                    _pawn.GetComponent<EnemyStats>().cameraDir = _camDir;
                    gm.NewPawn(_pawn);
                    pawnsList.Add(_pawn);

                    respawnCooldown = gm.GetTurnCount() + respawnMaxCooldown;
                }

                //Debug.Log("spawn knights");
            }
        }

        for (int i = 0; i < pawnsList.Count; i++)
        {
            if (pawnsList[i] == null)
            {
                pawnsList.Remove(pawnsList[i]);
                score += 100;
            }
        }

        if (endUI.activeInHierarchy)
        {
            DisplayScore();
        }

        scoreTxtInGame.text = "Score : " + score.ToString();
    }

    private void DisplayScore()
    {
        scoreTxt.text = "Score : " + score.ToString();

        if(score > PlayerPrefs.GetInt("BestScore"))
        {
            // new best score

            PlayerPrefs.SetInt("BestScore", score);
            bestScoreTxt.text = "High score : " + score.ToString();
        }
        else
        {
            // not your best score

            bestScoreTxt.text = "High score : " + PlayerPrefs.GetInt("BestScore").ToString();
        }

        if (PlayerPrefs.GetInt("BestScore", 0) <= 0)
        {
            // first best score

            PlayerPrefs.SetInt("BestScore", score);
            bestScoreTxt.text = "High score : " + score.ToString();
        }
    }
}
