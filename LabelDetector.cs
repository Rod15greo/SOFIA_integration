using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LabelDetector : MonoBehaviour
{
    public Camera vrCamera; // Referência à câmera do VR
    public Texture2D labelTexture; // Textura que contém as labels
    public TMP_Text labelText; // Texto na interface UI (TextMeshPro)
    public float colorTolerance = 0.05f; // Tolerância para comparação de cores

    private Dictionary<Color, string> labelMapping;

    void Start()
    {
        labelMapping = new Dictionary<Color, string>
        {
            { new Color(43/255f, 78/255f, 83/255f), "Earthy (B61)" },
            { new Color(205/255f, 212/255f, 6/255f), "Floral (B55)" },
            { new Color(199/255f, 133/255f, 247/255f), "Woody (B3)" },
            { new Color(2/255f, 241/255f, 110/255f), "Vegetation (B108)" },
            { new Color(119/255f, 201/255f, 180/255f), "Floral (B55)" },
            { new Color(83/255f, 162/255f, 164/255f), "Vegetation (B108)" }
        };
    }

    void Update()
    {
        Vector3 forward = vrCamera.transform.forward;
        Vector2 uv = DirectionToUV(forward);
        Color colorAtPoint = labelTexture.GetPixelBilinear(uv.x, uv.y);

        // Verifica se o ponto está próximo a uma cor no mapeamento
        string label = GetClosestLabel(colorAtPoint);
        if (!string.IsNullOrEmpty(label))
        {
            labelText.text = label;
        }
        else
        {
            labelText.text = "";
        }
    }

    Vector2 DirectionToUV(Vector3 direction)
    {
        float longitude = Mathf.Atan2(direction.z, direction.x) / (2 * Mathf.PI);
        float latitude = Mathf.Asin(direction.y) / Mathf.PI;

        float u = 0.5f - longitude;
        float v = 0.5f + latitude;

        return new Vector2(u, v);
    }

    string GetClosestLabel(Color color)
    {
        foreach (var entry in labelMapping)
        {
            if (ColorsAreClose(entry.Key, color, colorTolerance))
            {
                return entry.Value;
            }
        }
        return null;
    }

    bool ColorsAreClose(Color a, Color b, float tolerance)
    {
        float rDiff = Mathf.Abs(a.r - b.r);
        float gDiff = Mathf.Abs(a.g - b.g);
        float bDiff = Mathf.Abs(a.b - b.b);

        return rDiff < tolerance && gDiff < tolerance && bDiff < tolerance;
    }
}
