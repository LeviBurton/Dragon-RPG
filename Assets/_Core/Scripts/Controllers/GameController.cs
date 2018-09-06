using RPG.Characters;
using RPG.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace RPG.Character
{
    public class GameController : MonoBehaviour
    {
        // TODO: may not need this.
        [SerializeField] GameConfig gameConfig;

        private string abilitiesAssetBundlePath;
        [NonSerialized] public Dictionary<string, AbilityConfig> abilityConfigs = new Dictionary<string, AbilityConfig>();

        private string classAssetBundlePath;
        [NonSerialized] public Dictionary<string, ClassConfig> classConfigs = new Dictionary<string, ClassConfig>();

        private string raceAssetsPath;
        [NonSerialized] public Dictionary<string, RaceConfig> raceConfigs = new Dictionary<string, RaceConfig>();

        private string genderAssetsPath;
        [NonSerialized] public Dictionary<string, GenderConfig> genderConfigs = new Dictionary<string, GenderConfig>();

        private string weaponAssetsPath;
        [NonSerialized] public Dictionary<string, WeaponConfig> weaponConfigs = new Dictionary<string, WeaponConfig>();

        private string gameDataFileName = "gameData.json";

        public GameData gameData;

        public List<CharacterController> selectedCharacters;
        public List<CharacterController> playerCharacters;

        private void Awake()
        {
            LoadAssetBundles();
            LoadGameData();
        }

        void Start()
        {
            selectedCharacters = new List<CharacterController>();
            playerCharacters = FindObjectsOfType<CharacterController>().Where(c => c.characterType == ECharacterType.Player).ToList();
            string assetBundleDirectory = Application.streamingAssetsPath + "/AssetBundles";
        }

        public void AddHeroToParty(HeroData hero)
        {
            gameData.partyHeroes.Add(hero);
        }

        public void SetHeroData(HeroData heroData)
        {
            if (gameData == null)
                return;

            gameData.hero = heroData;
        }

        public void LoadAssetBundles()
        {
            // TODO: to support modularity in the future, we should be able to specify loading of a specific modules assetbundles.
            // Shouldn't be difficult at all.  
            genderAssetsPath = Path.Combine(Application.streamingAssetsPath, "AssetBundles/modules/core/genders");
            var genderAssetBundle = AssetBundle.LoadFromFile(genderAssetsPath);
            foreach (var assetName in genderAssetBundle.GetAllAssetNames())
            {
                Debug.Log("loading gender: " + assetName);
                var config = genderAssetBundle.LoadAsset<GenderConfig>(assetName);
                genderConfigs.Add(assetName, config);
            }

            classAssetBundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundles/modules/core/classes");
            var classAssetBundle = AssetBundle.LoadFromFile(classAssetBundlePath);
            foreach (var assetName in classAssetBundle.GetAllAssetNames())
            {
                Debug.Log("loading class: " + assetName);
                var config = classAssetBundle.LoadAsset<ClassConfig>(assetName);
                classConfigs.Add(assetName, config);
            }

            raceAssetsPath = Path.Combine(Application.streamingAssetsPath, "AssetBundles/modules/core/races");
            var raceAssetBundle = AssetBundle.LoadFromFile(raceAssetsPath);
            foreach (var assetName in raceAssetBundle.GetAllAssetNames())
            {
                Debug.Log("loading race: " + assetName);
                var config = raceAssetBundle.LoadAsset<RaceConfig>(assetName);
                raceConfigs.Add(assetName, config);
            }

            abilitiesAssetBundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundles/modules/core/abilities");
            var abilitesBudle = AssetBundle.LoadFromFile(abilitiesAssetBundlePath);
            foreach (var assetName in abilitesBudle.GetAllAssetNames())
            {
                Debug.Log("loading ability: " + assetName);
                var config = abilitesBudle.LoadAsset<AbilityConfig>(assetName);
                abilityConfigs.Add(assetName, config);
            }

            weaponAssetsPath = Path.Combine(Application.streamingAssetsPath, "AssetBundles/modules/core/items/weapons");
            var weaponAssetBundle = AssetBundle.LoadFromFile(weaponAssetsPath);
            foreach (var assetName in weaponAssetBundle.GetAllAssetNames())
            {
                Debug.Log("loading weapon: " + assetName);
                var config = weaponAssetBundle.LoadAsset<WeaponConfig>(assetName);
                weaponConfigs.Add(assetName, config);
            }
        }

        public void LoadGameData()
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);
            if (File.Exists(filePath))
            {
                string dataAsJson = File.ReadAllText(filePath);
                gameData = JsonUtility.FromJson<GameData>(dataAsJson);
            }
            else
            {
                gameData = new GameData();
            }
        }

        public void SaveGameData()
        {
            string dataAsJson = JsonUtility.ToJson(gameData);
            string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);
            File.WriteAllText(filePath, dataAsJson);
        }
    }
}
