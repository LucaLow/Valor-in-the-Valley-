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

    [Space]

    public int maxTroops;
    public int currentTroops = 0;

    public int troopsQueued = 0;

    public Transform _camera { get; set; }

    [Space]

    public GameObject progressBar;
    public GameObject generationProgressFill;
    public GameObject queueDisplay;

    float timeTraining = 0f;
    float trainTime = 5f;

    private void SpawnTroop()
    {
        Debug.Log("Troop Spawned");

        GameObject newTroop = Instantiate(troopPrefab, troopSpawnPoint.position, Quaternion.identity, troopParent);
    }

    void Start()
    {
        _camera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (troopsQueued > 0)
        {
            timeTraining += Time.deltaTime;

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
}
