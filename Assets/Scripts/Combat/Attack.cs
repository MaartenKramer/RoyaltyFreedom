using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Combat/Attack")]
public class Attack : ScriptableObject
{
    public string attackName;
    public string flavorText;
    public int damage;
}