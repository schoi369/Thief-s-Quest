using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float moveSpeed;
    Vector2 velocity;
    float horizontalViewDistance = 6f;
    float verticalViewDistance;
    bool hasReturned;
    
    // Start is called before the first frame update
    void Start()
    {
        verticalViewDistance = (horizontalViewDistance / 16f) * 9f;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.instance.isScouting) {
            velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, Input.GetAxis("Vertical") * moveSpeed);
            transform.Translate(velocity * Time.deltaTime);
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, target.position.x - horizontalViewDistance, target.position.x + horizontalViewDistance),
                                             Mathf.Clamp(transform.position.y, target.position.y - verticalViewDistance, target.position.y + verticalViewDistance),
                                             transform.position.z);
            hasReturned = false;
        } else {
            if (!hasReturned) {
                transform.position = new Vector3(Mathf.Lerp(transform.position.x, target.position.x, .1f), Mathf.Lerp(transform.position.y, target.position.y, .1f), transform.position.z);
                if (Vector2.Distance(transform.position, target.position) < .1f) {
                    hasReturned = true;
                }
            } else {
                transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
            }
        }
    }
}
