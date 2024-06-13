using UnityEngine;

public class Mover : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float speed = 55.0f;
    private float distanceTraveled;
    public beamManager _manager;
    public float transparency = 0f; 
    private Material[] materials;
  private  float threshold = 0.1f; 


void Start()
{
     Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            materials = renderer.materials;
        }
        else
        {
            Debug.LogError("No Renderer found on the GameObject.");
        }

        Invoke("EnableTransparency",0.3f);
}

void EnableTransparency()
{
      if (materials != null)
        {
            foreach (Material mat in materials)
            {
                SetMaterialTransparent(mat, 1);
            }
        }
}


    void Update()
    {
        if(_manager.isMove)
        {
        distanceTraveled += speed * Time.deltaTime;
        float pathLength = GetPathLength();
        float t = distanceTraveled / pathLength;
        if (t > 1.0f) t = 1.0f; // Stop at the end of the path
        Vector3 position = GetPointAt(t);
        transform.LookAt(position);
        transform.position=position;
         if (HasReachedPointB())
        {
          Invoke("MakeTransparent",0.7f);
        }
        }
    }

    void MakeTransparent()
    {
         if (materials != null)
        {
            foreach (Material mat in materials)
            {
                SetMaterialTransparent(mat, transparency);
            }
        }
    }

    bool HasReachedPointB()
    {
        if (_manager.points[1].transform != null)
        {
            float distance = Vector3.Distance(transform.position, _manager.points[1].transform.position);
            return distance <= threshold;
        }
        return false;
    }

void SetMaterialTransparent(Material mat, float alpha)
    {
        if (mat != null)
        {
            // Set the rendering mode to Transparent
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;

            // Set the alpha value
            Color color = mat.color;
            color.a = alpha;
            mat.color = color;
        }
    }
    float GetPathLength()
    {
        float length = 0.0f;
        for (int i = 0; i < lineRenderer.positionCount - 1; i++)
        {
            length += Vector3.Distance(lineRenderer.GetPosition(i), lineRenderer.GetPosition(i + 1));
        }
        return length;
    }

    Vector3 GetPointAt(float t)
    {
        int pointCount = lineRenderer.positionCount;
        if (t <= 0) return lineRenderer.GetPosition(0);
        if (t >= 1) return lineRenderer.GetPosition(pointCount - 1);

        float totalDistance = t * GetPathLength();
        float distanceCovered = 0.0f;

        for (int i = 0; i < pointCount - 1; i++)
        {
            Vector3 start = lineRenderer.GetPosition(i);
            Vector3 end = lineRenderer.GetPosition(i + 1);
            float segmentDistance = Vector3.Distance(start, end);
            if (distanceCovered + segmentDistance >= totalDistance)
            {
                float segmentT = (totalDistance - distanceCovered) / segmentDistance;
                return Vector3.Lerp(start, end, segmentT);
            }
            distanceCovered += segmentDistance;
        }

        return lineRenderer.GetPosition(pointCount - 1); // Fallback
    }
}
