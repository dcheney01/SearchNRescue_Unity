using UnityEngine;

public class AvS_TargetFound : MonoBehaviour
{
    /// <summary>
    /// The associated agent.
    /// This will be set by the agent script on Initialization.
    /// Don't need to manually set.
    /// </summary>
    [HideInInspector]
    public AvS_SearchNRescueEnvController envController;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("seeker"))
        {
            envController.TargetFound();
        }
    }
}
