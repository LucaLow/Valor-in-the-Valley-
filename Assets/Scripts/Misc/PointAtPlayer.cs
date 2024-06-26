using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PointAtPlayer : MonoBehaviour
{

    public Transform _camera;

    float defaultY;

    private void Start()
    {
        defaultY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Move up & down over time
        float yDisplacement = Mathf.Sin(Time.time);
        transform.position = new Vector3(transform.position.x, yDisplacement + defaultY, transform.position.z);

        // Set rotation to look at the player
        Vector3 rot = Quaternion.LookRotation(_camera.position - transform.position).eulerAngles;
        rot.x = rot.z = 0;
        transform.rotation = Quaternion.Euler(rot);
    }
}
