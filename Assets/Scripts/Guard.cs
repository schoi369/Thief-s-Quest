using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guard : MonoBehaviour
{
    Rigidbody2D rb;
    public float moveSpeed;
    bool facingRight;

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
        Standing, Waiting, MovingToNextPoint
    }
    [Header("State Related")]
    public State state;
    public PatrolState patrolState;

    public Transform leftPoint, rightPoint;
    bool movingRight;
    float patrolWaitTime;
    public float startPatrolWaitTime; // Set in inspector

    Vector2 suspiciousPosition;

    [Header("UI Related")]
    public Image detectMeter;
    public Transform statusIcon;

    // Start is called before the first frame update
    void Start()
    {
        Physics2D.queriesStartInColliders = false;
        rb = GetComponent<Rigidbody2D>();

        SetTimes();

        // Release move points
        leftPoint.parent = null;
        rightPoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        ManageFlipping();
        ManagePlayerDetection();
        ManageStateChangeBasedOnDetectMeasure();

        if (playerInVision) {
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
        }
    }

    void FixedUpdate() {
        switch (state) {
            case State.Patrol:
                Patrol();
                break;
        case State.Suspicious:

                break;
        }
    }

    void ManageFlipping() {
        if (facingRight) {
            transform.localScale = Vector3.one;
            lookDirection = transform.right;
        }
        if (!facingRight) {
            transform.localScale = new Vector3(-1f, 1, 1f);
            lookDirection = -transform.right;
        }
    }

    void ManagePlayerDetection() {
        if (PlayerActionController.instance.isHiding) {
            playerInVision = false;
            return;
        }
        // Shoots ray, checking objects in Player, Ground, Obstacle layer
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, lookDirection, viewDistance, playerAndObstacleLayerMask);
        if (hitInfo.collider != null) {
            Debug.DrawLine(transform.position, hitInfo.point, Color.red);
            if (hitInfo.collider.CompareTag("Player")) {
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
                }
                break;
        }
    }

    void SetTimes() {
        patrolWaitTime = startPatrolWaitTime;
    }

    void BecomeSuspiciousState(Vector2 positionToCheck) {
        state = State.Suspicious;
        detectMeasure = 0;
        suspiciousPosition = positionToCheck;
        if (!facingRight) {
            statusIcon.localScale = new Vector3(-1, 1, 1);
        }
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
        }
    }

    void PatrolFromMovingToWaiting() {
        patrolState = patrolState = PatrolState.Waiting;
        statusIcon.GetComponent<Animator>().SetBool("patrolWaiting", true);
    }
}
