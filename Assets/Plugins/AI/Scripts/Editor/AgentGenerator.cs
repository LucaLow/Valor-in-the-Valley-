using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class AgentGenerator : Editor
{
    [MenuItem("Tools/AI/Generate Friendly Agent")]
    public static void GenerateFriendlyAgent()
    {
        AIAgent.GenerateFriendlyAgent();
    }
    
    [MenuItem("Tools/AI/Generate Enemy Agent")]
    public static void GenerateEnemyAgent()
    {
        AIAgent.GenerateEnemyAgent();
    }
}
