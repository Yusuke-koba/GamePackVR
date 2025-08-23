using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NPCAIBase),true)]
public class NPCAIEditor : Editor
{
    private NPCAIBase NPCAIBase = null;

    private void OnEnable() => NPCAIBase = (NPCAIBase)target;

    public override void OnInspectorGUI()
    {
        UnityEditor.EditorGUILayout.LabelField("NPC名："+NPCAIBase.Title());
        base.OnInspectorGUI();
        if (GUILayout.Button("TurnStart"))
             NPCAIBase.TurnStart();
        // if (GUILayout.Button("Move"))
        //     NPCAIBase.Move();
        // if (GUILayout.Button("Select"))
        //     NPCAIBase.Select();
        // if (GUILayout.Button("Throw"))
        //     NPCAIBase.Throw();
    }
}
