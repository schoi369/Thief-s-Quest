﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarRemove : MonoBehaviour
{
    public GameObject FakeUI;
    public GameObject RealUI;

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
            FakeUI.SetActive(false);
            RealUI.SetActive(true);
        }
    }
}
