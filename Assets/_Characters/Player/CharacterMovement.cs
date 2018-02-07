using UnityEngine;
using UnityEngine.AI;

using RPG.CameraUI;

namespace RPG.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] float stoppingDistance = 1.0f;

        ThirdPersonCharacter character;   
        GameObject walkTarget;
        NavMeshAgent agent;

        void Start()
        {
            var cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            character = GetComponent<ThirdPersonCharacter>();

            agent = GetComponent<NavMeshAgent>();
            agent.updatePosition = true;
            agent.updateRotation = false;
            agent.stoppingDistance = stoppingDistance;

            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        void OnMouseOverEnemy(Enemy enemy)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(1))
            {
                agent.SetDestination(enemy.transform.position);
            }
        }

        void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                agent.SetDestination(destination);
            }
        }

        void Update()
        {
            if (agent.remainingDistance > stoppingDistance)
            {
                character.Move(agent.desiredVelocity);
            }
            else
            {
                character.Move(Vector3.zero);
            }
        }

        // TODO make this get called again
        void ProcessDirectMovement()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            // calculate camera relative direction to move:
            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 movement = v * cameraForward + h * Camera.main.transform.right;

            character.Move(movement);
        }
    }
}