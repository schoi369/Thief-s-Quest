using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame() {
        LevelRestartedOrNot.levelRestarted = true;
        Cursor.visible = false;
        SceneManager.LoadScene("Level");
    }

    public void ContinueGame() {
        LevelRestartedOrNot.levelRestarted = false;
        Cursor.visible = false;
        SceneManager.LoadScene("Level");
    }

    public void QuitGame() {
        Application.Quit();
    }
}
