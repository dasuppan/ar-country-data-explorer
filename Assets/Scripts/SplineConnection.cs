using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

namespace DefaultNamespace
{
    public class SplineConnection : MonoBehaviour
    {
        public Transform fromTransform;
        public Transform toTransform;
        private SplineContainer splineContainer;
        private Spline connectingSpline;

        private void Start()
        {
            splineContainer = gameObject.AddComponent<SplineContainer>();
            connectingSpline = splineContainer.AddSpline();
            connectingSpline.AddRange(new[]
            {
                new BezierKnot(), 
                new BezierKnot(), 
                new BezierKnot()
            });
            // startKnot. linear out?
            // endKnot. linear out?
        }
        
        // TODO: Spline height?
        // TODO: Add SplineExtrude

        private void Update()
        {
            if (toTransform == null || fromTransform == null) return;
            // We assume a three knotted spline
            var knot0 = connectingSpline.Knots.ToArray()[0];
            var knot1 = connectingSpline.Knots.ToArray()[1];
            var knot2 = connectingSpline.Knots.ToArray()[2];

            /*knot0.Position = connectingSpline.transform.InverseTransformPoint(fromTransform.position);
            knot0.Rotation = Quaternion.Inverse(connectingSpline.transform.rotation) * fromTransform.rotation;
            connectingSpline.Spline.SetKnot(0, knot0);

            var middlePos = toTransform.position - fromTransform.position;

            knot1.Position = connectingSpline.transform.InverseTransformPoint(middlePos);
            knot1.Rotation = Quaternion.Inverse(connectingSpline.transform.rotation) * toTransform.rotation;
            connectingSpline.Spline.SetKnot(1, knot1);

            knot2.Position = connectingSpline.transform.InverseTransformPoint(toTransform.position);
            knot2.Rotation = Quaternion.Inverse(connectingSpline.transform.rotation) * toTransform.rotation;
            connectingSpline.Spline.SetKnot(2, knot2);*/

            knot0.Position = transform.InverseTransformPoint(fromTransform.position);
            connectingSpline.SetKnot(0, knot0);
            knot1.Position = transform.InverseTransformPoint(toTransform.position - fromTransform.position);
            connectingSpline.SetKnot(0, knot1);
            knot2.Position = transform.InverseTransformPoint(toTransform.position);
            connectingSpline.SetKnot(0, knot2);
            
        }
    }
}