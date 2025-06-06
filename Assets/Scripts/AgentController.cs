using UnityEngine;
public enum AgentAction
{
    Up, Down, Left, Right
}

public class AgentController : MonoBehaviour
{
    [SerializeField] float moveDelay = 0.02f;
    [SerializeField] float moveCooldown = 0.0f;

    [SerializeField, Tooltip("learning rate, Range: 0-1")] float alpha = 0.1f;
    [SerializeField, Tooltip("Discount factor, Range: 0-1)")] float gamma = 0.95f;
    [SerializeField, Tooltip("Exploration rate, Range: 0-1")] float epsilon = 0.3f;
    [SerializeField] float epsilonDecay = 0.95f;
    [SerializeField] float epsilonMinimum = 0.1f;

    [SerializeField] float stepReward = -0.01f;
    [SerializeField] float goalReward = 1.0f;
    [SerializeField] float hazardReward = -1.0f;

    [SerializeField] Vector2Int startPosition = new Vector2Int(0, 0);
    Vector2Int gridPosition;

    public static QLearningAgent sharedAgent { get; private set; }
    QLearningAgent agent;

    int episode = 1;
    int stepsInEpisode = 0;
    float totalRewardInEpisode;

    void Awake()
    {
        gridPosition = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.z));

        sharedAgent = new QLearningAgent(alpha, gamma, epsilon);
        agent = sharedAgent;
    }

    void Update()
    {
        moveCooldown -= Time.deltaTime;
        //ManuallyMoveAgent();
        RunTrainingStep();

       // Debug.Log($"IsTerminalState:, {IsTerminalState(gridPosition)}");
       // Debug.Log($"GetDirection: {GetDirection(AgentAction.Up)}");
       //Debug.Log($"GetRewardAtPosition: {GetRewardAtPosition(gridPosition)}");
    }

    void RunTrainingStep()
    {
        if(moveCooldown > 0) return;

        AgentAction action = agent.ChooseAction(gridPosition);
        Vector2Int direction = GetDirection(action);
        Vector2Int targetPosition = gridPosition + direction;

        // Check for walls
        Vector3 checkPosition = new Vector3(targetPosition.x, 0.1f, targetPosition.y);
        Collider[] hits = Physics.OverlapSphere(checkPosition, 0.1f);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Wall")) return;
        }

        gridPosition = targetPosition;
        UpdateWorldPosition();

        float reward = GetRewardAtPosition(gridPosition);

        stepsInEpisode++;
        totalRewardInEpisode += reward;

        bool isTerminal = IsTerminalState(gridPosition);
        Vector2Int nextState = isTerminal ? new Vector2Int(0,0) : gridPosition;

        if (isTerminal)
        {

            agent.UpdateTerminalQ(gridPosition, action, reward);

            Debug.Log($"--- EPISODE {episode} ENDED ---");
            Debug.Log($"Steps: {stepsInEpisode}, Total Reward: {totalRewardInEpisode:F2}");
            episode++;
            stepsInEpisode = 0;
            totalRewardInEpisode = 0f;

            agent.DecayEpsilon(epsilonDecay, epsilonMinimum);
            gridPosition = startPosition;
            UpdateWorldPosition();
        }
        else
        {
            agent.UpdateQ(gridPosition, action, reward, nextState);
        }

        moveCooldown = moveDelay;

    }

    void ManuallyMoveAgent()
    {
        Vector2Int direction = Vector2Int.zero;

        if(moveCooldown > 0) return;

        if(Input.GetKeyDown(KeyCode.UpArrow)) direction = Vector2Int.up;
        else if(Input.GetKeyDown(KeyCode.DownArrow)) direction = Vector2Int.down;
        else if(Input.GetKeyDown(KeyCode.LeftArrow)) direction = Vector2Int.left;
        else if(Input.GetKeyDown(KeyCode.RightArrow)) direction = Vector2Int.right;

        if(direction != Vector2Int.zero)
        {
            Vector2Int targetPosition = gridPosition + direction;

            // Check for walls
            Vector3 checkPosition = new Vector3(targetPosition.x, 0.1f, targetPosition.y);
            Collider[] hits = Physics.OverlapSphere(checkPosition, 0.1f);

            foreach(Collider hit in hits)
            {
                if(hit.CompareTag("Wall")) return;
            }

            gridPosition = targetPosition;
            UpdateWorldPosition();
            moveCooldown = moveDelay;
        }
    }

    void UpdateWorldPosition()
    {
        transform.position = new Vector3(gridPosition.x, 0.1f, gridPosition.y);
    }

    Vector2Int GetDirection(AgentAction action)
    {
        return action switch
        {
            AgentAction.Up => Vector2Int.up,
            AgentAction.Down => Vector2Int.down,
            AgentAction.Left => Vector2Int.left,
            AgentAction.Right => Vector2Int.right,
            _ => Vector2Int.zero
        };
    }

    float GetRewardAtPosition(Vector2Int position)
    {
        Collider[] hits = Physics.OverlapSphere(new Vector3(position.x, 0.1f, position.y), 0.1f);

        foreach (Collider hit in hits)
        {
            if(hit.CompareTag("Goal")) return goalReward;
            if(hit.CompareTag("Hazard")) return hazardReward;
        }

        return stepReward;
    }

    bool IsTerminalState(Vector2Int position)
    {
        Collider[] hits = Physics.OverlapSphere(new Vector3(position.x, 0.1f, position.y), 0.1f);

        foreach(Collider hit in hits)
        {
            if (hit.CompareTag("Goal") || hit.CompareTag("Hazard"))
            {
                return true;
            }
        }

        return false;
    }
}
