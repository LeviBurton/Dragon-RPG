using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocationEntryPoint))]
public class LocationEntryPointHandle : MonoBehaviour
{
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    static void DrawHandles(LocationEntryPoint locationEntryPoint, GizmoType gizmoType)
    {
        if (Application.isPlaying)
            return;

        if (locationEntryPoint == null || locationEntryPoint.enableSceneViewLabel == false)
        {
            return;
        }
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 12;
        Handles.BeginGUI();
        Vector3 pos = locationEntryPoint.transform.position + Vector3.up * 6.0f;
        Vector2 pos2D = HandleUtility.WorldToGUIPoint(pos);
        Handles.DrawSolidRectangleWithOutline(new Rect(pos2D.x, pos2D.y, 120, 18), locationEntryPoint.sceneViewLabelColor, Color.black);
        GUI.Label(new Rect(pos2D.x, pos2D.y, 120, 18), locationEntryPoint.name, style);
        Handles.EndGUI();

        Handles.color = locationEntryPoint.sceneViewLabelColor;
        var collider = locationEntryPoint.bounds;
        var size = collider.size;
        collider.center = new Vector3(0, size.y / 2, 0);
        var position = locationEntryPoint.transform.position;

        Handles.DrawWireCube(position + collider.center, size);
    }
}
