using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/*
 * File     : PathFinding.cs
 * Desc     : 길찾기
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

public class PathFinding : MonoBehaviour
{
    private Vector3 _cacheStart, _cacheDest;
    public NodeGrid NodeGrid;

    public Transform StartPos;
    public Transform Target;

    public List<Node> Path;


    private void Awake()
    {
        StartPos = this.transform;
        Target = GameManager.Instance.Player;
    }

    public void UpdatePath()
    {
        if (StartPos.position != _cacheStart || Target.position != _cacheDest)
        {
            FindPath(StartPos.position, Target.position);

            _cacheStart = StartPos.position;
            _cacheDest = Target.position;
        }
    }


    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = NodeGrid.GetNodePosition(startPos);
        Node targetNode = NodeGrid.GetNodePosition(targetPos);

        PriorityQueue openSet = new PriorityQueue();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.MaxEelemnt = NodeGrid.NumberOfGrids.x * NodeGrid.NumberOfGrids.y;
        openSet.Initialize();
        openSet.Enqueue(startNode.FCost, startNode);

        while (openSet.CurrentSize > 0)
        {
            Node currentNode = openSet.Heap[1];

            // 가장 낮은 값을 가진 노드를 선택한다.
            for (int i = 1; i < openSet.CurrentSize; ++i)
            {
                if (currentNode.FCost > openSet.Heap[i].FCost || (currentNode.FCost == openSet.Heap[i].FCost && currentNode.HCost > openSet.Heap[i].HCost))
                {
                    currentNode = openSet.Heap[i];
                }
            }

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            // 현재 노드를 오픈 셋에서 빼서 클로즈드 셋으로 이동한다.
            openSet.Dequeue();
            closedSet.Add(currentNode);

            // 이웃노드를 가져와서 값을 계산한 후 오픈 셋에 추가한다.
            foreach (var n in NodeGrid.GetNeighbours(currentNode))
            {
                if (!n.IsWalkable || closedSet.Contains(n))
                {
                    continue;
                }

                int g = currentNode.GCost + GetDistance(currentNode, n);
                int h = GetDistance(n, targetNode);
                int f = g + h;

                // 오픈 셋에 이미 중복 노드가 있는 경우 값이 작은 쪽으로 변경한다.
                if (Array.IndexOf(openSet.Heap, n) == -1)
                {
                    n.GCost = g;
                    n.HCost = h;
                    n.ParentNode = currentNode;
                    openSet.Enqueue(n.FCost, n);
                }
                else
                {
                    if (n.FCost > f)
                    {
                        n.GCost = g;
                        n.ParentNode = currentNode;
                    }
                }
            }
        }
    }

    //void FindPath(Vector3 startPos, Vector3 targetPos)
    //{
    //    Stopwatch sw = new Stopwatch();

    //    Node startNode = NodeGrid.GetNodePosition(startPos);
    //    Node targetNode = NodeGrid.GetNodePosition(targetPos);

    //    List<Node> openSet = new List<Node>();
    //    HashSet<Node> closedSet = new HashSet<Node>();
    //    sw.Reset();
    //    sw.Start();
    //    openSet.Add(startNode);
    //    sw.Stop();
    //    print(sw.ElapsedTicks + "ticks");
    //    while (openSet.Count > 0)
    //    {
    //        #region 
    //        Node currentNode = openSet[0];
    //        for (int i = 1; i < openSet.Count; i++)
    //        {
    //            if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
    //            {
    //                currentNode = openSet[i];
    //            }
    //        }
    //        #endregion

    //        #region 가장 낮은 값을 가진 노드가 종착노드면 탐색을 종료한다.
    //        if (currentNode == targetNode)
    //        {
    //            RetracePath(startNode, targetNode);
    //            return;
    //        }
    //        #endregion

    //        #region 
    //        openSet.Remove(currentNode);
    //        closedSet.Add(currentNode);
    //        #endregion

    //        #region 
    //        foreach (Node n in NodeGrid.GetNeighbours(currentNode))
    //        {
    //            if (!n.IsWalkable || closedSet.Contains(n))
    //            {
    //                continue;
    //            }

    //            int g = currentNode.GCost + GetDistance(currentNode, n);
    //            int h = GetDistance(n, targetNode);
    //            int f = g + h;

    //            // 
    //            if (!openSet.Contains(n))
    //            {
    //                n.GCost = g;
    //                n.HCost = h;
    //                n.ParentNode = currentNode;
    //                openSet.Add(n);
    //            }
    //            else
    //            {
    //                if (n.FCost > f)
    //                {
    //                    n.GCost = g;
    //                    n.ParentNode = currentNode;
    //                }
    //            }
    //        }
    //        #endregion
    //    }
    //}

    private void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.ParentNode;
        }

        for (int i = 0; i < path.Count - 1; ++i)
        {
            path[i] = path[i + 1];
        }

        path.Reverse();
        Path = path;
        //NodeGrid.Path = path;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int dstZ = Mathf.Abs(nodeA.GridZ - nodeB.GridZ);

        if (dstX > dstZ)
        {
            return 14 * dstZ + 10 * (dstX - dstZ);
        }

        return 14 * dstX + 10 * (dstZ - dstX);
    }
}
