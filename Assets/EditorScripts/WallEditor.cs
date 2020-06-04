using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Wall))]
public class WallEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Wall myScript = (Wall)target;
        if (GUILayout.Button("Do 1 damage"))
        {
            myScript.ApplyDamage(1);
        }
    }
}
