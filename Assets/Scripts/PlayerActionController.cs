﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
    public static PlayerActionController instance;

    Rigidbody2D rb;
    Animator animator;

    public Transform hidingPlace;

    public bool isHiding;
    public bool isScouting;

    public int currentSkillNum = 0;

    public int sleepDaggerMPCost, coinThrowMPCost, dopplegangerMPCost, disguiseMPCost; // Set in inspector

    void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ManageSkillSelect();

        if (Input.GetKeyDown(KeyCode.LeftShift) && PlayerMovementController.instance.isGrounded) {
            isScouting = !isScouting;
            Debug.Log("isScouting: " + isScouting);
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            HideUnhide();
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

    void ManageSkillSelect() {
        if (Input.GetKeyDown(KeyCode.U)) {
            if (currentSkillNum == 0) {
                currentSkillNum = 3;
            } else {
                currentSkillNum--;
            }
            HUDController.instance.UpdateSkillDisplay();
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            if (currentSkillNum == 3) {
                currentSkillNum = 0;
            } else {
                currentSkillNum++;
            }
            HUDController.instance.UpdateSkillDisplay();
        }
    }
}