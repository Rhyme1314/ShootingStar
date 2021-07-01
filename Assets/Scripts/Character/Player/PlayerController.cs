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
	[SerializeField] private GameObject projectile1;//�ӵ� ��
	[SerializeField] private GameObject projectile2;//�ӵ� ��
	[SerializeField] private GameObject projectile3;//�ӵ� ��
	[SerializeField] private Transform muzzleMid;//ǹ�� ��
	[SerializeField] private Transform muzzleTop;//ǹ��	��
	[SerializeField] private Transform muzzleBot;//ǹ�� ��
	[SerializeField,Range(0,2)] private int weaponPower = 0;//��������
	[SerializeField] private float fireInterval = 0.2f;
	[Header("-----DODGE-----")]
	[SerializeField,Range(0,100)] private int dodgeEnergyCost = 20;
	[SerializeField]private float maxRoll = 720f;//��ת�Ƕ�--��Ȧ
	[SerializeField] private float rollSpeed = 360f;//��ת�ٶ� --һ��һȦ
	private float currentRoll;                      //��ǰ��ת�Ƕ�
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
		dodgeDuration = maxRoll / rollSpeed;//��ת����ʱ��

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
		healthHUD.UpdateStatus(health, maxHealth);//����HUDѪ��״̬
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
		healthHUD.UpdateStatus(0f, maxHealth);//����HUDѪ��״̬
		base.Die();
	}
	public override void RestoreHealth(float value)
	{
		healthHUD.UpdateStatus(health, maxHealth);//����HUDѪ��״̬
		base.RestoreHealth(value);
	}
	#region ����
	private void Dodge()
	{
		if (isDodge || !PlayerEnergy.instance.IsEnough(dodgeEnergyCost))//������������ڷ�ת
			return;
		StartCoroutine(nameof(DodgeCoroutine));


	}
	IEnumerator DodgeCoroutine()
	{
		//��ɫ�޵�
		collider.isTrigger = true;
		isDodge = true;
		PlayerEnergy.instance.Use(dodgeEnergyCost);//��������
		currentRoll = 0f;//���÷�ת
		//Vector3 scale = transform.localScale;
		var t1 = 0f; var t2=0f;
		while (currentRoll< maxRoll)    //��ɫ��ת
		{
			currentRoll += rollSpeed * Time.deltaTime;
			transform.rotation = Quaternion.AngleAxis(currentRoll, Vector3.right);
			//��ɫ����
			if (currentRoll < maxRoll / 2f)
			{
				t1 += Time.deltaTime;
				transform.localScale = Vector3.Lerp(Vector3.one, dodgeScale, 2f*t1);//��������
			}
			else
			{
				t2 += Time.deltaTime;
				transform.localScale = Vector3.Lerp(dodgeScale, Vector3.one,2f* t2);
			}
			yield return null;
		}
		//��ת����
	
		isDodge = false;
		collider.isTrigger = false;
	}
	#endregion
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
		while (t < 1f)
		{
			t += Time.fixedDeltaTime / time;
			rigidbody.velocity = Vector2.Lerp(rigidbody.velocity, moveVelocity, t );
			if (!isDodge)
			transform.rotation = Quaternion.Lerp(transform.rotation, moveRotation, t);
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

