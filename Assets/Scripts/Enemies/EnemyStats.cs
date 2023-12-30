using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EnemyBase))]
public class EnemyStats : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private StatsAI stats;

    [Header("Health")]
    [SerializeField] private Slider healthBar;
    private float maxHP;
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
    public int spl_disableAmount { get; private set; }

    [Header("UI")]
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Transform cameraDir;

    private EnemyBase enemyBase;

    [Header("Damages")]
    private int damage;
    [HideInInspector] public bool canAttack = false;

    [Header("Layer(s)")]
    [SerializeField] LayerMask whatIsObstacle;

    [Header("Raycasts")]
    [SerializeField] private float yOffset;

    private void Awake()
    {
        //Ref
        enemyBase = gameObject.GetComponent<EnemyBase>();

        // ScriptableObject stats
        maxHP = stats._maxHP;
        healthBar.maxValue = stats._maxHP;
        damage = stats._damage;
        spl_disableAmount = stats._spl_disableAmount;
        enemyBase.moveDist = stats._moveDist;

        // Fill HP
        currHP = maxHP;
        RefreshHealthBar();

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
        //Debug.Log(enemyBase.target.name + " hit");
        enemyBase.target.GetComponent<PlayerHP>().TakeDamage(damage);

        switch (stats.name)
        {
            case "Knight_Stats":
                FindObjectOfType<AudioManager>().Play("KnightAttack");
                break;
            case "Prince_Stats":
                FindObjectOfType<AudioManager>().Play("PrinceAttack");
                break;
            case "King_Stats":
                FindObjectOfType<AudioManager>().Play("KingAttack");
                break;
            case "Dragon_Stats":
                FindObjectOfType<AudioManager>().Play("DragonAttack");
                break;
        }

        // End turn
        enemyBase.SetcanMove(false);
        canAttack = false;
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
        if(!isPoisoned)
        isPoisoned = _isPoisoned;

        currHP -= damage;
        //Debug.Log(currHP);
        RefreshHealthBar();

        FindObjectOfType<AudioManager>().Play("Damage");

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
    /// take poison damage each turn
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
                //Debug.Log("Enemy take poison damage");

                // ">", because we don't want to count the first damage as a turn
                if (curPoisonDuration > maxPoisonDuration)
                {
                    isPoisoned = false;
                    poisonDuration = 0;
                    curPoisonDuration = 0;
                    fill.color = NormalColor;
                    //Debug.Log("Enemy is'nt poisoned anymore");
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
