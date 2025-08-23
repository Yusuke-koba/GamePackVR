using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WindowMonitor),true)]
public class WindowMonitorEditor : Editor
{
    private WindowMonitor WindowMonitor = null;
    private void OnEnable() => WindowMonitor = (WindowMonitor)target;
    int testElementNo;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        testElementNo = EditorGUILayout.IntField("ElementNo", testElementNo);
        if (GUILayout.Button("カメラ移動確認（ElementNoTestStart）"))
             WindowMonitor.CameraStart(testElementNo);
    }
}
