using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class AvS_SearchNRescueEnvController : MonoBehaviour
{
    public AvS_Agent seeker;
    [HideInInspector]
    public Rigidbody seeker_Rb;
    [HideInInspector]
    public Vector3 seeker_StartingPos;
    [HideInInspector]
    public Quaternion seeker_StartingRot;


    public AvS_Agent hider;
    [HideInInspector]
    public Rigidbody hider_Rb;
    [HideInInspector]
    public Vector3 hider_StartingPos;
    [HideInInspector]
    public Quaternion hider_StartingRot;

    public float agentRunSpeed = 3;
    [Tooltip("Max Environment Steps")] public int MaxEnvironmentSteps = 25000;

    public GameObject target;
    [HideInInspector]
    public Rigidbody targetRb;
    Vector3 m_TargetStartingPos;

    [HideInInspector]
    public AvS_TargetFound targetFound;
    private int m_ResetTimer;

    void Start()
    {
        targetRb = target.GetComponent<Rigidbody>();
        m_TargetStartingPos = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
        
        targetFound = target.GetComponent<AvS_TargetFound>();
        targetFound.envController = this;

        seeker_Rb = seeker.GetComponent<Rigidbody>();
        seeker_StartingPos = new Vector3(seeker.transform.position.x, seeker.transform.position.y, seeker.transform.position.z);
        seeker_StartingRot = seeker.transform.rotation;

        hider_Rb = hider.GetComponent<Rigidbody>();
        hider_StartingPos = new Vector3(hider.transform.position.x, hider.transform.position.y, hider.transform.position.z);
        hider_StartingRot = hider.transform.rotation;

        ResetScene();
    }

    void FixedUpdate()
    {
        //Time reward
        seeker.AddReward(-0.001f);
        hider.AddReward(0.001f);

        m_ResetTimer += 1;
        if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            //times out so the seeker loses
            seeker.AddReward(-1f);
            hider.AddReward(1f);
            
            seeker.EndEpisode();
            hider.EndEpisode();
            ResetScene();
        }
    }

    public void ResetTarget()
    {
        target.transform.position = m_TargetStartingPos;
        targetRb.velocity = Vector3.zero;
        targetRb.angularVelocity = Vector3.zero;
    }

    public void TargetFound()
    {
        //Seeker wins
        seeker.AddReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps);
        hider.AddReward(-1f);

        seeker.EndEpisode();
        hider.EndEpisode();
        ResetScene();
    }

    public void ResetScene()
    {
        m_ResetTimer = 0;

        seeker.transform.position = seeker_StartingPos;
        seeker.transform.rotation = seeker_StartingRot;

        hider.transform.position = hider_StartingPos;
        hider.transform.rotation = hider_StartingRot;
        
        //Reset target
        ResetTarget();
    }
}
