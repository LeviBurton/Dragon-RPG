using EditorGUITable;
using RPG.Characters;
using RPG.Config;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DamageTypeCell : TableCell
{
    string assetsPath = @"Assets/_Core/Configs/DamageTypes";

    List<DamageTypeConfig> items = new List<DamageTypeConfig>();

    SerializedProperty sp;
    SerializedObject so;

    int flags = 0;

    public void RefreshAssets()
    {
        items = new List<DamageTypeConfig>();

        var itemGuids = AssetDatabase.FindAssets("t:DamageTypeConfig", new string[] { assetsPath });

        foreach (string itemGuid in itemGuids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(itemGuid);
            var asset = AssetDatabase.LoadAssetAtPath<DamageTypeConfig>(assetPath);
            items.Add(asset);
        }
    }

    public DamageTypeCell(SerializedObject so, string propertyName)
    {
        sp = so.FindProperty(propertyName);
        this.so = so;
        RefreshAssets();

        Debug.LogFormat("sp {0} so: {1}", sp, this.so);
    }

    public override void DrawCell(Rect rect)
    {

        sp.intValue = EditorGUI.MaskField(rect, sp.intValue, items.Select(x => x.name).ToArray());


        so.ApplyModifiedProperties();
    }


    public override string comparingValue
    {
        get
        {
            return string.Empty;
        }
    }


}
public class WeaponEditor : EditorWindow
{
    static string AssetsBasePath = @"Assets/_Core/Configs/Weapons";
    public Vector2 scrollPosition;
    GUITableState tableState = new GUITableState();
    SerializedObject serializedObject;
    public static float numberFieldWidth = 50.0f;

    [Table]
    public List<WeaponConfig> items;

    [MenuItem("Tools/RPG/Weapons")]
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
        var assetPath = AssetsBasePath + string.Format(@"/{0}.asset", Name);

        int duplicateNumber = 1;
        while (File.Exists(assetPath))
        {
            assetPath = AssetsBasePath + string.Format(@"/{0} ({1}).asset", Name, duplicateNumber++);
        }

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


        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add"))
        {
            CreateAsset<WeaponConfig>("New Weapon");
            RefreshAssets();
            this.Repaint();
        }

        if (GUILayout.Button("Refresh"))
        {
            RefreshAssets();
            this.Repaint();
        }
        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        SerializedObject serializedObject = new SerializedObject(this);
        WeaponEditor targetObject = (WeaponEditor)serializedObject.targetObject;

        List<SelectorColumn> columns = new List<SelectorColumn>();
        columns.Add(new SelectObjectReferenceColumn("Asset", TableColumn.Width(100), TableColumn.Resizeable(true)));
        columns.Add(new SelectFromPropertyNameColumn("weaponName", "Name", TableColumn.Width(200), TableColumn.Sortable(true), TableColumn.Resizeable(true)));
        columns.Add(new SelectFromPropertyNameColumn("weaponType", "Weapon Type", TableColumn.Width(120), TableColumn.Sortable(true), TableColumn.Resizeable(true)));
        columns.Add(new SelectFromPropertyNameColumn("weaponPrefab", "Weapon Prefab", TableColumn.Width(120), TableColumn.Sortable(true), TableColumn.Resizeable(true)));
        columns.Add(new SelectFromFunctionColumn(x => Foobar(x), "Damage Types", TableColumn.Width(150)));

        columns.Add(new SelectFromPropertyNameColumn("baseDamage", "DMG", TableColumn.Width(numberFieldWidth), TableColumn.Sortable(true), TableColumn.Resizeable(true)));
        columns.Add(new SelectFromPropertyNameColumn("recoveryTimeSeconds", "RT", TableColumn.Width(numberFieldWidth), TableColumn.Sortable(true), TableColumn.Resizeable(true)));
        columns.Add(new SelectFromPropertyNameColumn("weaponRange", "R", TableColumn.Width(numberFieldWidth), TableColumn.Sortable(true), TableColumn.Resizeable(true)));
        columns.Add(new SelectFromPropertyNameColumn("attackSpeedSeconds", "AS", TableColumn.Width(numberFieldWidth), TableColumn.Sortable(true), TableColumn.Resizeable(true)));
        columns.Add(new SelectFromPropertyNameColumn("useOtherHand", "SwitchHand", TableColumn.Width(80), TableColumn.Sortable(true), TableColumn.Resizeable(true)));

        GUITableLayout.DrawTable(tableState, serializedObject.FindProperty("items"), columns, GUITableOption.AllowScrollView(true));

        GUILayout.EndScrollView();
    }

    DamageTypeCell Foobar(SerializedProperty prop)
    {
        return new DamageTypeCell(new SerializedObject(prop.objectReferenceValue), "damageTypes");
    }
}
