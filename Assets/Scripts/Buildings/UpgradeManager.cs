using System.Collections;
using System.Collections.Generic;
using System.Resources;
using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{

    public Transform _camera;

    // Reference to resource manager
    public GameObject _resourceManager;

    [Space]

    public GameObject UpgradePanelUI;

    public GameObject InfoDisplay;

    [Space]

    public GameObject _woodCostDisplay;
    public GameObject _stoneCostDisplay;
    public GameObject _foodCostDisplay;

    [Space]

    public GameObject _progressRadial;
    ProgressRadialManager progressRadialManager;

    // Upgrade display panel colours
    public Color canAffordCostColor;
    public Color cantAffordCostColor;

    [Space]

    public float rayLength = 0f;

    GameObject buildingUpgrading = null;

    private void Start()
    {
        UpgradePanelUI.SetActive(false);

        progressRadialManager = _progressRadial.GetComponent<ProgressRadialManager>();
    }

    private int[] GetUpgradeCost(GameObject building)
    {
        ResourceGenerator resourceGenerator = building.GetComponent<ResourceGenerator>();

        int[] cost = resourceGenerator.cost;
        
        int[] newCost = {0, 0, 0};

        // Update the cost based on the level

        for (int i = 0; i < cost.Length; i++)
        {
            newCost[i] = (int)Mathf.Floor(cost[i] * Mathf.Pow(2f, resourceGenerator.level)); // Formula for building upgrade costs (cost = base cost * (2^level))
        }

        return newCost;
    }

    // Updates the building upgrade UI
    private void UpdateBuildingInfo(GameObject building)
    {

        // Get the building's resource generator script
        ResourceGenerator resourceGenerator = building.GetComponent<ResourceGenerator>();

        int level = resourceGenerator.level;
        string name = building.name;

        // Construct the text to be display on the UI
        string InfoDisplayText = "Upgrade " + name + " to level: " + (level + 1).ToString() + " (F)";

        // Set the text
        InfoDisplay.GetComponent<TextMeshProUGUI>().text = InfoDisplayText;

        // Get the cost of upgrading
        int[] cost = GetUpgradeCost(building);

        // Update the UI to display that cost
        _woodCostDisplay.GetComponent<TextMeshProUGUI>().text = "Wood: " + cost[0];
        _stoneCostDisplay.GetComponent<TextMeshProUGUI>().text = "Stone: " + cost[1];
        _foodCostDisplay.GetComponent<TextMeshProUGUI>().text = "Food: " + cost[2];

        // Update the colour of said UI

        ResourceManager resourceManagerScript = _resourceManager.GetComponent<ResourceManager>();

        // Check whether or not there is enough of this resource to upgrade this building
        // If so, set the text colour to green, otherwise set it to red

        // Wood
        if (resourceManagerScript.wood >= cost[0])
        {
            _woodCostDisplay.GetComponent<TextMeshProUGUI>().color = canAffordCostColor;
        }
        else
        {
            _woodCostDisplay.GetComponent<TextMeshProUGUI>().color = cantAffordCostColor;
        }

        // Stone
        if (resourceManagerScript.stone >= cost[1])
        {
            _stoneCostDisplay.GetComponent<TextMeshProUGUI>().color = canAffordCostColor;
        }
        else
        {
            _stoneCostDisplay.GetComponent<TextMeshProUGUI>().color = cantAffordCostColor;
        }

        // Food
        if (resourceManagerScript.food >= cost[2])
        {
            _foodCostDisplay.GetComponent<TextMeshProUGUI>().color = canAffordCostColor;
        }
        else
        {
            _foodCostDisplay.GetComponent<TextMeshProUGUI>().color = cantAffordCostColor;
        }
        
    }

    private void UpgradeSuccess(ResourceManager resourceManager, GameObject building)
    {

        ResourceGenerator resourceGenerator = building.GetComponent<ResourceGenerator>();

        int[] cost = GetUpgradeCost(building);

        // If so, upgrade the building
        resourceGenerator.level += 1;

        // Subtract the cost from the player's resources

        resourceManager.wood -= cost[0];
        resourceManager.stone -= cost[1];
        resourceManager.food -= cost[2];

        // Play the build sound
        AudioSource SFX = GetComponent<AudioSource>();
        SFX.pitch = Random.Range(0.5f, 1f);

        SFX.Play();
    }

    KeyCode upgradeKey = KeyCode.F;

    private void CheckForUpgrade(GameObject building)
    {

        // When the upgrade key is pressed, check whether the building should be upgraded
        if (Input.GetKeyDown(upgradeKey))
        {

            int[] cost = GetUpgradeCost(building);

            ResourceManager resourceManager = _resourceManager.GetComponent<ResourceManager>();

            // Check whether each resource cost can be afforded
            if (resourceManager.wood >= cost[0] && resourceManager.stone >= cost[1] && resourceManager.food >= cost[2])
            {

                // Use the progress radial
                if (progressRadialManager.occupant == null)
                {
                    progressRadialManager.occupant = gameObject;
                    buildingUpgrading = building;
                }

            }
        }

    }

    private void ManageProgressRadial(GameObject building)
    {

        // If using the progress radial
        if (progressRadialManager.occupant == gameObject)
        {

            // Cancel if not hovering over a building
            if (building == buildingUpgrading)
            {

                // Check to see if the upgrade is cancelled
                if (Input.GetKeyUp(upgradeKey))
                {
                    progressRadialManager.occupant = null;
                }

                // If the progress radial is full, upgrade the building and stop using it
                if (progressRadialManager.progress >= 1f)
                {
                    ResourceManager resourceManager = _resourceManager.GetComponent<ResourceManager>();

                    UpgradeSuccess(resourceManager, building);

                    progressRadialManager.occupant = null;
                }
            }
            else
            {
                progressRadialManager.occupant = null;
            }

        }
    }

    GameObject lastBuilding = null;

    // Update is called once per frame
    void Update()
    {

        RaycastHit ray;

        // Check for buildings
        if (Physics.Raycast(_camera.position, _camera.transform.TransformDirection(Vector3.forward), out ray, rayLength))
        {

            GameObject rayTarget = ray.collider.gameObject;

            // If a building is found, update the upgrade UI to be visible
            if (rayTarget && rayTarget.tag == "Building")
            {
                UpgradePanelUI.SetActive(true);

                UpdateBuildingInfo(rayTarget);

                // Check for upgrade
                CheckForUpgrade(rayTarget);

                lastBuilding = rayTarget;

            } else
            {
                // Otherwise, hide the UI
                UpgradePanelUI.SetActive(false);

                lastBuilding = null;
            }

        } else
        {
            // If none are found, hide the UI
            UpgradePanelUI.SetActive(false);

            lastBuilding = null;

        }

        // Check on progress radial
        ManageProgressRadial(lastBuilding);

    }
}
