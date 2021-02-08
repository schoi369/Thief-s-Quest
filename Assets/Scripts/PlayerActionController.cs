using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
    public static PlayerActionController instance;

    Rigidbody2D rb;
    Animator animator;

    public int maxMP;
    public int currentMP;
    public int coinCount;
    public int potionCount;

    public Transform hidingPlace;
    public Transform movableDoor;
    public Transform shopIcon;

    public bool isHiding;
    public bool isScouting;

    public int currentSkillNum = 0;

    public int originalSleepDaggerMPCost, originalDoppelgangerMPCost, originalDisguiseMPCost;
    public int sleepDaggerMPCost, doppelgangerMPCost, disguiseMPCost;

    public Transform actionPoint;
    public bool isStealing, isUsingSleepDagger;
    public bool canMakeAction = true;
    public float stealRange;
    public float sleepDaggerRange;
    public LayerMask guardLayer;
    float sleepDaggerRate = 2f;
    public float sleepDaggerLength = 7f;
    public GameObject doppelganger;
    Vector2 doppelgangerPosition;
    float nextSleepDaggerTime;
    float nextDoppelgangerTime;
    public float doppelgangerDistance = 0;
    float nextDisguiseTime;
    public float disguiseLength = 3f;
    public bool isDisguising;

    [Header("Item related values")]
    public int maxMPIncrease;
    public float sleepDaggerTimeIncrease;
    public float doppelgangerDistanceIncrease;
    public float disguiseTimeIncrease;
    public int itemPrice;

    [Header("Others")]
    public bool hasTutorialKey;
    public bool hasOrbRoomKey;
    int MPRegeneratedByPotion = 5;
    public bool hasOrb;

    void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentMP = maxMP;
        HUDController.instance.UpdateMPDisplay();

        sleepDaggerMPCost = originalSleepDaggerMPCost;
        doppelgangerMPCost = originalDoppelgangerMPCost;
        disguiseMPCost = originalDisguiseMPCost;
        HUDController.instance.UpdateSkillDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.instance.isPaused && !DialogueManager.instance.isTalking) {
            ManageSkillSelect();

            if (Input.GetKeyDown(KeyCode.E)) {
                UsePotion();
            }

            if (Input.GetKeyDown(KeyCode.J) && canMakeAction) {
                Steal();
            }

            if (Input.GetKeyDown(KeyCode.I) && canMakeAction && PlayerMovementController.instance.isGrounded
                && !isHiding && !isDisguising) {
                if (currentSkillNum == 0) {
                    if (currentMP >= sleepDaggerMPCost && Time.time >= nextSleepDaggerTime) {
                        UseSleepDagger();
                        nextSleepDaggerTime = Time.time + 1f / sleepDaggerRate;
                    }
                }
                if (currentSkillNum == 1) {
                    if (currentMP >= doppelgangerMPCost && Time.time >= nextDoppelgangerTime) {
                        UseDoppelganger();
                        nextDoppelgangerTime = Time.time + 1f;
                    }
                }
                if (currentSkillNum == 2) {
                    if (currentMP >= disguiseMPCost && Time.time >= nextDisguiseTime) {
                        UseDisguise();
                        nextDisguiseTime = Time.time + disguiseLength;
                    }
                }
            }

            if (isDisguising) {
                if (Input.GetKeyUp(KeyCode.I) && currentSkillNum == 2) {
                    StopCoroutine(DisguiseCo());
                    isDisguising = false;
                    canMakeAction = true;
                    animator.SetBool("isDisguising", false);

                    sleepDaggerMPCost = originalSleepDaggerMPCost;
                    doppelgangerMPCost = originalDoppelgangerMPCost;
                    disguiseMPCost = originalDisguiseMPCost * 2;
                    HUDController.instance.UpdateSkillDisplay();
                }
            }



            if (Input.GetKeyDown(KeyCode.LeftShift) && (PlayerMovementController.instance.isGrounded || PlayerMovementController.instance.isGrabbingWall)) {
                isScouting = !isScouting;
            }

            if (Input.GetKeyDown(KeyCode.L)) {
                if (hidingPlace == null && movableDoor == null) {
                    Shop();
                } else if (movableDoor == null && shopIcon == null) {
                    HideUnhide();
                } else if (shopIcon == null && hidingPlace == null) {
                    OpenDoor();
                } else if (hidingPlace == null) {
                    if (Vector2.Distance(transform.position, movableDoor.position) <= Vector2.Distance(transform.position, shopIcon.position)) {
                        OpenDoor();
                    } else {
                        Shop();
                    }
                } else if (movableDoor == null) {
                    if (Vector2.Distance(transform.position, shopIcon.position) <= Vector2.Distance(transform.position, hidingPlace.position)) {
                        Shop();
                    } else {
                        HideUnhide();
                    }
                } else if (shopIcon == null) {
                    if (Vector2.Distance(transform.position, movableDoor.position) <= Vector2.Distance(transform.position, hidingPlace.position)) {
                        OpenDoor();
                    } else {
                        HideUnhide();
                    }
                } else {
                    if (Vector2.Distance(transform.position, hidingPlace.position) < Mathf.Min(Vector2.Distance(transform.position, movableDoor.position), Vector2.Distance(transform.position, shopIcon.position))) {
                        HideUnhide();
                    } else if (Vector2.Distance(transform.position, movableDoor.position) < Mathf.Min(Vector2.Distance(transform.position, hidingPlace.position), Vector2.Distance(transform.position, shopIcon.position))) {
                        OpenDoor();
                    } else if (Vector2.Distance(transform.position, shopIcon.position) < Mathf.Min(Vector2.Distance(transform.position, hidingPlace.position), Vector2.Distance(transform.position, movableDoor.position))) {
                        Shop();
                    }
                }
            }
        }
    }

    void HideUnhide() {
        if (hidingPlace == null || Vector2.Distance(transform.position, hidingPlace.position) >= 1.1f) {
            return;
        }

        if (isHiding) {
            isHiding = false;
        } else {
            transform.position = new Vector2(hidingPlace.position.x, hidingPlace.position.y);
            isHiding = true;
        }
        PlayerMovementController.instance.isGrabbingWall = false;

        // Set appropriate animation
        switch (hidingPlace.GetComponent<HidingPlace>().type) {
            case HidingPlace.HidingPlaceType.Statue:
                animator.SetBool("isHiding_Statue", isHiding);
                break;
            case HidingPlace.HidingPlaceType.Door:
                animator.SetBool("isHiding_Door", isHiding);
                break;
            case HidingPlace.HidingPlaceType.Barrel:
                animator.SetBool("isHiding_Barrel", isHiding);
                break;
        }
    }

    void OpenDoor() {
        if (movableDoor == null || Vector2.Distance(transform.position, movableDoor.position) >= 1.8f) {
            return;
        }
        if (!movableDoor.GetComponent<DoorController>().isOpen) {
            // Open the door
            movableDoor.GetComponent<DoorController>().Open();
            movableDoor.GetComponent<DoorController>().KeyCanvas.SetActive(false);
        }
    }

    void Shop() {
        if (shopIcon == null || Vector2.Distance(transform.position, shopIcon.position) >= 1.5f) {
            return;
        }
        ShopManager.instance.OpenCloseShopScreen();
    }

    void Steal() {
        StartCoroutine(StealCo());
    }

    IEnumerator StealCo() {
        // Manage bools
        isStealing = true;
        canMakeAction = false;

        Collider2D hitEnemy = Physics2D.OverlapCircle(actionPoint.position, stealRange, guardLayer);
        if (hitEnemy != null) {
            hitEnemy.GetComponent<Guard>().DropItem();
            // Wait
            yield return new WaitForSeconds(.35f);
        } else {
            yield return null;
        }

        // Manage bools
        isStealing = false;
        canMakeAction = true;
    }

    void ManageSkillSelect() {
        if (Input.GetKeyDown(KeyCode.U)) {
            if (currentSkillNum == 0) {
                currentSkillNum = 2;
            } else {
                currentSkillNum--;
            }
            HUDController.instance.UpdateSkillDisplay();
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            if (currentSkillNum == 2) {
                currentSkillNum = 0;
            } else {
                currentSkillNum++;
            }
            HUDController.instance.UpdateSkillDisplay();
        }
    }

    void UseSleepDagger() {
        StartCoroutine(SleepDaggerCo());
    }

    IEnumerator SleepDaggerCo() {
        // Manage bools
        isUsingSleepDagger = true;
        canMakeAction = false;

        // Play attack anim
        actionPoint.GetComponent<Animator>().SetTrigger("sleepDagger");

        // Detect enemies in range
        Collider2D hitEnemy = Physics2D.OverlapCircle(actionPoint.position, sleepDaggerRange, guardLayer);

        // Make them sleep
        if (hitEnemy != null) {
            currentMP -= sleepDaggerMPCost;
            HUDController.instance.UpdateMPDisplay();
            hitEnemy.transform.GetComponent<Guard>().FallAsleep(sleepDaggerLength);

            sleepDaggerMPCost = originalSleepDaggerMPCost * 2;
            doppelgangerMPCost = originalDoppelgangerMPCost;
            disguiseMPCost = originalDisguiseMPCost;
            HUDController.instance.UpdateSkillDisplay();
        }
        
        // Wait
        yield return new WaitForSeconds(0.267f);

        // Manage bools
        isUsingSleepDagger = false;
        canMakeAction = true;

    }

    void UseDoppelganger() {
        if (PlayerMovementController.instance.facingRight) {
            doppelgangerPosition = new Vector2(actionPoint.position.x + doppelgangerDistance, actionPoint.position.y);
        } else {
            doppelgangerPosition = new Vector2(actionPoint.position.x - doppelgangerDistance, actionPoint.position.y);
        }
        StartCoroutine(DoppelgangerCo());
    }

    IEnumerator DoppelgangerCo() {
        // Manage bools
        canMakeAction = false;

        currentMP -= doppelgangerMPCost;
        HUDController.instance.UpdateMPDisplay();

        // Instantiate Doppelganger
        
        GameObject doppelgangerPrefab = Instantiate(doppelganger, doppelgangerPosition, transform.rotation);
        if (!PlayerMovementController.instance.facingRight) {
            doppelgangerPrefab.transform.localScale = new Vector3(-1, 1, 1);
        }
        yield return new WaitForSeconds(.3f);

        // Manage bools
        canMakeAction = true;

        sleepDaggerMPCost = originalSleepDaggerMPCost;
        doppelgangerMPCost = originalDoppelgangerMPCost * 2;
        disguiseMPCost = originalDisguiseMPCost;
        HUDController.instance.UpdateSkillDisplay();

    }

    void UseDisguise() {
        StartCoroutine(DisguiseCo());
    }

    IEnumerator DisguiseCo() {
        isDisguising = true;
        canMakeAction = false;
        animator.SetBool("isDisguising", true);

        currentMP -= disguiseMPCost;
        HUDController.instance.UpdateMPDisplay();

        yield return new WaitForSeconds(disguiseLength);

        isDisguising = false;
        canMakeAction = true;
        animator.SetBool("isDisguising", false);

        sleepDaggerMPCost = originalSleepDaggerMPCost;
        doppelgangerMPCost = originalDoppelgangerMPCost;
        disguiseMPCost = originalDisguiseMPCost * 2;
        HUDController.instance.UpdateSkillDisplay();
    }

    void OnDrawGizmosSelected() {
        if (actionPoint == null) {
            return;
        }
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(actionPoint.position, stealRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(actionPoint.position, sleepDaggerRange);
    }

    public void BuyRingOfMana() {
        if (coinCount > itemPrice) {
            maxMP += maxMPIncrease;
            coinCount -= itemPrice;
            HUDController.instance.UpdateMPDisplay();
            ShopManager.instance.shopText.text = "Got it!";
        } else {
            ShopManager.instance.shopText.text = "Not enough coins!";
        }
    }

    public void BuyHypnosCharm() {
        if (coinCount > itemPrice) {
            sleepDaggerLength += sleepDaggerTimeIncrease;
            coinCount -= itemPrice;
            ShopManager.instance.shopText.text = "Got it!";
        } else {
            ShopManager.instance.shopText.text = "Not enough coins!";
        }
    }

    public void BuyShadowEarring() {
        if (coinCount > itemPrice) {
            doppelgangerDistance += doppelgangerDistanceIncrease;
            coinCount -= itemPrice;
            ShopManager.instance.shopText.text = "Got it!";
        } else {
            ShopManager.instance.shopText.text = "Not enough coins!";
        }
    }

    public void BuyMaskofArsene() {
        if (coinCount > itemPrice) {
            disguiseLength += disguiseTimeIncrease;
            coinCount -= itemPrice;
            ShopManager.instance.shopText.text = "Got it!";
        } else {
            ShopManager.instance.shopText.text = "Not enough coins!";
        }
    }

    void UsePotion() {
        Debug.Log("Potion used");
        if (potionCount > 0) {
            potionCount--;
            if (currentMP + MPRegeneratedByPotion >= maxMP) {
                currentMP = maxMP;
            } else {
                currentMP += MPRegeneratedByPotion;
            }
            HUDController.instance.UpdatePotionCountDisplay();
            HUDController.instance.UpdateMPDisplay();
        }
    }
}
