using System.Collections;
using UnityEngine;

public class WriteTexture : MonoBehaviour
{
    public RenderTexture segmentationTexture; // Assign this in the inspector

    private void Start()
    {
    }

    private void Update()
    {
        Write();
    }

    private void Write()
    {
        StartCoroutine(WriteCoroutine());
    }

    IEnumerator WriteCoroutine()
    {
        yield return new WaitForEndOfFrame();

        if (segmentationTexture != null)
        {
            // Optionally, check if the material is using the correct texture
            if (GetComponent<Renderer>() != null && GetComponent<Renderer>().material != null)
            {
                GetComponent<Renderer>().material.mainTexture = segmentationTexture;
            }
            else
            {
                Debug.LogError("Renderer or material is null");
            }
        }
        else
        {
            Debug.LogError("segmentationTexture or texture is null");
        }
    }
}
