using System;
using UnityEngine;
using System.Linq;

public class Camera_Controller : MonoBehaviour
{
    public GameObject player;
    public float Distance_ = 5f;
    Vector3 offset;
    
    float desiredAngleX = 0;
    void Start()
    {
        offset = transform.position - player.transform.position;
        offset = offset.normalized * Distance_;
    }

    void Update()
    {
        // Distance_ -= Input.GetAxis("Mouse ScrollWheel") * 2;
        Distance_ = Mathf.Clamp(Distance_, 2, 10);
    }

    void FixedUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * 2;
        float mouseY = Input.GetAxis("Mouse Y");

        desiredAngleX -= mouseY;
        desiredAngleX = Mathf.Clamp(desiredAngleX, -60, 50);
        float desiredAngleY = player.transform.eulerAngles.y + mouseX;
        player.transform.parent.eulerAngles = new Vector3(0, desiredAngleY, 0);

        Quaternion rotation = Quaternion.Euler(desiredAngleX, player.transform.eulerAngles.y, 0);
        
        RaycastHit hit;
        Vector3 direction = (player.transform.position - transform.position).normalized;
        RaycastHit[] hits = Physics.RaycastAll(player.transform.position, -direction, offset.magnitude, LayerMask.GetMask("Block"));
        if (hits.Length > 0)
        {
            RaycastHit closestHit = hits.OrderBy(h => h.distance).FirstOrDefault();
            if (closestHit.collider != null)
            {
                float distance = Vector3.Distance(player.transform.position, closestHit.point) - 0.8f;
                transform.position = player.transform.position + rotation * offset.normalized * distance;
            }
        }
        else
        {
            transform.position = player.transform.position + rotation * offset.normalized * Distance_;
        }
        transform.LookAt(player.transform.position);
    }
}
