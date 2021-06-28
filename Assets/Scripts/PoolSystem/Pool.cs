using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Pool
{
	public GameObject Prefab => prefab;
	public int Size => size;
	public int RuntimeSize => queue.Count;
	[SerializeField] private GameObject prefab;
	[SerializeField] private int size = 1;
	GameObject parent;
	Queue<GameObject> queue;
	public void Initialize(GameObject parent)//��ʼ��
	{
		this.parent = parent;
		queue = new Queue<GameObject>();
		for (int i = 0; i < size; i++)
		{
			queue.Enqueue(Copy());
		}
	}
	GameObject Copy()
	{
		GameObject copy = GameObject.Instantiate(prefab);
		copy.transform.SetParent(parent.transform);
		copy.SetActive(false);
		return copy;
	}
	GameObject AvailableObject()
	{
		GameObject availableObject = null;
		if (queue.Count > 0 && !queue.Peek().activeSelf)//�ж��Ƿ����Ѿ��ڹ�����prefab
		{
			availableObject = queue.Dequeue();
		}
		else
		{
			availableObject = Copy();
		}
		queue.Enqueue(availableObject);//��ȡ�����ö���ʱ��ֱ�����
		return availableObject;
	}
	#region ����
	public GameObject PreparedObject()
	{
		GameObject preparedObject = AvailableObject();
		preparedObject.SetActive(true);
		return preparedObject;
	}
	public GameObject PreparedObject(Vector3 position)
	{
		GameObject preparedObject = AvailableObject();
		preparedObject.transform.position = position;
		preparedObject.SetActive(true);
		return preparedObject;
	}
	public GameObject PreparedObject(Vector3 position, Quaternion rotation)
	{
		GameObject preparedObject = AvailableObject();
		preparedObject.transform.position = position;
		preparedObject.transform.rotation = rotation;
		preparedObject.SetActive(true);
		return preparedObject;
	}
	public GameObject PreparedObject(Vector3 position, Quaternion rotation, Vector3 localScale)
	{
		GameObject preparedObject = AvailableObject();
		preparedObject.transform.position = position;
		preparedObject.transform.rotation = rotation;
		preparedObject.transform.localScale = localScale;
		preparedObject.SetActive(true);
		return preparedObject;
	}
	#endregion



}
