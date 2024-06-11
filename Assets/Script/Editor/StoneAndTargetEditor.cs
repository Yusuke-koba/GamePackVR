using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StoneAndTarget))]
public class StoneAndTargetEditor : Editor
{
    private StoneAndTarget stoneAndTarget = null;

    private void OnEnable() => stoneAndTarget = (StoneAndTarget)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("WhiteStone"))
            stoneAndTarget.Change(StoneAndTarget.Type.WhiteStone);
        if (GUILayout.Button("BlackStone"))
            stoneAndTarget.Change(StoneAndTarget.Type.BlackStone);
        if (GUILayout.Button("Target"))
            stoneAndTarget.Change(StoneAndTarget.Type.Target);
        if (GUILayout.Button("None"))
            stoneAndTarget.Change(StoneAndTarget.Type.None);
        if (GUILayout.Button("SetAroundStoneAndTargetList"))
            stoneAndTarget.SetAroundStoneAndTargetList();
    }

}
