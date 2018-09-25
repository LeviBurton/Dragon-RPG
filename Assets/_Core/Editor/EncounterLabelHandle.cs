using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EncounterSystem))]
class EncounterLabelHandle : Editor
{
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    static void DrawHandles(EncounterSystem encounterSystem, GizmoType gizmoType)
    {
        if (Application.isPlaying)
            return;

        if (encounterSystem == null || encounterSystem.enableDebugView == false)
        {
            return;
        }

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 12;
        Handles.BeginGUI();
        Vector3 pos = encounterSystem.transform.position + Vector3.up * 6.0f;
        Vector2 pos2D = HandleUtility.WorldToGUIPoint(pos);
        Handles.DrawSolidRectangleWithOutline(new Rect(pos2D.x, pos2D.y, 120, 18), encounterSystem.sceneViewLabelColor, Color.black);
        GUI.Label(new Rect(pos2D.x, pos2D.y, 120, 18), encounterSystem.name, style);
        Handles.EndGUI();

        Handles.color = encounterSystem.sceneViewLabelColor;
        var collider = encounterSystem.encounterBounds;
        var size = collider.size;
        collider.center = new Vector3(0, size.y / 2, 0);
        var position = encounterSystem.transform.position;


        Handles.DrawWireCube(position + collider.center, size);
    }
}
