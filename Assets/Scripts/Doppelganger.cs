﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doppelganger : MonoBehaviour
{
    public float lifeTime;
    public bool countdownStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (countdownStarted) {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0) {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Guard")) {
            if (other.GetComponent<Guard>().isDistractedByDoppelganger) {
                countdownStarted = true;
            }
        }
    }


}
