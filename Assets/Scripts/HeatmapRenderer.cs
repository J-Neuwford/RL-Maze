using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HeatmapRenderer : MonoBehaviour
{
    [SerializeField] QLearningAgent agent;
    [SerializeField] float maxExpectedQ = 1f;
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
        agent = AgentController.sharedAgent;
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
            float normalized = Mathf.InverseLerp(-0.005f, maxExpectedQ, maxQ);
            tile.meshRenderer.material.color = Color.Lerp(Color.black, Color.blue, normalized);
        }    
    }

    float GetMaxQValueAtPosition(Vector2Int position)
    {
        float maxQ = float.NegativeInfinity;

        foreach (AgentAction action in System.Enum.GetValues(typeof(AgentAction)))
        {
            float q = agent.GetMaxQValue(position);
            if(q > maxQ) maxQ = q;
        }

        return maxQ;
    }
}
