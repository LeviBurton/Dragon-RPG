using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace RPG.Characters
{
    public class InventorySystem : MonoBehaviour
    {
        string itemAssetsPath;
        public List<ItemConfig> itemConfigs;
        AssetBundle itemsAssetBundle;

        private void Awake()
        {
            itemAssetsPath = Path.Combine(Application.streamingAssetsPath, "AssetBundles/items");
            itemConfigs = new List<ItemConfig>();
            itemsAssetBundle = AssetBundle.LoadFromFile(itemAssetsPath);
            itemConfigs = itemsAssetBundle.LoadAllAssets<ItemConfig>().ToList();
        }

        void Start()
        {
          
        }
    }
}