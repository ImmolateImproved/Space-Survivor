using UnityEngine;
using Random = UnityEngine.Random;

public class NoiseTest : MonoBehaviour
{
    private Texture2D texture;

    [Range(2, 512)]
    public int resolution;
    public Vector2 offset;
    [Min(0.0001f)]
    public float scale;
    [Min(0.0001f)]
    public float increment;

    public FilterMode filterMode;

    public bool useRandom;

    private void OnValidate()
    {
        if (texture == null)
        {
            CreateTxture();
        }

        GetComponent<Renderer>().sharedMaterial.mainTexture = texture;

        if (texture.width != resolution)
        {
            texture.Reinitialize(resolution, resolution);
        }

        texture.filterMode = filterMode;

        GenerateNoise();
    }

    private void CreateTxture()
    {
        texture = new Texture2D(resolution, resolution);
    }

    private void GenerateNoise()
    {
        offset.y = 0;
        for (int y = 0; y < resolution; y++)
        {
            offset.x = 0;

            for (int x = 0; x < resolution; x++)
            {
                var offset1 = offset * scale;

                var n = useRandom 
                    ? Random.Range(0, 1f)
                    : Mathf.PerlinNoise(offset1.x, offset1.y);//noise.snoise(offset1);//
                
                var color = new Color(n, n, n);
                texture.SetPixel(x, y, color);

                offset.x += increment;
            }
            offset.y += increment;
        }

        texture.Apply();
    }
}