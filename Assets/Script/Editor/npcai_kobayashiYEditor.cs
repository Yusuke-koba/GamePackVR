using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(npcai_kobayashiY))]
public class npcai_kobayashiYEditor : Editor
{
    private npcai_kobayashiY npcai_kobayashiY = null;

    private void OnEnable() => npcai_kobayashiY = (npcai_kobayashiY)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Select"))
            npcai_kobayashiY.Select();
        if (GUILayout.Button("Throw"))
            npcai_kobayashiY.Throw();
    }
}
