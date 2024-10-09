using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TownHallManager : MonoBehaviour
{
    // Singleton
    public static TownHallManager Instance { get; private set; }

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

    [Space]

    // Alert UI related objects
    public Transform _canvas;
    public GameObject _alertUI;

    bool displayNewLimit = false;

    private void Awake()
    {
        Instance = this;
    }

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

        // Create an alert to notify the player of how many slots were gained
        if (displayNewLimit)
        {
            GameObject slotsAlert = Instantiate(_alertUI, _canvas);

            int slotsGained = buildLimits[level] - buildLimits[level - 1];
            slotsAlert.GetComponent<TextMeshProUGUI>().text = "+" + slotsGained + " building slots unlocked!";
            slotsAlert.GetComponent<TextMeshProUGUI>().color = new Color(0, 1f, 0, 1f);
        }

        displayNewLimit = true;

    }

}