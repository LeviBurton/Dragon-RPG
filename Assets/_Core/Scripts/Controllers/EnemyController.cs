using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;
using System;

using Panda;
using UnityEngine.AI;
using UnityEngine.UI;
using RPG.Characters;

namespace RPG.Character
{
    [RequireComponent(typeof(HealthSystem))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(WeaponSystem))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] EnemyConfig enemyConfig;
        [SerializeField] Color chaseSphereColor = new Color(0, 1.0f, 0, .5f);
        [SerializeField] Color attackSphereColor = new Color(1.0f, 1.0f, 0, .5f);
        [SerializeField] Image recoveryCircleImage;
        [SerializeField] Sprite moveActionImage;
        [SerializeField] Sprite attackActionImage;
        [SerializeField] Image actionImage;

        public ECommandType command = ECommandType.Attack;

        public Queue<Command> commandQueue = new Queue<Command>();

        private float minRecoveryTimeSeconds;
        private float maxRecoveryTimeSeconds;
        private float currentRecoveryTimeSeconds;

        CharacterController character;
        WeaponSystem weaponSystem;
        HealthSystem healthSystem;

        void Start()
        {
            character = GetComponent<CharacterController>();
            AddOutlinesToMeshes();
            character.SetOutlinesEnabled(false);

            if (enemyConfig == null)
                throw new Exception("enemyConfig can't be null!");

            minRecoveryTimeSeconds = character.GetCharacterConfig().GetRecoveryTime();
            currentRecoveryTimeSeconds = 0.0f;
        }

        void AddOutlinesToMeshes()
        {
            // Outline stuff.
            var skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var mesh in skinnedMeshRenderers)
            {
                if (!mesh.gameObject.GetComponent<cakeslice.Outline>())
                {
                    var outline = mesh.gameObject.AddComponent<cakeslice.Outline>();
                    outline.color = 1;
                }
            }
            var meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var mesh in meshRenderers)
            {
                if (!mesh.gameObject.GetComponent<cakeslice.Outline>())
                {
                    var outline = mesh.gameObject.AddComponent<cakeslice.Outline>();
                    outline.color = 1;
                }
            }
        }
        void QueueCommand(Command command)
        {
            commandQueue.Enqueue(command);
        }

        public WeaponSystem GetWeaponSystem()
        {
            return weaponSystem;
        }

        #region Tasks
        [Task]
        bool AllHeroesDead()
        {
            var heroes = FindObjectsOfType<HeroController>();

            var allDead = true;

            foreach (var hero in heroes)
            {
                if (hero.GetComponent<HealthSystem>().IsAlive())
                {
                    allDead = false;
                }
            }

            return allDead;
        }
        [Task]
        bool IsCurrentCommandDone()
        {
            if (commandQueue.Count == 0)
                return true;

            return commandQueue.Peek().CommandStatus == ECommandStatus.Done;
        }

        [Task]
        bool PopCommand()
        {
            if (commandQueue.Count == 0)
                return true;

            commandQueue.Dequeue();

            return true;
        }
        
        [Task]
        bool CommandDone()
        {
            if (commandQueue.Count == 0)
                return true;

            var command = commandQueue.Peek();
            command.CommandStatus = ECommandStatus.Done;

            return true;

        }

        [Task]
        bool IsCurrentCommand_Move()
        {
            if (commandQueue.Count == 0)
                return false;

            return commandQueue.Peek().CommandType == ECommandType.Move;
        }

        [Task]
        bool IsCurrentCommand_Attack()
        {
            if (commandQueue.Count == 0)
                return false;

            return commandQueue.Peek().CommandType == ECommandType.Attack;
        }
        [Task]
        bool Enemy_MoveCommand()
        {
            return command == ECommandType.Move;

        }

        [Task]
        bool Enemy_AttackCommand()
        {
            return command == ECommandType.Attack;
        }

        #endregion

        void OnDrawGizmos()
        {
         
        }

   
    }
}