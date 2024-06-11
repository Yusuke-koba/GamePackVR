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
    }
}
