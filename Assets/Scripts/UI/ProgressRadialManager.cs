using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressRadialManager : MonoBehaviour
{

    public float progress = 0f;

    [Space]

    public GameObject occupant;

    float fillSpeed = 1f;
    float decaySpeed = 8f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     
        // Update the fill percent

        if (occupant != null)
        {
            progress += Time.deltaTime * fillSpeed;
        } else
        {
            progress -= Time.deltaTime * decaySpeed;
        }

        if (progress > 1f)
        {
            progress = 1f;
        } else if (progress < 0f)
        {
            progress = 0f;
        }

        // Set the fill
        gameObject.GetComponent<Image>().fillAmount = progress;

    }
}
