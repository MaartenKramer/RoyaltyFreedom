using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatDialogueTrigger
{
    [Tooltip("Which turn number this triggers on. Set to -1 to trigger ANY time the state happens.")]
    public int turnNumber = -1;

    [Tooltip("Which phase of combat this triggers in")]
    public BattleState triggerState;

    [Tooltip("The dialogue to show")]
    public DialogueLine[] dialogue;

    [HideInInspector]
    public bool hasTriggered = false;
}

public class Unit : MonoBehaviour
{
    public string unitName;
    public string combatIntro;
    public int unitLevel;
    public int attack;
    public int specialAttack;
    public int defense;
    public int specialDefense;
    public int maxHP;
    public int currentHP;

    public DamageType weaknessType;
    public DamageType resistantType;

    public List<Attack> attacks = new List<Attack>();

    public CombatDialogueTrigger[] combatDialogue;

    public bool TakeDamage(int atk, int def)
    {
        int damage = atk * 4 - def * 2;
        if (damage < 0) damage = 0;
        currentHP -= damage;
        return currentHP <= 0;
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP)
            currentHP = maxHP;
    }
}