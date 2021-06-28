using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : Character
{
	[SerializeField] private bool healthRegenerate = true;
	[SerializeField] private float healthRegenerateInterval;
	[SerializeField, Range(0f, 1f)] protected float healthRegeneratePercent;//Ѫ���ָ��ٷֱ�
	[Header("-----INPUT-----")]
	[SerializeField] PlayerInput input;
	[Header("-----MOVE-----")]
	[SerializeField] float moveSpeed = 10f;
	[SerializeField] private float paddingX = 0.2f;
	[SerializeField] private float paddingY = 0.2f;
	[SerializeField] private float acclerationTime = 3f;
	[SerializeField] private float declerationTime = 3f;
	[SerializeField] private float moveRotationAngle = 50f;
	[Header("-----FIRE-----")]
	[SerializeField] private GameObject projectile1;//�ӵ� ��
	[SerializeField] private GameObject projectile2;//�ӵ� ��
	[SerializeField] private GameObject projectile3;//�ӵ� ��
	[SerializeField] private Transform muzzleMid;//ǹ�� ��
	[SerializeField] private Transform muzzleTop;//ǹ��	��
	[SerializeField] private Transform muzzleBot;//ǹ�� ��
	[SerializeField,Range(0,2)] private int weaponPower = 0;//��������
	[SerializeField] private float fireInterval = 0.2f;
	//-------------------OTHERS-----------------------
	private Coroutine moveCoroutine;
	private Coroutine healthRegenerateCoroutine;
	new Rigidbody2D rigidbody;
	private WaitForSeconds waitTime;


	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		rigidbody.gravityScale = 0;
		waitTime = new WaitForSeconds(healthRegenerateInterval);
		
		input.EnableGameplayInput();
	}

	override protected void OnEnable()
	{
		base.OnEnable();
		input.onMove += Move;
		input.onStopMove += StopMove;
		input.onFire += Fire;
		input.onStopFire += StopFire;
	}



	private void OnDisable()
	{
		input.onMove -= Move;
		input.onStopMove -= StopMove;
		input.onFire -= Fire;
		input.onStopFire -= StopFire;
	}
	public override void TakeDamage(float damage)
	{
		base.TakeDamage(damage);
		if (gameObject.activeSelf)
		{
			if (healthRegenerate)
			{
				if (healthRegenerateCoroutine!=null)
				{
					StopCoroutine(healthRegenerateCoroutine);
				}
				healthRegenerateCoroutine=StartCoroutine(HealthRegenerateCoroutine(waitTime,healthRegeneratePercent));
			}
		}
	}

	#region �ƶ�
	private void StopMove()
	{
		if (moveCoroutine != null)
			StopCoroutine(moveCoroutine);
		moveCoroutine = StartCoroutine(MoveCoroutine(declerationTime, Vector2.zero, Quaternion.identity));
		StopCoroutine(PlayerMoveLimitCoroutine());
	}

	private void Move(Vector2 moveInput)
	{
		if (moveCoroutine != null)
			StopCoroutine(moveCoroutine);
		Quaternion moveRotation = Quaternion.AngleAxis(moveInput.y * moveRotationAngle, Vector3.right);
		moveCoroutine = StartCoroutine(MoveCoroutine(acclerationTime, moveInput.normalized * moveSpeed, moveRotation));
		StartCoroutine(PlayerMoveLimitCoroutine());
	}
	//�ƶ�����Э��
	IEnumerator MoveCoroutine(float time, Vector2 moveVelocity, Quaternion moveRotation)
	{
		float t = 0;
		while (t < time)
		{
			t += Time.fixedDeltaTime / time;
			rigidbody.velocity = Vector2.Lerp(rigidbody.velocity, moveVelocity, t / time);
			transform.rotation = Quaternion.Lerp(transform.rotation, moveRotation, t / time);
			yield return null;
		}
	}
	//�ƶ���Χ����Э��
	IEnumerator PlayerMoveLimitCoroutine()
	{
		while (true)
		{
			rigidbody.transform.position = ViewPort.instance.PlayerMoveablePosition(rigidbody.transform.position, paddingX, paddingY);
			yield return null;
		}
	}
	#endregion
	#region ����
	private void StopFire()
	{
		StopCoroutine(nameof(FireCoroutine));
		//StopCoroutine(FireCoroutine());//ֱ��ʹ��StopCoroutine����bug
	}
	private void Fire()
	{
		StartCoroutine(nameof(FireCoroutine));
	}
	IEnumerator FireCoroutine()
	{
		WaitForSeconds waitForFireInterval = new WaitForSeconds(fireInterval);
		while (true)
		{
			switch (weaponPower)
			{
				case 0:
					PoolManager.Release(projectile1, muzzleMid.position, Quaternion.identity);
					break;
				case 1:
					PoolManager.Release(projectile1, muzzleTop.position, Quaternion.identity);
					PoolManager.Release(projectile1, muzzleBot.position, Quaternion.identity);
					break;
				case 2:
					PoolManager.Release(projectile2, muzzleTop.position, Quaternion.identity);
					PoolManager.Release(projectile1, muzzleMid.position, Quaternion.identity);
					PoolManager.Release(projectile3, muzzleBot.position, Quaternion.identity);
					break;	
				default:
					break;
			}
			yield return waitForFireInterval;
		}
	}
	#endregion
}

