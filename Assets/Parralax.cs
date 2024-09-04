using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parralax : MonoBehaviour
{
    private Vector3 oldMousePos;
    private GameObject cam;
    public float parralaxEffect;
    private void Start()
    {
        cam = Camera.main.gameObject;
        oldMousePos = Input.mousePosition;
    }
    private void FixedUpdate()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 difference = mousePos - oldMousePos;
        cam.transform.position += difference/2000 * parralaxEffect;
        oldMousePos = mousePos;
    }
}
