using System;
using System.Collections.Generic;
using UnityEngine;

public enum AgentAction
{
    up, down, left, right
}

public class QLearningAgent : MonoBehaviour
{
    [SerializeField] float alpha = 0.1f;
    [SerializeField] float gamma = 0.95f;
    [SerializeField] float epsilon = 0.3f;
    [SerializeField] float epsilonDecay = 0.95f;
    [SerializeField] float epsilonMinimum = 0.1f;

    [SerializeField] float moveDelay = 0.02f;

    [SerializeField] float stepReward = -0.01f;
    [SerializeField] float goalReward = 1.0f;
    [SerializeField] float hazardReward = -1.0f;
    [SerializeField] float wallReward = -0.01f;

    [SerializeField] Vector2Int startPosition = new Vector2Int(0, 0);

    
    QLearningCore core;
    Vector2Int gridPosition;
    float moveCooldown = 0.0f;
    float totalRewardInEpisode = 0.0f;
    int stepsInEpisode = 0;
    int episode = 1;

    private void Awake()
    {
        core = new QLearningCore(alpha, gamma, epsilon, epsilonDecay, epsilonMinimum);
    }

    void Start()
    {
        
        gridPosition = startPosition;
        UpdateWorldPosition();
    }

    void Update()
    {
        moveCooldown -= Time.deltaTime;
        if (moveCooldown <= 0f)
        {
            RunTrainingStep();
        }
    }

    void RunTrainingStep()
    {
        Vector2Int state = gridPosition;
        AgentAction action = core.ChooseAction(gridPosition);
        Vector2Int direction = GetDirection(action);
        Vector2Int targetPosition = gridPosition + direction;

        if (IsWall(targetPosition))
        {
            core.UpdateQ(state, action, wallReward, state);

            moveCooldown = moveDelay;
            stepsInEpisode++;
            totalRewardInEpisode += wallReward;

            return;
        };

        gridPosition = targetPosition;
        UpdateWorldPosition();

        float reward = GetRewardAtPosition(gridPosition);
        bool isTerminal = IsTerminalState(gridPosition);
        Vector2Int nextState = isTerminal ? startPosition : gridPosition;

        if (isTerminal)
        {
            core.UpdateTerminalQ(state, action, reward);

            Debug.Log($"--- EPISODE {episode} ENDED ---");
            Debug.Log($"Steps in episode: {stepsInEpisode}, Total Reward: {totalRewardInEpisode}");

            episode++;
            stepsInEpisode = 0;
            totalRewardInEpisode = 0f;

            core.DecayEpsilon();

            gridPosition = startPosition;
            UpdateWorldPosition();
        }
        else
        {
            core.UpdateQ(state, action, reward, nextState);
        }

        stepsInEpisode++;
        totalRewardInEpisode += reward;
        moveCooldown = moveDelay;
    }


    float GetRewardAtPosition(Vector2Int position)
    {
        Collider[] hits = Physics.OverlapSphere(new Vector3(position.x, 0.1f, position.y), 0.1f);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Goal")) return goalReward;
            if (hit.CompareTag("Hazard")) return hazardReward;
        }

        return stepReward;
    }

    bool IsTerminalState(Vector2Int position)
    {
        Collider[] hits = Physics.OverlapSphere(new Vector3(position.x, 0.1f, position.y), 0.1f);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Goal") || hit.CompareTag("Hazard"))
            {
                return true;
            }
        }

        return false;
    }

    bool IsWall(Vector2Int position)
    {
        Vector3 checkPosition = new Vector3(position.x, 0.1f, position.y);
        Collider[] hits = Physics.OverlapSphere(checkPosition, 0.1f);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Wall")) return true;
        }

        return false;
    }

    Vector2Int GetDirection(AgentAction action)
    {
        return action switch
        {
            AgentAction.up => Vector2Int.up,
            AgentAction.down => Vector2Int.down,
            AgentAction.left => Vector2Int.left,
            AgentAction.right => Vector2Int.right,
            _ => Vector2Int.zero
        };
    }

    void UpdateWorldPosition()
    {
        transform.position = new Vector3(gridPosition.x, 0.1f, gridPosition.y);
    }

    public float GetMaxQValue(Vector2Int state)
    {
        return core.GetMaxQValue(state);
    }

    // UI
    public float GetEpsilon() => core.GetEpsilon();
    public float GetEpsilonDecay() => epsilonDecay;
    public float GetAlpha() => alpha;
    public float GetGamma() => gamma;


    public void SetEpsilon(float e)
    {
        epsilon = e;
    }


}
