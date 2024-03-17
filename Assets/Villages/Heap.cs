using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T>
{
	T[] items;
	int currentItemCount;

	public Heap(int maxHeapSize)
	{
		items = new T[maxHeapSize];
	}

	public void Add(T item)
	{
		item.HeapIndex = currentItemCount;
		items[currentItemCount] = item;

		SortUp(item);
		currentItemCount++;
	}

	void SortUp(T item)
	{
		int parentIndex = (item.HeapIndex - 1) / 2;
	}
}

public interface IHeapItem<T> : IComparable<T>
{
	int HeapIndex
	{
		get;
		set;
	}
}