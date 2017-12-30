using UnityEngine;

public class CameraRaycaster : MonoBehaviour
{
    public ELayer[] LayerPriorities = {
        ELayer.Enemy,
        ELayer.Walkable
    };

    [SerializeField]
    float DistanceToBackground = 100f;
    Camera ViewCamera;

    RaycastHit raycastHit;
    public RaycastHit RaycastHit
    {
        get { return raycastHit; }
    }

    ELayer currentLayerHit;
    public ELayer CurrentLayerHit
    {
        get { return currentLayerHit; }
    }

    public delegate void OnLayerChange(ELayer NewLayer);       // delcare new delegate type
    public OnLayerChange LayerChangeObservers;   // instantiate an observer set

   
    void Start() 
    {
        ViewCamera = Camera.main;
    }

    void Update()
    {
        // Look for and return the first priority layer hit
        foreach (ELayer LayerToCheck in LayerPriorities)
        {
            var LayerHit = RaycastForLayer(LayerToCheck);
            if (LayerHit == null)
                continue;

            raycastHit = LayerHit.Value;

            // Has the layer changed?
            if (currentLayerHit != LayerToCheck)
            {
                LayerChangeObservers(LayerToCheck);
            }

            currentLayerHit = LayerToCheck;

            return;
        }

        // Otherwise return background hit
        raycastHit.distance = DistanceToBackground;
        currentLayerHit = ELayer.RaycastEndStop;
    }

    RaycastHit? RaycastForLayer(ELayer Layer)
    {
        int LayerMask = 1 << (int)Layer; // See Unity docs for mask formation
        Ray CameraToWorld = ViewCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit RayCastHit; // used as an out parameter
        bool bHit = Physics.Raycast(CameraToWorld, out RayCastHit, DistanceToBackground, LayerMask);
        if (bHit)
        {
            return RayCastHit;
        }
        return null;
    }
}
