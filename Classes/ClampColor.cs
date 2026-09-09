using UnityEngine;

public class ClampColor : MonoBehaviour
{
    public Renderer targetRenderer;
    private Renderer myRenderer;

    private void Start()
    {
        myRenderer = GetComponent<Renderer>();
        if (targetRenderer != null && myRenderer != null)
        {
            myRenderer.sharedMaterial = targetRenderer.sharedMaterial;
        }
    }

    private void Update()
    {
        if (targetRenderer != null && myRenderer != null)
        {
            myRenderer.material.color = targetRenderer.material.color;
        }
    }
}