using UnityEngine;
using System.Collections;
using Pathfinding;
public class WanderingDestinationSetter : MonoBehaviour
{
    public float radius = 20;
    IAstarAI ai;
    void Start()
    {
        ai = GetComponent<IAstarAI>();
    }

    public Vector3 GetRandomPoint()
    {
        bool flag = true;
        Vector3 pos = new Vector3();
        while (flag)
        {
            GridGraph gridGraph = GridNode.GetGridGraph(0);
            GridNodeBase gridNode = gridGraph.GetNode(Random.Range(0, gridGraph.width), Random.Range(0, gridGraph.depth));
            pos = (Vector3)gridNode.position;
            flag = !gridNode.Walkable;
        }
        return pos;
    }

    Vector3 PickRandomPoint()
    {
        //var point = Random.insideUnitSphere * radius;
        var point = GetRandomPoint();
        point.y = 0;
        point += ai.position;
        return point;
    }

    void Update()
    {
        // Update the destination of the AI if
        // the AI is not already calculating a path and
        // the ai has reached the end of the path or it has no path at all
        if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
        {
            ai.destination = PickRandomPoint();
            ai.SearchPath();
        }
    }
}