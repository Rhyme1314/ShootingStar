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
				Debug.LogError("��������Ѿ�����Prefab, Prefab:" + pool.Prefab.name);
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
	//����������سߴ�
	void CheckPoolSize(Pool[] pools)
	{
		foreach (var pool in pools)
		{
			if (pool.RuntimeSize>pool.Size)
			{
				Debug.LogWarning(string.Format("{0}����صĳ�ʼ�ߴ�Ϊ{1}����Ϸ����ʱ�ĳߴ�Ϊ{2}",
				pool.Prefab,
				pool.Size,
				pool.RuntimeSize));
			}
			
		}
	}


	/// <summary>
	/// ���ݴ����prefab ���ض������Ԥ���õ���Ϸ����
	/// </summary>
	/// <param name="prefab">ָ������Ϸ����Ԥ����</param>
	/// <returns>���е���Ϸ����</returns>
	public static GameObject Release(GameObject prefab)
	{
#if UNITY_EDITOR
		if (!dictionary.ContainsKey(prefab))
		{
			Debug.LogError("û��prefab�Ķ����,Prefab:" + prefab.name);
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
			Debug.LogError("û��prefab�Ķ����,Prefab:" + prefab.name);
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
			Debug.LogError("û��prefab�Ķ����,Prefab:" + prefab.name);
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
			Debug.LogError("û��prefab�Ķ����,Prefab:" + prefab.name);
			return null;
		}
#endif
		return dictionary[prefab].PreparedObject(position, rotation, localScale);
	}
}
