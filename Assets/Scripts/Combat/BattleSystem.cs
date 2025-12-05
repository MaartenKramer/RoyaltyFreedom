using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, PLAYERCOMBO, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject[] enemyPrefabs;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    Unit playerUnit;
    Unit enemyUnit;

    public TextMeshProUGUI dialogueText;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public BattleState state;
    public string currentCombo;

    public Button attackButton;
    public Button healButton;
    public Button comboButton;


    void Start()
    {
        state = BattleState.START;
        UpdateButtons(false);
        StartCoroutine(SetupBattle());
    }

    void Update()
    {
        if (state == BattleState.PLAYERCOMBO)
        {
            bool keyPressed = false;

            if (Input.GetKeyDown(KeyCode.W))
            {
                currentCombo += "W";
                keyPressed = true;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                currentCombo += "A";
                keyPressed = true;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                currentCombo += "S";
                keyPressed = true;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                currentCombo += "D";
                keyPressed = true;
            }

            if (keyPressed)
            {
                enemyUnit.TakeDamage(4, 1);
                enemyHUD.SetHP(enemyUnit.currentHP);
                Debug.Log(enemyUnit.currentHP);
            }

            dialogueText.text = currentCombo;

            if (currentCombo.Length == 3)
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(ComboExecute());
            }
        }
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();
        GameObject enemyGO = Instantiate(enemyPrefabs[BattleData.enemyIndex], enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();
        dialogueText.text = "Office worker " + enemyUnit.unitName + " blocks your path!";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        state = BattleState.PLAYERTURN;

        yield return new WaitForSeconds(2f);

        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        bool isDead = enemyUnit.TakeDamage(1, 0);
        enemyHUD.SetHP(enemyUnit.currentHP);
        dialogueText.text = "You strike " + enemyUnit.unitName + "!";

        if (isDead)
        {
            state = BattleState.WON;

            yield return new WaitForSeconds(2f);

            StartCoroutine(EndBattle());
        }
        else
        {
            state = BattleState.ENEMYTURN;

            yield return new WaitForSeconds(2f);

            StartCoroutine(EnemyTurn());
        }

    }

    IEnumerator EnemyTurn()
    {
        Attack chosenAttack = enemyUnit.attacks[Random.Range(0, enemyUnit.attacks.Count)];

        dialogueText.text = enemyUnit.unitName + " " + chosenAttack.flavorText + "!";

        float damageVariety = Random.Range(0.9f, 1.1f);
        int finalDamage = Mathf.RoundToInt((chosenAttack.damage + enemyUnit.attack) * damageVariety);

        bool isDead = playerUnit.TakeDamage(finalDamage, playerUnit.defense);

        enemyHUD.SetHP(enemyUnit.currentHP);
        playerHUD.SetHP(playerUnit.currentHP);

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.LOST;
            StartCoroutine(EndBattle());
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    IEnumerator EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogueText.text = "You won the battle!";
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "You were defeated.";
        }

        yield return new WaitForSeconds(4f);

        if (enemyUnit.unitName == "Harold")
        {
            SceneManager.LoadScene("Hub");
        }

        else if (enemyUnit.unitName == "Seth Garth")
        {
            SceneManager.LoadScene("RightHall");
        }

        else
        {
            SceneManager.LoadScene("Tutorial");
        }
    }

    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
        UpdateButtons(true);
    }

    IEnumerator PlayerHeal()
    {
        playerUnit.Heal(30);

        playerHUD.SetHP(playerUnit.currentHP);
        dialogueText.text = "You take a bite from an energy bar";

        state = BattleState.PLAYERTURN;

        yield return new WaitForSeconds(2f);

        StartCoroutine(EnemyTurn());
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;
        UpdateButtons(false);
        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;
        UpdateButtons(false);
        StartCoroutine(PlayerHeal());
    }

    public void OnComboButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;
        UpdateButtons(false);
        state = BattleState.PLAYERCOMBO;
    }

    public IEnumerator ComboExecute()
    {
        Attack successfulCombo = null;
        foreach (Attack attack in playerUnit.attacks)
        {
            if (attack.comboSequence == currentCombo)
            {
                successfulCombo = attack;
                break;
            }
        }

        currentCombo = "";

        if (successfulCombo != null)
        {
            float damageVariety = Random.Range(0.9f, 1.1f);
            int finalDamage = Mathf.RoundToInt((successfulCombo.damage + playerUnit.specialAttack) * damageVariety);

            bool isDead = enemyUnit.TakeDamage(finalDamage, enemyUnit.specialDefense);
            enemyHUD.SetHP(enemyUnit.currentHP);
            dialogueText.text = playerUnit.unitName + " " + successfulCombo.flavorText + "!";

            yield return new WaitForSeconds(2f);

            if (isDead)
            {
                state = BattleState.WON;
                StartCoroutine(EndBattle());
            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }
        else
        {
            dialogueText.text = "You fail to realize a combo.";
            yield return new WaitForSeconds(2f);

            if (enemyUnit.currentHP <= 0)
            {
                state = BattleState.WON;
                StartCoroutine(EndBattle());
            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }
    }
    void UpdateButtons(bool isInteractable)
    {
        attackButton.interactable = isInteractable;
        healButton.interactable = isInteractable;
        comboButton.interactable = isInteractable;
    }
}
