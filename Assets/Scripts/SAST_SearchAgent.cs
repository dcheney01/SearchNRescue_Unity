//Put this script on your blue cube.

using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class SAST_SearchAgent : Agent
{
    public GameObject ground;
    public GameObject area;
    public GameObject target;

    public bool useVectorObs;

    [HideInInspector]
    public Bounds areaBounds;

    [HideInInspector]
    public SAST_TargetFound targetFound;

    SearchNRescueSettings m_SearchNRescueSettings;

    Rigidbody m_TargetRb;  //cached on initialization
    Rigidbody m_AgentRb;  //cached on initialization
    Material m_GroundMaterial; //cached on Awake()

    Renderer m_GroundRenderer;
    EnvironmentParameters m_ResetParams;

    void Awake()
    {
        m_SearchNRescueSettings = FindObjectOfType<SearchNRescueSettings>();
    }

    public override void Initialize()
    {
        targetFound = target.GetComponent<SAST_TargetFound>();
        targetFound.agent = this;

        // Cache the agent rigidbody
        m_AgentRb = GetComponent<Rigidbody>();
        // Cache the Target rigidbody
        m_TargetRb = target.GetComponent<Rigidbody>();
        // Get the ground's bounds
        areaBounds = ground.GetComponent<Collider>().bounds;
        // Get the ground renderer so we can change the material when the target is found
        m_GroundRenderer = ground.GetComponent<Renderer>();
        // Starting material
        m_GroundMaterial = m_GroundRenderer.material;

        m_ResetParams = Academy.Instance.EnvironmentParameters;

        SetResetParameters();
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
            if (Physics.CheckBox(randomSpawnPos, new Vector3(2.5f, 0.01f, 2.5f)) == false)
            {
                foundNewSpawnLocation = true;
            }
        }
        return randomSpawnPos;
    }

    /// <summary>
    /// Called when the agent finds the target.
    /// </summary>
    public void FoundTarget()
    {
        AddReward(1f);

        // By marking an episode as done AgentReset() will be called automatically.
        EndEpisode();

        // Swap ground material for a bit to indicate we found target.
        StartCoroutine(TargetFoundSwapGroundMaterial(m_SearchNRescueSettings.targetFoundMaterial, 0.5f));
    }

    /// <summary>
    /// Swap ground material, wait time seconds, then swap back to the regular material.
    /// </summary>
    IEnumerator TargetFoundSwapGroundMaterial(Material mat, float time)
    {
        m_GroundRenderer.material = mat;
        yield return new WaitForSeconds(time);
        m_GroundRenderer.material = m_GroundMaterial;
    }

    IEnumerator AgentFailedSwapGroundMaterial(float time)
    {
        m_GroundRenderer.material = m_SearchNRescueSettings.failMaterial;
        yield return new WaitForSeconds(time);
        m_GroundRenderer.material = m_GroundMaterial;

    }


    /// <summary>
    /// Moves the agent according to the selected action.
    /// </summary>
    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];

        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
            case 5:
                dirToGo = transform.right * -0.75f;
                break;
            case 6:
                dirToGo = transform.right * 0.75f;
                break;
        }
        transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
        m_AgentRb.AddForce(dirToGo * m_SearchNRescueSettings.agentRunSpeed,
            ForceMode.VelocityChange);
    }

    /// <summary>
    /// Called every step of the engine. Here the agent takes an action.
    /// </summary>
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Move the agent using the action.
        MoveAgent(actionBuffers.DiscreteActions);

        // Penalty given each step to encourage agent to finish task quickly.
        // Penalizes the agent more when there are less available steps (max steps is small)
        AddReward(-1f / MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
    }

    /// <summary>
    /// Resets the target position
    /// </summary>
    void ResetTarget()
    {
        // Get a random position for the target.
        target.transform.position = GetRandomSpawnPos();
        m_TargetRb.velocity = Vector3.zero;
        m_TargetRb.angularVelocity = Vector3.zero;
    }

    /// <summary>
    /// In the editor, if "Reset On Done" is checked then AgentReset() will be
    /// called automatically anytime we mark done = true in an agent script.
    /// </summary>
    public override void OnEpisodeBegin()
    {
        StartCoroutine(AgentFailedSwapGroundMaterial(0.5f));

        var rotation = Random.Range(0, 4);
        var rotationAngle = rotation * 90f;
        area.transform.Rotate(new Vector3(0f, rotationAngle, 0f));

        ResetTarget();
        transform.position = GetRandomSpawnPos();
        m_AgentRb.velocity = Vector3.zero;
        m_AgentRb.angularVelocity = Vector3.zero;

        SetResetParameters();
    }

    public void SetGroundMaterialFriction()
    {
        var groundCollider = ground.GetComponent<Collider>();

        groundCollider.material.dynamicFriction = m_ResetParams.GetWithDefault("dynamic_friction", 0);
        groundCollider.material.staticFriction = m_ResetParams.GetWithDefault("static_friction", 0);
    }

    public void SetTargetProperties()
    {
        var scale = m_ResetParams.GetWithDefault("target_scale", 2);
        //Set the scale of the target
        m_TargetRb.transform.localScale = new Vector3(scale, 0.75f, scale);

        // Set the drag of the target
        m_TargetRb.drag = m_ResetParams.GetWithDefault("target_drag", 0.5f);
    }

    void SetResetParameters()
    {
        SetGroundMaterialFriction();
        SetTargetProperties();
    }
}
