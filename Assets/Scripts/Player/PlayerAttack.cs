using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    // Can player attack ?
    private bool canAttack = false;
    // Damage amount
    private int damage = 20;
    private bool _isPoisoned = false;
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
    private bool enemyAround = false;

    private void Awake()
    {
        playerController = gameObject.GetComponent<PlayerController>();
    }

    private void Update()
    {
        DetectTargetAround();

        // If player can attack and choose a target with his spell
        if (canAttack && Input.GetKeyDown(KeyCode.Mouse0) && playerController.GetcanMove())
        {
            DetectTarget();
        }

        CheckSpell();
    }

    /// <summary>
    /// detect target, who player choose to attack
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

            // Check if the object is an enemy and if he next to player, in order to give him damage
            if (hitObject.CompareTag("Enemy"))
            {
                float dist = Mathf.Round(Vector3.Distance(hitObject.transform.position, transform.position));
                //Debug.Log(dist);

                if (dist <= 1)
                {
                    //Debug.Log(hitObject.name + " hit");
                    hitObject.GetComponent<EnemyStats>().TakeDamage(damage, _isPoisoned);
                    attackBtn.SetActive(true);

                    // End turn
                    playerController.SetcanMove(false);

                    canAttack = false;
                }
            }
        }
    }

    /// <summary>
    /// detect if there is an enemy around the player in order to attack
    /// </summary>
    private void DetectTargetAround()
    {
        Vector3 forwardPos = transform.position + Vector3.forward;
        Vector3 backPos = transform.position + Vector3.back;
        Vector3 rightPos = transform.position + Vector3.right;
        Vector3 leftPos = transform.position + Vector3.left;
        float rad = 0.35f;

        Collider[] hitForward = Physics.OverlapSphere(forwardPos, rad);
        Collider[] hitBack = Physics.OverlapSphere(backPos, rad);
        Collider[] hitRight = Physics.OverlapSphere(rightPos, rad);
        Collider[] hitLeft = Physics.OverlapSphere(leftPos, rad);
        
        if(hitForward.Length > 0)
        {
            foreach (var hitCollider in hitForward)
            {
                // An object is hit
                GameObject hitObject = hitCollider.gameObject;

                // Check if the object is an Enemy
                if (hitObject.CompareTag("Enemy"))
                {
                    //Debug.Log("enemy around");
                    enemyAround = true;
                }
            }
        }
        else if (hitBack.Length > 0)
        {
            foreach (var hitCollider in hitBack)
            {
                // An object is hit
                GameObject hitObject = hitCollider.gameObject;

                // Check if the object is an Enemy
                if (hitObject.CompareTag("Enemy"))
                {
                    //Debug.Log("enemy around");
                    enemyAround = true;
                }
            }
        }
        else if (hitRight.Length > 0)
        {
            foreach (var hitCollider in hitRight)
            {
                // An object is hit
                GameObject hitObject = hitCollider.gameObject;

                // Check if the object is an Enemy
                if (hitObject.CompareTag("Enemy"))
                {
                    //Debug.Log("enemy around");
                    enemyAround = true;
                }
            }
        }
        else if (hitLeft.Length > 0)
        {
            foreach (var hitCollider in hitLeft)
            {
                // An object is hit
                GameObject hitObject = hitCollider.gameObject;

                // Check if the object is an Enemy
                if (hitObject.CompareTag("Enemy"))
                {
                    //Debug.Log("enemy around");
                    enemyAround = true;
                }
            }
        }
        else
        {
            enemyAround = false;
        }
    }

    /// <summary>
    /// player select attack button
    /// </summary>
    public void Attack()
    {
        // only if there is an enemy next to player
        if (enemyAround)
        {
            canAttack = true;
            attackBtn.SetActive(false);
        }
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
                _isPoisoned = spellGreen._isPoisoned;
                break;
            case "Black":
                //Debug.Log("Trou noir");
                attackBtn.GetComponent<Image>().color = Color.black;
                damage = spellBlack.damage;
                _isPoisoned = spellBlack._isPoisoned;
                break;
            case "Pink":
                //Debug.Log("Charme");
                attackBtn.GetComponent<Image>().color = Color.magenta;
                damage = spellPink.damage;
                _isPoisoned = spellPink._isPoisoned;
                break;
            case "Yellow":
                //Debug.Log("Ultime");
                attackBtn.GetComponent<Image>().color = Color.yellow;
                damage = spellYellow.damage;
                _isPoisoned = spellYellow._isPoisoned;
                break;
            case "Red":
                //Debug.Log("Vol de vie");
                attackBtn.GetComponent<Image>().color = Color.red;
                damage = spellRed.damage;
                _isPoisoned = spellRed._isPoisoned;
                break;
            case "Blue":
                //Debug.Log("Recupération de mana");
                attackBtn.GetComponent<Image>().color = Color.blue;
                damage = spellBlue.damage;
                _isPoisoned = spellBlue._isPoisoned;
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 forwardPos = transform.position + Vector3.forward;
        Vector3 backPos = transform.position + Vector3.back;
        Vector3 rightPos = transform.position + Vector3.right;
        Vector3 leftPos = transform.position + Vector3.left;
        float rad = 0.35f;

        Gizmos.DrawSphere(forwardPos, rad);
        Gizmos.DrawSphere(backPos, rad);
        Gizmos.DrawSphere(rightPos, rad);
        Gizmos.DrawSphere(leftPos, rad);
    }
}
