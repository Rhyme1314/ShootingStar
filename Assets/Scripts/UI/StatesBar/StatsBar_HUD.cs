using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar_HUD : StatsBar
{
	[SerializeField]Text currentHealthPercent;
	void SetHealthPercentText()
	{
		currentHealthPercent.text = Mathf.RoundToInt(targetFillAmout*100f) + "%";
	}
	public override void Initialize(float currentValue, float maxValue)
	{
		base.Initialize(currentValue, maxValue);
		SetHealthPercentText();
	}
	public override void UpdateStatus(float currentValue, float maxValue)
	{
		base.UpdateStatus(currentValue, maxValue);
		SetHealthPercentText();
	}
}
