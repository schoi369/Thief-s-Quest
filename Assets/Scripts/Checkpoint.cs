using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool isLastCheckpoint;

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
            if (isLastCheckpoint) {
                CheckpointController.instance.spawnPoint = transform.position;
                CheckpointController.instance.savedMaxMP = PlayerActionController.instance.maxMP;
                CheckpointController.instance.savedSleepDaggerLength = PlayerActionController.instance.sleepDaggerLength;
                CheckpointController.instance.savedDoppelgangerDistance = PlayerActionController.instance.doppelgangerDistance;
                CheckpointController.instance.savedDisguiseLength = PlayerActionController.instance.disguiseLength;

                CheckpointController.instance.savedHasTutorialKey = PlayerActionController.instance.hasTutorialKey;
                CheckpointController.instance.savedHasOrbRoomKey = PlayerActionController.instance.hasOrbRoomKey;

                SaveSystem.SaveCurrentData();
            } else {
                if (!PlayerActionController.instance.hasOrb) {
                    CheckpointController.instance.spawnPoint = transform.position;
                    CheckpointController.instance.savedMaxMP = PlayerActionController.instance.maxMP;
                    CheckpointController.instance.savedSleepDaggerLength = PlayerActionController.instance.sleepDaggerLength;
                    CheckpointController.instance.savedDoppelgangerDistance = PlayerActionController.instance.doppelgangerDistance;
                    CheckpointController.instance.savedDisguiseLength = PlayerActionController.instance.disguiseLength;

                    CheckpointController.instance.savedHasTutorialKey = PlayerActionController.instance.hasTutorialKey;
                    CheckpointController.instance.savedHasOrbRoomKey = PlayerActionController.instance.hasOrbRoomKey;

                    SaveSystem.SaveCurrentData();
                }
            }
        }
    }
}
