using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : NodeGrid.cs
 * Desc     : ��ã�� ���� �׸���
 * Date     : 2024-06-18
 * Writer   : ������
 */

public class NodeGrid : MonoBehaviour
{
    private readonly float _nodeSize = 1;

    public Node[,] Grid;
    [SerializeField]
    private List<Node> _nodeList = new List<Node>();
    private int _unwalkableMask;
    
    private int _gridSizeX, _gridSizeZ;
    private float _nodeHalfSize;

    public Vector2Int NumberOfGrids;

    [HideInInspector]
    public Transform StartPos;
    //public List<Node> Path = new List<Node>();

    private void Awake()
    {
        _nodeHalfSize = _nodeSize * 0.5f;

        _gridSizeX = Mathf.RoundToInt(NumberOfGrids.x / _nodeSize);
        _gridSizeZ = Mathf.RoundToInt(NumberOfGrids.y / _nodeSize);

        _unwalkableMask = 1 << (int)EnumTypes.LayerIndex.Unwalkable;

        CreateGrid();
    }

    public void CreateGrid()
    {
        Grid = new Node[_gridSizeX, _gridSizeZ];

        Vector3 bottomLeft = transform.position - (Vector3.right * _gridSizeX / 2) - (Vector3.forward * _gridSizeZ / 2);

        for (int x = 0; x < _gridSizeX; ++x)
        {
            for (int z = 0; z < _gridSizeZ; ++z)
            {
                Vector3 nodePosition = bottomLeft + Vector3.right * (x * _nodeSize + _nodeHalfSize) + Vector3.forward * (z * _nodeSize + _nodeHalfSize);
                bool isWalkable = !Physics.CheckSphere(nodePosition, _nodeHalfSize, _unwalkableMask);
                Grid[x, z] = new Node(isWalkable, nodePosition, x, z);
                _nodeList.Add(Grid[x, z]);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; ++x)
        {
            for (int z = -1; z <= 1; ++z)
            {
                if (x == 0 && z == 0)
                {
                    continue;
                }

                int checkX = node.GridX + x;
                int checkZ = node.GridZ + z;

                if (checkX > -1 && checkX < _gridSizeX && checkZ > -1 && checkZ < _gridSizeZ)
                {
                    neighbours.Add(Grid[checkX, checkZ]);
                }
            }
        }
        return neighbours;
    }

    public Node GetNodePosition(Vector3 position)
    {
        int _x = 0;
        int _z = 0;

        for (int x = 0; x < _gridSizeX; ++x)
        {
            for (int z = 0; z < _gridSizeZ; ++z)
            {
                if (Mathf.CeilToInt(Grid[x, z].NodePosition.x) == Mathf.CeilToInt(position.x))
                {
                    _x = x;
                }
                else if (Mathf.CeilToInt(Grid[x, z].NodePosition.z) == Mathf.CeilToInt(position.z))
                {
                    _z = z;
                }
            }
        }


        return Grid[_x, _z];
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(NumberOfGrids.x, 1, NumberOfGrids.y));

        //if (Grid != null)
        //{

        //    Node playernode = GetNodePosition(StartPos.position);

        //    foreach (Node n in Grid)
        //    {
        //        Gizmos.color = (n.IsWalkable) ? Color.white : Color.red;

        //        if (playernode == n)
        //        {
        //            Gizmos.color = Color.black;
        //        }
        //        else
        //        {
        //            if (Path != null && Path.Contains(n))
        //                Gizmos.color = Color.black;
        //        }

        //        Gizmos.DrawCube(n.NodePosition, Vector3.one * (_nodeSize - 0.1f));
        //    }
        //}
    }
}