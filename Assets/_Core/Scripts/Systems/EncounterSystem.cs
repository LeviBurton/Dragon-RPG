using RPG.Characters;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class EncounterSystem : MonoBehaviour
{
    [Header("Events")]
    public Event_EncounterStart onEncounterStart;
    public Event_EncounterEnd onEncounterEnd;
    
    public List<HeroController> heroes;
    public List<EnemyController> enemies;
    public bool isEncounterStarted = false;

   
    public bool enableDebugView = false;

    public BoxCollider encounterBounds;
    public bool isRequired = false;

    public List<CharacterSystem> combatants;

    [Header("Gizmos")]
    public Color sceneViewLabelColor = Color.blue;

    private void Awake()
    {
   
    }

    private void OnEnable()
    {
        encounterBounds = GetComponent<BoxCollider>();
    }

    private void OnDisable()
    {
        
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        // make sure only a single thing can perform an action.
    }

    public void StartEncounter(HeroController heroThatStartedEncounter)
    {
        isEncounterStarted = true;

        var heroes = FindObjectsOfType<HeroController>();

        // establish participants, roll initiative.
        // TODO: consider if they have an advantage/disadvantage modifier.
        foreach (var hero in heroes)
        {
            var initiativeRoll = Dice.Roll(1, 20);
    
            hero.gameObject.AddComponent<Initiative>().Value = initiativeRoll;
            Debug.LogFormat("{0} initiative roll: {1}", hero.name, hero.gameObject.GetComponent<Initiative>().Value);
            combatants.Add(hero.GetComponent<CharacterSystem>());
        }

        foreach (var enemy in enemies)
        {
            var initiativeRoll = Dice.Roll(1, 20);
            enemy.gameObject.AddComponent<Initiative>().Value = initiativeRoll;
            Debug.LogFormat("{0} initiative roll: {1}", enemy.name, enemy.gameObject.GetComponent<Initiative>().Value);
            combatants.Add(enemy.GetComponent<CharacterSystem>());
        }

        // TODO: 
        // sort all combatants by initiative.  group enemies by their type (type is yet to be determined, will probably be a scriptable object.)

        onEncounterStart.Invoke(this);
    }

    public void EndEncounter()
    {
        foreach (var enemy in enemies)
        {
            Destroy(enemy.GetComponent<Initiative>());
        }

        foreach (var hero in heroes)
        {
            Destroy(hero.GetComponent<Initiative>());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isEncounterStarted)
            return;

        var hero = other.GetComponent<HeroController>();
        if (hero == null)
            return;

        var selecteable = hero.GetComponent<Selectable>();

        if (selecteable == null || selecteable.isSelected == false)
            return;

        StartEncounter(hero);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isEncounterStarted)
            return;

        var hero = other.GetComponent<HeroController>();
        if (hero == null)
            return;

        var selecteable = hero.GetComponent<Selectable>();

        if (selecteable == null || selecteable.isSelected == false)
            return;

        StartEncounter(hero);
    }


    #region Gizmos

    void OnDrawGizmos()
    {
        if (!enableDebugView)
            return;

        var previousColor = Gizmos.color;
        var collider = GetComponent<BoxCollider>();
        if (collider)
        {
            Gizmos.color = sceneViewLabelColor;
            Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(collider.transform.TransformPoint(collider.center), collider.transform.rotation, collider.transform.lossyScale);
            Gizmos.DrawWireCube(Vector3.zero, collider.size); 
            Gizmos.matrix = oldGizmosMatrix;
            Gizmos.color = previousColor;
        }   
    }
    #endregion

}
