using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	[SerializeField] protected float moveSpeed = 10f;
	[SerializeField] protected  Vector2 moveDirection;
	protected GameObject target;
	protected virtual void OnEnable()
	{
		StartCoroutine(nameof(MoveDirect));
	}
	IEnumerator MoveDirect() {
		while (gameObject.activeSelf)
		{
			transform.Translate(moveSpeed * moveDirection * Time.deltaTime);
			yield return null;
		}
	}
}
