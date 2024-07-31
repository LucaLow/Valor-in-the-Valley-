using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RangedProjectile : MonoBehaviour
{
    [Tooltip("The units per second the projectile will travel.")]
    public float speed = 15;
    [Tooltip("The time after spawning in which the projectile will be destroyed (for performance).")]
    public float destroyDelay = 10f;

    [Space]

    [Tooltip("The tags of gameobjects that when hit, will send the on hit messages.")]
    public string[] affectedTags;
    [Tooltip("The tags that this projectile will ignore.")]
    public string[] ignoreTags;
    [Tooltip("The message(s) sent to the hit gameobject for a function to be called as.")]
    public string[] onHitMessages;

    [HideInInspector]
    public Vector3 direction;

    private void Start()
    {
        // Prepare the delayed destroy event
        Destroy(gameObject, destroyDelay);
    }

    private void Update()
    {
        // Move the projectile in the given direction
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (string tag in affectedTags)
        {
            if (other.CompareTag(tag))
            {
                foreach (string method in onHitMessages)
                {
                    other.gameObject.SendMessage(method);
                }

                break;
            }
        }
        Destroy(gameObject);
    }
}
