using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EnemyBase))]
public class EnemyStats : MonoBehaviour
{
    [Header("Health Points")]
    [SerializeField] private Slider healthBar;
    [SerializeField][Range(1, 300)] private float maxHP;
    private float currHP;

    [Header("Malus")]
    [SerializeField] int poisonDamage;
    [HideInInspector] bool isPoisoned;
    [SerializeField] int maxPoisonDuration;
    [SerializeField] Image fill;
    [SerializeField] Color NormalColor;
    [SerializeField] Color PoisonedColor;
    int poisonDuration;
    int curPoisonDuration;

    [Header("UI")]
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Transform cameraDir;

    private EnemyBase enemyBase;

    [Header("Damages")]
    [SerializeField][Range(1, 50)] int damage;
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

        // Set
        poisonDuration = 0;
        curPoisonDuration = 0;
        fill.color = NormalColor;
    }

    private void Update()
    {
        CanvasDisplay();

        // If he is next to player he can attack (detection is in EnemyBase script)
        if (canAttack && enemyBase.GetcanMove())
        {
            Attack();
        }

        PoisonEffect();
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

                // End turn
                enemyBase.SetcanMove(false);
            }
        }
        if (Physics.Raycast(transform.position + transform.up * yOffset - transform.forward, transform.TransformDirection(-Vector3.up), out hitPlayer, Mathf.Infinity, whatIsObstacle))
        {
            GameObject go = hitPlayer.collider.gameObject;

            if (go.CompareTag("Player"))
            {
                //Debug.Log(go.name + " hit");
                go.GetComponent<PlayerHP>().TakeDamage(damage);

                // End turn
                enemyBase.SetcanMove(false);
            }
        }
        if (Physics.Raycast(transform.position + transform.up * yOffset + transform.right, transform.TransformDirection(-Vector3.up), out hitPlayer, Mathf.Infinity, whatIsObstacle))
        {
            GameObject go = hitPlayer.collider.gameObject;

            if (go.CompareTag("Player"))
            {
                //Debug.Log(go.name + " hit");
                go.GetComponent<PlayerHP>().TakeDamage(damage);

                // End turn
                enemyBase.SetcanMove(false);
            }
        }
        if (Physics.Raycast(transform.position + transform.up * yOffset - transform.right, transform.TransformDirection(-Vector3.up), out hitPlayer, Mathf.Infinity, whatIsObstacle))
        {
            GameObject go = hitPlayer.collider.gameObject;

            if (go.CompareTag("Player"))
            {
                //Debug.Log(go.name + " hit");
                go.GetComponent<PlayerHP>().TakeDamage(damage);

                // End turn
                enemyBase.SetcanMove(false);
            }
        }
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
    public void TakeDamage(int damage, bool _isPoisoned)
    {
        // take or not, poison damage
        isPoisoned = _isPoisoned;

        currHP -= damage;
        Debug.Log(currHP);
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
    /// Take poison damage each turn
    /// </summary>
    void PoisonEffect()
    {
        // If he is poisoned
        if (enemyBase.GetcanMove() && isPoisoned)
        {
            fill.color = PoisonedColor;

            if (curPoisonDuration == poisonDuration)
            {
                TakeDamage(poisonDamage, true);

                curPoisonDuration++;
                Debug.Log("Enemy take poison damage");

                // ">", because we don't want to count the first damage as a turn
                if (curPoisonDuration > maxPoisonDuration)
                {
                    isPoisoned = false;
                    poisonDuration = 0;
                    curPoisonDuration = 0;
                    fill.color = NormalColor;
                    Debug.Log("Enemy is'nt poisoned anymore");
                }
            }
        }
        else if (!enemyBase.GetcanMove())
        {

            poisonDuration = curPoisonDuration;
        }
    }

    /// <summary>
    /// make UI look at the camera
    /// </summary>
    void CanvasDisplay()
    {
        _canvas.transform.LookAt(cameraDir);
    }
}
