using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public float waitToRespawn;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            SetLoadedData();
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
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
        PlayerActionController.instance.gameObject.SetActive(false);
        // PlayerActionController.instance.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(waitToRespawn);
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
    }
}
