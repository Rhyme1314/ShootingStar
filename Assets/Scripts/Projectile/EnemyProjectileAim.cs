using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileAim : Projectile
{
	//private Vector3 target;
	private void Awake()
	{
		target = GameObject.FindGameObjectWithTag("Player");
	}

	protected override void OnEnable()
	{
		StartCoroutine(nameof(MoveDirectionCoroutine));
		base.OnEnable();
		
	}
	//private void OnDisable()
	//{
	//	StopAllCoroutines();
	//}
	IEnumerator MoveDirectionCoroutine()
	{
		yield return null;
		if (target.activeSelf)
		{
			moveDirection = (target.transform.position - transform.position).normalized;
		}
	}
}
