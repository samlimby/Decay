using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // This will be your player
    public float smoothing = 5f; // This will dictate how quickly the camera catches up

    Vector3 offset; // This will be the initial offset

    void Start()
    {
        offset = transform.position - target.position; // This calculates the initial offset
    }

    void FixedUpdate()
    {
        Vector3 targetCamPos = target.position + offset; // This calculates where we want the camera to be
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime); // This smoothly transitions the camera to that location
    }
}
