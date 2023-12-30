using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_Stats", menuName = "ScriptableObjects/Stats")]
public class StatsAI : ScriptableObject
{
    public float _maxHP;
    public int _damage;
    public int _spl_disableAmount;
    public int _moveDist;
}
