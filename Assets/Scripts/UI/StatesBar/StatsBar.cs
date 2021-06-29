using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar : MonoBehaviour
{
	[SerializeField]Image fillImageBack;//对应current
	[SerializeField] Image fillImageFront;//对应target
	[SerializeField] private float fillSpeed = 0.1f;
	[SerializeField] private bool delayFill = true;
	[SerializeField] private float delayTime = 0.1f;
	private float currentFillAmout;//当前血条百分比 0~1
	private float targetFillAmout;//目标血条百分比  0~1

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
	//更新血条长度 
	public void UpdateStatus(float currentValue, float maxValue)
	{
		if (bufferedFillCoroutine!=null)
			StopCoroutine(bufferedFillCoroutine);
		targetFillAmout = currentValue / maxValue;
		if (currentFillAmout>targetFillAmout)//状态减少时
		{
			fillImageFront.fillAmount = targetFillAmout;
			bufferedFillCoroutine=StartCoroutine(BufferedFillingCoroutine(fillImageBack));
		}
		if (currentFillAmout<targetFillAmout)//状态增加时
		{
			fillImageBack.fillAmount = targetFillAmout;
			bufferedFillCoroutine=StartCoroutine(BufferedFillingCoroutine(fillImageFront));
		}
		
	}
	//掉血/加血的血条缓冲效果
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
