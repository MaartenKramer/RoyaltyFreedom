using UnityEngine;
using TMPro;
using System.Collections;

public enum BattleState { START, PLAYERTURN, PLAYERCOMBO, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    Unit playerUnit;
    Unit enemyUnit;

    public TextMeshProUGUI dialogueText;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public BattleState state;
    public string currentCombo;

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    void Update()
    {
        if (state == BattleState.PLAYERCOMBO)
        {
            if (Input.GetKeyDown(KeyCode.W)) currentCombo += "W";
            if (Input.GetKeyDown(KeyCode.A)) currentCombo += "A";
            if (Input.GetKeyDown(KeyCode.S)) currentCombo += "S";
            if (Input.GetKeyDown(KeyCode.D)) currentCombo += "D";

            
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

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
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
        bool isDead = enemyUnit.TakeDamage(playerUnit.attack, enemyUnit.defense);
        enemyHUD.SetHP(enemyUnit.currentHP);
        dialogueText.text = "You strike " + enemyUnit.unitName;

        if (isDead ) 
        {
            state = BattleState.WON;

            yield return new WaitForSeconds(2f);

            EndBattle();
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
        bool isDead;

        int attackChoice = Random.Range(0, 4);

        if (attackChoice != 0) {


            dialogueText.text = enemyUnit.unitName + " Attacks!";

            yield return new WaitForSeconds(2f);

            isDead = playerUnit.TakeDamage(enemyUnit.attack, playerUnit.defense);
        }

        else
        {
            dialogueText.text = enemyUnit.unitName + " fumbles his attack.";


            yield return new WaitForSeconds(2f);

            isDead = playerUnit.TakeDamage(1, playerUnit.defense);
        }

        playerHUD.SetHP(playerUnit.currentHP);

        if (isDead) 
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn() ;
        }
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogueText.text = "You won the battle!";
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "You were defeated.";
        }
    }

    void PlayerTurn()
    {
        currentCombo = "";
        dialogueText.text = "Choose an action:";
    }

    IEnumerator PlayerHeal()
    {
        playerUnit.Heal(12);

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

        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());
    }

    public void OnComboButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        state = BattleState.PLAYERCOMBO;
    }

    public IEnumerator ComboExecute()
    {

        if (currentCombo == "WWW")
        {
            bool isDead = enemyUnit.TakeDamage(playerUnit.attack, enemyUnit.defense);
            enemyHUD.SetHP(enemyUnit.currentHP);
            dialogueText.text = "You throw a pushpin at " + enemyUnit.unitName + "!";


            if (isDead)
            {
                state = BattleState.WON;

                yield return new WaitForSeconds(2f);

                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;

                yield return new WaitForSeconds(2f);

                StartCoroutine(EnemyTurn());
            }
        }

        if (currentCombo == "SSS")
        {
            bool isDead = enemyUnit.TakeDamage(playerUnit.attack, enemyUnit.defense);
            enemyHUD.SetHP(enemyUnit.currentHP);
            dialogueText.text = "You slice" + enemyUnit.unitName + " with a piece of paper!";


            if (isDead)
            {
                state = BattleState.WON;

                yield return new WaitForSeconds(2f);

                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;

                yield return new WaitForSeconds(2f);

                StartCoroutine(EnemyTurn());
            }
        }

        if (currentCombo == "AAA")
        {
            bool isDead = enemyUnit.TakeDamage(playerUnit.attack, enemyUnit.defense);
            enemyHUD.SetHP(enemyUnit.currentHP);
            dialogueText.text = "You give " + enemyUnit.unitName + " a powerful flick on the forehead!";


            if (isDead)
            {
                state = BattleState.WON;

                yield return new WaitForSeconds(2f);

                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;

                yield return new WaitForSeconds(2f);

                StartCoroutine(EnemyTurn());
            }
        }

        if (currentCombo == "DDD")
        {
            bool isDead = enemyUnit.TakeDamage(playerUnit.attack, enemyUnit.defense);
            enemyHUD.SetHP(enemyUnit.currentHP);
            dialogueText.text = "You shine a laserpointer in " + enemyUnit.unitName + "'s eyes!";


            if (isDead)
            {
                state = BattleState.WON;

                yield return new WaitForSeconds(2f);

                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;

                yield return new WaitForSeconds(2f);

                StartCoroutine(EnemyTurn());
            }
        }

        {
            yield return new WaitForSeconds(2f);

            StartCoroutine(EnemyTurn());
        }
    }
}
