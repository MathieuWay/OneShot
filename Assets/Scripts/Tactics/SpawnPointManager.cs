using UnityEngine;
using System.Collections.Generic;


public static class SpawnPointManager
{
	public static List<PointData> PointDatas { get; private set; } = new List<PointData>();

	public static PointData[] LoadPoints()
	{
		return PointDatas.ToArray();
	}

	public static void AddPoint(int id, float time, Vector2 position, Vector3 spawnPosition)
	{
		PointData pointData = new PointData(id, time, position, spawnPosition);

		PointDatas.Add(pointData);
	}

	public static void ClearPoints()
	{
		PointDatas.Clear();
	}
}

public class PointData
{
	public PointData(int id, float time, Vector2 position, Vector3 spawnPosition)
	{
		_ID = id;
		_Time = time;
		_Position = position;
		_SpawnPosition = spawnPosition;
	}

	public int _ID { get; set; }
	public float _Time { get; private set; }
	public Vector2 _Position { get; private set; }
	public Vector3 _SpawnPosition { get; private set; }
}

