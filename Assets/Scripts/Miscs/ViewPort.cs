using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPort : Singleton<ViewPort>
{
	[SerializeField]float minX, maxX, minY, maxY, midX;
	private void Start()
	{
		Camera mainCamera = Camera.main;
		//将视口左下角和右上角的值转换成世界坐标
		Vector2 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector2(0f, 0f));
		Vector2 topRight = mainCamera.ViewportToWorldPoint(new Vector2(1f, 1f));

		minX = bottomLeft.x;
		minY = bottomLeft.y;
		maxX = topRight.x;
		maxY = topRight.y;
		midX = (maxX+minX)/ 2;
	}

	public Vector3 PlayerMoveablePosition(Vector3 playerPosition,float paddingX,float paddingY) {
		Vector3 position = Vector3.zero;
		position.x = Mathf.Clamp(playerPosition.x, minX+paddingX, maxX-paddingX);
		position.y = Mathf.Clamp(playerPosition.y, minY+paddingY, maxY-paddingY);
		return position;
	}

	public Vector3 RandomlyEnemySpawnPosition(float paddingX,float paddingY) {
		Vector3 position = Vector3.zero;
		position.x = maxX+paddingX;
		position.y = Random.Range(minY + paddingY, maxY - paddingY);
		return position;
	}


	public Vector3 RandomlyRightHalfPosition(float paddingX, float paddingY)
	{
		Vector3 position = Vector3.zero;
		position.x = Random.Range(midX, maxX - paddingX);
		position.y = Random.Range(minY + paddingY, maxY - paddingY);
		return position;
	}
}
