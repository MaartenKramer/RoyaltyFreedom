using UnityEngine;

public enum DamageType
{
    Blunt,
    Piercing,
    Electronic,
    Slashing,
    None
}

[CreateAssetMenu(fileName = "New Attack", menuName = "Combat/Attack")]
public class Attack : ScriptableObject
{
    public string attackName;
    public string flavorText;
    public int damage;
    public DamageType damageType;
    

    [Header("Player only")]
    public string comboSequence;

}