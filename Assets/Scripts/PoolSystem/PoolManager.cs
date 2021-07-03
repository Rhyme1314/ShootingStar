using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
	[SerializeField] private Pool[] playerProjectilePools;
	[SerializeField] private Pool[] enemyProjectilePools;
	[SerializeField] private Pool[] vFXPools;
	[SerializeField] private Pool[] enemyPools;
	private static Dictionary<GameObject, Pool> dictionary;
	private void Awake()
	{
		dictionary = new Dictionary<GameObject, Pool>();
		Initialize(playerProjectilePools);
		Initialize(enemyProjectilePools);
		Initialize(vFXPools);
		Initialize(enemyPools);
	}
	void Initialize(Pool[] pools)
	{
		foreach (Pool pool in pools)
		{
#if UNITY_EDITOR
			if (dictionary.ContainsKey(pool.Prefab))
			{
				Debug.LogError("对象池中已经存在Prefab, Prefab:" + pool.Prefab.name);
				continue;
			}
#endif
			dictionary.Add(pool.Prefab, pool);
			GameObject parent = new GameObject("Pool:" + pool.Prefab.name);
			parent.transform.SetParent(transform);
			pool.Initialize(parent);
		}
	}
#if UNITY_EDITOR
	private void OnDisable()
	{
		CheckPoolSize(enemyPools);
		CheckPoolSize(playerProjectilePools);
		CheckPoolSize(enemyProjectilePools);
		CheckPoolSize(vFXPools);
	}
#endif
	//用来检测对象池尺寸
	void CheckPoolSize(Pool[] pools)
	{
		foreach (var pool in pools)
		{
			if (pool.RuntimeSize>pool.Size)
			{
				Debug.LogWarning(string.Format("{0}对象池的初始尺寸为{1}，游戏结束时的尺寸为{2}",
				pool.Prefab,
				pool.Size,
				pool.RuntimeSize));
			}
			
		}
	}


	/// <summary>
	/// 根据传入的prefab 返回对象池中预备好的游戏对象
	/// </summary>
	/// <param name="prefab">指定的游戏对象预制体</param>
	/// <returns>池中的游戏对象</returns>
	public static GameObject Release(GameObject prefab)
	{
#if UNITY_EDITOR
		if (!dictionary.ContainsKey(prefab))
		{
			Debug.LogError("没有prefab的对象池,Prefab:" + prefab.name);
			return null;
		}
#endif
		return dictionary[prefab].PreparedObject();
	}
	public static GameObject Release(GameObject prefab, Vector3 position)
	{
#if UNITY_EDITOR
		if (!dictionary.ContainsKey(prefab))
		{
			Debug.LogError("没有prefab的对象池,Prefab:" + prefab.name);
			return null;
		}
#endif
		return dictionary[prefab].PreparedObject(position);
	}
	public static GameObject Release(GameObject prefab, Vector3 position, Quaternion rotation)
	{
#if UNITY_EDITOR
		if (!dictionary.ContainsKey(prefab))
		{
			Debug.LogError("没有prefab的对象池,Prefab:" + prefab.name);
			return null;
		}
#endif
		return dictionary[prefab].PreparedObject(position, rotation);
	}
	public static GameObject Release(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 localScale)
	{
#if UNITY_EDITOR
		if (!dictionary.ContainsKey(prefab))
		{
			Debug.LogError("没有prefab的对象池,Prefab:" + prefab.name);
			return null;
		}
#endif
		return dictionary[prefab].PreparedObject(position, rotation, localScale);
	}
}
