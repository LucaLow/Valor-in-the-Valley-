using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System;
using TMPro;
using UnityEngine;

/*
This code is admittedly a little bit of a mess as a result of how it worked in the original project.

This could be improved significantly if the buildings used a base prefab,
but we're a bit too far into developement to rewrite the code to such a degree,
and this should work fine as long as we don't add too many more building types.
*/

public class UpgradeManager : MonoBehaviour
{

    public Transform _camera;

    // Reference to resource manager
    public GameObject _resourceManager;

    [Space]

    public GameObject UpgradePanelUI;
    public GameObject InfoDisplay;

    [Space]

    public string[] buildingTags = { "Building", "Town Hall", "Blacksmith", "Barricade" };

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

    public Transform _canvas;
    public GameObject _alertUI;

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
        TownHallManager townHallManager = building.GetComponent<TownHallManager>();
        BlacksmithManager blacksmithManager = building.GetComponent<BlacksmithManager>();

        int[] cost = {10, 10, 10};

        if (resourceGenerator != null)
        {
            cost = resourceGenerator.cost;

            int[] newCost = { 0, 0, 0 };

            // Update the cost based on the level

            for (int i = 0; i < cost.Length; i++)
            {
                if (resourceGenerator != null)
                {
                    newCost[i] = (int)Mathf.Floor(cost[i] * Mathf.Pow(2f, resourceGenerator.level)); // Formula for building upgrade costs (cost = base cost * (2^level))
                }
            }

            return newCost;
        }
        else if (townHallManager != null)
        {
            int level = townHallManager.level;

            if (level < townHallManager.maxLevel)
            {
                cost = new int[] { townHallManager.woodCosts[level], townHallManager.stoneCosts[level], townHallManager.foodCosts[level] };
                return cost;
            }
        }
        else if (blacksmithManager != null)
        {
            int level = blacksmithManager.level;

            if (level < blacksmithManager.maxLevel)
            {
                cost = new int[] {blacksmithManager.woodCostPerLevel[level], blacksmithManager.stoneCostPerLevel[level], blacksmithManager.foodCostPerLevel[level] };
                return cost;
            }
        }

        return null;

    }

    // Updates the building upgrade UI
    private void UpdateBuildingInfo(GameObject building)
    {

        // Get the building's resource generator script
        ResourceGenerator resourceGenerator = building.GetComponent<ResourceGenerator>();

        string name = building.name;
        int level = 1;

        // Get the level of the building
        if (resourceGenerator != null)
        {
            level = resourceGenerator.level;
        } else
        {

            TownHallManager townHallManager = building.GetComponent<TownHallManager>();
            BlacksmithManager blacksmithManager = building.GetComponent<BlacksmithManager>();

            if (townHallManager != null)
            {
                level = townHallManager.level;
            } else if(blacksmithManager != null) {
                level = blacksmithManager.level;
            }

        }

        // Construct the text to be displayed on the UI
        string InfoDisplayText = "Upgrade " + name + " to level: " + (level + 1).ToString() + " (F)";

        // Set the text
        InfoDisplay.GetComponent<TextMeshProUGUI>().text = InfoDisplayText;

        // Get the cost of upgrading
        int[] cost = GetUpgradeCost(building);

        if (cost != null)
        {

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

        } else
        {
            UpgradePanelUI.SetActive(false);
        }
        
    }

    private void UpgradeSuccess(ResourceManager resourceManager, GameObject building)
    {

        ResourceGenerator resourceGenerator = building.GetComponent<ResourceGenerator>();
        TownHallManager townHallManager = building.GetComponent<TownHallManager>();
        BlacksmithManager blacksmithManager = building.GetComponent<BlacksmithManager>();

        int[] cost = GetUpgradeCost(building);

        // If so, upgrade the building
        if (resourceGenerator!= null)
        {
            resourceGenerator.level += 1;
        } else if (townHallManager != null)
        {
            townHallManager.level += 1;
            townHallManager.UpdateBuildingLimit();
        } else if (blacksmithManager != null)
        {
            blacksmithManager.level += 1;
            blacksmithManager.UpdateTroopStats();

            // Inform the player of their troops' new health when upgrading the blacksmith
            GameObject slotsAlert = Instantiate(_alertUI, _canvas);
            int newHealth = blacksmithManager.healthPerLevel[blacksmithManager.level] - blacksmithManager.healthPerLevel[blacksmithManager.level - 1];
            slotsAlert.GetComponent<TextMeshProUGUI>().text = "+"+ newHealth +" Troop Health ("+ blacksmithManager.healthPerLevel[blacksmithManager.level] +")";
            slotsAlert.GetComponent<TextMeshProUGUI>().color = new Color(0f, 1f, 0f, 1f);
        }

        // Upgrade the max health of the building if it is a barricade
        // Also set the health to max (i.e. fully repair it)

        BuildingHealth _BuildingHealthManager = building.GetComponent<BuildingHealth>();

        if (_BuildingHealthManager != null)
        {

            if (building.tag == "Barricade")
            {

                if (_BuildingHealthManager != null)
                {

                    _BuildingHealthManager.maxHealth += 200;

                }
            }

        }

        _BuildingHealthManager.health = _BuildingHealthManager.maxHealth;

        // Subtract the cost from the player's resources

        resourceManager.wood -= cost[0];
        resourceManager.stone -= cost[1];
        resourceManager.food -= cost[2];

        // Play the build sound
        AudioSource SFX = GetComponent<AudioSource>();
        SFX.pitch = UnityEngine.Random.Range(0.5f, 1f);

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

            if (cost != null)
            {
                
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
                if (building == null)
                {
                    progressRadialManager.occupant = null;
                }
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
            if (rayTarget && Array.IndexOf(buildingTags, rayTarget.tag) > -1) //buildingTags.Contains(rayTarget.tag))
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
