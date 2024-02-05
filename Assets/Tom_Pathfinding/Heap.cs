using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T>//  -----CURENTLY SOMETHING WRONG HERE-------
{
    T[] m_items;
    int m_itemCount;

    public int Count
    {
        get { return m_itemCount; }
    }

    public Heap(int maxHeapSize)
    {
        m_items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = m_itemCount;
        m_items[m_itemCount] = item;
        SortUp(item);
        m_itemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = m_items[0];
        m_itemCount--;

        m_items[0] = m_items[m_itemCount];
        m_items[0].HeapIndex = 0;

        SortDown(m_items[0]);
        return firstItem;
    }

    public void Update(T item)
    { 
        SortUp(item); //with pathfinding, we only ever increase priority
    }

    public bool Contains(T item)
    {
        return Equals(m_items[item.HeapIndex], item);
    }

    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2; //formula for finding parent node in heap

        while(true)
        {
            T parentItem = m_items[parentIndex];
            if(item.CompareTo(parentItem) > 0) //if item has higher priorety (lower f cost), return 1
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }
        }
    }

    void SortDown(T item)
    {
        while(true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1; //formula for finding left child node in heap
            int childIndexRight = item.HeapIndex * 2 + 2; //formula for finding right child node in heap
            int swapIndex = 0;

            //set swapIndex to child with ighest priority
            if(childIndexLeft < m_itemCount)
            {
                swapIndex = childIndexLeft;

                if(childIndexRight < m_itemCount)
                {
                    if (m_items[childIndexLeft].CompareTo(m_items[childIndexRight]) < 0)
                    {
                        //lower priority
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(m_items[swapIndex]) < 0)
                {
                    Swap(item, m_items[swapIndex]);
                }
                else
                {
                    //parent has highest priority
                    return;
                }
            }
            else
            {
                //parent has no children
                return;
            }
        }
    }

    void Swap(T itemA, T itemB)
    {
        m_items[itemA.HeapIndex] = itemB;
        m_items[itemB.HeapIndex] = itemA;

        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }

}

// using an interface here as each item needs to keep track of its own index,
// also useful to compare two items
public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get; set;
    }

}

