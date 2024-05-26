using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CueBallTrajectory : MonoBehaviour
{
    public Transform cueBall; 
    public Transform cue; 
    public float maxDistance = 10f; 
    public LayerMask collisionMask; 

    private LineRenderer lineRenderer;

    void Start()
    {
        
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; 
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
    }

    void Update()
    {
        
        DrawTrajectory();
    }

    void DrawTrajectory()
    {
        
        Vector3 start = cueBall.position;
        Vector3 direction = (cue.position - cueBall.position).normalized;

        
        RaycastHit hit;
        Vector3 end = start + direction * maxDistance;

        if (Physics.Raycast(start, direction, out hit, maxDistance, collisionMask))
        {
            end = hit.point;
        }

        
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        
        Debug.DrawLine(start, end, Color.red);
    }
}
