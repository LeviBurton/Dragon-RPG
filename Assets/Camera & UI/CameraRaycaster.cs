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

    RaycastHit m_hit;
    public RaycastHit Hit
    {
        get { return m_hit; }
    }

    ELayer m_layerHit;
    public ELayer LayerHit
    {
        get { return m_layerHit; }
    }

    void Start() // TODO Awake?
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
                m_hit = LayerHit.Value;
                m_layerHit = Layer;
                return;
            }
        }

        // Otherwise return background hit
        m_hit.distance = DistanceToBackground;
        m_layerHit = ELayer.RaycastEndStop;
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
