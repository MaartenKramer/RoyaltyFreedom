using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleEncounter : MonoBehaviour
{
    public int enemyIndex;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && enemyIndex != 2)
        {
            BattleData.enemyIndex = enemyIndex;
            SceneManager.LoadScene("LunCombat");
           
        }

       else
        {
            BattleData.enemyIndex = enemyIndex;
            SceneManager.LoadScene("TutRoomCombat");

        }
    }
}