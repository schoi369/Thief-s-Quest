using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public static PlayerMovementController instance;

    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    
    public float moveSpeed;
    public float jumpForce;

    public bool facingRight;

    public bool isGrounded;
    public Transform groundCheckPoint;
    public LayerMask groundLayerMask;

    bool canGrabWall;
    public bool isGrabbingWall;
    public Transform wallGrabPoint;
    private float gravityStore;
    public float wallJumpTime = .1f;
    private float wallJumpCounter;
    public float extraGrabTime = .1f;
    private float extraGrabTimeCounter;
    float lastLocalScale;
    bool hasWallJumped;

    public bool canMove;

    void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        gravityStore = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.instance.isPaused && !DialogueManager.instance.isTalking) {
            ManageCanMove();

            if (canMove) {
                if (wallJumpCounter <= 0) {

                    rb.velocity = new Vector2(moveSpeed * Input.GetAxisRaw("Horizontal"), rb.velocity.y);

                    isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, .1f, groundLayerMask);
                    if (!PlayerActionController.instance.isDisguising) {

                        if (Input.GetKeyDown(KeyCode.K)) {
                            if (isGrounded) {
                                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                            }
                        }

                        // Handle wall jumping
                        isGrabbingWall = false;
                        canGrabWall = Physics2D.OverlapCircle(wallGrabPoint.position, .2f, groundLayerMask);
                        if (canGrabWall && !isGrounded) {
                            if ((facingRight && Input.GetAxisRaw("Horizontal") >= 0) || (!facingRight && Input.GetAxisRaw("Horizontal") <= 0)) {
                                isGrabbingWall = true;
                            }
                        }
                        if (isGrabbingWall) {
                            rb.gravityScale = 0f;
                            rb.velocity = Vector2.zero;
                            lastLocalScale = transform.localScale.x;
                            extraGrabTimeCounter = extraGrabTime;
                            hasWallJumped = false;
                        } else {
                            rb.gravityScale = gravityStore;
                            extraGrabTimeCounter -= Time.deltaTime;
                        }
                        if (Input.GetKeyDown(KeyCode.K) && extraGrabTimeCounter > 0 && !hasWallJumped) {
                            wallJumpCounter = wallJumpTime;
                            rb.velocity = new Vector2(-lastLocalScale * moveSpeed, jumpForce);
                            rb.gravityScale = gravityStore;
                            isGrabbingWall = false;
                            hasWallJumped = true;
                        }
                    }

                    ManageFlipping();
                } else {
                    wallJumpCounter -= Time.deltaTime;
                }
            } else {
                rb.velocity = Vector2.zero;
            }

            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isGrabbingWall", isGrabbingWall);
        }

    }


    void ManageFlipping() {
        if (rb.velocity.x > 0) {
            transform.localScale = Vector3.one;
            facingRight = true;
        }
        if (rb.velocity.x < 0) {
            transform.localScale = new Vector3(-1f, 1, 1f);
            facingRight = false;
        }
    }

    void ManageCanMove() {
        if (PlayerActionController.instance.isHiding || PlayerActionController.instance.isScouting
            || !PlayerActionController.instance.canMakeAction) {
            canMove = false;
        } else {
            canMove = true;
        }
        if (PlayerActionController.instance.isDisguising) {
            canMove = true;
        }
    }
}
