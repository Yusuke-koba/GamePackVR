using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OthelloGameManager))]
public class OthelloGameManagerEditor : Editor
{
    private OthelloGameManager othelloGameManager = null;

    private void OnEnable() => othelloGameManager = (OthelloGameManager)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("AllSetAroundStoneAndTargetList"))
            othelloGameManager.AllSetAroundStoneAndTargetList();
        if (GUILayout.Button("ChangeTurn"))
            othelloGameManager.ChangeTurn();
        if (GUILayout.Button("DebugMoveThrwStone"))
            othelloGameManager.DebugMoveThrwStone();
        if (GUILayout.Button("GameLogLoad"))
            othelloGameManager.GameLogLoad();
        EditorGUILayout.LabelField("①GameLogFile.logから読み込む行番号(loadLine)を指定する");
        EditorGUILayout.LabelField("②GameLogLoadを押下するとそのターンの盤面を再現する");
    }
}
