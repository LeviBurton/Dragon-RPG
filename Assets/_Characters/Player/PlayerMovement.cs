using UnityEngine;
using UnityEngine.Assertions;

using RPG.CameraUI;

namespace RPG.Characters
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] float baseDamage = 10f;
        [Range(.1f, 1f)]
        [SerializeField] float criticalHitChance = .1f;
        [SerializeField] float criticalHitMultiplier = 1.25f;
        [SerializeField] ParticleSystem criticalHitParticleSystem = null;

        [SerializeField] Weapon currentWeaponConfig;
        [SerializeField] AnimatorOverrideController animatorOverrideController;

        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";

        Enemy enemy = null;
        Animator animator = null;
        CameraRaycaster cameraRaycaster = null;
        float lastHitTime = 0f;
        AudioSource audioSource = null;
        GameObject weaponObject = null;
        SpecialAbilities abilities;
        Character character;

        void Start()
        {
            character = GetComponent<Character>();
            abilities = GetComponent<SpecialAbilities>();

            RegisterForMouseEvents();
            PutWeaponInHand(currentWeaponConfig); // move to weapon system.
            SetAttackAnimation();                 // move to weapon system.
        }

        void Update()
        {
            ScanForAbilityKeydown();
        }


        public void PutWeaponInHand(Weapon weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = weaponToUse.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();

            Destroy(weaponObject);

            weaponObject = Instantiate(weaponPrefab, dominantHand.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.transform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.transform.localRotation;
        }


        void RegisterForMouseEvents()
        {
            cameraRaycaster = FindObjectOfType<CameraRaycaster>();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
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

        void SetAttackAnimation()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip();
        }

        GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;
            Assert.AreNotEqual(numberOfDominantHands, 0, "No dominant hand found on player, please add one.");
            Assert.IsFalse(numberOfDominantHands > 1, "Multiple dominant hand scripts on player.  Only 1 allowed.");
            return dominantHands[0].gameObject;
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
                AttackTarget();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                abilities.AttemptSpecialAbility(0);
            }
        }

        bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= currentWeaponConfig.GetMaxAttackRange();
        }

        void AttackTarget()
        {
            if (Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBetweenHits())
            {
                SetAttackAnimation();
                animator.SetTrigger(ATTACK_TRIGGER);
                lastHitTime = Time.time;
            }
        }

        float CalculateDamage()
        {
            var isCriticalHit = UnityEngine.Random.Range(0f, 1f) <= criticalHitChance;
            float damageBeforeCritical = baseDamage + currentWeaponConfig.GetAdditionalDamage();

            if (isCriticalHit)
            {
                criticalHitParticleSystem.Play();
                return damageBeforeCritical * criticalHitMultiplier;
            }
            else
            {
                return damageBeforeCritical;
            }
        }
    }
}