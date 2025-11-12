#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        DungeonGenerator generator = (DungeonGenerator)target;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Dungeon Generation", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Generate Dungeon", GUILayout.Height(30)))
        {
            generator.GenerateDungeon();
        }
        
        if (GUILayout.Button("Clear Dungeon", GUILayout.Height(30)))
        {
            generator.ClearDungeon();
        }
    }
}
#endif