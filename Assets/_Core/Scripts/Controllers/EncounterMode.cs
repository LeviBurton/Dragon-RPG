using Rewired;
using RPG.Character;
using RPG.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterMode : MonoBehaviour
{
    // TODO: these are duplicated from ExplorationMode.  Need to re-factor.
    [SerializeField] LayerMask enemyLayerMask = (1 << 9);
    [SerializeField] LayerMask walkableLayerMask = (1 << 8);
    [SerializeField] LayerMask friendlyMask = (1 << 10);
    [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);
    [SerializeField] Texture2D walkCursor = null;
    [SerializeField] Texture2D npcCursor = null;
    [SerializeField] Texture2D enemyCursor = null;
    [SerializeField] Transform axisToolPrefab = null;
    [SerializeField] GameController gameController = null;
    Transform mainCamera;
    bool isMouseOverEnemy = false;
    bool isMouseOverFriendly = false;
    bool isMouseOverPotentiallyWalkable = false;
    bool isSelecting = false;
    bool isFindingMoveToPosition;
    Vector3 mousePosition;
    float maxRaycastDepth = 100f; // Hard coded value
    GameObject objectUnderMouseCursor;
    Vector3 walkablePosition;
    Transform walkToClickTransform;
    Quaternion moveToRotation;
    Vector3 walkToClickRotationPosition;
    Vector3 controllerMove;
    ExploreModeCameraController explorerModeCamera;
    [SerializeField] CharacterGroupController heroGroupController;

    public List<HeroController> playerHeroes;   // TODO: player will only have a single hero here
    public int selectedHeroIndex;
    public HeroController selectedHero;
    public EnemyController selectedEnemy;
    public Transform mouseWorldTransform;

    #region Rewired stuff
    int rewiredPlayerId = 0;
    Player rewiredPlayer;
    #endregion

    void OnEnable()
    {
        Debug.Log("Encounter Mode Player Controller Enabled");
        mainCamera = Camera.main.transform;
        rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);
        playerHeroes = gameController.heroesInPlay;
    }

    void OnDisable()
    {
        Debug.Log("Encounter Mode Player Controller Disabled");
    }

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        GetRewiredInput();
    }

    void GetRewiredInput()
    {
        Debug.Log("Getting Rewired input for encounter mode");
    }
}
