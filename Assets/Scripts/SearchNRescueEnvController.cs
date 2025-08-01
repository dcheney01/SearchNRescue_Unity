using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class SearchNRescueEnvController : MonoBehaviour
{
    [System.Serializable]
    public class PlayerInfo
    {
        public MAST_SeekerAgent Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
    }

    [Header("Max Environment Steps")] public int MaxEnvironmentSteps = 1000;

    /// <summary>
    /// The ground. The bounds are used to spawn the elements.
    /// </summary>
    public GameObject ground;

    public GameObject target;

    //List of Agents On Platform
    public List<PlayerInfo> AgentsList = new List<PlayerInfo>();

    private SearchNRescueSettings m_SearchNRescueSettings;

    // private int m_NumberOfRemainingTargets;

    /// <summary>
    /// Detects when the target is found.
    /// </summary>
    [HideInInspector]
    public MAST_TargetFound targetFound;

    [HideInInspector]
    public CollisionDetect collisionDetect;

    /// <summary>
    /// The area bounds.
    /// </summary>
    [HideInInspector]
    public Bounds areaBounds;

    private SimpleMultiAgentGroup m_AgentGroup;
    private int m_ResetTimer;
    private Rigidbody m_TargetRb;

    Material m_GroundMaterial; //cached on Awake()

    /// <summary>
    /// We will be changing the ground material based on success/failue
    /// </summary>
    Renderer m_GroundRenderer;

    void Start()
    {
        // Get the ground's bounds
        areaBounds = ground.GetComponent<Collider>().bounds;
        // Get the ground renderer so we can change the material when a target is found
        m_GroundRenderer = ground.GetComponent<Renderer>();

        targetFound = target.GetComponent<MAST_TargetFound>();
        targetFound.envController = this;

        m_TargetRb = target.GetComponent<Rigidbody>();

        // Starting material
        m_GroundMaterial = m_GroundRenderer.material;
        m_SearchNRescueSettings = FindObjectOfType<SearchNRescueSettings>();

        m_AgentGroup = new SimpleMultiAgentGroup();
        foreach (var item in AgentsList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.Rb = item.Agent.GetComponent<Rigidbody>();
            collisionDetect = item.Agent.GetComponent<CollisionDetect>();
            collisionDetect.envController = this;
            m_AgentGroup.RegisterAgent(item.Agent);
        }
        ResetScene();
    }

    void FixedUpdate()
    {
        m_ResetTimer += 1;
        if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            m_AgentGroup.GroupEpisodeInterrupted();
            ResetScene();
        }

        //Hurry Up Penalty, 2 for the two agents
        m_AgentGroup.AddGroupReward(-2f / MaxEnvironmentSteps);
    }

    /// <summary>
    /// Use the ground's bounds to pick a random spawn position.
    /// </summary>
    public Vector3 GetRandomSpawnPos()
    {
        var foundNewSpawnLocation = false;
        var randomSpawnPos = Vector3.zero;
        while (foundNewSpawnLocation == false)
        {
            var randomPosX = Random.Range(-areaBounds.extents.x * m_SearchNRescueSettings.spawnAreaMarginMultiplier,
                areaBounds.extents.x * m_SearchNRescueSettings.spawnAreaMarginMultiplier);

            var randomPosZ = Random.Range(-areaBounds.extents.z * m_SearchNRescueSettings.spawnAreaMarginMultiplier,
                areaBounds.extents.z * m_SearchNRescueSettings.spawnAreaMarginMultiplier);
            randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 1f, randomPosZ);
            if (Physics.CheckBox(randomSpawnPos, new Vector3(1.5f, 0.01f, 1.5f)) == false)
            {
                foundNewSpawnLocation = true;
            }
        }
        return randomSpawnPos;
    }

    /// <summary>
    /// Resets the Target position and velocities.
    /// </summary>
    void ResetTarget()
    {
        target.transform.position = GetRandomSpawnPos();
        m_TargetRb.velocity = Vector3.zero;
        m_TargetRb.angularVelocity = Vector3.zero;
    }

    /// <summary>
    /// Swap ground material, wait time seconds, then swap back to the regular material.
    /// </summary>
    IEnumerator TargetFoundSwapMaterial(Material mat, float time)
    {
        m_GroundRenderer.material = mat;
        yield return new WaitForSeconds(time); // Wait for 2 sec
        m_GroundRenderer.material = m_GroundMaterial;
    }

    /// <summary>
    /// Called when the agent finds the target.
    public void FoundTarget()
    {
        //Give Agent Group Rewards
        m_AgentGroup.AddGroupReward(1f);

        // Swap ground material for a bit to indicate we scored.
        StartCoroutine(TargetFoundSwapMaterial(m_SearchNRescueSettings.targetFoundMaterial, 0.5f));

        m_AgentGroup.EndGroupEpisode();
        ResetScene();
    }

    public void OtherCollision(float score)
    {
        m_AgentGroup.AddGroupReward(score);
    }

    Quaternion GetRandomRot()
    {
        return Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
    }

    public void ResetScene()
    {
        m_ResetTimer = 0;

        //Reset Agents
        foreach (var item in AgentsList)
        {
            item.Agent.transform.SetPositionAndRotation(GetRandomSpawnPos(),  GetRandomRot());
            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
        }

        ResetTarget();
    }
}
