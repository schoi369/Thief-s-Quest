using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public float[] savedSpawnPoint;
    public int savedMaxMP;
    public float savedSleepDaggerLength;
    public float savedDoppelgangerDistance;
    public float savedDisguiseLength;

    public bool savedHasTutorialKey;
    public bool savedHasOrbRoomKey;

    public SaveData() {
        savedSpawnPoint = new float[3];
        savedSpawnPoint[0] = CheckpointController.instance.spawnPoint.x;
        savedSpawnPoint[1] = CheckpointController.instance.spawnPoint.y;
        savedSpawnPoint[2] = CheckpointController.instance.spawnPoint.z;

        savedMaxMP = CheckpointController.instance.savedMaxMP;
        savedSleepDaggerLength = CheckpointController.instance.savedSleepDaggerLength;
        savedDoppelgangerDistance = CheckpointController.instance.savedDoppelgangerDistance;
        savedDisguiseLength = CheckpointController.instance.savedDisguiseLength;

        savedHasTutorialKey = CheckpointController.instance.savedHasTutorialKey;
        savedHasOrbRoomKey = CheckpointController.instance.savedHasOrbRoomKey;
    }

}
