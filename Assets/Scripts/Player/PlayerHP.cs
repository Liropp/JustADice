using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    // Ref
    [SerializeField] private Slider healthBar;
    
    [SerializeField] [Range(1,1000)] private float maxHP;
    private float currHP;

    private void Awake()
    {
        // Fill HP 
        currHP = maxHP;
        RefreshHealthBar();
    }

    /// <summary>
    /// refresh the health bar with the current player's health
    /// </summary>
    void RefreshHealthBar()
    {
        healthBar.value = currHP;
    }

    /// <summary>
    /// enemy call this give damage to player
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        currHP -= damage;
        RefreshHealthBar();

        // if the player have 0 or less HP, he is dead
        if (currHP <= 0)
        {
            Dead();
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
