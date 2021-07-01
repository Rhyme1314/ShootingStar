using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : Singleton<PlayerEnergy>
{
	public const int MAX = 100;
	public const int PERCENT = 1;
	[SerializeField] EnergyBar energyBar;
	private int energy = 0;
	private void Start()
	{
		energyBar.Initialize(energy, MAX);
	}

	/// <summary>
	/// 获得能量值
	/// </summary>
	/// <param name="value">能量值</param>
	public void Obatin(int value)
	{
		if (energy == MAX)
			return;
		energy = Mathf.Clamp(energy + value, 0, MAX);
		energyBar.UpdateStatus(energy, MAX);
	}

	public void Use(int value)
	{
		energy -= value;
		energyBar.UpdateStatus(energy, MAX);
	}

	public bool IsEnough(int value) => energy >= value;
}
