using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : PriorityQueue.cs
 * Desc     : 우선순위 큐
 * Date     : 2024-07-11
 * Writer   : 정지훈
 */

public class PriorityQueue
{
    public int MaxElement;

    public Node[] Heap;
    public int CurrentSize;

    public void Initialize()
    {
        Heap = new Node[MaxElement];
    }
    public bool IsEmpty()
    {
        return CurrentSize == 0;
    }

    public bool IsFull()
    {
        return CurrentSize == MaxElement;
    }

    public void Enqueue(int fCost, Node node)
    {
        if (IsFull())
        {
            return;
        }

        int index = ++(CurrentSize);

        while ((index != 1) && fCost < Heap[index / 2].FCost)
        {
            Heap[index] = Heap[index / 2];
            index /= 2;
        }

        Heap[index] = node;
    }

    public void Dequeue()
    {
        if (IsEmpty())
        {
            return;
        }

        int parent = 1;
        int child = 2;
        Node temp = Heap[CurrentSize--];

        while (child <= CurrentSize)
        {
            if ((child < CurrentSize) && (Heap[child].FCost > Heap[child + 1].FCost))
            {
                ++child;
            }

            if (temp.FCost <= Heap[child].FCost)
            {
                break;
            }

            Heap[parent] = Heap[child];

            parent = child;
            child *= 2;
        }

        Heap[parent] = temp;
    }
}
