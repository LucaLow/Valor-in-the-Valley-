using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BarracksManager : MonoBehaviour
{

    // Placement cost
    public int[] cost;

    public GameObject troopPrefab;
    public Transform troopSpawnPoint;
    public Transform troopParent;

    public BlacksmithManager blacksmithManager;

    [Space]

    public int maxTroops;
    public int currentTroops = 0;

    public int troopsQueued = 0;

    public Transform _camera { get; set; }

    public List<GameObject> spawnedTroops = new List<GameObject>();

    [Space]

    public GameObject progressBar;
    public GameObject generationProgressFill;
    public GameObject queueDisplay;

    float timeTraining = 0f;
    float trainTime = 5f;

    private void SpawnTroop()
    {
        GameObject newTroop = Instantiate(troopPrefab, troopSpawnPoint.position, Quaternion.identity, troopParent);

        int newHealth = blacksmithManager.healthPerLevel[blacksmithManager.level];
        newTroop.GetComponent<Health>().maxHealth = newHealth;
        newTroop.GetComponent<Health>().health = newHealth;

        spawnedTroops.Add(newTroop);
    }

    void Start()
    {
        _camera = Camera.main.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        CheckForDeadTroops();

        if (troopsQueued > 0)
        {
            timeTraining += Time.fixedDeltaTime;

            if (timeTraining > trainTime)
            {
                timeTraining -= trainTime;
                troopsQueued -= 1;
                SpawnTroop();
            }

            // Update Progress Display
            progressBar.SetActive(true);
            queueDisplay.GetComponent<TextMeshProUGUI>().text = "Queued: " + troopsQueued;
            generationProgressFill.GetComponent<UnityEngine.UI.Image>().fillAmount = timeTraining / trainTime;

            // Direction
            progressBar.transform.LookAt(_camera);
            // Do this because the LookAt function makes the bar look away from the camera for some reason
            // So we rotate it around 180 degrees
            progressBar.transform.eulerAngles = progressBar.transform.eulerAngles + 180f * Vector3.forward;
            progressBar.transform.eulerAngles = progressBar.transform.eulerAngles + 180f * Vector3.right;
        } else
        {
            timeTraining = 0f;
            progressBar.SetActive(false);
        }

    }

    // Check to see if any troops have died
    // If they have, clear up space for training

    // Deaths are detected by checking for missing objects
    // If any are found then it means they were destroyed
    // This code might need to be changed if an object pooling system is implemented

    void CheckForDeadTroops()
    {

        int priorLength = spawnedTroops.Count;

        spawnedTroops.RemoveAll(x => x == null);

        if (priorLength != spawnedTroops.Count) {
            currentTroops = troopsQueued + spawnedTroops.Count;
            Debug.Log(priorLength);
        }
        
    }

}
