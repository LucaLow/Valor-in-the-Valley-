using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using System.Drawing;

public class ResourceGenerator : MonoBehaviour
{

    public GameObject _resourceManager;
    public Transform _camera;

    [Space]

    public GameObject generationProgressBar;
    public GameObject generationProgressFill;

    [Space]

    public GameObject _generatedDisplay;

    private List<GameObject> _genDisplays = new List<GameObject>();

    private float genDisplayTargetHeight = 1.5f;
    private float genDisplayTweenRate = 2f;

    private float genDisplayLifetime = 1f;
    private float genDisplayFadeTime = 0.5f;

    private UnityEngine.Color genColor;

    Dictionary<GameObject, float> genDisplaysTimeActive = new Dictionary<GameObject, float>();

    [Space]

    public int generationAmount;
    public string generatedResource;

    [Space]

    public float generationInterval;

    float timeSinceGeneration = 0f;

    public int level = 1;

    public int[] cost;

    // Look through the generated resource displays to see if any are available
    private GameObject FindUnusuedDisplay()
    {
        foreach (GameObject display in _genDisplays)
        {
            if (!display.activeSelf)
            {
                return display;
            }
        }

        return null;

    }

    private void CreateGeneratedDisplay()
    {

        // First check for already instantiated displays (object pooling)
        GameObject _newGeneratedDisplay = FindUnusuedDisplay();

        // If none are found, create a new one and add it to the lists
        if (_newGeneratedDisplay == null)
        {
            _newGeneratedDisplay = Instantiate(_generatedDisplay, generationProgressBar.transform);
            genDisplaysTimeActive.Add(_newGeneratedDisplay, 0);
            _genDisplays.Add(_newGeneratedDisplay);
        } else // Otherwise, reactivate the existing one
        {
            _newGeneratedDisplay.transform.SetParent(generationProgressBar.transform);
            _newGeneratedDisplay.transform.localPosition = _generatedDisplay.transform.localPosition;
            _newGeneratedDisplay.SetActive(true);

            genDisplaysTimeActive[_newGeneratedDisplay] = 0f;

            // Update the colour
            _newGeneratedDisplay.GetComponent<TextMeshProUGUI>().color = genColor;
        }

        // Update its text to reflect how many resources were generated
        _newGeneratedDisplay.GetComponent<TextMeshProUGUI>().text = "+"+(generationAmount * level).ToString();

    }

    private void GenerateResources()
    {
        ResourceManager resourceManagerScript = _resourceManager.GetComponent<ResourceManager>();

        // Check what resource was generated, and then generate an amount of that equivalent to the default amount * the current level
        if (generatedResource == "wood")
        {
            resourceManagerScript.wood += generationAmount * level;
        } else if (generatedResource == "stone")
        {
            resourceManagerScript.stone += generationAmount * level;
        }  else if (generatedResource == "food")
        {
            resourceManagerScript.food += generationAmount * level;
        }

        CreateGeneratedDisplay();

    }

    private void Start()
    {
        generationProgressBar.SetActive(true);

        // Get the colour of the generated display
        genColor = _generatedDisplay.GetComponent<TextMeshProUGUI>().color;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceGeneration += Time.deltaTime;

        // Keep track of when to generate more resources
        if (_resourceManager != null)
        {
            
            if (timeSinceGeneration >= generationInterval)
            {
                timeSinceGeneration -= generationInterval;
                GenerateResources();
            }

        }

        // Update the progress bar
        if (generationProgressBar != null)
        {
            // Direction
            generationProgressBar.transform.LookAt(_camera);
            // Do this because the LookAt function makes the bar look away from the camera for some reason
            // So we rotate it around 180 degrees
            generationProgressBar.transform.eulerAngles = generationProgressBar.transform.eulerAngles + 180f * Vector3.forward;
            generationProgressBar.transform.eulerAngles = generationProgressBar.transform.eulerAngles + 180f * Vector3.right;

            // Fill
            generationProgressFill.GetComponent<UnityEngine.UI.Image>().fillAmount = (timeSinceGeneration / generationInterval);
        }

        
    }

    private void FixedUpdate()
    {

        // Update the generated resource displays (the numbers that appear when resources are generated which tell you how many were generated)
        foreach (GameObject display in _genDisplays)
        {

            // Make sure the display is active
            if (display.activeSelf)
            {
                RectTransform rt = display.GetComponent<RectTransform>();

                // Lerp the Y position of the display
                float targetY = Mathf.Lerp(rt.localPosition.y, genDisplayTargetHeight, Time.fixedDeltaTime * genDisplayTweenRate);
                // Update the position to use the new Y position
                rt.localPosition = new Vector3(rt.localPosition.x, targetY, rt.localPosition.z);

                genDisplaysTimeActive[display] += Time.fixedDeltaTime;

                // Fade the display if it has been active for the duration
                if (genDisplaysTimeActive[display] >= genDisplayLifetime)
                {
                    // How transparent the text should be
                    float fadeAmount = (genDisplaysTimeActive[display] - genDisplayLifetime) / genDisplayFadeTime;

                    // Fade the colour based on the default colour, its alpha, and the current fade amount
                    display.GetComponent<TextMeshProUGUI>().color = new UnityEngine.Color(genColor.r, genColor.g, genColor.b, (1 - fadeAmount) * genColor.a);

                    // Destroy the display if it has been active for the duration & fade time
                    if (fadeAmount >= 1)
                    {
                        display.SetActive(false);
                    }
                }
            }
        }
    }

}