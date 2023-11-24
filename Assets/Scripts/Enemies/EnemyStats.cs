using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EnemyBase))]
public class EnemyStats : MonoBehaviour
{
    [Header("Health Points")] [SerializeField] private Slider healthBar;
    [SerializeField][Range(1, 300)] private float maxHP;
    private float currHP;

    [Header("UI")][SerializeField] private Canvas _canvas;
    [SerializeField] private Transform cameraDir;

    private EnemyBase enemyBase;

    [Header("Damages")]
    [SerializeField][Range(1, 50)] int damage;
    [SerializeField] private float attackCooldown;
    private float timer;
    [HideInInspector] public bool canAttack = false;

    [Header("Layer(s)")]
    [SerializeField] LayerMask whatIsObstacle;

    [Header("Raycasts")]
    [SerializeField] private float yOffset;

    private void Awake()
    {
        // Fill HP
        currHP = maxHP;
        RefreshHealthBar();

        //Ref
        enemyBase = gameObject.GetComponent<EnemyBase>();
    }

    private void Update()
    {
        CanvasDisplay();

        // If he is next to player he can attack (detection is in EnemyBase script)
        if (canAttack && enemyBase.GetcanMove())
        {
            // The attackCooldown timer increase each second
            timer += Time.deltaTime;

            // Enemy can attack every cooldown
            if (timer >= attackCooldown)
            {
                Attack();

                // Reset attackCooldown timer
                timer = 0;
            }
        }
    }

    /// <summary>
    /// attack player
    /// </summary>
    void Attack()
    {
        // Check around the enemy (but not in diagonal) if there is a player. If a player is detected he take damage.
        RaycastHit hitPlayer;
        if (Physics.Raycast(transform.position + transform.up * yOffset + transform.forward, transform.TransformDirection(-Vector3.up), out hitPlayer, Mathf.Infinity, whatIsObstacle))
        {
            GameObject go = hitPlayer.collider.gameObject;

            if (go.CompareTag("Player"))
            {
                //Debug.Log(go.name + " hit");
                go.GetComponent<PlayerHP>().TakeDamage(damage);
            }
        }
        if (Physics.Raycast(transform.position + transform.up * yOffset - transform.forward, transform.TransformDirection(-Vector3.up), out hitPlayer, Mathf.Infinity, whatIsObstacle))
        {
            GameObject go = hitPlayer.collider.gameObject;

            if (go.CompareTag("Player"))
            {
                //Debug.Log(go.name + " hit");
                go.GetComponent<PlayerHP>().TakeDamage(damage);
            }
        }
        if (Physics.Raycast(transform.position + transform.up * yOffset + transform.right, transform.TransformDirection(-Vector3.up), out hitPlayer, Mathf.Infinity, whatIsObstacle))
        {
            GameObject go = hitPlayer.collider.gameObject;

            if (go.CompareTag("Player"))
            {
                //Debug.Log(go.name + " hit");
                go.GetComponent<PlayerHP>().TakeDamage(damage);
            }
        }
        if (Physics.Raycast(transform.position + transform.up * yOffset - transform.right, transform.TransformDirection(-Vector3.up), out hitPlayer, Mathf.Infinity, whatIsObstacle))
        {
            GameObject go = hitPlayer.collider.gameObject;

            if (go.CompareTag("Player"))
            {
                //Debug.Log(go.name + " hit");
                go.GetComponent<PlayerHP>().TakeDamage(damage);
            }
        }

        // End turn
        enemyBase.SetcanMove(false);
    }

    /// <summary>
    /// refresh health bar with enemy's current health
    /// </summary>
    void RefreshHealthBar()
    {
        healthBar.value = currHP;
    }

    /// <summary>
    /// player call this give damage to enemies
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        currHP -= damage;
        RefreshHealthBar();

        // if the enemy have 0 or less HP, he is dead
        if (currHP <= 0)
        {
            Dead();
        }
    }

    /// <summary>
    /// this enemy is dead
    /// </summary>
    void Dead()
    {
        Destroy(this.gameObject);
    }

    /// <summary>
    /// make UI look at the camera
    /// </summary>
    void CanvasDisplay()
    {
        _canvas.transform.LookAt(cameraDir);
    }
}
