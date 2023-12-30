using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

public class PlayerHP : MonoBehaviour
{
    // Ref
    [SerializeField] private Slider healthBar;
    
    [SerializeField] [Range(1,1000)] private float maxHP;
    private float currHP;

    [HideInInspector] public bool canTakeDamage = true;

    [Header("Malus")]
    [SerializeField] int poisonDamage;
    [HideInInspector] bool isPoisoned;
    [SerializeField] int maxPoisonDuration;
    private Image fill;
    [SerializeField] Color NormalColor;
    [SerializeField] Color PoisonedColor;
    int poisonDuration;
    int curPoisonDuration;

    private void Awake()
    {
        fill = healthBar.fillRect.GetComponent<Image>();

        // Fill HP 
        currHP = maxHP;
        RefreshHealthBar();
    }

    private void Update()
    {
        if (!gameObject.GetComponent<PlayerController>().isTuto)
        {
            if (FindObjectOfType<GameManager>().isMulti)
            {
                PoisonEffect();
            }
        }
    }

    /// <summary>
    /// take poison damage each turn
    /// </summary>
    void PoisonEffect()
    {
        // If he is poisoned
        if (gameObject.GetComponent<PlayerController>().GetcanMove() && isPoisoned)
        {
            fill.color = PoisonedColor;

            if (curPoisonDuration == poisonDuration)
            {
                PlayerTakeDamage(poisonDamage, true);

                curPoisonDuration++;
                //Debug.Log("Player take poison damage");

                // ">", because we don't want to count the first damage as a turn
                if (curPoisonDuration > maxPoisonDuration)
                {
                    isPoisoned = false;
                    poisonDuration = 0;
                    curPoisonDuration = 0;
                    fill.color = NormalColor;
                    //Debug.Log("Player is'nt poisoned anymore");
                }
            }
        }
        else if (!gameObject.GetComponent<PlayerController>().GetcanMove())
        {
            poisonDuration = curPoisonDuration;
        }
    }

    /// <summary>
    /// refresh the health bar with the current player's health
    /// </summary>
    void RefreshHealthBar()
    {
        if(healthBar != null)
        healthBar.value = currHP;
    }

    /// <summary>
    /// enemy call this give damage to player
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        if (canTakeDamage)
        {
            currHP -= damage;
            RefreshHealthBar();

            FindObjectOfType<AudioManager>().Play("Damaged");

            // if the player have 0 or less HP, he is dead
            if (currHP <= 0)
            {
                Dead();
            }
        }
    }

    /// <summary>
    /// player call this give damage to another player
    /// </summary>
    /// <param name="damage"></param>
    public void PlayerTakeDamage(int damage, bool _isPoisoned)
    {
        if (canTakeDamage)
        {
            // take or not, poison damage
            if (!isPoisoned)
                isPoisoned = _isPoisoned;

            currHP -= damage;
            RefreshHealthBar();

            FindObjectOfType<AudioManager>().Play("Damaged");

            // if the player have 0 or less HP, he is dead
            if (currHP <= 0)
            {
                Dead();
            }
        }
    }

    /// <summary>
    /// player call this give HP to himself
    /// </summary>
    /// <param name="damage"></param>
    public void Heal(int pv)
    {
        //Debug.Log("Hurt : " + currHP);
        currHP += pv;
        //Debug.Log("Heal : " + currHP);
        RefreshHealthBar();

        // The player cannot have more HP than the maxHP.
        if (currHP > maxHP)
        {
            currHP = maxHP;
        }
    }

    /// <summary>
    /// the dice is dead and loose
    /// </summary>
    void Dead()
    {
        gameObject.SetActive(false);
    }
}
