using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform goal;
    [SerializeField] private GameObject floor;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));

        do
        {
            goal.localPosition = new Vector3(Random.Range(-4f, 4f), 1f, Random.Range(-4f, 4f));
        }
        while (Vector3.Distance(transform.localPosition, goal.localPosition) < 2);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(goal.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        float moveSpeed = 3f;

        transform.localPosition += new Vector3(moveX, 0f, moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> actionSegment = actionsOut.ContinuousActions;
        actionSegment[0] = Input.GetAxisRaw("Horizontal");
        actionSegment[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            SetReward(1f);
            floor.GetComponent<MeshRenderer>().material.color = Color.green;
            EndEpisode();
        }
        else if (other.CompareTag("Wall"))
        {
            SetReward(-1f);
            floor.GetComponent<MeshRenderer>().material.color = Color.red;
            EndEpisode();
        }
    }
}
