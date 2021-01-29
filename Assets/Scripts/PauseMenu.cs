using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // TODO: Not here, but when loading scenes, set timescale back to 1.
    public void PauseUnpause() {
        if (isPaused) {
            isPaused = false;
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;
        } else {
            isPaused = true;
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
