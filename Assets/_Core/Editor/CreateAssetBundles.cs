using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class CreateAssetBundles
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    public class Startup
    {
        static Startup()
        {
            BuildAllAssetBundles();
        }
    }
#endif

    [MenuItem("Assets/Build All AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = Application.streamingAssetsPath + "/AssetBundles";

        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}
