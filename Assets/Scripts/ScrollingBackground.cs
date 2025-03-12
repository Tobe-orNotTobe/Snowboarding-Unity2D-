using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public float speed;

    [SerializeField]
    private Renderer bgRenderl;

    void Update()
    {
        bgRenderl.material.mainTextureOffset += new Vector2(speed * Time.deltaTime, 0);
    }
}
