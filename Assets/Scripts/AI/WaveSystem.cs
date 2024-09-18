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

    IEnumerator wave()
    {
        yield return new WaitForSeconds(60);
        for(int x = 0; x <= waveNumber; x++) Instantiate(EnemyWave).transform.position = spawnLocations[UnityEngine.Random.Range(0, 4)].position;
        waveNumber++;
        waveNumberDisplay.text = "Wave Number: " + waveNumber.ToString();
        StartCoroutine(wave());
    }
}
