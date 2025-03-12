using UnityEngine;

public class TerrainSegment : MonoBehaviour
{
	[Header("Connection Points")]
	[SerializeField] private Transform startPoint; 
	[SerializeField] private Transform endPoint;   

	public Transform StartPoint => startPoint;
	public Transform EndPoint => endPoint;


	[SerializeField] private GameObject[] obstacles;
	[SerializeField] private GameObject[] collectibles;

	private void Start()
	{
		InitializeObstacles();
	}

	private void InitializeObstacles()
	{
		foreach (GameObject collectible in collectibles)
		{
			if (collectible != null)
			{
				collectible.SetActive(Random.value < 0.5f);
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (startPoint != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(startPoint.position, 0.5f);
			Gizmos.DrawLine(transform.position, startPoint.position);
		}

		if (endPoint != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(endPoint.position, 0.5f);
			Gizmos.DrawLine(transform.position, endPoint.position);
		}
	}
}