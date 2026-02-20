
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SliderGoalCheck : UdonSharpBehaviour
{
    [Header("Target to Check and Reward")]
    [SerializeField] private int verifyLayer = 23; // The entering object
    [SerializeField] private KeyLocationReveal keyLocator; // The hint for key location
    
    private void OnTriggerEnter(Collider enteringObject)
    {
        if (enteringObject.gameObject.layer == verifyLayer)
        {
            GetComponent<AudioSource>().Play();
            keyLocator.AddNumberOfBoxes();
        }
    }
}
