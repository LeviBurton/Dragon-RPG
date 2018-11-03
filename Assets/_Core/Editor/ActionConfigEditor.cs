using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

[CustomEditor(typeof(ActionConfig))]
public class ActionConfigEditor : Editor
{
    private Assembly methodAssembly = typeof(ActionMethodAttribute).Assembly;
    private ActionConfig targetActionConfig;
    public Dictionary<string, MethodInfo> actionMethods;
    private int selectedActionMethod;
    private List<string> actionMethodNames;

    SerializedProperty actionNameProp;
    SerializedProperty spriteIconProp;
    SerializedProperty descriptionProp;
    
    int DefaultWidth = 100;
    int SmallWidth = 50;
    int IconWidth = 16;
    int LargeWidth = 150;
    bool bIsChecked = false;

    private void OnEnable()
    {
        targetActionConfig = (ActionConfig)target;

        actionNameProp = serializedObject.FindProperty("actionName");
        descriptionProp = serializedObject.FindProperty("Description");
        spriteIconProp = serializedObject.FindProperty("spriteIcon");

        GetAvailableMethods();
    }

    private void OnDisable()
    {
        targetActionConfig = null;
    }

    private void GetAvailableMethods()
    {
        selectedActionMethod = 0;
        actionMethods = methodAssembly
                        .GetTypes()
                        .SelectMany(x => x.GetMethods())
                        .Where(y => y.GetCustomAttributes(true).OfType<ActionMethodAttribute>().Any())
                        .ToDictionary(z => z.Name);

        actionMethodNames = actionMethods.Keys.ToList<string>();
        actionMethodNames.Insert(0, "None");

        for (int i = 0; i < actionMethodNames.Count; i++)
        {
            if (actionMethodNames[i] == targetActionConfig.executeMethodName)
            {
                selectedActionMethod = i;
                break;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.PropertyField(actionNameProp);
        EditorGUILayout.PropertyField(descriptionProp);
        EditorGUILayout.PropertyField(spriteIconProp);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Action Method", GUILayout.ExpandWidth(false));
        selectedActionMethod = EditorGUILayout.Popup(selectedActionMethod, actionMethodNames.ToArray());
        targetActionConfig.executeMethodName = actionMethodNames[selectedActionMethod];
        targetActionConfig.BindExecuteMethod(targetActionConfig.executeMethodName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(targetActionConfig);
    }

}