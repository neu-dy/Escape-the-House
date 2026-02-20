
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class KeyLocationReveal : UdonSharpBehaviour
{
    private int successCount = 0;
    [SerializeField] private int numberOfPuzzleBoxes = 3; // Puzzle slider boxes needed for reward
    public Light_Controller lightController;
    [SerializeField] private int lightRevealNumber; // This is the set of key location that gets revealed

    public void AddNumberOfBoxes()
    {
        successCount++;
        Debug.Log(successCount.ToString());
    }

    void Update()
    {
        if (successCount >= numberOfPuzzleBoxes)
        {
            lightController.TurnOnStage(lightRevealNumber);
        }
    }
}
