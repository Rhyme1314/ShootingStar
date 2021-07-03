using UnityEngine;
using UnityEngine.UI;

public class WaveUI : MonoBehaviour
{
	Text waveText;
	private void Awake()
	{
		GetComponent<Canvas>().worldCamera = Camera.main;
		waveText = GetComponentInChildren<Text>();
	}
	private void OnEnable()
	{
		waveText.text = "-Wave" + EnemyManager.instance.WaveNumber + "-";
	}
}
