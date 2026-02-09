using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] private float distanceBetweenSegments;
    [SerializeField] private float ropeRadius;
    [SerializeField] private int iterationCount;
    [SerializeField] private int segmentCount;
    [SerializeField] private int ropeFixedUpdateSteps;
    [SerializeField] private Transform ropeStart;
    Nodes[] nodes;
    void Awake()
    {
        nodes = new Nodes[segmentCount];
    }
    private void FixedUpdate()
    {
        for (int i = 0; i < ropeFixedUpdateSteps; i++)
        {
            CalculateNewPositions(Time.fixedDeltaTime / ropeFixedUpdateSteps);
            for (int y = 0; y < iterationCount; y++)
            {
                FixNodeDistances();
                if (y % 2 == 0)
                    ResolveCollision();
            }
        }

    }
    private void CalculateNewPositions(float deltaTime)
    {
        Vector3 gravityStep = Physics.gravity * (float)Mathf.Pow(deltaTime, 2);
        for (int i = 0; i < nodes.Length; i++)
        {
            var currNode = nodes[i];
            var newPreviousPosition = currNode.Position;

            var newPosition = (2 * currNode.Position) - currNode.PrevPosition + gravityStep;

            Vector3 direction = newPosition - currNode.Position;
            float distance = direction.magnitude;
            direction.Normalize();

            if (Physics.SphereCast(currNode.Position, ropeRadius, direction, out RaycastHit hit, distance))
            {
                newPosition = hit.point + hit.normal * ropeRadius;
            }

            nodes[i].PrevPosition = newPreviousPosition;
            nodes[i].Position = newPosition;
        }
    }
    private void FixNodeDistances()
    {

        nodes[0].Position = transform.position;
        for (int i = 0; i < nodes.Length - 1; i++)
        {
            var n1 = nodes[i];
            var n2 = nodes[i + 1];

            var d1 = n1.Position - n2.Position;
            var d2 = d1.magnitude;
            var d3 = (d2 - distanceBetweenSegments) / d2;

            nodes[i].Position -= d1 * (0.5f * d3);
            nodes[i + 1].Position += d1 * (0.5f * d3);
        }
        nodes[nodes.Length - 1].Position = ropeStart.position;

    }
    Collider[] overlapResults = new Collider[32];
    private void ResolveCollision()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            if (i == 0 || i == nodes.Length - 1)
                continue;
            int hitCount = Physics.OverlapSphereNonAlloc(
                nodes[i].Position,
                ropeRadius,
                overlapResults
            );

            for (int h = 0; h < hitCount; h++)
            {
                var col = overlapResults[h];
                if (col == null || col.isTrigger) continue;

                Vector3 closestPoint = col.ClosestPoint(nodes[i].Position);
                float distance = Vector3.Distance(nodes[i].Position, closestPoint);

                if (distance < ropeRadius && distance > 1e-6f)
                {
                    Vector3 penetrationNormal = (nodes[i].Position - closestPoint) / distance;
                    float penetrationDepth = ropeRadius - distance;

                    nodes[i].PrevPosition = nodes[i].Position;
                    nodes[i].Position += penetrationNormal * penetrationDepth * 1.01f;
                }
            }
        }
    }


    void OnDrawGizmos()
    {
        if (nodes == null)
            return;

        foreach (var a in nodes)
        {
            Gizmos.DrawSphere(a.Position, ropeRadius);
        }
    }
}

public struct Nodes
{
    public Vector3 Position;
    public Vector3 PrevPosition;
}
