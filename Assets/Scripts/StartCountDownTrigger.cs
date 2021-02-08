using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartCountDownTrigger : MonoBehaviour
{
    public Text countdownText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && PlayerActionController.instance.hasOrb) {
            countdownText.gameObject.SetActive(true);
            HUDController.instance.transform.GetComponent<CountdownTimer>().enabled = true;
        }
    }
}
