using UnityEngine;
using UnityEngine.UI;

public class ParametersUI : MonoBehaviour
{
    [SerializeField] QLearningAgent agent;
    [SerializeField] Text epsilonText;
    [SerializeField] Text epsilonDecayText;
    [SerializeField] Text alphaText;
    [SerializeField] Text gammaText;

    float epsilon;

    void Start()
    {
        epsilonText.text = $"Epsilon: {epsilon.ToString("F2")}";
        epsilonDecayText.text = $"Epsilon Decay: {agent.GetEpsilonDecay().ToString("F2")}";
        alphaText.text = $"Alpha: {agent.GetAlpha().ToString("F2")}";
        gammaText.text = $"Gamma: {agent.GetGamma().ToString("F2")}";
    }

    void Update()
    {
        UpdateEpsilonValue();
        epsilonText.text = $"Epsilon: {epsilon.ToString("F2")}";
    }

    void UpdateEpsilonValue()
    {
        epsilon = agent.GetEpsilon();
        Debug.Log($"GetEpsilon: {agent.GetEpsilon()}");
    }
}

