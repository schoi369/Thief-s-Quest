using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardStopTrigger : MonoBehaviour
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
        if (other.CompareTag("Guard")) {
            if (other.GetComponent<Guard>().state == Guard.State.Suspicious) {
                other.GetComponent<Guard>().SuspiciousFromMovingToWaiting();
            }
        }
    }
}
