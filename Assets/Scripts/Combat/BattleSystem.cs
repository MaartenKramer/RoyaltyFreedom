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


    [Header("Battle Stations")]
    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    Unit playerUnit;
    Unit enemyUnit;

    [Header("HUD")]
    public TextMeshProUGUI dialogueText;
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    [Header("Battle State")]
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
    public AudioSource comboFound;

    private Animator playerAnim;
    private Animator enemyAnim;

    [Header("Animation")]
    public Animator effectiveAnim;
    public float moveDistance = 2f;
    public float moveSpeed = 5f;

    private Vector3 playerOriginalPos;
    private Vector3 enemyOriginalPos;

    public GameObject effectiveHider;
    public GameObject ineffectiveHider;

    private bool effectiveDiscovered = false;
    private bool ineffectiveDiscovered = false;

    private int turnCount = 0;

    private bool isTyping = false;



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
                enemyAnim.Play("Hurt");
                keyPressed = true;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                currentCombo += "A";
                playerAnim.Play("DialA");
                enemyAnim.Play("Hurt");
                keyPressed = true;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                currentCombo += "S";
                playerAnim.Play("DialS");
                enemyAnim.Play("Hurt");
                keyPressed = true;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                currentCombo += "D";
                playerAnim.Play("DialD");
                enemyAnim.Play("Hurt");
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

        playerOriginalPos = playerGO.transform.position;
        enemyOriginalPos = enemyGO.transform.position;

        StartCoroutine(TypeCombatText(""));

        yield return new WaitForSeconds(0.1f);

        StartCoroutine(TypeCombatText(enemyUnit.combatIntro));

        state = BattleState.PLAYERTURN;
        yield return WaitForTyping();

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
        StartCoroutine(TypeCombatText("You strike " + enemyUnit.unitName + "!"));

        yield return WaitForTyping();

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

        StartCoroutine(TypeCombatText(enemyUnit.unitName + " " + chosenAttack.flavorText + "!"));

        yield return StartCoroutine(MoveUnit(enemyUnit.transform, enemyOriginalPos + Vector3.left * moveDistance));

        playerAnim.Play("Hurt");

        if (!string.IsNullOrEmpty(chosenAttack.animationName))
        {
            enemyAnim.Play(chosenAttack.animationName);
        }


        float damageVariety = Random.Range(0.9f, 1.1f);
        int finalDamage = Mathf.RoundToInt((chosenAttack.damage + enemyUnit.attack) * damageVariety);

        bool isDead = playerUnit.TakeDamage(finalDamage, playerUnit.defense);

        enemyHUD.SetHP(enemyUnit.currentHP);
        playerHUD.SetHP(playerUnit.currentHP);

        yield return WaitForTyping();

        yield return StartCoroutine(MoveUnit(enemyUnit.transform, enemyOriginalPos));

        if (isDead)
        {
            state = BattleState.LOST;
            StartCoroutine(EndBattle());
        }
        else
        {
            yield return StartCoroutine(CheckForDialogue(BattleState.ENEMYTURN));
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

        yield return WaitForTyping();
        yield return new WaitForSeconds(1f);

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
            SceneManager.LoadScene("PostPrinterCutscene");
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
        playerAnim.Play("Heal");

        playerUnit.Heal(50);
        playerHUD.SetHP(playerUnit.currentHP);

        StartCoroutine(TypeCombatText("You take a bite from an energy bar"));

        yield return WaitForTyping();

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

        StartCoroutine(MoveUnit(playerUnit.transform, playerOriginalPos + Vector3.right * moveDistance));

        StartCoroutine(CheckForDialogue(BattleState.PLAYERCOMBO));
    }

    public IEnumerator ComboExecute()
    {
        yield return new WaitForSeconds(0.5f);
        Attack successfulCombo = null;
        int successfulIndex = -1;

        for (int i = 0; i <= 2; i++)
        {
            string comboAttempt = currentCombo.Substring(i, 3);

            foreach (Attack attack in playerUnit.attacks)
            {
                if (attack.comboSequence == comboAttempt)
                {
                    successfulCombo = attack;
                    successfulIndex = i;
                    break;
                }
            }

            if (successfulCombo != null)
                break;
        }

        if (successfulCombo != null && successfulIndex != -1)
        {
            string preCombo = currentCombo.Substring(0, successfulIndex);
            string combo = currentCombo.Substring(successfulIndex, 3);
            string postCombo = currentCombo.Substring(successfulIndex + 3);

            dialogueText.text = preCombo + "<color=#00FF00>" + combo + "</color>" + postCombo;
            comboFound.Play(0);

            yield return new WaitForSeconds(0.5f);
        }

        if (successfulCombo != null)
        {
            if (!string.IsNullOrEmpty(successfulCombo.animationName))
            {
                playerAnim.Play(successfulCombo.animationName);
            }

            StartCoroutine(TypeCombatText(playerUnit.unitName + " " + successfulCombo.flavorText + "!"));


            enemyAnim.Play("ComboHurt");

            float damageVariety = Random.Range(0.9f, 1.1f);
            float baseDamage = (successfulCombo.damage + playerUnit.specialAttack) * damageVariety;

            int finalDamage;
            string effectiveMarker = "none";

            if (enemyUnit.weaknessType == successfulCombo.damageType)
            {
                if (!effectiveDiscovered)
                {
                    yield return new WaitForSeconds(0.5f);
                    effectiveAnim.Play("EffectiveDiscovered");
                    yield return new WaitForSeconds(0.5f);
                    effectiveHider.SetActive(false);
                    effectiveDiscovered = true;
                }
                else
                {
                    effectiveAnim.Play("Effective");
                }

                finalDamage = Mathf.RoundToInt(baseDamage * 1.25f);
                effectiveMarker = "very effective!";
            }
            else if (enemyUnit.resistantType == successfulCombo.damageType)
            {
                if (!ineffectiveDiscovered)
                {
                    yield return new WaitForSeconds(0.5f);
                    effectiveAnim.Play("IneffectiveDiscovered");
                    yield return new WaitForSeconds(0.5f);
                    ineffectiveHider.SetActive(false);
                    ineffectiveDiscovered = true;
                }
                else
                {
                    effectiveAnim.Play("Ineffective");
                }

                finalDamage = Mathf.RoundToInt(baseDamage * 0.75f);
                effectiveMarker = "not very effective.";
            }
            else
            {
                finalDamage = Mathf.RoundToInt(baseDamage);
            }

            bool isDead = enemyUnit.TakeDamage(finalDamage, enemyUnit.specialDefense);
            enemyHUD.SetHP(enemyUnit.currentHP);

            yield return WaitForTyping();

            yield return StartCoroutine(MoveUnit(playerUnit.transform, playerOriginalPos));

            if (effectiveMarker != "none")
            {
                yield return StartCoroutine(TypeCombatText("It's " + effectiveMarker));
            }

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
        else
        {
            yield return StartCoroutine(MoveUnit(playerUnit.transform, playerOriginalPos));

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

    IEnumerator MoveUnit(Transform unit, Vector3 targetPos)
    {
        while (Vector3.Distance(unit.position, targetPos) > 0.01f)
        {
            unit.position = Vector3.Lerp(unit.position, targetPos, Time.deltaTime * moveSpeed);
            yield return null;
        }
        unit.position = targetPos;
    }
    IEnumerator TypeCombatText(string message, float speed = 0.02f)
    {
        isTyping = true;
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
        isTyping = false;
    }

    IEnumerator WaitForTyping()
    {
        yield return new WaitUntil(() => !isTyping);
        yield return new WaitForSeconds(1f);
    }

    void UpdateButtons(bool isInteractable)
    {
        attackButton.interactable = isInteractable;
        healButton.interactable = isInteractable;
        comboButton.interactable = isInteractable;
    }
}