using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;

public class Leaderboard : MonoBehaviour
{
    
    public TextMeshProUGUI leaderboardText;
    public List<Tuple<int, string>> leaderboard = new List<Tuple<int, string>>();
    void Start()
    {
       // if(!File.Exists("C:\\leaderboard.text")) 
       //     using (StreamWriter sw = File.CreateText(Application.persistentDataPath + "leaderboard.text"))
        //    {
         //       sw.Write("");
        //    }

        // read leaderboard file
        StreamReader sr = new StreamReader(Application.persistentDataPath+"leaderboard.txt");
        string line;
        //read each line of the file into the line variable
      //  while ((line= sr.ReadLine())!= null)
      //  {
      //      Debug.Log(line);
            // add each line to the list
      //      leaderboard.Add(
        //        new Tuple<int, string>(
         //           Convert.ToInt16(line.Split(',')[1]), 
          //          line.Split(',')[0]
       //             ));
       // }
        
        // To be used to compare the value of each score
        int compare(Tuple<int, string> x, Tuple<int, string> y)
        {
            return y.Item1.CompareTo(x.Item1);
        }
        leaderboard.Sort(compare);
        string leaderboardString = "";
        foreach(Tuple<int, string> s in leaderboard)
        {
            leaderboardString += Convert.ToString(s.Item1) + ", " + Convert.ToString(s.Item2) + "\n";
        }
        // write the leaderboard string into the scene
        leaderboardText.text = leaderboardString;
        Debug.Log(leaderboard);
        Debug.Log(leaderboard[0]);
    }
}
