using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
	[SerializeField] private int destoryEnergyBonus = 3;
	public override void Die()
	{
		PlayerEnergy.instance.Obatin(destoryEnergyBonus);
		base.Die();
	}
}
