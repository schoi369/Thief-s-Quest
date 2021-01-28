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

    void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateSkillDisplay();
        UpdateCoinCountDisplay();
        UpdateMPDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
