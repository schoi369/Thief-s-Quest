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
    public float viewDistance; // Set in inspector
    float detectMeasure;
    float detectMeasureIncrease = 3f;
    float detectMeasureDecrese = 1f;
    float PatrolMax = 10f;
    float SuspiciousMax = 5f;
    public LayerMask playerAndObstacleLayerMask;

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
    public float startSleepTime; // Set in inspector

    Vector2 suspiciousPosition;

    [Header("UI Related")]
    public Image detectMeter;
    public Transform statusIcon;

    [Header("Others")]
    public GameObject stealEffect;
    public int itemCount;
    public GameObject coin;
    public GameObject potion;

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
            detectMeasure += detectMeasureIncrease * Time.deltaTime;
        } else {
            detectMeasure -= detectMeasureDecrese * Time.deltaTime;
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
        if (hitInfo.collider != null) {
            Debug.DrawLine(transform.position, hitInfo.point, Color.red);
            if (state == State.Patrol && hitInfo.collider.CompareTag("Doppelganger")) {
                BecomeSuspiciousState(hitInfo.point);
            }
            if (hitInfo.collider.CompareTag("Player") && !PlayerActionController.instance.isHiding) {
                playerInVision = true;
            } else {
                playerInVision = false;
            }
        } else {
            Debug.DrawLine(transform.position, transform.position + lookDirection * viewDistance, Color.green);
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
                    // PlayerFound()
                    Debug.Log("Player FOUND!");
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
                // if (suspiciousPosition.x < leftPoint.position.x) {
                //     facingRight = true;
                //     rb.MovePosition(rb.position + new Vector2(lookDirection.x * moveSpeed * Time.fixedDeltaTime, 0));
                //     if (rb.position.x > rightPoint.position.x) {
                //         state = State.Patrol;
                //         patrolState = PatrolState.MovingToNextPoint;
                //         patrolWaitTime = startPatrolWaitTime;
                //         statusIcon.GetComponent<Animator>().SetBool("Suspicious_Permanent", false);
                //     }
                // } else if (suspiciousPosition.x >= leftPoint.position.x && suspiciousPosition.x <= rightPoint.position.x) {
                //     if (facingRight) {
                //         rb.MovePosition(rb.position + new Vector2(lookDirection.x * moveSpeed * Time.fixedDeltaTime, 0));
                //         if (rb.position.x > rightPoint.position.x) {
                //             state = State.Patrol;
                //             patrolState = PatrolState.MovingToNextPoint;
                //             patrolWaitTime = startPatrolWaitTime;
                //             statusIcon.GetComponent<Animator>().SetBool("Suspicious_Permanent", false);
                //         }
                //     } else {
                //         rb.MovePosition(rb.position + new Vector2(lookDirection.x * moveSpeed * Time.fixedDeltaTime, 0));
                //         if (rb.position.x < leftPoint.position.x) {
                //             state = State.Patrol;
                //             patrolState = PatrolState.MovingToNextPoint;
                //             patrolWaitTime = startPatrolWaitTime;
                //             statusIcon.GetComponent<Animator>().SetBool("Suspicious_Permanent", false);
                //         }
                //     }
                // } else if (suspiciousPosition.x > rightPoint.position.x) {
                //     facingRight = false;
                //     rb.MovePosition(rb.position + new Vector2(lookDirection.x * moveSpeed * Time.fixedDeltaTime, 0));
                //     if (rb.position.x < leftPoint.position.x) {
                //         state = State.Patrol;
                //         patrolState = PatrolState.MovingToNextPoint;
                //         patrolWaitTime = startPatrolWaitTime;
                //         statusIcon.GetComponent<Animator>().SetBool("Suspicious_Permanent", false);
                //     }
                // }
                break;
        }
    }

    void SuspiciousFromMovingToWaiting() {
        suspiciousState = SuspiciousState.Waiting;
        statusIcon.GetComponent<Animator>().SetBool("suspiciousWaiting", true);        
    }





    public void FallAsleep() {
        statusIcon.GetComponent<Animator>().SetBool("patrolWaiting", false);
        statusIcon.GetComponent<Animator>().SetBool("suspiciousWaiting", false);
        statusIcon.GetComponent<Animator>().SetBool("Suspicious_Permanent", false);
        statusIcon.GetComponent<Animator>().SetBool("isSleeping", true);
        patrolWaitTime = startPatrolWaitTime;
        suspiciousWaitTime = startSuspiciousWaitTime;
        state = State.Sleeping;
    }

    public void DropItem() {
        Instantiate(stealEffect, PlayerActionController.instance.actionPoint.position, transform.rotation);
        if (itemCount > 0) {
            itemCount--;
            int randomNum = Random.Range(1, 100);
            if (randomNum <= 60) {
                Instantiate(coin, PlayerActionController.instance.actionPoint.position, transform.rotation);        
            } else {
                Instantiate(potion, PlayerActionController.instance.actionPoint.position, transform.rotation);
            }
        }
    }

}
