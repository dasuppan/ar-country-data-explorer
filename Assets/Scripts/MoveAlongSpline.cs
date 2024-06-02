using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class MoveAlongSpline: MonoBehaviour
{
    public SplineContainer container;
    // Between 0 and 1
    public float startOffset;
    public float timeToTravel = 1f;
    //public float speed = 1f;
    private float distancePercentage = 0f;
    

    /*void Start()
    {
        splineLength = container.CalculateLength();
        distancePercentage = startOffset;
    }*/
    
    public void Init(SplineContainer container, float startOffset, float timeToTravel)
    {
        this.container = container;
        this.startOffset = startOffset;
        this.timeToTravel = timeToTravel;
        distancePercentage = startOffset;
        initCompleted = true;
    }

    public bool initCompleted;

    void Update()
    {
        if (!initCompleted) return;
        var splineLength = container.CalculateLength();
        var maxSpeed = splineLength / timeToTravel;
        distancePercentage += maxSpeed * Time.deltaTime / splineLength;

        Vector3 currentPosition = container.Spline.EvaluatePosition(distancePercentage);
        /*Debug.Log(currentPosition);
        Debug.Log(distancePercentage);*/
        transform.position = currentPosition;

        if (distancePercentage > 1f)
        {
            distancePercentage = 0f;
        }

        Vector3 nextPosition = container.EvaluatePosition(distancePercentage + 0.05f);
        Vector3 direction = nextPosition - currentPosition;
        transform.rotation = Quaternion.LookRotation(direction, transform.up);
    }
}