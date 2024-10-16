using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WaveSystem : MonoBehaviour
{
    public GameObject EnemyWave;
    public Transform[] spawnLocations;
    public TextMeshProUGUI waveNumberDisplay;
    public int waveNumber = 0;
    private void Start()
    {
        waveNumberDisplay.text = "Wave Number: "+waveNumber.ToString();
        StartCoroutine(wave());
    }

    float initialWaveTime = 60f;
    float shortestWaveTime = 10f;

    // The amount of time (in seconds) to shorten the wave cooldown by
    float intermissionShortenCoefficient = 5f;

    IEnumerator wave()
    {
        float time = 0;

        // The amount of time to wait between waves should gradually decrease every wave until it hits a minimum value
        // The reduction in cooldown between waves is determined by: (the current wave) multiplied by (some coefficient)

        while (time <= initialWaveTime - (waveNumber * intermissionShortenCoefficient) | time <= shortestWaveTime)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        for (int x = 0; x <= waveNumber; x++)
        {
            Instantiate(EnemyWave).transform.position = spawnLocations[UnityEngine.Random.Range(0, 4)].position;
        }
        waveNumber++;
        waveNumberDisplay.text = "Wave Number: " + waveNumber.ToString();
        StartCoroutine(wave());
    }
}
