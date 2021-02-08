using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public bool hasPlayed;

    public void TriggerDialogue() {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !hasPlayed && PlayerActionController.instance.hasOrb) {
            StartCoroutine(EndingCo());
        }
    }

    IEnumerator EndingCo() {
        HUDController.instance.FadeToBlack();
        yield return new WaitForSeconds((1f / HUDController.instance.fadeSpeed) + .2f);
        TriggerDialogue();
        hasPlayed = true;
    }
}
