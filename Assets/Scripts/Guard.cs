using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guard : MonoBehaviour
{
    Rigidbody2D rb;
    public float moveSpeed;
    public bool facingRight;

    [Header("Detection Related")]
    public bool playerInVision;
    Vector3 lookDirection;
    float viewDistance = 5f; // Set in inspector
    float detectMeasure;
    float detectMeasureIncrease = 5f;
    float detectMeasureDecrease = .5f;
    float PatrolMax = 1f;
    float SuspiciousMax = 1f;
    public LayerMask playerAndObstacleLayerMask;
    bool inLine1, inLine2;

    public enum State {
        Patrol, Suspicious, Alert, Sleeping
    }
    public enum PatrolState {
        Standing, Waiting, MovingToNextPoint, Returning
    }
    public enum SuspiciousState {
        MovingToSuspiciousPosition, Waiting, ReturningToPoint
    }
    [Header("State Related")]
    public State state;
    public PatrolState patrolState;
    public SuspiciousState suspiciousState;
    public bool isStandingGuard;

    public Transform leftPoint, rightPoint, originPoint;
    bool facingRightAtStart;
    bool movingRight;
    float patrolWaitTime;
    public float startPatrolWaitTime; // Set in inspector
    float suspiciousWaitTime;
    public float startSuspiciousWaitTime; // Set in inspector
    float sleepTime;
    float startSleepTime;

    Vector2 suspiciousPosition;

    bool isDistractedByDoppelganger;

    [Header("UI Related")]
    public Image detectMeter;
    public Transform statusIcon;

    [Header("Others")]
    public GameObject stealEffect;
    public int itemCount;
    public GameObject coin;
    public GameObject potion;

    public bool hasTutorialKey;
    public GameObject tutorialKey;
    public bool hasOrbRoomKey;
    public GameObject orbRoomKey;
    public bool hasNote2;
    public GameObject note2;

    // Start is called before the first frame update
    void Start()
    {
        Physics2D.queriesStartInColliders = false;
        rb = GetComponent<Rigidbody2D>();

        SetTimes();

        facingRightAtStart = facingRight;

        // Release move points
        leftPoint.parent = null;
        rightPoint.parent = null;
        originPoint.parent = null;

        // Set number of itemCounts randomly (how many things a player can steal from this enemy)
        int randomNum = Random.Range(1, 100);
        if (randomNum <= 10) {
            itemCount = 1;
        } else if (randomNum <= 50) {
            itemCount = 2;
        } else {
            itemCount = 3;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ManageFlipping();
        ManagePlayerDetection();
        ManageStateChangeBasedOnDetectMeasure();

        if (playerInVision && state != State.Sleeping) {
            float distanceToPlayer = Vector2.Distance(transform.position, PlayerMovementController.instance.transform.position);
            detectMeasure += detectMeasureIncrease * Time.deltaTime * (1 - distanceToPlayer / viewDistance);
        } else {
            detectMeasure -= detectMeasureDecrease * Time.deltaTime;
        }
        detectMeasure = Mathf.Clamp(detectMeasure, 0, PatrolMax);

        switch (state) {
            case State.Patrol:
                detectMeter.color = new Color(1f, 1f, 1f, 1f);
                detectMeter.fillAmount = detectMeasure / PatrolMax;
                break;
            case State.Suspicious:
                detectMeter.color = new Color(240f / 255f, 79f / 255f, 120f/ 255f, 1f);
                detectMeter.fillAmount = detectMeasure / SuspiciousMax;
                break;
            case State.Sleeping:
                if (sleepTime <= 0) {
                    sleepTime = startSleepTime;
                    statusIcon.GetComponent<Animator>().SetBool("isSleeping", false);
                    state = State.Patrol;
                } else {
                    sleepTime -= Time.deltaTime;
                    statusIcon.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, ((float) sleepTime / (float) startSleepTime));
                }
                break;
        }
    }

    void FixedUpdate() {
        switch (state) {
            case State.Patrol:
                Patrol();
                break;
        case State.Suspicious:
                Suspicious();
                break;
        }
    }

    void ManageFlipping() {
        if (facingRight) {
            transform.localScale = Vector3.one;
            lookDirection = transform.right;
            statusIcon.localScale = new Vector3(1, 1, 1);
        }
        if (!facingRight) {
            transform.localScale = new Vector3(-1f, 1, 1f);
            lookDirection = -transform.right;
            statusIcon.localScale = new Vector3(-1, 1, 1);
        }
    }

    void ManagePlayerDetection() {
        // Shoots ray, checking objects in Player, Ground, Obstacle layer
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, lookDirection, viewDistance, playerAndObstacleLayerMask);
        RaycastHit2D hitInfo2 = Physics2D.Raycast(transform.position + new Vector3(0, 1, 0), lookDirection, viewDistance, playerAndObstacleLayerMask);

        if (!isDistractedByDoppelganger) {
            if (hitInfo.collider != null) {
                Debug.DrawLine(transform.position, hitInfo.point, Color.red);
                if (state == State.Patrol && hitInfo.collider.CompareTag("Doppelganger")) {
                    BecomeSuspiciousState(hitInfo.point);
                    isDistractedByDoppelganger = true;
                }
                if (hitInfo.collider.CompareTag("Player")
                    && !PlayerActionController.instance.isHiding
                    && !PlayerActionController.instance.isDisguising) {
                    // playerInVision = true;
                    inLine1 = true;
                } else {
                    // playerInVision = false;
                    inLine1 = false;
                }
            } else {
                Debug.DrawLine(transform.position, transform.position + lookDirection * viewDistance, Color.green);
                // playerInVision = false;
                inLine1 = false;
            }

            if (hitInfo2.collider != null) {
                Debug.DrawLine(transform.position + new Vector3(0, 1, 0), hitInfo2.point, Color.red);
                if (hitInfo2.collider.CompareTag("Player")
                    && !PlayerActionController.instance.isHiding
                    && !PlayerActionController.instance.isDisguising) {
                    // playerInVision = true;
                    inLine2 = true;
                } else {
                    // playerInVision = false;
                    inLine2 = false;
                }
            } else {
                Debug.DrawLine(transform.position + new Vector3(0, 1, 0), transform.position + new Vector3(0, 1, 0) + lookDirection * viewDistance, Color.green);
                // playerInVision = false;
                inLine2 = false;
            }
        } else {
            playerInVision = false;
        }

        if (inLine1 || inLine2) {
            playerInVision = true;
        } else {
            playerInVision = false;
        }

    }

    void ManageStateChangeBasedOnDetectMeasure() {
        switch (state) {
            case State.Patrol:
                if (detectMeasure >= PatrolMax) {
                    BecomeSuspiciousState(PlayerMovementController.instance.transform.position);
                }
                break;
            case State.Suspicious:
                if (detectMeasure >= SuspiciousMax) {
                    // Show that player is caught
                    LevelManager.instance.RespawnPlayer();
                }
                break;
        }
    }

    void SetTimes() {
        patrolWaitTime = startPatrolWaitTime;
        suspiciousWaitTime = startSuspiciousWaitTime;
        sleepTime = startSleepTime;
    }

    void BecomeSuspiciousState(Vector2 positionToCheck) {
        state = State.Suspicious;
        suspiciousState = SuspiciousState.MovingToSuspiciousPosition;
        detectMeasure = 0;
        suspiciousPosition = positionToCheck;
        
        statusIcon.GetComponent<SpriteRenderer>().color = Color.white;
        statusIcon.GetComponent<Animator>().SetBool("patrolWaiting", false);
        statusIcon.GetComponent<Animator>().SetTrigger("isSuspicious");
        statusIcon.GetComponent<Animator>().SetBool("Suspicious_Permanent", true);
    }





    void Patrol() {
        switch (patrolState) {
            case PatrolState.Standing:
                break;
            case PatrolState.MovingToNextPoint:
                if (movingRight) {
                    if (!facingRight) {
                        facingRight = true;
                    }
                    rb.MovePosition(rb.position + new Vector2(moveSpeed * Time.fixedDeltaTime, 0));
                    if (transform.position.x > rightPoint.position.x) {
                        movingRight = false;
                        PatrolFromMovingToWaiting();
                    }
                } else if (!movingRight) {
                    if (facingRight) {
                        facingRight = false;
                    }
                    rb.MovePosition(rb.position - new Vector2(moveSpeed * Time.fixedDeltaTime, 0));
                    if (transform.position.x < leftPoint.position.x) {
                        movingRight = true;
                        PatrolFromMovingToWaiting();
                    }
                }
                break;
            case PatrolState.Waiting:
                if (patrolWaitTime <= 0) {
                    patrolWaitTime = startPatrolWaitTime;
                    statusIcon.GetComponent<Animator>().SetBool("patrolWaiting", false);
                    patrolState = PatrolState.MovingToNextPoint;
                } else {
                    patrolWaitTime -= Time.deltaTime;
                    statusIcon.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, ((float) patrolWaitTime / (float) startPatrolWaitTime));
                }
                break;
            case PatrolState.Returning:
                if (suspiciousPosition.x > originPoint.position.x) {
                    facingRight = false;
                    rb.MovePosition(rb.position - new Vector2(moveSpeed * Time.fixedDeltaTime, 0));
                    if (transform.position.x < originPoint.position.x) {
                        patrolState = PatrolState.Standing;
                        facingRight = facingRightAtStart;
                    }
                } else if (suspiciousPosition.x <= originPoint.position.x) {
                    facingRight = true;
                    rb.MovePosition(rb.position + new Vector2(moveSpeed * Time.fixedDeltaTime, 0));
                    if (transform.position.x > originPoint.position.x) {
                        patrolState = PatrolState.Standing;
                        facingRight = facingRightAtStart;
                    }
                }
                break;
        }
    }

    void PatrolFromMovingToWaiting() {
        patrolState = PatrolState.Waiting;
        statusIcon.GetComponent<Animator>().SetBool("patrolWaiting", true);
    }





    void Suspicious() {
        switch (suspiciousState) {
            case SuspiciousState.MovingToSuspiciousPosition:
                // if (suspiciousPosition.x <= rb.position.x) {
                //     rb.MovePosition(rb.position + new Vector2(lookDirection.x * moveSpeed * Time.fixedDeltaTime, 0));
                //     if (rb.position.x < suspiciousPosition.x) {
                //         SuspiciousFromMovingToWaiting();
                //     }
                // }
                // if (suspiciousPosition.x >= rb.position.x) {
                //     rb.MovePosition(rb.position + new Vector2(lookDirection.x * moveSpeed * Time.fixedDeltaTime, 0));
                //     if (rb.position.x > suspiciousPosition.x) {
                //         SuspiciousFromMovingToWaiting();
                //     }
                // }
                rb.MovePosition(rb.position + new Vector2(lookDirection.x * moveSpeed * Time.fixedDeltaTime, 0));
                if (Vector2.Distance(rb.position, suspiciousPosition) < .1f) {
                    SuspiciousFromMovingToWaiting();
                }
                break;
            case SuspiciousState.Waiting:
                if (suspiciousWaitTime <= 0) {
                    suspiciousWaitTime = startSuspiciousWaitTime;
                    statusIcon.GetComponent<Animator>().SetBool("suspiciousWaiting", false);
                    statusIcon.GetComponent<SpriteRenderer>().color = Color.white;
                    suspiciousState = SuspiciousState.ReturningToPoint;
                    isDistractedByDoppelganger = false;
                } else {
                    suspiciousWaitTime -= Time.deltaTime;
                    statusIcon.GetComponent<Animator>().SetBool("Suspicious_Permanent", false);
                    statusIcon.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, ((float) suspiciousWaitTime / (float) startSuspiciousWaitTime));
                }
                break;
            case SuspiciousState.ReturningToPoint:
                if (isStandingGuard) {
                    state = State.Patrol;
                    patrolState = PatrolState.Returning;  
                    patrolWaitTime = startPatrolWaitTime;
                } else {
                    state = State.Patrol;
                    patrolState = PatrolState.MovingToNextPoint;
                    patrolWaitTime = startPatrolWaitTime;
                }
                break;
        }
    }

    public void SuspiciousFromMovingToWaiting() {
        suspiciousState = SuspiciousState.Waiting;
        statusIcon.GetComponent<Animator>().SetBool("suspiciousWaiting", true);        
    }





    public void FallAsleep(float sleepLength) {
        statusIcon.GetComponent<Animator>().SetBool("patrolWaiting", false);
        statusIcon.GetComponent<Animator>().SetBool("suspiciousWaiting", false);
        statusIcon.GetComponent<Animator>().SetBool("Suspicious_Permanent", false);
        statusIcon.GetComponent<Animator>().SetBool("isSleeping", true);
        patrolWaitTime = startPatrolWaitTime;
        suspiciousWaitTime = startSuspiciousWaitTime;
        state = State.Sleeping;
        sleepTime = sleepLength;
        startSleepTime = sleepLength;
    }

    public void DropItem() {
        Instantiate(stealEffect, PlayerActionController.instance.actionPoint.position, transform.rotation);
        if (hasTutorialKey) {
            Instantiate(tutorialKey, PlayerActionController.instance.actionPoint.position, transform.rotation);
            hasTutorialKey = false;
        } else if (hasOrbRoomKey) {
            Instantiate(orbRoomKey, PlayerActionController.instance.actionPoint.position, transform.rotation);
            hasOrbRoomKey = false;
        } else if (hasNote2) {
            Instantiate(note2, PlayerActionController.instance.actionPoint.position, transform.rotation);
            hasNote2 = false;
        } else if (itemCount > 0) {
            itemCount--;
            int randomNum = Random.Range(1, 100);
            if (randomNum <= 60) {
                Instantiate(coin, PlayerActionController.instance.actionPoint.position, transform.rotation);        
            } else {
                Instantiate(potion, PlayerActionController.instance.actionPoint.position, transform.rotation);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            if (detectMeasure >= .2f || state == State.Suspicious) {
                LevelManager.instance.RespawnPlayer();
            }
        }
    }

}
