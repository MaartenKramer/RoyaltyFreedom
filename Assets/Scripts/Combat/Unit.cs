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

    public bool TakeDamage(int atk, int def)
    {
        currentHP -= (atk * 4 - def * 2);

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
