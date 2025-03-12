using UnityEngine;

public class FarthestPointMarker : MonoBehaviour
{
	[SerializeField] private float bobSpeed = 1f;
	[SerializeField] private float bobHeight = 0.3f;
	[SerializeField] private float rotateSpeed = 30f;
	[SerializeField] private bool shouldRotate = true;

	private Vector3 startPosition;

	void Start()
	{
		startPosition = transform.position;
		gameObject.tag = "FarthestMarker";
	}

	void Update()
	{
		// Create a bobbing motion
		float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
		transform.position = new Vector3(transform.position.x, newY, transform.position.z);

		// Rotate the marker
		if (shouldRotate)
		{
			transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
		}
	}
}