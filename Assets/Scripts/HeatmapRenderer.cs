using System.Collections.Generic;
using UnityEngine;

public class HeatmapRenderer : MonoBehaviour
{
    [SerializeField] QLearningAgent agent;
    [SerializeField] float maxExpectedQ = 1.0f;
    [SerializeField] float minExpectedQ = -1.0f;
    [SerializeField] List<TileData> floorTiles = new();


    [System.Serializable] struct TileData
    {
        public MeshRenderer meshRenderer;
        public Vector2Int gridPosition;

        public TileData(MeshRenderer r, Vector2Int pos)
        {
            meshRenderer = r;
            gridPosition = pos;
        }
    }
    
    void Start()
    {
        floorTiles.Clear();

        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Floor"))
        {
            if(tile.TryGetComponent(out MeshRenderer meshRenderer))
            {
                meshRenderer.material = new Material(meshRenderer.material);

                Vector3 position = tile.transform.position;
                Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
                floorTiles.Add(new TileData(meshRenderer, gridPosition));
            }

        }
    }

    void Update()
    {
        foreach(TileData tile in floorTiles)
        {
            float maxQ = GetMaxQValueAtPosition(tile.gridPosition);
            float normalized = Mathf.InverseLerp(minExpectedQ, maxExpectedQ, maxQ);
            tile.meshRenderer.material.color = Color.Lerp(Color.black, Color.blue, normalized);
        }    
    }

    float GetMaxQValueAtPosition(Vector2Int position)
    {
        return agent.GetMaxQValue(position);
    }
}
