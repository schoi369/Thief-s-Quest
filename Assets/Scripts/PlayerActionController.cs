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

    public Transform hidingPlace;
    public Transform movableDoor;

    public bool isHiding;
    public bool isScouting;

    public int currentSkillNum = 0;

    public int sleepDaggerMPCost, doppelgangerMPCost, disguiseMPCost; // Set in inspector

    public Transform actionPoint;
    public bool isStealing, isUsingSleepDagger;
    public bool canMakeAction = true;
    public float stealRange;
    public float sleepDaggerRange;
    public LayerMask guardLayer;
    float sleepDaggerRate = 2f;
    float sleepDaggerLength = 7f;
    public GameObject doppelganger;
    float nextSleepDaggerTime;
    float nextDoppelgangerTime;
    float nextDisguiseTime;
    public float disguiseLength = 3f;
    public bool isDisguising;

    [Header("Item related values")]
    public int maxMPIncrease;
    public float sleepDaggerTimeIncrease;
    public int doppelgangerCostDecrease;
    public float disguiseTimeIncrease;
    public int itemPrice;

    void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentMP = maxMP;
    }

    // Update is called once per frame
    void Update()
    {
        ManageSkillSelect();

        if (Input.GetKeyDown(KeyCode.J) && canMakeAction) {
            Steal();
        }

        if (Input.GetKeyDown(KeyCode.I) && canMakeAction && PlayerMovementController.instance.isGrounded) {
            if (currentSkillNum == 0) {
                if (Time.time >= nextSleepDaggerTime) {
                    UseSleepDagger();
                    nextSleepDaggerTime = Time.time + 1f / sleepDaggerRate;
                }
            }
            if (currentSkillNum == 1) {
                if (Time.time >= nextDoppelgangerTime) {
                    UseDoppelganger();
                    nextDoppelgangerTime = Time.time + 1f;
                }
            }
            if (currentSkillNum == 2) {
                if (Time.time >= nextDisguiseTime) {
                    UseDisguise();
                    nextDisguiseTime = Time.time + disguiseLength;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && PlayerMovementController.instance.isGrounded) {
            isScouting = !isScouting;
            Debug.Log("isScouting: " + isScouting);
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            HideUnhide();
            OpenDoor();
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
        }
        
        // Wait
        yield return new WaitForSeconds(0.267f);

        // Manage bools
        isUsingSleepDagger = false;
        canMakeAction = true;
    }

    void UseDoppelganger() {
        StartCoroutine(DoppelgangerCo());
    }

    IEnumerator DoppelgangerCo() {
        // Manage bools
        canMakeAction = false;

        // Instantiate Doppelganger
        GameObject doppelgangerPrefab = Instantiate(doppelganger, actionPoint.position, transform.rotation);
        if (!PlayerMovementController.instance.facingRight) {
            doppelgangerPrefab.transform.localScale = new Vector3(-1, 1, 1);
        }
        yield return new WaitForSeconds(.3f);

        // Manage bools
        canMakeAction = true;
    }

    void UseDisguise() {
        StartCoroutine(DisguiseCo());
    }

    IEnumerator DisguiseCo() {
        isDisguising = true;
        canMakeAction = false;
        animator.SetBool("isDisguising", true);

        yield return new WaitForSeconds(disguiseLength);

        isDisguising = false;
        canMakeAction = true;
        animator.SetBool("isDisguising", false);
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
        }
    }

    public void BuyHypnosCharm() {
        if (coinCount > itemPrice) {
            sleepDaggerLength += sleepDaggerTimeIncrease;
            coinCount -= itemPrice;
        }
    }

    public void BuyShadowEarring() {
        if (coinCount > itemPrice) {
            doppelgangerMPCost -= doppelgangerCostDecrease;
            coinCount -= itemPrice;
        }
    }

    public void BuyMaskofArsene() {
        if (coinCount > itemPrice) {
            disguiseLength += disguiseTimeIncrease;
            coinCount -= itemPrice;
        }
    }
}
