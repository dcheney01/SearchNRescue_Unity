//Put this script on your blue cube.

using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class AvS_Agent : Agent
{
    public enum Role
    {
        Hider,
        Seeker
    }

    public Role role;

    AvS_SearchNRescueEnvController m_AvS_SearchNRescueEnvController;

    [HideInInspector]
    public Rigidbody agentRb;

    EnvironmentParameters m_ResetParams;
    
    BehaviorParameters m_BehaviorParameters;


    public override void Initialize()
    {
        m_AvS_SearchNRescueEnvController = FindObjectOfType<AvS_SearchNRescueEnvController>();

        agentRb = GetComponent<Rigidbody>();
        // m_ResetParams = Academy.Instance.EnvironmentParameters;
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
        agentRb.AddForce(dirToGo * m_AvS_SearchNRescueEnvController.agentRunSpeed,
            ForceMode.VelocityChange);
    }

    /// <summary>
    /// Called every step of the engine. Here the agent takes an action.
    /// </summary>
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // if(role == Role.Hider)
        // {
        //     AddReward(-0.0001f);
        // }
        // else if (role == Role.Hider)
        // {
        //     AddReward(0.0001f);
        // }
        // Move the agent using the action.
        MoveAgent(actionBuffers.DiscreteActions);
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
}
