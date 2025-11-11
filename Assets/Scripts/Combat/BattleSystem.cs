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
        dialogueText.text = "You strike " + enemyUnit.unitName + "!";

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

        int attackChoice = Random.Range(0, 5);

        if (attackChoice == 1 || attackChoice == 2) {


            dialogueText.text = enemyUnit.unitName + " Strikes!";

            yield return new WaitForSeconds(2f);

            isDead = playerUnit.TakeDamage(enemyUnit.attack, playerUnit.defense);
        }

        else if (attackChoice == 3 || attackChoice == 4)
        {


            dialogueText.text = enemyUnit.unitName + " Rams into you!";

            yield return new WaitForSeconds(2f);

            enemyUnit.TakeDamage(2, 2);
            if (enemyUnit.currentHP < 1)
            {
                enemyUnit.currentHP = 1;
            }


            isDead = playerUnit.TakeDamage(enemyUnit.attack + 2, playerUnit.defense);
        }

        else
        {
            dialogueText.text = enemyUnit.unitName + " fumbles his attack.";


            yield return new WaitForSeconds(2f);

            isDead = playerUnit.TakeDamage(1, 0);
        }
        enemyHUD.SetHP(enemyUnit.currentHP);
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
        if (currentCombo == "WWA")
        {
            bool isDead = enemyUnit.TakeDamage(playerUnit.specialAttack, enemyUnit.specialDefense);
            enemyHUD.SetHP(enemyUnit.currentHP);
            currentCombo = "";
            dialogueText.text = "You Throw a pushpin at " + enemyUnit.unitName + "!";

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

        else if (currentCombo == "DDD")
        {
            bool isDead = enemyUnit.TakeDamage(playerUnit.specialAttack, enemyUnit.specialDefense);
            enemyHUD.SetHP(enemyUnit.currentHP);
            currentCombo = "";
            dialogueText.text = "You point a laserpointer in " + enemyUnit.unitName + "'s eyes!";

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

        else if (currentCombo == "SSS")
        {
            bool isDead = enemyUnit.TakeDamage(playerUnit.specialAttack, enemyUnit.specialDefense);
            enemyHUD.SetHP(enemyUnit.currentHP);
            currentCombo = "";
            dialogueText.text = "You cut " + enemyUnit.unitName + "'s finger with some paper!";

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

        else
        {
            currentCombo = "";
            dialogueText.text = "You fail to realize a combo.";
            yield return new WaitForSeconds(2f);

            StartCoroutine(EnemyTurn());
        }

    }
}
