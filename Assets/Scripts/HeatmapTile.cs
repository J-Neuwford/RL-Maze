using UnityEngine;

public class HeatmapTile : MonoBehaviour
{
    MeshRenderer meshRenderer;
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetHeatColor(Color color)
    {
            meshRenderer.material.color = color;
    }
}
