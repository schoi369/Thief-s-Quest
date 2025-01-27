﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    public GameObject pauseScreen;
    public bool isPaused;

    void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            PauseUnpause();
        }
    }

    public void PauseUnpause() {
        if (isPaused) {
            isPaused = false;
            pauseScreen.SetActive(false);
            Cursor.visible = false;
            Time.timeScale = 1f;
            Debug.Log(ShopManager.instance.isShopOpen);
            if (ShopManager.instance.isShopOpen) {
                Cursor.visible = true;
                Time.timeScale = 0f;
            }
        } else {
            isPaused = true;
            pauseScreen.SetActive(true);
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
    }

    public void RestartFromCheckpoint() {
        LevelRestartedOrNot.levelRestarted = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public void RestartLevel() {
        LevelRestartedOrNot.levelRestarted = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public void ToMainMenu() {
        SceneManager.LoadScene("Main Menu");
        Cursor.visible = true;
    }
}
