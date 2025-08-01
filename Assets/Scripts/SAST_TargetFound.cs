using UnityEngine;

public class SAST_TargetFound : MonoBehaviour
{
    /// <summary>
    /// The associated agent.
    /// This will be set by the agent script on Initialization.
    /// Don't need to manually set.
    /// </summary>
    [HideInInspector]
    public SAST_SearchAgent agent;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("agent"))
        {
            agent.FoundTarget();
        }
    }
}
