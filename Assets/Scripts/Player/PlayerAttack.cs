using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    // Can player attack ?
    private bool canAttack = false;
    // Damage amount
    private int damage = 20;
    // Attack Btn
    [SerializeField] GameObject attackBtn;
    //Ref
    private PlayerController playerController;

    [Header("Spells")]
    [SerializeField] private Spell spellGreen;
    [SerializeField] private Spell spellBlack;
    [SerializeField] private Spell spellPink;
    [SerializeField] private Spell spellYellow;
    [SerializeField] private Spell spellRed;
    [SerializeField] private Spell spellBlue;

    private void Awake()
    {
        playerController = gameObject.GetComponent<PlayerController>();
    }

    private void Update()
    {
        // If player can attack and choose a target with his spell
        if (canAttack && Input.GetKeyDown(KeyCode.Mouse0) && playerController.GetcanMove())
        {
            DetectTarget();
        }

        CheckSpell();
    }

    /// <summary>
    /// detect the target the player choose to attack
    /// </summary>
    private void DetectTarget()
    {
        // Raycast to detect the gameObject at the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // An object is hit
            GameObject hitObject = hit.collider.gameObject;

            // Check if the object is an Enemy and give him damage
            if (hitObject.CompareTag("Enemy"))
            {
                //Debug.Log(hitObject.name + " hit");
                hitObject.GetComponent<EnemyStats>().TakeDamage(damage);
                attackBtn.SetActive(true);

                // End turn
                playerController.SetcanMove(false);

                canAttack = false;
            }
        }
    }

    /// <summary>
    /// player select attack button
    /// </summary>
    public void Attack()
    {
        canAttack = true;
        attackBtn.SetActive(false);
    }

    /// <summary>
    /// What is the current spell ?
    /// </summary>
    private void CheckSpell()
    {
        switch (playerController.DiceUpColor())
        {
            case "Green":
                //Debug.Log("Feuille tranchante");
                attackBtn.GetComponent<Image>().color = Color.green;
                damage = spellGreen.damage;
                break;
            case "Black":
                //Debug.Log("Trou noir");
                attackBtn.GetComponent<Image>().color = Color.black;
                damage = spellBlack.damage;
                break;
            case "Pink":
                //Debug.Log("Charme");
                attackBtn.GetComponent<Image>().color = Color.magenta;
                damage = spellPink.damage;
                break;
            case "Yellow":
                //Debug.Log("Ultime");
                attackBtn.GetComponent<Image>().color = Color.yellow;
                damage = spellYellow.damage;
                break;
            case "Red":
                //Debug.Log("Vol de vie");
                attackBtn.GetComponent<Image>().color = Color.red;
                damage = spellRed.damage;
                break;
            case "Blue":
                //Debug.Log("Recupération de mana");
                attackBtn.GetComponent<Image>().color = Color.blue;
                damage = spellBlue.damage;
                break;
        }
    }
}
