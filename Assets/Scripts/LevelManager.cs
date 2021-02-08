using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public float waitToRespawn;

    public GameObject FakeUI;
    public GameObject RealUI;

    void Awake() {
        if (instance == null) {
            instance = this;
            if (!LevelRestartedOrNot.levelRestarted) {
                SetLoadedData();
                FakeUI.SetActive(false);
                RealUI.SetActive(true);
            }
        } else {
            if (LevelRestartedOrNot.levelRestarted) {
                Destroy(gameObject);
            } else {
                SetLoadedData();
                FakeUI.SetActive(false);
                RealUI.SetActive(true);
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RespawnPlayer() {
        StartCoroutine(RespawnCo());
    }

    IEnumerator RespawnCo() {
        LevelRestartedOrNot.levelRestarted = false;
        PlayerActionController.instance.gameObject.SetActive(false);
        // PlayerActionController.instance.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(waitToRespawn - (1f / HUDController.instance.fadeSpeed));
        HUDController.instance.FadeToBlack();
        yield return new WaitForSeconds((1f / HUDController.instance.fadeSpeed) + .2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void SetLoadedData() {
        SaveData data = SaveSystem.LoadSavedData();
        
        PlayerActionController.instance.transform.position = new Vector3(data.savedSpawnPoint[0], data.savedSpawnPoint[1], data.savedSpawnPoint[2]);
        PlayerActionController.instance.maxMP = data.savedMaxMP;
        PlayerActionController.instance.sleepDaggerLength = data.savedSleepDaggerLength;
        PlayerActionController.instance.doppelgangerDistance = data.savedDoppelgangerDistance;
        PlayerActionController.instance.disguiseLength = data.savedDisguiseLength;
        PlayerActionController.instance.hasTutorialKey = data.savedHasTutorialKey;
        PlayerActionController.instance.hasOrbRoomKey = data.savedHasOrbRoomKey;
    }

    // void SetBeginningData() {
    //     PlayerActionController.instance.transform.position = new Vector3(-9.75f, -4.25f, 0);
    //     PlayerActionController.instance.maxMP = 64;
    // }
}
