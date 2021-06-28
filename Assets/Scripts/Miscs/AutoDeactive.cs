using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDeactive : MonoBehaviour
{
	[SerializeField]private float lifetime = 3f;
	[SerializeField]private bool destoryGameObejct = false;
	private WaitForSeconds waitForLifetime;
	private void Awake()
	{
		waitForLifetime = new WaitForSeconds(lifetime);
	}
	private void OnEnable()
	{
		StartCoroutine(DeactiveCoroutine());
	}
	//等待生命周期协程
	IEnumerator DeactiveCoroutine() {
		yield return waitForLifetime;

		if (destoryGameObejct)
		{
			Destroy(gameObject);
		}
		else
		{
			gameObject.SetActive(false);
		}
	}
}
