using System;
using System.Collections.Generic;
using UnityEngine;

public class QLearningAgent
{
    readonly Dictionary<StateActionPair, float> qTable = new();

    readonly float alpha;
    readonly float gamma;
    float epsilon;

    readonly System.Random rng = new();

    public QLearningAgent(float alpha, float gamma, float epsilon)
    {
        this.alpha = alpha;
        this.gamma = gamma;
        this.epsilon = epsilon;
    }

    public AgentAction ChooseAction(Vector2Int state)
    {
        if(rng.NextDouble() < epsilon)
        {
            return (AgentAction)rng.Next(0, 4);
        }

        float MaxQ = float.NegativeInfinity;
        AgentAction bestAction = AgentAction.Up;

        foreach(AgentAction action in System.Enum.GetValues(typeof(AgentAction))) 
        {
            StateActionPair pair = new StateActionPair(state, action);
            float q = qTable.ContainsKey(pair) ? qTable[pair] : 0f;

            if(q > MaxQ)
            {
                MaxQ = q;
                bestAction = action;
            }
        }

        //Debug.Log($"[ChooseAction] State: {state}, Best Action: {bestAction}, Q-value: {MaxQ:F3}, Epsilon: {epsilon:F3}");
        return bestAction;
    }

    public void UpdateQ(Vector2Int state, AgentAction action, float reward, Vector2Int nextState)
    {
        StateActionPair currentPair = new StateActionPair(state, action);

        float currentQ = qTable.ContainsKey(currentPair) ? qTable[currentPair] : 0f;

        float maxNextQ = float.NegativeInfinity;
        foreach(AgentAction nextAction in Enum.GetValues(typeof(AgentAction)))
        {
            StateActionPair nextPair = new StateActionPair(nextState, nextAction);
            float nextQ = qTable.ContainsKey(nextPair) ? qTable[nextPair] : 0f;
            if(nextQ > maxNextQ)
            {
                maxNextQ = nextQ;
            }
        }

        float updatedQ = currentQ + alpha * (reward + gamma * maxNextQ - currentQ);
        qTable[currentPair] = updatedQ;

       //Debug.Log($"[UpdateQ] ({state}, {action}) -> {updatedQ:F3} | r: {reward}, maxQ(s'): {maxNextQ:F3}, oldQ: {currentQ:F3}");
    }

    public void UpdateTerminalQ(Vector2Int state, AgentAction action, float reward)
    {
        StateActionPair pair = new StateActionPair(state, action);
        float currentQ = qTable.ContainsKey(pair) ? qTable[pair] : 0f;
        float updatedQ = currentQ + alpha * (reward - currentQ);
        qTable[pair] = updatedQ;
       //Debug.Log($"[UpdateQ - Terminal] ({state}, {action}) -> {updatedQ:F3} | r: {reward}, oldQ: {currentQ:F3}");
    }

    public float GetQValue(Vector2Int state, AgentAction action)
    {
        StateActionPair pair = new StateActionPair();
        return qTable.ContainsKey(pair) ? qTable[pair] : 0f;
    }

    public float GetMaxQValue(Vector2Int state)
    {
        float maxQ = float.NegativeInfinity;

        foreach(AgentAction action in System.Enum.GetValues(typeof(AgentAction)))
        {
            StateActionPair pair = new StateActionPair(state, action);
            float q = qTable.ContainsKey(pair) ? qTable[pair] : 0f;

            if (q > maxQ) maxQ = q;
        }
        return maxQ;
    }

    public void DecayEpsilon(float decayRate, float minEpsilon)
    {
        epsilon = Mathf.Max(minEpsilon, epsilon * decayRate);
        Debug.Log($"Epsilon: {epsilon}");
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
