using SurvivalElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    public Transform target; // the player gameobject to follow
    public float smoothSpeed = 0.125f; // the speed at which the camera follows the player
    public Vector3 offset; // the offset distance from the player
    public bool followY;
    public bool followX;
    public bool isSprite;
    public Sprite sprite;

    public void Start()
    {

        target = PlayerManager.Instance.transform;
        float pointX = followX ? target.position.x + offset.x : transform.position.x;
        float pointY = followY ? target.position.y + offset.y : transform.position.y;

        transform.position = new(pointX, pointY, transform.position.z);
        //transform.position += Vector3.back;
    }

    void FixedUpdate()
    {


        float pointX = followX ? target.position.x + offset.x : transform.position.x;
        float pointY = followY ? target.position.y + offset.y : transform.position.y;
        Vector3 desiredPosition = new(pointX, pointY, transform.position.z);
        //desiredPosition += Vector3.back;

        // update the camera's position to the smoothed position
        transform.position = desiredPosition;
    }
}
