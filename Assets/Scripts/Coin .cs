using UnityEngine;

public class Coin : MonoBehaviour
{
	public int pointValue = 1;
	[SerializeField] private AudioSource pickupSound;

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			pickupSound.Play();

			ScoreManager.Instance.AddPoints(pointValue);

			Destroy(gameObject);

		}
	}
}