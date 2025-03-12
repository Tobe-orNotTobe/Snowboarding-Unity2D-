using UnityEngine;

public class SnowTrail : MonoBehaviour
{
    // ▼ "Serialize Field" Attribute ▼
    [SerializeField] ParticleSystem snowParticles;



    // ▬ "OnCollisionEnter2D()" Built-In Method 
    //       → that "Detects" when the "Player Touches" the "Ground" ▬
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Ground")
        {
            snowParticles.Play();
        }
    }



    // ▬ "OnCollisionExit2D()" Built-In Method 
    //       → that "Detects" when the "Player No Longer Touching" the "Ground" ▬
    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Ground")
        {
            snowParticles.Stop();
        }
    }
}
