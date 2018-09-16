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
        private string skillsAssetBundlePath;
        [NonSerialized] public Dictionary<string, SkillConfig> skillConfigs = new Dictionary<string, SkillConfig>();

        private string featsAssetBundlePath;
        [NonSerialized] public Dictionary<string, FeatConfig> featConfigs = new Dictionary<string, FeatConfig>();

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
        public List<HeroController> heroesInPlay;
        public ExplorationModeCamera explorationModeCameraRig;

        private void Awake()
        {
            LoadCoreAssetBundles();
            LoadGameData();
        }

        private void Start()
        {
            InitScene();
        }

        public void InitScene()
        {
            // TODO: 
            // Spawn heroes
            // etc..

            // For now, just grab them from the scene
            heroesInPlay = FindObjectsOfType<HeroController>().ToList();
            explorationModeCameraRig = FindObjectOfType<ExplorationModeCamera>();

            foreach (var hero in heroesInPlay)
            {
                hero.GetComponent<Selectable>().Deselect();
            }

            heroesInPlay[0].GetComponent<Selectable>().Select();

            explorationModeCameraRig.SetTarget(heroesInPlay[0].transform);
        }

        public void LoadCoreAssetBundles()
        {
            // TODO: to support modularity in the future, we should be able to specify loading of a specific modules assetbundles.
            // Shouldn't be difficult at all.  
            LoadGenderAssetBundle();
            LoadClassesAssetBundle();
            LoadRacesAssetBundle();
            LoadAbilitiesAssetBundle();
            LoadWeaponsAssetBundle();
            LoadSkillsAssetBundle();
            LoadFeatsAssetBundle();
        }

        private void LoadFeatsAssetBundle()
        {
            featsAssetBundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundles/modules/core/feats");
            var featsAssetBundle = AssetBundle.LoadFromFile(featsAssetBundlePath);
            foreach (var assetName in featsAssetBundle.GetAllAssetNames())
            {
                Debug.Log("loading feats: " + assetName);
                var config = featsAssetBundle.LoadAsset<FeatConfig>(assetName);
                featConfigs.Add(config.name, config);
            }
        }

        private void LoadSkillsAssetBundle()
        {
            skillsAssetBundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundles/modules/core/skills");
            var skillsAssetBundle = AssetBundle.LoadFromFile(skillsAssetBundlePath);
            foreach (var assetName in skillsAssetBundle.GetAllAssetNames())
            {
                Debug.Log("loading skills: " + assetName);
                var config = skillsAssetBundle.LoadAsset<SkillConfig>(assetName);
                skillConfigs.Add(config.name, config);
            }
        }

        private void LoadWeaponsAssetBundle()
        {
            weaponAssetsPath = Path.Combine(Application.streamingAssetsPath, "AssetBundles/modules/core/items/weapons");
            var weaponAssetBundle = AssetBundle.LoadFromFile(weaponAssetsPath);
            foreach (var assetName in weaponAssetBundle.GetAllAssetNames())
            {
                Debug.Log("loading weapon: " + assetName);
                var config = weaponAssetBundle.LoadAsset<WeaponConfig>(assetName);
                weaponConfigs.Add(config.name, config);
            }
        }

        private void LoadAbilitiesAssetBundle()
        {
            abilitiesAssetBundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundles/modules/core/abilities");
            var abilitiesAssetBundle = AssetBundle.LoadFromFile(abilitiesAssetBundlePath);
            foreach (var assetName in abilitiesAssetBundle.GetAllAssetNames())
            {
                Debug.Log("loading ability: " + assetName);
                var config = abilitiesAssetBundle.LoadAsset<AbilityConfig>(assetName);
                abilityConfigs.Add(config.name, config);
            }
        }

        private void LoadRacesAssetBundle()
        {
            raceAssetsPath = Path.Combine(Application.streamingAssetsPath, "AssetBundles/modules/core/races");
            var raceAssetBundle = AssetBundle.LoadFromFile(raceAssetsPath);
            foreach (var assetName in raceAssetBundle.GetAllAssetNames())
            {
                Debug.Log("loading race: " + assetName);
                var config = raceAssetBundle.LoadAsset<RaceConfig>(assetName);
                raceConfigs.Add(config.name, config);
            }
        }

        private void LoadClassesAssetBundle()
        {
            classAssetBundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundles/modules/core/classes");
            var classAssetBundle = AssetBundle.LoadFromFile(classAssetBundlePath);
            foreach (var assetName in classAssetBundle.GetAllAssetNames())
            {
                Debug.Log("loading class: " + assetName);
                var config = classAssetBundle.LoadAsset<ClassConfig>(assetName);
                classConfigs.Add(config.name, config);
            }
        }

        private void LoadGenderAssetBundle()
        {
            genderAssetsPath = Path.Combine(Application.streamingAssetsPath, "AssetBundles/modules/core/genders");
            var genderAssetBundle = AssetBundle.LoadFromFile(genderAssetsPath);
            foreach (var assetName in genderAssetBundle.GetAllAssetNames())
            {
                Debug.Log("loading gender: " + assetName);
                var config = genderAssetBundle.LoadAsset<GenderConfig>(assetName);
                genderConfigs.Add(config.name, config);
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

    }
}
