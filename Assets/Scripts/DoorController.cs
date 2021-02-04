using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public GameObject KeyCanvas;

    public bool isVertical;
    public bool isOpen;
    public bool isTutorialDoor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(PlayerActionController.instance.transform.position, transform.position) > 1.5) {
            Close();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            if (isTutorialDoor) {
                if (PlayerActionController.instance.hasTutorialKey) {
                    PlayerActionController.instance.movableDoor = transform;
                    if (!isOpen) {
                        KeyCanvas.SetActive(true);
                    }
                }
            } else {
                PlayerActionController.instance.movableDoor = transform;
                if (!isOpen) {
                    KeyCanvas.SetActive(true);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            KeyCanvas.SetActive(false);
        }
    }

    public void Close() {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        isOpen = false;
    }

    public void Open() {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);  
        isOpen = true;      
    }
}
