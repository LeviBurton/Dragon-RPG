using RPG.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EncounterSystem : MonoBehaviour
{
    public Event_EncounterStart onEncounterStart;
    public Event_EncounterEnd onEncounterEnd;
    
    public List<HeroController> heroes;
    public List<EnemyController> enemies;
    public bool isEncounterStarted = false;

    public Color sceneViewLabelColor = Color.blue;
    public bool enableDebugView = false;

    public BoxCollider encounterBounds;
    public bool isRequired = false;

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

        Debug.LogFormat("{0} started an encounter!", heroThatStartedEncounter.name);

        onEncounterStart.Invoke(this);

        // TODO: 
        // establish participants
        // establish turn order
        // establish starting participant
        // 
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
}
