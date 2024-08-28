using System.Collections;
using System.Collections.Generic;
using System.Resources;
using TMPro;
using UnityEngine;

public class TroopTrainManager : MonoBehaviour
{

    public Transform _camera;

    // Reference to resource manager
    public GameObject _resourceManager;

    [Space]

    public GameObject TrainTroopUI;

    [Space]

    public GameObject _progressRadial;
    ProgressRadialManager progressRadialManager;

    // Upgrade display panel colours
    public Color canAffordCostColor;
    public Color cantAffordCostColor;

    [Space]

    public GameObject currentTroopsLabel;
    public GameObject costLabel;

    [Space]

    public float rayLength = 0f;

    GameObject barracksInUse = null;

    private int trainingCost = 5;

    private void Start()
    {
        TrainTroopUI.SetActive(false);

        progressRadialManager = _progressRadial.GetComponent<ProgressRadialManager>();
    }

    KeyCode trainKey = KeyCode.F;

    private void CheckForTrain(GameObject building)
    {

        BarracksManager barracksManager = building.GetComponent<BarracksManager>();

        // When the upgrade key is pressed, check whether the building should be upgraded
        if (Input.GetKeyDown(trainKey) && barracksManager.currentTroops < barracksManager.maxTroops)
        {
            
            ResourceManager resourceManager = _resourceManager.GetComponent<ResourceManager>();

            // Check whether the player can afford to train a troop
            if (resourceManager.food >= trainingCost)
            {
                // Use the progress radial
                if (progressRadialManager.occupant == null)
                {
                    progressRadialManager.occupant = gameObject;
                    barracksInUse = building;
                }

            }
            
        }

    }
    
    private void TrainTroop(BarracksManager barracksManager, ResourceManager resourceManager)
    {
        // Add the troop to queue
        barracksManager.troopsQueued += 1;
        barracksManager.currentTroops += 1;

        // Deduct the funds from the player
        resourceManager.food -= trainingCost;
    }

    private void ManageProgressRadial(GameObject barracks)
    {
        
        // If using the progress radial
        if (progressRadialManager.occupant == gameObject)
        {

            // Cancel if not hovering over a building
            if (barracks == barracksInUse)
            {

                // Check to see if the upgrade is cancelled
                if (Input.GetKeyUp(trainKey))
                {
                    progressRadialManager.occupant = null;
                }

                // If the progress radial is full, upgrade the building and stop using it
                if (progressRadialManager.progress >= 1f)
                {
                    ResourceManager resourceManager = _resourceManager.GetComponent<ResourceManager>();
                    BarracksManager barracksManager = barracks.GetComponent<BarracksManager>();

                    // Add a troop to queue
                    TrainTroop(barracksManager, resourceManager);

                    progressRadialManager.occupant = null;
                }
            }
            else
            {
                progressRadialManager.occupant = null;
            }

        }
    }
    
        GameObject lastBarracks = null;

    // Update is called once per frame
    void Update()
    {

        RaycastHit ray;

        // Check for barracks
        if (Physics.Raycast(_camera.position, _camera.transform.TransformDirection(Vector3.forward), out ray, rayLength))
        {

            GameObject rayTarget = ray.collider.gameObject;

            // If a barracks is found, update the train troops UI to be visible

            BarracksManager barracksManager = rayTarget.GetComponent<BarracksManager>();

            if (rayTarget && rayTarget.tag == "Building" && barracksManager != null)
            {
                TrainTroopUI.SetActive(true);

                //  UpdateBuildingInfo(rayTarget);

                // Check for training
                CheckForTrain(rayTarget);

                lastBarracks = rayTarget;

                // Update the UI Display
                currentTroopsLabel.GetComponent<TextMeshProUGUI>().text = barracksManager.currentTroops.ToString() + " / " + barracksManager.maxTroops.ToString();

                if (barracksManager.currentTroops < barracksManager.maxTroops)
                {

                    costLabel.GetComponent<TextMeshProUGUI>().text = "Food: " + trainingCost.ToString();

                    if (_resourceManager.GetComponent<ResourceManager>().food >= trainingCost)
                    {
                        costLabel.GetComponent<TextMeshProUGUI>().color = canAffordCostColor;
                    }
                    else
                    {
                        costLabel.GetComponent<TextMeshProUGUI>().color = cantAffordCostColor;
                    }

                } else
                {
                    costLabel.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 1f);
                    costLabel.GetComponent<TextMeshProUGUI>().text = "FULL!";
                }

            } else
            {
                // Otherwise, hide the UI
                TrainTroopUI.SetActive(false);

                lastBarracks = null;
            }

        } else
        {
            // If none are found, hide the UI
            TrainTroopUI.SetActive(false);

            lastBarracks = null;

        }

        // Check on progress radial
        ManageProgressRadial(lastBarracks);

    }
}
