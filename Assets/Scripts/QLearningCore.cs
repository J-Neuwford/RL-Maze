using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class QLearningCore
{
    readonly float alpha = 0.1f;
    readonly float gamma = 0.95f;
    float epsilon = 0.3f;
    readonly float epsilonDecay = 0.95f;
    readonly float epsilonMinimum = 0.1f;

    Dictionary<StateActionPair, float> qTable = new();
    System.Random rng = new();

    public QLearningCore(float alpha, float gamma, float epsilon, float epsilonDecay, float epsilonMinimum)
    {
        this.alpha = alpha;
        this.gamma = gamma;
        this.epsilon = epsilon;           
        this.epsilonDecay = epsilonDecay;
        this.epsilonMinimum = epsilonMinimum;
    }

    public AgentAction ChooseAction(Vector2Int state)
    {
        if (rng.NextDouble() < epsilon)
        {
            return (AgentAction)rng.Next(0, 4);
        }

        float maxQ = float.NegativeInfinity;
        AgentAction bestAction = AgentAction.up;

        foreach (AgentAction action in Enum.GetValues(typeof(AgentAction)))
        {
            StateActionPair pair = new(state, action);
            float q = qTable.ContainsKey(pair) ? qTable[pair] : 0.0f;

            if (q > maxQ)
            {
                maxQ = q;
                bestAction = action;
            }
        }

        return bestAction;
    }

    public void UpdateQ(Vector2Int state, AgentAction action, float reward, Vector2Int nextState)
    {
        StateActionPair currentPair = new(state, action);
        float currentQ = qTable.ContainsKey(currentPair) ? qTable[currentPair] : 0.0f;

        float maxNextQ = float.NegativeInfinity;
        foreach (AgentAction nextAction in Enum.GetValues(typeof(AgentAction)))
        {
            StateActionPair nextPair = new(nextState, nextAction);
            float nextQ = qTable.ContainsKey(nextPair) ? qTable[nextPair] : 0.0f;
            if (nextQ > maxNextQ) maxNextQ = nextQ;
        }

        float updatedQ = currentQ + alpha * (reward + gamma * maxNextQ - currentQ);
        qTable[currentPair] = updatedQ;
    }

    public void UpdateTerminalQ(Vector2Int state, AgentAction action, float reward)
    {
        StateActionPair pair = new(state, action);
        float currentQ = qTable.ContainsKey(pair) ? qTable[pair] : 0.0f;
        float updatedQ = currentQ + alpha * (reward - currentQ);
        qTable[pair] = updatedQ;
    }

    public void DecayEpsilon()
    {
        epsilon = Mathf.Max(epsilonMinimum, epsilon * epsilonDecay);
    }

    public float GetQValue(Vector2Int state, AgentAction action)
    {
        StateActionPair pair = new StateActionPair(state, action);
        return qTable.TryGetValue(pair, out float value) ? value : 0.0f;
    }

    public float GetMaxQValue(Vector2Int state)
    {
        float maxQ = float.NegativeInfinity;

        foreach (AgentAction action in Enum.GetValues(typeof(AgentAction)))
        {
            var pair = new StateActionPair(state, action);
            float q = qTable.TryGetValue(pair, out float value) ? value : 0.0f;
            if (q > maxQ) maxQ = q;
        }

        return maxQ;
    }

    struct StateActionPair
    {
        public Vector2Int state;
        public AgentAction action;

        public StateActionPair(Vector2Int s, AgentAction a)
        {
            state = s;
            action = a;
        }

        public override bool Equals(object obj)
        {
            return obj is StateActionPair other &&
                state.Equals(other.state) &&
                action == other.action;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(state, action);
        }
    }


}
