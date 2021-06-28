using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	[SerializeField]GameObject deathVFX;
	[Header("-----HEALTH------")]
	[SerializeField]protected float maxHealth;
	protected float health;
	protected virtual void OnEnable()
	{
		health = maxHealth;
	}

	public virtual void TakeDamage(float damage)
	{
		health -= damage;
		if (health<=0f)
		{
			Die();
		}
	}

	public virtual void Die()
	{
		health = 0f;
		PoolManager.Release(deathVFX, transform.position);
		gameObject.SetActive(false);
	}

	public virtual void RestoreHealth(float value)
	{
		if (health == maxHealth)
			return;
		health = Mathf.Clamp(health + value, 0f, maxHealth);
	}
	/// <summary>
	/// 自动回血协程
	/// </summary>
	/// <param name="waitTime">回血间隔</param>
	/// <param name="percent">每次恢复的最大生命值百分比</param>
	/// <returns></returns>
	 protected IEnumerator HealthRegenerateCoroutine(WaitForSeconds waitTime,float percent)
	{
		while (health<=maxHealth)
		{
			yield return waitTime;
			RestoreHealth(maxHealth * percent);
		}
	}
	//持续受伤协程
	protected IEnumerator DamagerOverTimeCoroutine(WaitForSeconds waitTime, float damage)
	{
		while (health >0f)
		{
			yield return waitTime;
			TakeDamage(damage);
		}
	}
}
