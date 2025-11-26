using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;
    public int attack;
    public int specialAttack;
    public int defense;
    public int specialDefense;

    public int maxHP;
    public int currentHP;
    
    
    public List<Attack> attacks = new List<Attack>();

    public bool TakeDamage(int atk, int def)
    {
        int damage = atk * 4 - def * 2;
        if (damage < 0) damage = 0;

        currentHP -= damage;
        if (currentHP <= 0)
        {
            return true;
        }
        else
            return false;
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP)
            currentHP = maxHP;
    }
}
