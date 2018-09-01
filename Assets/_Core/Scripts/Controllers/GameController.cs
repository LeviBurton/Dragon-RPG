using RPG.Characters;
using RPG.Config;
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

        private string gameDataFileName = "gameData.json";
        public GameData gameData;

        public List<CharacterController> selectedCharacters;
        public List<CharacterController> playerCharacters;

        private void Awake()
        {
            LoadGameData();
        }

        void Start()
        {
            selectedCharacters = new List<CharacterController>();
            playerCharacters = FindObjectsOfType<CharacterController>().Where(c => c.characterType == ECharacterType.Player).ToList();

            foreach (var player in playerCharacters)
            {
                var heroData = new HeroData();
                heroData.name = player.name;
                heroData.currentXP = 0;

                var abilityScoreSet = new AbilityScoreSet();

                player.GetComponent<AbilityScoreSystem>().SetAbilityScoreSet(abilityScoreSet);
                heroData.abilityScores = abilityScoreSet;
                heroData.currentHealth = player.GetComponent<HealthSystem>().GetCurrentHealth();
                heroData.maxHealth = player.GetComponent<HealthSystem>().GetMaxHealth();
                heroData.characterType = ECharacterType.Player;
                heroData.characterSize = ECharacterSize.Medium;

                if (!gameData.allHeroes.Exists(x => x.name == heroData.name))
                {
                    gameData.allHeroes.Add(heroData);
                    gameData.currentHeroes.Add(heroData);
                }
            }

            string assetBundleDirectory = Application.streamingAssetsPath + "/AssetBundles";
        }

        void Update()
        {

        }

        private void OnDisable()
        {
            SaveGameData();
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

        public void OnCharacterSpawn(CharacterController character)
        {
            Debug.LogFormat("OnCharacterSpawn: {0}", character.name);
        }

        public void OnCharacterSelected(CharacterController character)
        {

        }
    }
}
