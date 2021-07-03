using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
	public int WaveNumber => waveNumber;
	public float TimeBetweenWave => timeBetweenWave;

	[SerializeField] GameObject[] enemyPrefabKinds;
	[SerializeField] GameObject waveUI;
	[SerializeField] private int maxCount = 10;
	[SerializeField] private int minCount = 4;
	[SerializeField] private float timeBetweenSpawn = 0.2f;
	[SerializeField] private float timeBetweenWave = 3f;
	[SerializeField] List<GameObject> aliveEnemyList;
	[SerializeField] private bool spawnEnemy = true;
	 private int waveNumber = 1;//当前波数
	WaitForSeconds waitForTimeBetweenSpawn;
	WaitUntil waitUntilNoEnemy;
	WaitForSeconds waitForTimeBetweenWaves;
	private int enemyCount;
	

	protected override void Awake()
	{
		base.Awake();
		waitForTimeBetweenSpawn = new WaitForSeconds(timeBetweenSpawn);
		waitUntilNoEnemy = new WaitUntil(() => { return aliveEnemyList.Count == 0; });
		waitForTimeBetweenWaves = new WaitForSeconds(timeBetweenWave);
		aliveEnemyList = new List<GameObject>();
	}

	IEnumerator Start()
	{
		while (spawnEnemy)
		{
			yield return waitUntilNoEnemy;
			waveUI.SetActive(true);
			yield return waitForTimeBetweenWaves;
			waveUI.SetActive(false);
			yield return StartCoroutine(nameof(RandomlySpawnCoroutine));
		}
	}

	IEnumerator RandomlySpawnCoroutine()
	{
		enemyCount = Mathf.Clamp(enemyCount, minCount + waveNumber / 3, maxCount);
		for (int i = 0; i < enemyCount; i++)
		{
			var enemy = enemyPrefabKinds[Random.Range(0, enemyPrefabKinds.Length)];//随机取一个敌人的预制体
			aliveEnemyList.Add(PoolManager.Release(enemy, ViewPort.instance.RandomlyEnemySpawnPosition(1f, 1f)));

			yield return waitForTimeBetweenSpawn;
		}
		waveNumber++;
	}

	public void RemoveFromList(GameObject enemy) => aliveEnemyList.Remove(enemy);
}
