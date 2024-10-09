using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RaiderMovement : MonoBehaviour
{
    public float speed = 1.0f;
    public GameObject Player;
    private Transform PlayerPosition;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPosition = Player.transform;
    }
    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position,PlayerPosition.position, step);
        transform.LookAt(PlayerPosition.position, transform.up);
    }
}
