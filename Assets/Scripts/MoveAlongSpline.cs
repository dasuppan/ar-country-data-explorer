using UnityEngine;
using UnityEngine.Splines;

public class MoveAlongSpline: MonoBehaviour
{
    private const float DirectionStepSize = 0.05f;
    
    public SplineContainer container;
    public float timeToTravel = 1f;
    private float distancePercentage = 0f;
    private Vector2 range;
    
    public void Init(SplineContainer container, float startOffset, float timeToTravel = 1f)
    {
        this.container = container;
        this.timeToTravel = timeToTravel;
        range = GetComponentInParent<SplineExtrude>().Range;
        distancePercentage = Mathf.Lerp(range.x, range.y, startOffset);;
        initCompleted = true;
    }

    public bool initCompleted;

    void Update()
    {
        if (!initCompleted) return;
        var splineLength = container.CalculateLength(); // This might be expensive
        var maxSpeed = splineLength / timeToTravel;
        
        distancePercentage += maxSpeed * Time.deltaTime / splineLength;
        distancePercentage = Mathf.Clamp(distancePercentage, range.x, range.y);

        Vector3 currentPosition = container.Spline.EvaluatePosition(distancePercentage);
        transform.position = currentPosition;

        if (distancePercentage + DirectionStepSize > range.y)
        {
            distancePercentage = range.x;
        }

        Vector3 nextPosition = container.EvaluatePosition(distancePercentage + DirectionStepSize);
        Vector3 direction = nextPosition - currentPosition;
        transform.rotation = Quaternion.LookRotation(direction, transform.up);
    }
}
