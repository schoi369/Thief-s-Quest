using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public bool isCoin;
    bool isCollected;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !isCollected) {
            if (isCoin) {
                PlayerActionController.instance.coinCount++;
                Debug.Log(PlayerActionController.instance.coinCount);
                isCollected = true;
                Destroy(gameObject);
            }
        }
    }
}
