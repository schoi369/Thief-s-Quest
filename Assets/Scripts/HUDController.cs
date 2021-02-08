using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController instance;

    [Header("Skill Related")]
    public Text skillText;
    public Image leftSkill, selectedSkill, rightSkill;
    public Sprite sleepDagger, sleepDaggerSelected, doppelganger, doppelgangerSelected, disguise, disguiseSelected;

    [Header("MP Related")]
    public GameObject fakeUIHolder;
    public Image MPBar;
    public Text MPText;

    [Header("Coin Related")]
    public Text coinCountText;

    [Header("Potion Related")]
    public Text potionCountText;

    [Header("Height Meter Related")]
    public Image heightMeterBar;
    float playerHeight;

    [Header("Others")]
    public Image fadeScreen;
    public float fadeSpeed;
    private bool shouldFadeToBlack, shouldFadeFromBlack;

    void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateSkillDisplay();
        UpdateCoinCountDisplay();
        UpdatePotionCountDisplay();
        UpdateMPDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        heightMeterBar.fillAmount = (PlayerMovementController.instance.transform.position.y + 6.5f) / 132f;

        if (shouldFadeToBlack) {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 1f, fadeSpeed * Time.deltaTime));
            if (fadeScreen.color.a == 1f) {
                shouldFadeToBlack = false;
            }
        }

        if (shouldFadeFromBlack) {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 0f, fadeSpeed * Time.deltaTime));
            if (fadeScreen.color.a == 0f) {
                shouldFadeFromBlack = false;
            }
        }
    }

    public void UpdateSkillDisplay() {
        switch (PlayerActionController.instance.currentSkillNum) {
            case 0:
                leftSkill.sprite = disguise;
                selectedSkill.sprite = sleepDaggerSelected;
                rightSkill.sprite = doppelganger;
                skillText.text = "Sleep Dagger\nMP " + PlayerActionController.instance.sleepDaggerMPCost;
                break;
            case 1:
                leftSkill.sprite = sleepDagger;
                selectedSkill.sprite = doppelgangerSelected;
                rightSkill.sprite = disguise;
                skillText.text = "Doppelganger\nMP " + PlayerActionController.instance.doppelgangerMPCost;
                break;
            case 2:
                leftSkill.sprite = doppelganger;
                selectedSkill.sprite = disguiseSelected;
                rightSkill.sprite = sleepDagger;
                skillText.text = "Disguise\nMP " + PlayerActionController.instance.disguiseMPCost;
                break;
        }
    }

    public void UpdateMPDisplay() {
        MPBar.fillAmount = (float) PlayerActionController.instance.currentMP / (float) PlayerActionController.instance.maxMP;
        MPText.text = PlayerActionController.instance.currentMP.ToString();
    }

    public void UpdateCoinCountDisplay() {
        coinCountText.text = PlayerActionController.instance.coinCount.ToString();
    }

    public void UpdatePotionCountDisplay() {
        potionCountText.text = PlayerActionController.instance.potionCount.ToString();
    }

    public void FadeToBlack() {
        shouldFadeToBlack = true;
        shouldFadeFromBlack = false;
    }

    public void FadeFromBlack() {
        shouldFadeFromBlack = true;
        shouldFadeToBlack = false;
    }
}
