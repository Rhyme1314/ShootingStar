using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	[Header("-----MOVE------")]
	[SerializeField] float moveSpeed;
	[SerializeField] float paddingX, paddingY;
	[SerializeField] float rotateAngle = 20f;
	[Header("-----FIRE------")]
	[SerializeField] float minFireInterval, maxFireInterval;
	[SerializeField] GameObject[] projectiles;
	[SerializeField] Transform muzzle;

	private void OnEnable()
	{
		StartCoroutine(nameof(RandomlyMoveingCoroutine));
		StartCoroutine(nameof(RandomlyFireCoroutine));
	}
	private void OnDisable()
	{
		StopAllCoroutines();
	}

	IEnumerator RandomlyMoveingCoroutine()
	{
		yield return null;
		transform.position = ViewPort.instance.RandomlyEnemySpawnPosition(paddingX, paddingY);

		Vector3 targetPos = ViewPort.instance.RandomlyRightHalfPosition(paddingX, paddingY);

		while (gameObject.activeSelf)
		{
			//��������ƶ�
			if (Vector3.Distance(transform.position, targetPos) > Mathf.Epsilon)
			{
				//��δ����Ŀ��λ��
				transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
				transform.rotation = Quaternion.AngleAxis((transform.position - targetPos).normalized.y * rotateAngle, Vector3.right);
			}
			else
			{
				//�Ѿ�������Ŀ��λ��
				targetPos = ViewPort.instance.RandomlyRightHalfPosition(paddingX, paddingY);
			}
			yield return null;
		}

	}

	IEnumerator RandomlyFireCoroutine()
	{
		yield return null;
		while (gameObject.activeSelf)
		{
			yield return new WaitForSeconds(Random.Range(minFireInterval, maxFireInterval));
			foreach (GameObject projectile in projectiles)
			{
				PoolManager.Release(projectile, muzzle.position);
			}
			
		}
	}
}
