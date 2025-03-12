using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
	[SerializeField] private Transform player;
	[SerializeField] private float spawnDistanceAhead = 50f;
	[SerializeField] private float deleteDistanceBehind = 30f;
	[SerializeField] private float connectHeight = 0.2f;
	[SerializeField] private List<GameObject> terrainSegmentPrefabs;
	[SerializeField] private int initialSegments = 1;
	[SerializeField] private bool forceSpawnSegment = false;

	private Vector2 lastEndPoint = Vector2.zero;
	private bool isInitialized = false;
	private Dictionary<int, Queue<GameObject>> segmentPools = new Dictionary<int, Queue<GameObject>>();
	private List<GameObject> activeSegments = new List<GameObject>();
	private Camera mainCamera;

	void Start()
	{
		mainCamera = Camera.main;
		InitializeSegmentPools();
		SpawnInitialSegments();
		isInitialized = true;
	}

	void Update()
	{
		if (!isInitialized || player == null) return;
		if (forceSpawnSegment)
		{
			forceSpawnSegment = false;
			SpawnRandomSegment();
		}
		ManageSegments();
	}

	private void InitializeSegmentPools()
	{
		segmentPools.Clear();
		for (int i = 0; i < terrainSegmentPrefabs.Count; i++)
		{
			if (!segmentPools.ContainsKey(i))
			{
				segmentPools[i] = new Queue<GameObject>();
				for (int j = 0; j < initialSegments; j++)
				{
					GameObject segment = Instantiate(terrainSegmentPrefabs[i], transform);
					segment.SetActive(false);
					segmentPools[i].Enqueue(segment);
				}
			}
		}
	}

	private void SpawnInitialSegments()
	{
		SpawnSegmentByIndex(0);
		for (int i = 0; i < initialSegments; i++)
		{
			SpawnRandomSegment();
		}
	}

	private void SpawnSegmentByIndex(int segmentIndex)
	{
		GameObject newSegment = GetSegmentFromPool(segmentIndex);
		TerrainSegment segmentComponent = newSegment.GetComponent<TerrainSegment>();
		if (segmentComponent != null)
		{
			PositionSegmentAtEndPoint(newSegment, segmentComponent);
			lastEndPoint = segmentComponent.EndPoint.position;
			activeSegments.Add(newSegment);
		}
	}

	private void ManageSegments()
	{
		if (ShouldSpawnNewSegment())
		{
			SpawnRandomSegment();
		}
		RemoveDistantSegments();
	}

	private bool ShouldSpawnNewSegment()
	{
		if (activeSegments.Count == 0) return true;
		Vector2 farthestPoint = Vector2.zero;
		foreach (GameObject segment in activeSegments)
		{
			TerrainSegment segmentComponent = segment.GetComponent<TerrainSegment>();
			if (segmentComponent != null && segmentComponent.EndPoint.position.x > farthestPoint.x)
			{
				farthestPoint.x = segmentComponent.EndPoint.position.x;
			}
		}
		float distanceToFarthest = farthestPoint.x - player.position.x;
		return distanceToFarthest < spawnDistanceAhead;
	}

	private void RemoveDistantSegments()
	{
		List<GameObject> segmentsToRemove = new List<GameObject>();
		foreach (GameObject segment in activeSegments)
		{
			TerrainSegment segmentComponent = segment.GetComponent<TerrainSegment>();
			if (segmentComponent == null) continue;
			if (player.position.x - segmentComponent.EndPoint.position.x > deleteDistanceBehind)
			{
				segmentsToRemove.Add(segment);
			}
		}
		foreach (GameObject segment in segmentsToRemove)
		{
			ReturnSegmentToPool(segment);
		}
	}

	private void SpawnRandomSegment()
	{
		int segmentIndex = Random.Range(0, terrainSegmentPrefabs.Count);
		GameObject newSegment = GetSegmentFromPool(segmentIndex);
		TerrainSegment segmentComponent = newSegment.GetComponent<TerrainSegment>();
		if (segmentComponent != null)
		{
			PositionSegmentAtEndPoint(newSegment, segmentComponent);
			lastEndPoint = segmentComponent.EndPoint.position;
			activeSegments.Add(newSegment);
		}
	}

	private void PositionSegmentAtEndPoint(GameObject segment, TerrainSegment segmentComponent)
	{
		if (activeSegments.Count == 0)
		{
			segment.transform.position = Vector3.zero;
			lastEndPoint = segmentComponent.EndPoint.position;
			return;
		}
		Vector3 offset = new Vector3(
			lastEndPoint.x - segmentComponent.StartPoint.position.x,
			lastEndPoint.y - segmentComponent.StartPoint.position.y,
			0
		);
		segment.transform.position += offset;
	}

	private GameObject GetSegmentFromPool(int segmentIndex)
	{
		Queue<GameObject> pool = segmentPools[segmentIndex];
		if (pool.Count > 0)
		{
			GameObject segment = pool.Dequeue();
			segment.SetActive(true);
			return segment;
		}
		GameObject newSegment = Instantiate(terrainSegmentPrefabs[segmentIndex], transform);
		return newSegment;
	}

	private void ReturnSegmentToPool(GameObject segment)
	{
		if (segment == null) return;
		for (int i = 0; i < terrainSegmentPrefabs.Count; i++)
		{
			if (segment.name.Contains(terrainSegmentPrefabs[i].name))
			{
				segment.SetActive(false);
				segmentPools[i].Enqueue(segment);
				activeSegments.Remove(segment);
				return;
			}
		}
		segment.SetActive(false);
		activeSegments.Remove(segment);
	}
}