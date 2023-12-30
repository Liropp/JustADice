using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellName", menuName = "ScriptableObjects/Spell")]
public class Spell : ScriptableObject
{
    public int damage = 20;
    public int healPoints = 0;
    public bool _isPoisoned = false;
    public bool _isDistantAttack = false;
    public bool _canTeleport = false;
    public bool _canHeal = false;
    [HideInInspector] public bool canUseSpell = true;
    [HideInInspector] public int curUseSpellCooldown = 2;
    public int useSpellMaxCooldown = 2;
    public bool enable = true;

    // only for inspector info
    public string description;
}
