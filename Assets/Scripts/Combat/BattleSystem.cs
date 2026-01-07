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

    [Header("Audio")]
    public AudioSource typeAudioSource;
    public AudioClip typeSound;

    private Animator playerAnim;
    private Animator enemyAnim;
    private int turnCount = 0;

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

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return StartCoroutine(TypeCombatText(enemyUnit.combatIntro));

        state = BattleState.PLAYERTURN;
        yield return new WaitForSeconds(1f);

        PlayerTurn();
    }

    IEnumerator CheckForDialogue(BattleState currentState)
    {
        if (DialogueManager.Instance == null)
        {
            yield break;
        }

        yield return CheckUnitDialogue(playerUnit, currentState);
        yield return CheckUnitDialogue(enemyUnit, currentState);
    }

    IEnumerator CheckUnitDialogue(Unit unit, BattleState currentState)
    {
        if (unit.combatDialogue == null)
            yield break;

        foreach (var trigger in unit.combatDialogue)
        {
            bool turnMatches = (trigger.turnNumber == -1) || (trigger.turnNumber == turnCount);

            if (!trigger.hasTriggered && turnMatches && trigger.triggerState == currentState)
            {
                trigger.hasTriggered = true;

                bool dialogueFinished = false;
                DialogueManager.Instance.ShowDialogue(trigger.dialogue, () => dialogueFinished = true);

                yield return new WaitUntil(() => dialogueFinished);
            }
        }
    }

    IEnumerator PlayerAttack()
    {
        bool isDead = enemyUnit.TakeDamage(1, 0);
        enemyHUD.SetHP(enemyUnit.currentHP);
        yield return StartCoroutine(TypeCombatText("You strike " + enemyUnit.unitName + "!"));

        yield return new WaitForSeconds(1f);

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

    IEnumerator EnemyTurn()
    {
        comboDial.SetActive(false);

        yield return StartCoroutine(CheckForDialogue(BattleState.ENEMYTURN));

        Attack chosenAttack = enemyUnit.attacks[Random.Range(0, enemyUnit.attacks.Count)];

        yield return StartCoroutine(TypeCombatText(enemyUnit.unitName + " " + chosenAttack.flavorText + "!"));

        if (!string.IsNullOrEmpty(chosenAttack.animationName))
        {
            enemyAnim.Play(chosenAttack.animationName);
        }

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
        yield return StartCoroutine(CheckForDialogue(state));

        if (state == BattleState.WON)
        {
            yield return StartCoroutine(TypeCombatText("You won the battle!"));
        }
        else if (state == BattleState.LOST)
        {
            yield return StartCoroutine(TypeCombatText("You were defeated."));
        }

        yield return new WaitForSeconds(2f);

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
        turnCount++;
        StartCoroutine(PlayerTurnDialogue());
    }

    IEnumerator PlayerTurnDialogue()
    {
        yield return StartCoroutine(CheckForDialogue(BattleState.PLAYERTURN));
        yield return StartCoroutine(TypeCombatText("Choose an action:"));
        UpdateButtons(true);
    }

    IEnumerator PlayerHeal()
    {
        playerUnit.Heal(50);
        playerHUD.SetHP(playerUnit.currentHP);
        yield return StartCoroutine(TypeCombatText("You take a bite from an energy bar"));

        yield return new WaitForSeconds(1f);

        state = BattleState.ENEMYTURN;
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

        StopAllCoroutines();
        dialogueText.text = "";

        comboDial.SetActive(true);
        currentCombo = "";
        state = BattleState.PLAYERCOMBO;
        StartCoroutine(CheckForDialogue(BattleState.PLAYERCOMBO));
    }

    IEnumerator TypeCombatText(string message, float speed = 0.02f)
    {
        dialogueText.text = "";
        foreach (char c in message.ToCharArray())
        {
            dialogueText.text += c;

            if (typeAudioSource != null && typeSound != null)
            {
                typeAudioSource.PlayOneShot(typeSound);
            }

            yield return new WaitForSeconds(speed);
        }
    }

    public IEnumerator ComboExecute()
    {
        yield return new WaitForSeconds(1.5f);
        Attack successfulCombo = null;

        for (int i = 0; i <= 2; i++)
        {
            string comboAttempt = currentCombo.Substring(i, 3);

            foreach (Attack attack in playerUnit.attacks)
            {
                if (attack.comboSequence == comboAttempt)
                {
                    successfulCombo = attack;
                    break;
                }
            }

            if (successfulCombo != null)
                break;
        }

        if (successfulCombo != null)
        {
            if (!string.IsNullOrEmpty(successfulCombo.animationName))
            {
                playerAnim.Play(successfulCombo.animationName);
            }

            yield return StartCoroutine(TypeCombatText(playerUnit.unitName + " " + successfulCombo.flavorText + "!"));
            yield return new WaitForSeconds(1f);

            float damageVariety = Random.Range(0.9f, 1.1f);
            float baseDamage = (successfulCombo.damage + playerUnit.specialAttack) * damageVariety;

            int finalDamage;
            string effectiveMarker = "none";

            if (enemyUnit.weaknessType == successfulCombo.damageType)
            {
                finalDamage = Mathf.RoundToInt(baseDamage * 1.25f);
                effectiveMarker = "very effective!";
            }
            else if (enemyUnit.resistantType == successfulCombo.damageType)
            {
                finalDamage = Mathf.RoundToInt(baseDamage * 0.75f);
                effectiveMarker = "not very effective.";
            }
            else
            {
                finalDamage = Mathf.RoundToInt(baseDamage);
            }

            bool isDead = enemyUnit.TakeDamage(finalDamage, enemyUnit.specialDefense);
            enemyHUD.SetHP(enemyUnit.currentHP);

            yield return new WaitForSeconds(1f);

            if (effectiveMarker != "none")
            {
                yield return StartCoroutine(TypeCombatText("It's " + effectiveMarker));
                yield return new WaitForSeconds(1f);
            }

            

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