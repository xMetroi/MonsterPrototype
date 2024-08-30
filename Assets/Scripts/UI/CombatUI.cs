using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [SerializeField] private GameObject panelCombat;

    [Header("Sprites Keys")]
    [SerializeField] private Sprite keyR;
    [SerializeField] private Sprite keyE;
    [SerializeField] private Sprite keyQ;

    [Header("Sprites Buttons")]
    [SerializeField] private Sprite buttonX;
    [SerializeField] private Sprite buttonY;
    [SerializeField] private Sprite buttonB;

    [Header("Player Data Combat")]
    [SerializeField] private Image playerSprite;
    [SerializeField] private Slider playerSliderHP;

    [Header("Player Attack 1")]
    [SerializeField] private Image playerAttack1Image;
    [SerializeField] private Image playerAttack1Cooldown;
    [SerializeField] private Image playerSkill1Image;

    [Header("Player Attack 2")]
    [SerializeField] private Image playerAttack2Image;
    [SerializeField] private Image playerAttack2Cooldown;
    [SerializeField] private Image playerSkill2Image;

    [Header("Player Attack 3")]
    [SerializeField] private Image playerAttack3Image;
    [SerializeField] private Image playerAttack3Cooldown;
    [SerializeField] private Image playerSkill3Image;

    [Header("Enemy Data Combat")]
    [SerializeField] private Image enemySprite;
    [SerializeField] private Slider enemySliderHP;

    [Header("Enemy Attack 1")]
    [SerializeField] private Image enemyAttack1Image;
    [SerializeField] private Image enemyAttack1Cooldown;

    [Header("Enemy Attack 2")]
    [SerializeField] private Image enemyAttack2Image;
    [SerializeField] private Image enemyAttack2Cooldown;

    [Header("Enemy Attack 3")]
    [SerializeField] private Image enemyAttack3Image;
    [SerializeField] private Image enemyAttack3Cooldown;


    private TrainerMovement trainerMovement;
    private PlayerReferences playerReferences;
    private PlayerCombat playerCombat;
    private AIReferences enemyReferences;
    private EnemyBrain enemyCombat;

    private void Start()
    {
        panelCombat.SetActive(false);

        trainerMovement = FindObjectOfType<TrainerMovement>();

        trainerMovement.DeviceChanged += OnDeviceChanged;

    }

    private void Update()
    {
        if (playerCombat != null)
        {
            if (playerReferences.currentMonster.monsterSprite != playerSprite)
                AddSpritesPlayer();

            if (playerSliderHP.value != playerCombat.GetHP())
                playerSliderHP.value = playerCombat.GetHP();
        }

        if (enemyCombat != null)
        {
            if (enemyReferences.currentMonster.monsterSprite != enemySprite)
                AddSpritesEnemy();

            if (enemySliderHP.value != enemyCombat.GetHP())
                enemySliderHP.value = enemyCombat.GetHP();
        }
    }


    public void AddDataUiFistTime(GameObject playerData, GameObject enemyData)
    {

        playerReferences = playerData.GetComponent<PlayerReferences>();
        playerCombat = playerData.GetComponent<PlayerCombat>();

        enemyReferences = enemyData.GetComponent<AIReferences>();
        enemyCombat = enemyData.GetComponent<EnemyBrain>();

        AddDataUi();

        SetActivePanelCombat(true);

    }

    private void OnDeviceChanged(TrainerMovement.PlayerDevice device)
    {
        switch (device)
        {
            case TrainerMovement.PlayerDevice.KeyboardMouse:
                playerSkill1Image.sprite = keyR;
                playerSkill2Image.sprite = keyE;
                playerSkill3Image.sprite = keyQ;
                break;
            case TrainerMovement.PlayerDevice.Controller:
                playerSkill1Image.sprite = buttonX;
                playerSkill2Image.sprite = buttonY;
                playerSkill3Image.sprite = buttonB;
                break;
        }
    }

    public void AddDataUi()
    {
        AddSpritesPlayer();
        playerSliderHP.maxValue = playerReferences.currentMonster.monsterHealth;
        playerSliderHP.value = playerReferences.currentMonster.monsterHealth;

        //Enemigo
        AddSpritesEnemy();
        enemySliderHP.maxValue = enemyReferences.currentMonster.monsterHealth;
        enemySliderHP.value = enemyReferences.currentMonster.monsterHealth;
    }

    public void AddSpritesPlayer()
    {
        playerSprite.transform.parent.gameObject.SetActive(playerSprite.sprite = playerReferences.currentMonster?.monsterSprite);
        playerAttack1Image.transform.parent.gameObject.SetActive(playerAttack1Image.sprite = playerReferences.currentMonster?.basickAttack1?.attackSprite);
        playerAttack2Image.transform.parent.gameObject.SetActive(playerAttack2Image.sprite = playerReferences.currentMonster?.basickAttack2?.attackSprite);
        playerAttack3Image.transform.parent.gameObject.SetActive(playerAttack3Image.sprite = playerReferences.currentMonster?.specialAttack?.attackSprite);
    }

    public void AddSpritesEnemy()
    {
        enemySprite.transform.parent.gameObject.SetActive(enemySprite.sprite = enemyReferences.currentMonster?.monsterSprite);
        enemyAttack1Image.transform.parent.gameObject.SetActive(enemyAttack1Image.sprite = enemyReferences.currentMonster?.basickAttack1?.attackSprite);
        enemyAttack2Image.transform.parent.gameObject.SetActive(enemyAttack2Image.sprite = enemyReferences.currentMonster?.basickAttack2?.attackSprite);
        enemyAttack3Image.transform.parent.gameObject.SetActive(enemyAttack3Image.sprite = enemyReferences.currentMonster?.specialAttack?.attackSprite);
    }

    public void SetActivePanelCombat(bool condition)
    {
        panelCombat.SetActive(condition);
    }

    #region CooldownPlayer

    public void StartCooldownPlayerAttack1(float duration)
    {
        StartCoroutine(ChangeFillAmountCooldown(playerAttack1Cooldown, duration));
    }

    public void StartCooldownPlayerAttack2(float duration)
    {
        StartCoroutine(ChangeFillAmountCooldown(playerAttack2Cooldown, duration));
    }

    public void StartCooldownPlayerAttack3(float duration)
    {
        StartCoroutine(ChangeFillAmountCooldown(playerAttack3Cooldown, duration));
    }

    #endregion

    #region CooldownEnemy

    public void StartCooldownEnemyAttack1(float duration)
    {
        StartCoroutine(ChangeFillAmountCooldown(enemyAttack1Cooldown, duration));
    }

    public void StartCooldownEnemyAttack2(float duration)
    {
        StartCoroutine(ChangeFillAmountCooldown(enemyAttack2Cooldown, duration));
    }

    public void StartCooldownEnemyAttack3(float duration)
    {
        StartCoroutine(ChangeFillAmountCooldown(enemyAttack3Cooldown, duration));
    }

    #endregion

    private IEnumerator ChangeFillAmountCooldown(Image fillImage, float targetFillAmount)
    {
        if (fillImage == null)
        {
            yield break;
        }

        float startFillAmount = fillImage.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < targetFillAmount)
        {
            if (fillImage == null)
            {
                yield break;
            }

            elapsedTime += Time.deltaTime;
            fillImage.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, elapsedTime / targetFillAmount);
            yield return null;
        }

        if (fillImage != null)
        {
            fillImage.fillAmount -= targetFillAmount;
        }
    }
}
