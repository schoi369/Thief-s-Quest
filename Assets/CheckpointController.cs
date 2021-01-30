using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    public static CheckpointController instance;
    public Vector3 spawnPoint;
    public int savedMaxMP;
    public float savedSleepDaggerLength;
    public float savedDoppelgangerDistance;
    public float savedDisguiseLength;

    void Awake() {
        instance = this; 
    }
    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = PlayerMovementController.instance.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
