﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public bool hasPlayed;
    public bool shouldRepeat;

    public void TriggerDialogue() {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !hasPlayed) {
            if (shouldRepeat) {
                TriggerDialogue();
            } else {
                TriggerDialogue();
                hasPlayed = true;
            }
        }
    }
}
