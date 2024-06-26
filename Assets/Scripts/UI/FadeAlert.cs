using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FadeAlert : MonoBehaviour
{
    float timePassed = 0f;
    float lifeTime = 1f;
    float fadeTime = 0.5f;

    float speed = 50f;

    Color defaultColor;

    private void Start()
    {
        defaultColor = gameObject.GetComponent<TextMeshProUGUI>().color;
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;

        transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + speed * Time.deltaTime);

        if (timePassed >= lifeTime)
        {

            float fadeAmount = timePassed / (lifeTime + fadeTime);

            float transparency = 1 - (defaultColor.a * fadeAmount);

            gameObject.GetComponent<TextMeshProUGUI>().color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, transparency);

            if (fadeAmount >= 1f)
            {
                Destroy(gameObject);
            }
        }
    }
}
