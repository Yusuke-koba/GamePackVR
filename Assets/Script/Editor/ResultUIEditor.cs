using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResultUI))]
public class ResultUIEditor : Editor
{
    private ResultUI ResultUI = null;

    private void OnEnable() => ResultUI = (ResultUI)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("UI配置確認_DRAW"))
        {
            ResultUI.ResultUISet(true,0,0);
            ResultUI.UILayoutSet();
        }
        if (GUILayout.Button("UI配置確認_Player1WIN"))
        {
            ResultUI.ResultUISet(true,10,0);
            ResultUI.UILayoutSet();
        }
        if (GUILayout.Button("UI配置確認_Player2WIN"))
        {
            ResultUI.ResultUISet(true,0,10);
            ResultUI.UILayoutSet();
        }
    }
}
