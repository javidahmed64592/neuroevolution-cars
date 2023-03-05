using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowCamMove : MonoBehaviour
{
    private Transform cam;
    private float smoothStep = 10f;

    private void Awake()
    {
        cam = GetComponent<Transform>();
    }

    private void LateUpdate()
    {
        cam.position = Vector3.Lerp(cam.position, cam.position + cam.forward, 1 / smoothStep);
    }
}