using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar : MonoBehaviour
{
	[SerializeField]Image fillImageBack;//��Ӧcurrent
	[SerializeField] Image fillImageFront;//��Ӧtarget
	[SerializeField] private float fillSpeed = 0.1f;
	[SerializeField] private bool delayFill = true;
	[SerializeField] private float delayTime = 0.1f;
	private float currentFillAmout;//��ǰѪ���ٷֱ� 0~1
	private float targetFillAmout;//Ŀ��Ѫ���ٷֱ�  0~1

	WaitForSeconds waitForDelayFill;
	Coroutine bufferedFillCoroutine;
	float t;

	Canvas canvas;
	private void Awake()
	{
		canvas = GetComponent<Canvas>();
		canvas.worldCamera = Camera.main;
		waitForDelayFill = new WaitForSeconds(delayTime);
	}

	public void Initialize(float currentValue,float maxValue)
	{
		currentFillAmout = currentValue / maxValue;
		targetFillAmout = currentFillAmout;
		fillImageBack.fillAmount = currentFillAmout;
		fillImageFront.fillAmount = targetFillAmout;
	}
	//����Ѫ������ 
	public void UpdateStatus(float currentValue, float maxValue)
	{
		if (bufferedFillCoroutine!=null)
			StopCoroutine(bufferedFillCoroutine);
		targetFillAmout = currentValue / maxValue;
		if (currentFillAmout>targetFillAmout)//״̬����ʱ
		{
			fillImageFront.fillAmount = targetFillAmout;
			bufferedFillCoroutine=StartCoroutine(BufferedFillingCoroutine(fillImageBack));
		}
		if (currentFillAmout<targetFillAmout)//״̬����ʱ
		{
			fillImageBack.fillAmount = targetFillAmout;
			bufferedFillCoroutine=StartCoroutine(BufferedFillingCoroutine(fillImageFront));
		}
		
	}
	//��Ѫ/��Ѫ��Ѫ������Ч��
	IEnumerator BufferedFillingCoroutine(Image image)
	{
		if (delayFill)
		{
			yield return waitForDelayFill;
		}
		t = 0f;
		while (t<1f)
		{
			t += Time.deltaTime * fillSpeed;
			currentFillAmout = Mathf.Lerp(currentFillAmout, targetFillAmout, t);
			image.fillAmount = currentFillAmout;

			yield return null;
		}
		
	}
}
