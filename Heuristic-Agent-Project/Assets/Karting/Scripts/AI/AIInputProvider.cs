using UnityEngine;
using KartGame.KartSystems;

public class AIInputProvider : MonoBehaviour, IInput
{
    private AI_Agent aiAgent;
    
    void Awake()
    {
        aiAgent = GetComponent<AI_Agent>();
    }
    
    public InputData GenerateInput()
    {
        // Always provide forward input for the AI kart
        return new InputData
        {
            Accelerate = true,
            Brake = false,
            TurnInput = aiAgent.GetSteerInput()
        };
    }
}
