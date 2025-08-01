using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    /// <summary>
    /// The associated agent.
    /// This will be set by the agent script on Initialization.
    /// Don't need to manually set.
    /// </summary>
    [HideInInspector]
    public SearchNRescueEnvController envController;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("agent"))
        {
            envController.OtherCollision(-0.01f);
        }
        else if (col.gameObject.CompareTag("wall"))
        {
            envController.OtherCollision(-0.005f);
        }
    }
}
