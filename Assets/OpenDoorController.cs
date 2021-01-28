using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            transform.parent.GetComponent<DoorController>().KeyCanvas.SetActive(true);
            transform.parent.GetComponent<DoorController>().Close();
        }
    }
}
