using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

namespace RPG.AssetHelpers
{
    //public static class AssetHelpers
    //{
    //    public static T[] GetAtPath<T>(string path)
    //    {

    //        ArrayList al = new ArrayList();
    //        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);
    //        foreach (string fileName in fileEntries)
    //        {
    //            int index = fileName.LastIndexOf("/");
    //            string localPath = "Assets/" + path;

    //            if (index > 0)
    //                localPath += fileName.Substring(index);

    //            UnityEngine.Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

    //            if (t != null)
    //                al.Add(t);
    //        }
    //        T[] result = new T[al.Count];
    //        for (int i = 0; i < al.Count; i++)
    //            result[i] = (T)al[i];

    //        return result;
    //    }
    //}
}
