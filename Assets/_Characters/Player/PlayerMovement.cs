using UnityEngine;

using RPG.CameraUI;

namespace RPG.Characters
{
    public class PlayerMovement : MonoBehaviour
    {
        [Range(.1f, 1f)]
        [SerializeField] float criticalHitChance = .1f;
        [SerializeField] float criticalHitMultiplier = 1.25f;
       
        Enemy enemy = null;

        CameraRaycaster cameraRaycaster = null;
     
        AudioSource audioSource = null;
      
        SpecialAbilities abilities;
        Character character;
        WeaponSystem weaponSystem;

        void Start()
        {
            character = GetComponent<Character>();
            abilities = GetComponent<SpecialAbilities>();
            weaponSystem = GetComponent<WeaponSystem>();

            RegisterForMouseEvents();
        }

        void Update()
        {
            ScanForAbilityKeydown();
        }

        void RegisterForMouseEvents()
        {
            cameraRaycaster = FindObjectOfType<CameraRaycaster>();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
        }

        void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                character.SetDestination(destination);
            }
        }

        void OnMouseOverEnemy(Enemy enemy)
        {
            this.enemy = enemy;

            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                weaponSystem.AttackTarget(enemy.gameObject); 
            }
            else if (Input.GetMouseButtonDown(1))
            {
                abilities.AttemptSpecialAbility(0);
            }
        }

        bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
        }

        void ScanForAbilityKeydown()
        {
            for (int keyIndex = 1; keyIndex < abilities.GetNumberOfAbilitie(); keyIndex++)
            {
                if (Input.GetKeyDown(keyIndex.ToString()))
                {
                    abilities.AttemptSpecialAbility(keyIndex);
                }
            }
        }
    }
}