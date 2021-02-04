using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopIcon : MonoBehaviour
{
    public GameObject KeyCanvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            PlayerActionController.instance.shopIcon = transform;
            KeyCanvas.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            KeyCanvas.SetActive(false);
        }
    }
}
