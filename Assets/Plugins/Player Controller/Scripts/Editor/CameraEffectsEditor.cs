using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraEffects))]
public class CameraEffectsEditor : Editor
{
    private CameraEffects cameraEffects;

    private void OnEnable()
    {
        cameraEffects = (CameraEffects) target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying)
        {
            if (GUILayout.Button("Shake Camera"))
            {
                cameraEffects.ShakeCamera(1f, 20f, 2f);
            }
        }
    }
}
