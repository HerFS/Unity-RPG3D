using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/*
 * File     : Node.cs
 * Desc     : ��ã�� �׸��� ��ĭ�� ��� ����
 * Date     : 2024-06-06
 * Writer   : ������
 */
[System.Serializable]
public class Node
{
    public int GridX;
    public int GridZ;

    public bool IsWalkable;
    public Vector3 NodePosition;
    public Node ParentNode;

    public int GCost;
    public int HCost;
    public int FCost
    {
        get { return GCost + HCost; }
    }

    public Node(bool isWalkable, Vector3 position, int gridX, int gridZ)
    {
        this.IsWalkable = isWalkable;
        this.NodePosition = position;
        this.GridX = gridX;
        this.GridZ = gridZ;
    }
}
