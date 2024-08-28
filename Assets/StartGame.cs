using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
public class StartGame : MonoBehaviour
{
    public string name;
    public void updateName()
    {
        //name = gameObject.GetComponent<InputField ;
    }
    public void gameStart()
    {
        Debug.Log($"Player name: {name}");
        return;
        SceneManager.LoadScene("Backup 3");
    }
}
