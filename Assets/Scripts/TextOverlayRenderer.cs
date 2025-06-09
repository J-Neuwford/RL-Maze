using System.Collections.Generic;
using UnityEngine;

public class TextOverlayRenderer : MonoBehaviour
{
    [SerializeField] QLearningAgent agent;
    [SerializeField] GameObject textPrefab;
    [SerializeField] Vector3 textOffset = new Vector3(0.0f, 0.3f, 0.0f);

    private List<TextOverlay> overlays = new();

    struct TextOverlay
    {
        public Vector2Int gridPosition;
        public TextMesh textMesh;

        public TextOverlay(Vector2Int pos, TextMesh mesh)
        {
            gridPosition = pos;
            textMesh = mesh;
        }
    }

    void Start()
    {
        overlays.Clear();

        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Floor"))
        {
            Vector3 position = tile.transform.position;
            Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));

            GameObject textObject = Instantiate(textPrefab, position + textOffset, Quaternion.Euler(90f, Camera.main.transform.eulerAngles.y, 0), transform);
            if(textObject.TryGetComponent(out TextMesh textMesh))
            {
                overlays.Add(new TextOverlay(gridPosition, textMesh));
            }
        }
    }

    void Update()
    {
        foreach (TextOverlay overlay in overlays)
        {
            float maxQ = agent.GetMaxQValue(overlay.gridPosition);
            overlay.textMesh.text = maxQ.ToString("F3");
        }   
    }
}
