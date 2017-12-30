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

    ELayer layerHit;
    public ELayer CurrentLayerHit
    {
        get { return layerHit; }
    }

    void Start() 
    {
        ViewCamera = Camera.main;
    }

    void Update()
    {
        // Look for and return priority layer hit
        foreach (ELayer Layer in LayerPriorities)
        {
            var LayerHit = RaycastForLayer(Layer);
            if (LayerHit != null)
            {
                raycastHit = LayerHit.Value;
                layerHit = Layer;
                return;
            }
        }

        // Otherwise return background hit
        raycastHit.distance = DistanceToBackground;
        layerHit = ELayer.RaycastEndStop;
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
