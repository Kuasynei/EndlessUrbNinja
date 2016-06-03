using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Create a new class that inherits from this pool, and set its type during inheritance. (ex. public class MyClass : UTIL_DynamicObjectPool<MyType>)
public class UTIL_DynamicObjectPool <T> : MonoBehaviour 
{
	List<T> pool = new List<T>();

	public virtual void AddToPool(T someElement)
	{
		pool.Add (someElement);
	}

	public virtual T PopOffPool()
	{
		if (pool.Count > 0)
		{
			T returnValue = pool [0];
			pool.RemoveAt (0);
			return returnValue;
		}
		else
		{
			Debug.LogWarning ("DynamicObjectPool is empty! Cannot PullFromPool(), make sure the pool has enough objects.");
			return default(T);
		}
	}

	public virtual T RemoveIndexFromPool(int i)
	{
		T removedValue = pool [i];
		pool.RemoveAt (i);
		return removedValue;
	}

	public int Count()
	{
		return pool.Count;
	}

	//Returns the object at index 0.
	public T GetNext()
	{
		return pool [0];
	}
}
