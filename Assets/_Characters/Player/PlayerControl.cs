using UnityEngine;

using RPG.CameraUI;
using System.Collections;
using System.Collections.Generic;

namespace RPG.Characters
{
    public class PlayerControl : MonoBehaviour
    {
        [SerializeField] LayerMask enemyLayer;


        List<Selectable> selectedEnemies;
        Selectable selectedEnemy;
        SpecialAbilities abilities;
        Character character;
        WeaponSystem weaponSystem;
        Selectable selectable;

        void Start()
        {
            character = GetComponent<Character>();
            abilities = GetComponent<SpecialAbilities>();
            weaponSystem = GetComponent<WeaponSystem>();
            selectable = GetComponent<Selectable>();

            if (weaponSystem != null)
            {
                weaponSystem.onWeaponHit += OnWeaponHit;
            }

            RegisterForMouseEvents();
        }

        void OnWeaponHit(WeaponSystem weaponSystem, GameObject hitObject, float damage)
        {
            Debug.LogFormat("{0} hit {1} with {2} for {3} damage", weaponSystem.name, hitObject.name, weaponSystem.GetCurrentWeapon().name, damage);
        }

        void Update()
        {
            // todo handle controllers with these methods.
            // ProcessDirectMovement();
            // ScanControllerForInput();

            // handle keyboard.
            ScanForAbilityKeydown();
        }

        void RegisterForMouseEvents()
        {
            var cameraRaycaster = FindObjectOfType<CameraRaycaster>();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
            cameraRaycaster.onMouseOverNPC += OnMouseOverNPC;
        }

        void ProcessDirectMovement()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            // calculate camera relative direction to move:
            var cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            var movement = v * cameraForward + h * Camera.main.transform.right;

            character.ControllerMove(movement);
        }

        void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                character.SetDestination(destination);
            }
        }

        void OnMouseOverNPC(GameObject NPC)
        {
            if (Input.GetMouseButton(0))
            {
                StartCoroutine(MoveToTarget(NPC));
                transform.LookAt(NPC.transform);
            }
        }

        void OnMouseOverEnemy(EnemyAI enemy)
        {
            bool isTargetInRange = IsTargetInRange(enemy.gameObject);

            if (Input.GetMouseButton(0) && isTargetInRange)
            {
                weaponSystem.Attack();
            }
            else if (Input.GetMouseButton(0) && !isTargetInRange)
            {
                StartCoroutine(MoveAndAttack(enemy));
            }
            else if (Input.GetMouseButtonDown(1) && isTargetInRange)
            {
                abilities.AttemptSpecialAbility(0, enemy.gameObject);
            }
            else if (Input.GetMouseButtonDown(1) && !isTargetInRange)
            {
                StartCoroutine(MoveAndPowerAttack(enemy));
            }
        }

        IEnumerator MoveToTarget(GameObject target)
        {
            character.SetDestination(target.transform.position);

            while (!IsTargetInRange(target.gameObject))
            {
                yield return new WaitForEndOfFrame();
            }
            transform.LookAt(target.transform);
            yield return new WaitForEndOfFrame();
        }

        IEnumerator MoveAndAttack(EnemyAI enemy)
        {
            yield return StartCoroutine(MoveToTarget(enemy.gameObject));
            //weaponSystem.AttackTarget(enemy.gameObject);
        }

        IEnumerator MoveAndPowerAttack(EnemyAI enemy)
        {
            yield return StartCoroutine(MoveToTarget(enemy.gameObject));
            abilities.AttemptSpecialAbility(0, enemy.gameObject);
        }

        // todo: move to WeaponSystem
        bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = Vector3.Distance(character.transform.position, target.transform.position);
            return distanceToTarget <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
        }

        void ScanControllerForInput()
        {
            if (Input.GetButton("Fire1"))
            {
                weaponSystem.Attack();
            }

            // Input mappings
            // joystick button 0: A
            // joystick button 1: B
            // joystick button 2: X
            // joystick button 3: Y
            // joystick button 4: Left Bumper
            // joystick button 5: Right Bumper
            // joystick button 6: Start
            // joystick button 7: Options
            // joystick button 8: Left Stick 
            // joystick button 9: Right Stick 

            if (Input.GetKeyDown("joystick button 4"))
            {
                // if no selected enemy, select closest enemy within a sphere.
                if (selectedEnemy == null)
                {
                    RaycastHit[] hits = Physics.SphereCastAll(transform.position, 15.0f, Vector3.up, enemyLayer);
                    foreach (var hit in hits)
                    {
                        print("found: " + hit.transform.gameObject.name);
                    }
                }
            }
            else if (Input.GetKeyDown("joystick button 5"))
            {
                Debug.Log("select forward");
            }
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


