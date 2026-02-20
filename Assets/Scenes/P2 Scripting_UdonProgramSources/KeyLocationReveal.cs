
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class KeyLocationReveal : UdonSharpBehaviour
{
    private int successCount = 0;
    [SerializeField] private int numberOfPuzzleBoxes = 3; // Puzzle slider boxes needed for reward
    // [SerializeField] private LightManager lightClue;
    public GameObject cubeTest;

    public void AddNumberOfBoxes()
    {
        successCount++;
        Debug.Log(successCount.ToString());
    }

    void Update()
    {
        if (successCount >= numberOfPuzzleBoxes)
        {
            cubeTest.SetActive(true);
        }
    }
}
