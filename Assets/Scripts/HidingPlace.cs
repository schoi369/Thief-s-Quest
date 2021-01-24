using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingPlace : MonoBehaviour
{
    public GameObject KeyCanvas;

    public enum HidingPlaceType {
        Statue, Door, Barrel
    }

    public HidingPlaceType type;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        // When player enters, sets its hiding position next to the hiding place
        if (other.CompareTag("Player")) {
            PlayerActionController.instance.hidingPlace = transform;
            KeyCanvas.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            KeyCanvas.SetActive(false);
        }
    }
}
