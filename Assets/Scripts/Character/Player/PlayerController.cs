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
	[SerializeField, Range(0f, 1f)] protected float healthRegeneratePercent;//血量恢复百分比
	[SerializeField] StatsBar_HUD healthHUD;
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
	[SerializeField] private GameObject projectile1;//子弹 中
	[SerializeField] private GameObject projectile2;//子弹 上
	[SerializeField] private GameObject projectile3;//子弹 下
	[SerializeField] private Transform muzzleMid;//枪口 中
	[SerializeField] private Transform muzzleTop;//枪口	上
	[SerializeField] private Transform muzzleBot;//枪口 下
	[SerializeField,Range(0,2)] private int weaponPower = 0;//武器威力
	[SerializeField] private float fireInterval = 0.2f;
	[Header("-----DODGE-----")]
	[SerializeField,Range(0,100)] private int dodgeEnergyCost = 20;
	[SerializeField]private float maxRoll = 720f;//翻转角度--两圈
	[SerializeField] private float rollSpeed = 360f;//翻转速度 --一秒一圈
	private float currentRoll;                      //当前翻转角度
	private Vector3 dodgeScale = new Vector3(0.5f, 0.5f, 0.5f);

	private float dodgeDuration;
	private bool isDodge = false;
	//-------------------OTHERS-----------------------
	private Coroutine moveCoroutine;
	private Coroutine healthRegenerateCoroutine;
	new Rigidbody2D rigidbody;
	new Collider2D collider;
	private WaitForSeconds waitTime;


	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();
		collider = GetComponent<Collider2D>();
	}

	private void Start()
	{
		rigidbody.gravityScale = 0;
		dodgeDuration = maxRoll / rollSpeed;//翻转持续时间

		waitTime = new WaitForSeconds(healthRegenerateInterval);
		healthHUD.Initialize(health, maxHealth);
		input.EnableGameplayInput();
		PlayerEnergy.instance.Obatin(PlayerEnergy.MAX);
	}

	override protected void OnEnable()
	{
		base.OnEnable();
		input.onMove += Move;
		input.onStopMove += StopMove;
		input.onFire += Fire;
		input.onStopFire += StopFire;
		input.onDodge += Dodge;
	}

	private void OnDisable()
	{
		input.onMove -= Move;
		input.onStopMove -= StopMove;
		input.onFire -= Fire;
		input.onStopFire -= StopFire;
		input.onDodge -= Dodge;
	}
	public override void TakeDamage(float damage)
	{
		base.TakeDamage(damage);
		healthHUD.UpdateStatus(health, maxHealth);//更新HUD血条状态
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
	public override void Die()
	{
		healthHUD.UpdateStatus(0f, maxHealth);//更新HUD血条状态
		base.Die();
	}
	public override void RestoreHealth(float value)
	{
		healthHUD.UpdateStatus(health, maxHealth);//更新HUD血条状态
		base.RestoreHealth(value);
	}
	#region 闪避
	private void Dodge()
	{
		if (isDodge || !PlayerEnergy.instance.IsEnough(dodgeEnergyCost))//能量不足或正在翻转
			return;
		StartCoroutine(nameof(DodgeCoroutine));


	}
	IEnumerator DodgeCoroutine()
	{
		//角色无敌
		collider.isTrigger = true;
		isDodge = true;
		PlayerEnergy.instance.Use(dodgeEnergyCost);//消耗能量
		currentRoll = 0f;//重置翻转
		//Vector3 scale = transform.localScale;
		var t1 = 0f; var t2=0f;
		while (currentRoll< maxRoll)    //角色翻转
		{
			currentRoll += rollSpeed * Time.deltaTime;
			transform.rotation = Quaternion.AngleAxis(currentRoll, Vector3.right);
			//角色缩放
			if (currentRoll < maxRoll / 2f)
			{
				t1 += Time.deltaTime;
				transform.localScale = Vector3.Lerp(Vector3.one, dodgeScale, 2f*t1);//匀速缩放
			}
			else
			{
				t2 += Time.deltaTime;
				transform.localScale = Vector3.Lerp(dodgeScale, Vector3.one,2f* t2);
			}
			yield return null;
		}
		//翻转结束
	
		isDodge = false;
		collider.isTrigger = false;
	}
	#endregion
	#region 移动
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
	//移动加速协程
	IEnumerator MoveCoroutine(float time, Vector2 moveVelocity, Quaternion moveRotation)
	{
		float t = 0;
		while (t < 1f)
		{
			t += Time.fixedDeltaTime / time;
			rigidbody.velocity = Vector2.Lerp(rigidbody.velocity, moveVelocity, t );
			if (!isDodge)
			transform.rotation = Quaternion.Lerp(transform.rotation, moveRotation, t);
			yield return null;
		}
	}
	//移动范围限制协程
	IEnumerator PlayerMoveLimitCoroutine()
	{
		while (true)
		{
			rigidbody.transform.position = ViewPort.instance.PlayerMoveablePosition(rigidbody.transform.position, paddingX, paddingY);
			yield return null;
		}
	}
	#endregion
	#region 开火
	private void StopFire()
	{
		StopCoroutine(nameof(FireCoroutine));
		//StopCoroutine(FireCoroutine());//直接使用StopCoroutine会有bug
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

