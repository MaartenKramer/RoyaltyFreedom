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

    [Header("UI Buttons")]
    public Button attackButton;
    public Button healButton;
    public Button comboButton;

    [Header("Combos")]
    public string currentCombo;
    public GameObject comboDial;
    public int dialAttack = 4;
    public int dialDefense = 1;

    private Animator playerAnim;
    private Animator enemyAnim;

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
            if (currentCombo.Length >= 5)
                return;

            bool keyPressed = false;

            if (Input.GetKeyDown(KeyCode.W))
            {
                currentCombo += "W";
                playerAnim.Play("DialW");
                keyPressed = true;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                currentCombo += "A";
                playerAnim.Play("DialA");
                keyPressed = true;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                currentCombo += "S";
                playerAnim.Play("DialS");
                keyPressed = true;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                currentCombo += "D";
                playerAnim.Play("DialD");
                keyPressed = true;
            }

            if (keyPressed)
            {
                enemyUnit.TakeDamage(dialAttack, dialDefense);
                enemyHUD.SetHP(enemyUnit.currentHP);
                Debug.Log(enemyUnit.currentHP);
            }

            dialogueText.text = currentCombo;

            if (currentCombo.Length == 5)
            {   

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
        playerAnim = playerGO.GetComponentInChildren<Animator>();
        enemyAnim = enemyGO.GetComponentInChildren<Animator>();
        dialogueText.text = enemyUnit.combatIntro;

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
        comboDial.SetActive(false);

        Attack chosenAttack = enemyUnit.attacks[Random.Range(0, enemyUnit.attacks.Count)];

        dialogueText.text = enemyUnit.unitName + " " + chosenAttack.flavorText + "!";

        enemyAnim.Play("Attack");

        yield return new WaitForSeconds(1f);

        float damageVariety = Random.Range(0.9f, 1.1f);
        int finalDamage = Mathf.RoundToInt((chosenAttack.damage + enemyUnit.attack) * damageVariety);

        bool isDead = playerUnit.TakeDamage(finalDamage, playerUnit.defense);

        enemyHUD.SetHP(enemyUnit.currentHP);
        playerHUD.SetHP(playerUnit.currentHP);

        yield return new WaitForSeconds(1f);

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
            Progress.Instance.flags.Add("Harold_Defeated");
            SceneManager.LoadScene("Hub");
        }

        else if (enemyUnit.unitName == "Seth Garth")
        {
            SceneManager.LoadScene("RightHall");
        }

        else if (enemyUnit.unitName == "Printer 335")
        {
            Progress.Instance.flags.Add("PrinterDies");
            SceneManager.LoadScene("TutorialAlt");
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
        playerUnit.Heal(50);

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
        comboDial.SetActive(true);
        currentCombo = "";
        state = BattleState.PLAYERCOMBO;

    }

    public IEnumerator ComboExecute()
    {
        yield return new WaitForSeconds(1.5f);
        Attack successfulCombo = null;

        for (int i = 0; i <= 2; i++)
        {
            string hopper = currentCombo.Substring(i, 3);

            foreach (Attack attack in playerUnit.attacks)
            {
                if (attack.comboSequence == hopper)
                {
                    successfulCombo = attack;
                    Debug.Log("Found combo: " + hopper);
                    break;
                }
            }

            if (successfulCombo != null)
                break;
        }

        if (successfulCombo != null)
        {
            playerAnim.Play("ComboAttack");
            dialogueText.text = playerUnit.unitName + " " + successfulCombo.flavorText + "!";
            yield return new WaitForSeconds(1f);

            float damageVariety = Random.Range(0.9f, 1.1f);
            float baseDamage = (successfulCombo.damage + playerUnit.specialAttack) * damageVariety;

            int finalDamage;
            string effectiveMarker;

            if (enemyUnit.weaknessType == successfulCombo.damageType)
            {
                finalDamage = Mathf.RoundToInt(baseDamage * 1.25f);
                Debug.Log("INCREASING DAMAGE RAAAAAAAAAH" + finalDamage);
                effectiveMarker = "very effective!";
            }
            else if (enemyUnit.resistantType == successfulCombo.damageType)
            {
                finalDamage = Mathf.RoundToInt(baseDamage * 0.75f);
                Debug.Log("REDUCING DAMAGE OH NOOOOOO" + finalDamage);
                effectiveMarker = "not very effective.";
            }
            else
            {
                finalDamage = Mathf.RoundToInt(baseDamage);
                Debug.Log("basic bitch ass attack" + finalDamage);
                effectiveMarker = "none";
            }

            bool isDead = enemyUnit.TakeDamage(finalDamage, enemyUnit.specialDefense);
            enemyHUD.SetHP(enemyUnit.currentHP);

            yield return new WaitForSeconds(1f);

            if (effectiveMarker != "none")
            {
                dialogueText.text = "It's " + effectiveMarker;
            }

            yield return new WaitForSeconds(1.5f);

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