using RPG.Config;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Controllers
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] GameConfig gameConfig;

        public List<CharacterController> selectedCharacters;
        public List<CharacterController> playerCharacters;

        void Awake()
        {
         
        }

        // Use this for initialization
        void Start()
        {
            selectedCharacters = new List<CharacterController>();
            playerCharacters = FindObjectsOfType<CharacterController>().Where(c => c.characterType == ECharacterType.Player).ToList();

            foreach (var c in playerCharacters)
            {
                c.onSpawn += OnCharacterSpawn;
            }
        }

        // Update is called once per frame
        void Update()
        {

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
