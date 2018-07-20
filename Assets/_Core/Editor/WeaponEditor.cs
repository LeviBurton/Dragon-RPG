using EditorGUITable;
using RPG.Characters;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WeaponEditor : EditorWindow
{
    static string AssetsBasePath = @"Assets/_Core/Configs/Weapons";

    GUITableState tableState = new GUITableState();
    SerializedObject serializedObject;

    [Table]
    public List<WeaponConfig> items;

    [MenuItem("Window/RPG/Weapons")]
    public static void ShowWindow()
    {
        WeaponEditor editor = EditorWindow.GetWindow<WeaponEditor>();

        editor.titleContent = new GUIContent("Weapon Editor");
        editor.Show();
    }

    private void OnEnable()
    {
        RefreshAssets();
    }

    public void RefreshAssets()
    {
        items = new List<WeaponConfig>();

        var itemGuids = AssetDatabase.FindAssets("t:WeaponConfig", new string[] { AssetsBasePath });

        foreach (string itemGuid in itemGuids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(itemGuid);
            var asset = AssetDatabase.LoadAssetAtPath<WeaponConfig>(assetPath);
            items.Add(asset);
        }
    }

    public T CreateAsset<T>(string Name, bool bActuallyCreateAsset = true) where T : WeaponConfig
    {
        var assetPath = AssetsBasePath + string.Format(@"/{0}/{0}.asset", Name.Replace(" ", "_"));
        T itemAsset = ScriptableObject.CreateInstance<T>();
        if (bActuallyCreateAsset)
        {
            AssetDatabase.CreateAsset(itemAsset, assetPath);
        }
        return itemAsset;
    }

    void OnGUI()
    {
        EditorStyles.boldLabel.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("Weapons", EditorStyles.boldLabel);

        GUILayout.BeginVertical();

        if (GUILayout.Button("Add"))
        {
            CreateAsset<WeaponConfig>("New Weapon");
            RefreshAssets();
            this.Repaint();
        }

        SerializedObject serializedObject = new SerializedObject(this);
        WeaponEditor targetObject = (WeaponEditor)serializedObject.targetObject;

        List<SelectorColumn> columns = new List<SelectorColumn>();

        columns.Add(new SelectFromPropertyNameColumn("Name", "Name", TableColumn.Width(200), TableColumn.Sortable(true), TableColumn.Resizeable(true)));
        columns.Add(new SelectFromPropertyNameColumn("weaponType", "Weapon Type", TableColumn.Width(200), TableColumn.Sortable(true), TableColumn.Resizeable(true)));
        columns.Add(new SelectFromPropertyNameColumn("armedWeapon", "Armed Weapon", TableColumn.Width(200), TableColumn.Sortable(true), TableColumn.Resizeable(true)));
        columns.Add(new SelectFromPropertyNameColumn("weaponPrefab", "Weapon Prefab", TableColumn.EnabledTitle(false), TableColumn.Width(200), TableColumn.Sortable(true), TableColumn.Resizeable(true)));

        GUITableLayout.DrawTable(tableState, serializedObject.FindProperty("items"), columns, GUITableOption.AllowScrollView(true));

        GUILayout.EndVertical();
    }

}
