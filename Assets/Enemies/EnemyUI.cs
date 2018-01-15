using UnityEngine;

// Add a UI Socket transform to your enemy
// Attack this script to the socket
// Link to a canvas prefab that contains NPC UI
public class EnemyUI : MonoBehaviour {

    // Works around Unity 5.5's lack of nested prefabs
    [Tooltip("The UI canvas prefab")]
    [SerializeField]
    GameObject enemyCanvasPrefab = null;

    Camera cameraToLookAt;

    // Use this for initialization 
    void Start()
    {
        cameraToLookAt = Camera.main;
        Instantiate(enemyCanvasPrefab, transform.position, Quaternion.identity, transform);
    }

    private void OnDrawGizmos()
    {
       // Gizmos.DrawLine(transform.position, Camera.main.transform.position);

    }
  
    // Update is called once per frame 
    void LateUpdate()
    {
        var Parent = transform.parent.transform;

        transform.LookAt(transform.position + (cameraToLookAt.transform.rotation *  Parent.transform.rotation) * Vector3.forward,
                         cameraToLookAt.transform.rotation * Vector3.up);
    }
}