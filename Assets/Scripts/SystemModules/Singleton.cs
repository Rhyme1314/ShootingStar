using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T:Component  //T必须的是组件 才能写成单例
{
	public static T instance { get; private set; }
	protected virtual void Awake()
	{
		instance = this as T;
	}
}
