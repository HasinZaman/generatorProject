using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testPoint : MonoBehaviour
{
    public double value;
    public GameObject kewl;
    // Start is called before the first frame update
    void Start()
    {
        Color color = new Color((float)value, (float)value, (float)value);

        Texture2D texture = new Texture2D(1, 1);

        texture.SetPixel(0, 0, color);

        texture.Apply();

        Renderer renderer = GetComponent<Renderer>();
            

        renderer.material.mainTexture = texture;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
