using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public bool isCoin;
    public bool isPotion;
    public bool isTutorialKey;
    public bool isOrbRoomKey;
    // int MPRegeneratedByPotion = 5;
    bool isCollected;
    float colliderActivateTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        colliderActivateTime -= Time.deltaTime;
        if (colliderActivateTime < 0) {
            GetComponent<CircleCollider2D>().enabled = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !isCollected) {
            if (isCoin) {
                PlayerActionController.instance.coinCount++;
                isCollected = true;
                Destroy(gameObject);

                HUDController.instance.UpdateCoinCountDisplay();
            }

            if (isPotion) {
                // if (PlayerActionController.instance.currentMP + MPRegeneratedByPotion > PlayerActionController.instance.maxMP) {
                //     PlayerActionController.instance.currentMP = PlayerActionController.instance.maxMP;
                // } else {
                //     PlayerActionController.instance.currentMP += MPRegeneratedByPotion;
                // }
                PlayerActionController.instance.potionCount++;
                isCollected = true;
                Destroy(gameObject);

                // HUDController.instance.UpdateMPDisplay();
                HUDController.instance.UpdatePotionCountDisplay();
            }

            if (isTutorialKey) {
                PlayerActionController.instance.hasTutorialKey = true;
                Destroy(gameObject);
            }

            if (isOrbRoomKey) {
                PlayerActionController.instance.hasOrbRoomKey = true;
                Destroy(gameObject);
            }
        }
    }
}
