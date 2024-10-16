using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{

    public Transform _camera;

    public static string selectedBuilding = null;
    public static GameObject currentPreview = null;

    // Building Prefabs Array
    public GameObject[] buildingPrefabs;

    // Building Hotbar UI Array
    public RectTransform[] hotbarUI;

    // Building Prefabs Dictionary
    // Connects prefabs to building string identifiers
    Dictionary<string, GameObject> buildingPrefabDict = new Dictionary<string, GameObject>();

    // Building Keycodes Dictionary
    Dictionary<string, KeyCode> buildingKeycodes = new Dictionary<string, KeyCode>();

    // Parameters

    // Preview offset
    Vector3 offset = new Vector3(0, 0, 8f);

    // Building height
    float buildingYPos = 4f;

    // Preview lerp speed
    float lerpSpeed = 25f;

    float previewOpacity = 0.75f;

    // Blacklisted Tags
    // If the preview is colliding with any of these tags,
    // then it cannot be built
    public string[] blacklistedTags;

    [Space]

    // Reference to resource manager
    public GameObject _resourceManager;

    // Reference to the buildings gameobject
    public Transform _buildingsParent;

    public GameObject blacksmith;
    BlacksmithManager blacksmithManager;

    [Space]

    // Reference to cost display panel
    public GameObject _costDisplayPanel;

    public GameObject _woodCostDisplay;
    public GameObject _stoneCostDisplay;
    public GameObject _foodCostDisplay;

    [Space]

    // Cost display panel colours
    public UnityEngine.Color canAffordCDPColor;
    public UnityEngine.Color cantAffordCDPColor;

    [Space]

    public int buildingLimit;
    public int currentBuildingCount = 0;

    [Space]

    public Transform _canvas;
    public GameObject _progressRadial;
    public GameObject _alertUI;
    public GameObject _buildLimitProgressDisplay;

    [Space]

    public Transform troopParent;

    UnityEngine.Color previewColor;

    // PARAMETERS & SETTINGS !!!
    // Where certain stats are set
    // Unity doesn't have good support for public dictionaries
    // So everything is just added to the dictionaries at runtime
    private void Start()
    {
        // Building Keycodes

        buildingKeycodes.Add("Sawmill", KeyCode.Alpha1);
        buildingKeycodes.Add("Mine", KeyCode.Alpha2);
        buildingKeycodes.Add("Farm", KeyCode.Alpha3);
        buildingKeycodes.Add("Barracks", KeyCode.Alpha4);
        buildingKeycodes.Add("Barricade", KeyCode.Alpha5);

        // Reference to which building prefab relates to what index in the public prefab array
        // e.g. if the sawmill is the first item in the prefab array,
        // set the sawmill item in the prefab dictionary to be the first item in the array

        buildingPrefabDict.Add("Sawmill", buildingPrefabs[0]);
        buildingPrefabDict.Add("Mine", buildingPrefabs[1]);
        buildingPrefabDict.Add("Farm", buildingPrefabs[2]);
        buildingPrefabDict.Add("Barracks", buildingPrefabs[3]);
        buildingPrefabDict.Add("Barricade", buildingPrefabs[4]);

        blacksmithManager = blacksmith.GetComponent<BlacksmithManager>();

    }

    // The tag of the building currently shown as a preview
    // The tag is overwritten to "Building Preview" when placement begins so we need to keep track of what it was originally
    string previewBuildingTag = "";

    // Update cost display
    private void UpdateCostDisplay()
    {

        if (_costDisplayPanel != null)
        {

            ResourceManager resourceManagerScript = _resourceManager.GetComponent<ResourceManager>();

            // Only proceed if a building is selected
            if (selectedBuilding != null && currentPreview != null)
            {

                ResourceGenerator resourceGenerator = currentPreview.GetComponent<ResourceGenerator>();

                // Show the cost display panel
                _costDisplayPanel.SetActive(true);

                int[] cost = {-1, -1, -1};

                if (resourceGenerator != null)
                {
                    cost = resourceGenerator.cost;
                } else
                {
                    BarracksManager barracksManager = currentPreview.GetComponent<BarracksManager>();
                    if (barracksManager != null)
                    {
                        cost = barracksManager.cost;
                    }
                }

                if (_woodCostDisplay != null)
                {
                    // Update the text display for wood
                    _woodCostDisplay.GetComponent<TextMeshProUGUI>().text = "Wood: " + cost[0].ToString();

                    // Check whether or not there is enough of this resource to build this building
                    // If so, set the text colour to green, otherwise set it to red
                    if (resourceManagerScript.wood >= cost[0])
                    {
                        _woodCostDisplay.GetComponent<TextMeshProUGUI>().color = canAffordCDPColor;
                    } else
                    {
                        _woodCostDisplay.GetComponent<TextMeshProUGUI>().color = cantAffordCDPColor;
                    }
                }

                if (_stoneCostDisplay != null)
                {
                    // Update the text display for stone
                    _stoneCostDisplay.GetComponent<TextMeshProUGUI>().text = "Stone: " + cost[1].ToString();

                    // Check whether or not there is enough of this resource to build this building
                    // If so, set the text colour to green, otherwise set it to red
                    if (resourceManagerScript.stone >= cost[1])
                    {
                        _stoneCostDisplay.GetComponent<TextMeshProUGUI>().color = canAffordCDPColor;
                    }
                    else
                    {
                        _stoneCostDisplay.GetComponent<TextMeshProUGUI>().color = cantAffordCDPColor;
                    }
                }

                if (_foodCostDisplay != null)
                {
                    // Update the text display for food
                    _foodCostDisplay.GetComponent<TextMeshProUGUI>().text = "Food: " + cost[2].ToString();

                    // Check whether or not there is enough of this resource to build this building
                    // If so, set the text colour to green, otherwise set it to red
                    if (resourceManagerScript.food >= cost[2])
                    {
                        _foodCostDisplay.GetComponent<TextMeshProUGUI>().color = canAffordCDPColor;
                    }
                    else
                    {
                        _foodCostDisplay.GetComponent<TextMeshProUGUI>().color = cantAffordCDPColor;
                    }
                }


            } else
            {
                // If nothing is selected, hide the cost display panel
                _costDisplayPanel.SetActive(false);
            }

        }

    }

    private void UpdateBuildingColour(UnityEngine.Color colour, bool setChildrenWhite)
    {
        // Update preview transparency
        currentPreview.GetComponent<MeshRenderer>().material.color = new UnityEngine.Color(colour.r, colour.g, colour.b, colour.a);

        // Update the transparency of all children of the preview
        // As well as all children of those children
        // https://stackoverflow.com/questions/37943729/get-all-children-children-of-children-in-unity3d

        foreach (Transform child in currentPreview.GetComponentInChildren<Transform>())
        {
            if (child.GetComponent<MeshRenderer>() != null)
            {
                // Update transparency if possible
                if (setChildrenWhite)
                {
                    child.GetComponent<MeshRenderer>().material.color = new UnityEngine.Color(1, 1, 1, colour.a);
                } else
                {
                    child.GetComponent<MeshRenderer>().material.color = new UnityEngine.Color(colour.r, colour.g, colour.b, colour.a);
                }
            }

            // Update the transparency of any children of that child
            foreach (Transform child2 in child.GetComponentInChildren<Transform>())
            {
                if (child2.GetComponent<MeshRenderer>() != null)
                {
                    if (setChildrenWhite)
                    {
                        child2.GetComponent<MeshRenderer>().material.color = new UnityEngine.Color(1, 1, 1, colour.a);
                    } else
                    {
                        child2.GetComponent<MeshRenderer>().material.color = new UnityEngine.Color(colour.r, colour.g, colour.b, colour.a);
                    }
                }

            }

        }
    }

    // Update Selected Building Function
    // If any keys related to any buildings are pressed, update the selected building variable
    private void UpdateSelectedBuilding()
    {
        if (FriendlyAIDirector.isDirecting)
            return;

        int i = 0;

        foreach (string building in buildingKeycodes.Keys)
        {

            //  Building Placement
            if (Input.GetKeyDown(buildingKeycodes[building]))
            {

                if (currentBuildingCount < buildingLimit)
                {

                    // Destroy the current preview prefab
                    Destroy(currentPreview);

                    // Update the new prefab
                    if (selectedBuilding == building)
                    {
                        selectedBuilding = null;
                        currentPreview = null;

                        // Update the hotbar

                        foreach (RectTransform panel in hotbarUI)
                        {
                            panel.GetComponent<Image>().color = new UnityEngine.Color(0, 0, 0, 100f / 255f);
                        }

                    }
                    else
                    {

                        // Update the hotbar
                        int j = 0;

                        foreach (RectTransform panel in hotbarUI)
                        {

                            if (j != i)
                            {
                                // Deselected
                                panel.GetComponent<Image>().color = new UnityEngine.Color(0, 0, 0, 100f / 255f);
                            }
                            else
                            {
                                // Selected
                                panel.GetComponent<Image>().color = new UnityEngine.Color(1, 1, 1, 100f / 255f);
                            }

                            j++;

                        }

                        // Create preview object
                        selectedBuilding = building;
                        currentPreview = Instantiate(buildingPrefabDict[building]);

                        currentPreview.transform.parent = _buildingsParent;

                        // Update preview collisionUpdatePreviewCollision(false);
                        UpdatePreviewCollision(false);

                        // Set position to be in front of the player
                        UpdatePreviewPosition(false);

                        previewColor = currentPreview.GetComponent<MeshRenderer>().material.color;
                        previewColor.a = previewOpacity;

                        UpdateBuildingColour(previewColor, true);

                        SetBuildingMode(false);

                        // Update preview name (otherwise it leaves '(clone)' at the end)
                        currentPreview.name = buildingPrefabDict[building].name;

                        // Update preview tag
                        previewBuildingTag = currentPreview.tag;
                        currentPreview.tag = "BuildingPreview";

                        // Disaable the resource generator
                        ResourceGenerator resourceGenerator = currentPreview.GetComponent<ResourceGenerator>();
                        if (resourceGenerator != null) {
                            resourceGenerator.enabled = false;

                            // Hide the progress bar
                            if (resourceGenerator.generationProgressBar != null)
                            {
                                resourceGenerator.generationProgressBar.SetActive(false);
                            }
                        }
                    }
                } else // If there are an insufficient number of slots, alert the player
                {
                    // Only create an alert if not on cooldown
                    if (timeSinceLastAlert >= alertCooldown)
                    {
                        GameObject slotsAlert = Instantiate(_alertUI, _canvas);
                        // Reset the cooldown
                        timeSinceLastAlert = 0f;
                    }
                }

            }

             i++;

            }
    }

    float gridSize = 5f;

    float xGridMax = 175f;
    float xGridMin = 100f;
    float zGridMax = 170f;
    float zGridMin = 100f;

    float xInnerGridMax = 163f;
    float xInnerGridMin = 112f;
    float zInnerGridMax = 158f;
    float zInnerGridMin = 112f;

    // Update prefab position function
    // Updates the prefab to be in front of the camera
    private void UpdatePreviewPosition(bool shouldLerp)
    {
        if (currentPreview != null)
        {
            // Set the target position
            Vector3 targetPosition = _camera.position + _camera.forward * offset.z;

            // Round the target position
            targetPosition = new Vector3(
                Mathf.Round(targetPosition.x / gridSize) * gridSize,
                buildingYPos,
                Mathf.Round(targetPosition.z / gridSize) * gridSize
                );

            // Lerp the transform of the preview to the target position
            if (shouldLerp)
            {
                currentPreview.transform.position = Vector3.Lerp(currentPreview.transform.position, targetPosition, Time.deltaTime * lerpSpeed);
            } else
            {
                currentPreview.transform.position = targetPosition;
            }
        }
    }

    // Update preview collision function
    // Sets the collision of each of the building's children to the desired value
    // E.G. If the value is false, collision is disabled

    private void UpdatePreviewCollision(bool collisionsEnabled)
    {
        
        currentPreview.GetComponent<Collider>().enabled = collisionsEnabled;

        foreach (Transform child in currentPreview.GetComponentInChildren<Transform>())
        {

            // Check for box colliders
            BoxCollider childBoxCollider = child.GetComponent<BoxCollider>();

            if (childBoxCollider != null)
            {
                childBoxCollider.enabled = collisionsEnabled;
            }

            // Check for mesh colliders
            MeshCollider childMeshCollider = child.GetComponent<MeshCollider>();

            if (childMeshCollider != null)
            {
                childMeshCollider.enabled = collisionsEnabled;
            }

            // Repeat for all children of children

            foreach (Transform child2 in child.GetComponentInChildren<Transform>())
            {

                // Check for box colliders
                BoxCollider child2BoxCollider = child2.GetComponent<BoxCollider>();

                if (child2BoxCollider != null)
                {
                    child2BoxCollider.enabled = collisionsEnabled;
                }

                // Check for mesh colliders
                MeshCollider child2MeshCollider = child2.GetComponent<MeshCollider>();

                if (child2MeshCollider != null)
                {
                    child2MeshCollider.enabled = collisionsEnabled;
                }

            }

        }

        // Also toggle the nav mesh obstacle component of the building
        currentPreview.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = collisionsEnabled;
        
    }

    // https://forum.unity.com/threads/change-rendering-mode-via-script.476437/
    void ToOpaqueMode(Material material)
    {
        material.SetOverrideTag("RenderType", "");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = -1;
    }

    void ToFadeMode(Material material)
    {
        material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    private bool CheckForBuildingsInPreview()
    {

        // https://discussions.unity.com/t/getting-a-list-of-gameobjects-in-a-collider/230927

        BoxCollider previewCollider = currentPreview.GetComponent<BoxCollider>();

        Vector3 colliderSize = new Vector3(((previewCollider.size.x * currentPreview.transform.localScale.x) / 2),
            ((previewCollider.size.y * currentPreview.transform.localScale.y) / 2),
            ((previewCollider.size.z * currentPreview.transform.localScale.z) / 2));

        Collider[] hitColliders = Physics.OverlapBox(previewCollider.transform.position, 
            colliderSize, Quaternion.identity);

        foreach (Collider colliderInPreviewCollider in hitColliders)
        {
            if (colliderInPreviewCollider != previewCollider)
            {
                foreach (string tag in blacklistedTags)
                {
                    if (colliderInPreviewCollider.gameObject.tag == tag)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    // Return whether or not resources are sufficient for the current building
    private bool CheckResourceSufficiency()
    {

        if (currentPreview != null && selectedBuilding != null)
        {

            int woodRequired = -1;
            int stoneRequired = -1;
            int foodRequired = -1;

            ResourceGenerator resourceGenerator = currentPreview.GetComponent<ResourceGenerator>();

            if (resourceGenerator != null)
            {
                woodRequired = resourceGenerator.cost[0];
                stoneRequired = resourceGenerator.cost[1];
                foodRequired = resourceGenerator.cost[2];
            } else
            {
                BarracksManager barracksManager = currentPreview.GetComponent<BarracksManager>();

                if (barracksManager != null)
                {
                    woodRequired = barracksManager.cost[0];
                    stoneRequired = barracksManager.cost[1];
                    foodRequired = barracksManager.cost[2];
                }
            }

            ResourceManager resourceManagerScript = _resourceManager.GetComponent<ResourceManager>();

            return (woodRequired <= resourceManagerScript.wood && stoneRequired <= resourceManagerScript.stone && foodRequired <= resourceManagerScript.food);

        }

        return false;
    }

    // TODO: The following function would ussually update the transparency of the building preview
    // It would do this with the ToOpaqueMode and ToFadeMode functions
    // However, these are currently disabled due to complications with setting transparency with HDRP
    // i.e. idk how to do so, so I'll need to fix that at some point.

    private void SetBuildingMode(bool opaque)
    {

        /*

        // Update preview transparency
        if (opaque)
        {
            ToOpaqueMode(currentPreview.GetComponent<MeshRenderer>().material);
        } else
        {
            ToFadeMode(currentPreview.GetComponent<MeshRenderer>().material);
        }

        // Update the transparency of all children of the preview
        // As well as all children of those children
        // https://stackoverflow.com/questions/37943729/get-all-children-children-of-children-in-unity3d

        foreach (Transform child in currentPreview.GetComponentInChildren<Transform>())
        {
            if (child.GetComponent<MeshRenderer>() != null)
            {
                // Update transparency if possible
                if (opaque)
                {
                    ToOpaqueMode(child.GetComponent<MeshRenderer>().material);
                } else
                {
                    ToFadeMode(child.GetComponent<MeshRenderer>().material);
                }
            }

            // Update the transparency of any children of that child
            foreach (Transform child2 in child.GetComponentInChildren<Transform>())
            {
                if (child2.GetComponent<MeshRenderer>() != null)
                {
                    if (opaque)
                    {
                        ToOpaqueMode(child2.GetComponent<MeshRenderer>().material);
                    } else
                    {
                        ToFadeMode(child2.GetComponent<MeshRenderer>().material);
                    }
                }
            }
        }

        */
    }

    private void CheckForPlacement()
    {

        // Make sure a prefab is selected
        if (currentPreview != null && selectedBuilding != null) {

            int woodRequired = -1;
            int stoneRequired = -1;
            int foodRequired = -1;

            ResourceGenerator resourceGenerator = currentPreview.GetComponent<ResourceGenerator>();

            if (resourceGenerator != null)
            {
                woodRequired = resourceGenerator.cost[0];
                stoneRequired = resourceGenerator.cost[1];
                foodRequired = resourceGenerator.cost[2];
            }
            else
            {
                BarracksManager barracksManager = currentPreview.GetComponent<BarracksManager>();

                if (barracksManager != null)
                {
                    woodRequired = barracksManager.cost[0];
                    stoneRequired = barracksManager.cost[1];
                    foodRequired = barracksManager.cost[2];
                    barracksManager.troopParent = troopParent;
                    barracksManager.blacksmithManager = blacksmithManager;
                }
            }

            ResourceManager resourceManagerScript = _resourceManager.GetComponent<ResourceManager>();

            Vector3 originalPosition = currentPreview.transform.position;

            // Set the preview position to be precisely on grid
            UpdatePreviewPosition(false);

            // Only place the building IF;
            // Left mouse button is down, no objects are within the preview's collider, and resources are sufficient

            if (Input.GetMouseButtonDown(0) && !CheckForBuildingsInPreview() && CheckResourceSufficiency() && ValidateBuildingPosition())
            {

                // Enable the resource generator
                if (resourceGenerator != null)
                {
                    resourceGenerator.enabled = true;
                }

                // Update material render mode to opaque
                //ToOpaqueMode(currentPreview.GetComponent<MeshRenderer>().material);
                SetBuildingMode(true);

                // Update the colour
                UnityEngine.Color currentColour = new UnityEngine.Color(previewColor.r, previewColor.g, previewColor.b, previewOpacity);
                UpdateBuildingColour(currentColour, true);

                // Remove the relevant resources
                resourceManagerScript.wood -= woodRequired;
                resourceManagerScript.stone -= stoneRequired;
                resourceManagerScript.food -= foodRequired;

                // Set the reference to the resource manager if the new building needs it
                if (currentPreview.GetComponent<ResourceGenerator>() != null)
                {
                    resourceGenerator._resourceManager = _resourceManager;
                }

                // Set the reference to the camera
                if (resourceGenerator != null)
                {
                    resourceGenerator._camera = _camera;
                }

                // Update the building's tag
                currentPreview.tag = previewBuildingTag;

                // Enable the building's collisions
                UpdatePreviewCollision(true);

                // Clear references to the preview
                currentPreview = null;
                selectedBuilding = null;

                // Add one to the current building count
                currentBuildingCount += 1;

                // Update the hotbar

                foreach (RectTransform panel in hotbarUI)
                {
                    panel.GetComponent<Image>().color = new UnityEngine.Color(0, 0, 0, 100f / 255f);
                }

                // Update the building limit & current count UI
                string buildLimitText = "Building Limit: " + currentBuildingCount + " / " + buildingLimit;
                _buildLimitProgressDisplay.GetComponent<TextMeshProUGUI>().text = buildLimitText;

                // Play the build sound
                AudioSource SFX = GetComponent<AudioSource>();
                SFX.pitch = Random.Range(0.5f, 1f);

                SFX.Play();

            }
            else
            {
                // Revert the exact grid placement of the preview
                currentPreview.transform.position = originalPosition;

                // Update current building preview position
                UpdatePreviewPosition(true);
            }
        }
    }

    private bool ValidateBuildingPosition()
    {

        // Get the position of the current preview
        Vector3 targetPosition = _camera.position + _camera.forward * offset.z;

        // Round the target position
        targetPosition = new Vector3(
            Mathf.Round(targetPosition.x / gridSize) * gridSize,
            buildingYPos,
            Mathf.Round(targetPosition.z / gridSize) * gridSize
            );

        bool isWall = (previewBuildingTag == "Barricade");

        // Check if the building is within the bounds of the player's plot
        if (targetPosition.x > xGridMax 
            | targetPosition.x < xGridMin
            | targetPosition.z > zGridMax
            | targetPosition.z < zGridMin
            )
        {
            return false;
        } else
        {

            bool insideInnerBounds =
            !(targetPosition.x > xInnerGridMax
            | targetPosition.x < xInnerGridMin
            | targetPosition.z > zInnerGridMax
            | targetPosition.z < zInnerGridMin
            );

            // In this context, != is used as an XOR operator
            // If the building is in the inner bounds and is not a wall, the placement is valid
            // If the building is a wall and is outside the bounds, then it is also valid
            if (isWall != insideInnerBounds)
                return true;
            else
                return false;

        }
    }

    // Updates the colour of the preview based on whether or not the prefab can be placed
    private void UpdatePreviewColor()
    {

        if (currentPreview != null)
        {
            if (!CheckForBuildingsInPreview() && CheckResourceSufficiency() && ValidateBuildingPosition())
            {
                // If there are currently no objects inside of the preview AND resources are sufficient, set the colour to the default
                UnityEngine.Color currentColour = new UnityEngine.Color(previewColor.r, previewColor.g, previewColor.b, previewOpacity);


                UpdateBuildingColour(currentColour, true);
            }
            else
            {
                // Otherwise, set the colour to red
                UnityEngine.Color currentColour = new UnityEngine.Color(1f, 0f, 0f, previewOpacity);

                UpdateBuildingColour(currentColour, false);
            }
        }

    }
    
    float timePassed = 0f;
    /*
    float transparencyOscillationPeriod = 0.5f;
    float transparencyOscillationAmplitude = 0.2f;
    float transparencyOffset = 0.5f;
    */
        float alertCooldown = 0.25f;
    float timeSinceLastAlert = 100f;


    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        timeSinceLastAlert += Time.deltaTime;

        // Update the currently selected building type
        UpdateSelectedBuilding();

        UpdatePreviewColor();

        CheckForPlacement();

        // DISABLED until I can figure out how to get dynamic transparency working again :/
        // Update the preview opacity using a sine wave
        // Makes it look a little nicer and lively
        //previewOpacity = Mathf.Sin(timePassed / transparencyOscillationPeriod) * transparencyOscillationAmplitude + transparencyOffset;

        UpdateCostDisplay();

    }
}
