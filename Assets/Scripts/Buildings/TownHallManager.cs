using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TownHallManager : MonoBehaviour
{

    public GameObject _buildManager;
    BuildManager buildManager;

    public GameObject _buildLimitProgressDisplay;
    public GameObject _limitUpgradeInfoDisplay;

    public int level = 0;
    public int maxLevel = 3;

    // Build limit per level
    public int[] buildLimits;

    // Cost per upgrade level
    public int[] woodCosts;
    public int[] stoneCosts;
    public int[] foodCosts;

    private void Start()
    {
        buildManager = _buildManager.GetComponent<BuildManager>();

        UpdateBuildingLimit();
    }
    
    // Stolen from order manager from the other game

    public void UpdateBuildingLimit()
    {
        // Update the building limit to that of the new order
        buildManager.buildingLimit = buildLimits[level];

        // Update the building limit & current count UI
        string buildLimitText = "Building Limit: " + buildManager.currentBuildingCount + " / " + buildManager.buildingLimit;
        _buildLimitProgressDisplay.GetComponent<TextMeshProUGUI>().text = buildLimitText;
        
        // Update the display for the next limit
        if (level < maxLevel)
        {
            string nextLimitText = buildLimits[level + 1] + " slots (level: " + (level + 1) + ")";
            _limitUpgradeInfoDisplay.GetComponent<TextMeshProUGUI>().text = nextLimitText;
        } else
        {
            _limitUpgradeInfoDisplay.GetComponent<TextMeshProUGUI>().text = "MAX LEVEL";
        }

    }

}