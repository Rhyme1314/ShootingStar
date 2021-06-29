using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	[SerializeField] protected GameObject hitVFX;
	[SerializeField] protected float damage;
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
	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.TryGetComponent<Character>(out Character character))
		{//·¢ÉúÅö×²
			character.TakeDamage(damage);

			var contactPoint = other.GetContact(0);
			PoolManager.Release(hitVFX, contactPoint.point,Quaternion.LookRotation(contactPoint.normal));
			gameObject.SetActive(false);

		}
	}
}
