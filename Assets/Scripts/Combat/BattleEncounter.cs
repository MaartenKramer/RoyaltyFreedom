using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleEncounter : MonoBehaviour
{
    public int enemyIndex;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BattleData.enemyIndex = enemyIndex;
            SceneManager.LoadScene("LunCombat");
           
        }
    }
}