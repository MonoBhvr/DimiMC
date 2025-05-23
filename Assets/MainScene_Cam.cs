using Unity.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MainScene_Cam : MonoBehaviour
{
    public float radius = 5f; // The radius of the sphere
    //0,0을 기준으로 하고 평면 XZ에서의 원 위를 이동하며 0, 0, 0을 바라보는 카메라
    public float speed = 10f; // The speed of rotation
    public float height = 2f; // The height of the camera above the ground
    public float xl = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {

    // }

    // Update is called once per frame
    void Update()
    {
        xl += Time.deltaTime;
        // Debug.Log($"frame: {Time.frameCount}, delta: {Time.deltaTime}, scale: {Time.timeScale}");
        // Calculate the angle of rotation based on time
        float angle = xl * speed;

        // Calculate the new position of the camera
        float x = radius * Mathf.Cos(angle);
        float z = radius * Mathf.Sin(angle);
        float y = height;

        // Set the camera's position
        transform.position = new Vector3(x, y, z);

        // Make the camera look at the origin (0, 0, 0)
        transform.LookAt(Vector3.zero);
        
    }

    // IEnumerator Start()
    // {
    //     while (true)
    //     {
    //         yield return null;
    //         Debug.Log($"Coroutine frame: {Time.frameCount}, delta: {Time.deltaTime}");
    //     }
    // }
}
