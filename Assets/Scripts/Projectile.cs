using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject player;
    private PlayerHealth playerHealth;
    public int damage;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float LaunchForce = 50f;
    [SerializeField] private float destroyAfterSeconds = 5f;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        rb.velocity = transform.forward * LaunchForce;
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            playerHealth.TakeDamage(damage);
        }
    }
       
}
