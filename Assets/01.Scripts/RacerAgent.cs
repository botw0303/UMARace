using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RacerAgent : Agent
{
    private float _moveSpeed = 10f;
    private float _turnSpeed = 25f;
    private float _stamina = 100f;
    private Vector3 _direction;

    private Rigidbody _rigidbody;
    private Vector3 StartPos;

    public override void Initialize()
    {
        _rigidbody = GetComponent<Rigidbody>();
        StartPos = transform.localPosition;
    }

    public override void OnEpisodeBegin()
    {
        _moveSpeed = Random.Range(10f, 15f);
        _stamina = 100f;
        _direction = transform.forward;

        _rigidbody.velocity = _rigidbody.angularVelocity = Vector3.zero;
        transform.localPosition = StartPos;
        transform.localRotation = Quaternion.identity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var DiscreteActions = actions.DiscreteActions;

        Vector3 rotationAxis = Vector3.zero;

        // DiscreteActions[0] : 지속(0), 가속(1), 감속(2)
        switch(DiscreteActions[0])
        {
            case 1: _moveSpeed = Mathf.Clamp(_moveSpeed + 0.2f, 10f, 15f); break;
            case 2: _moveSpeed = Mathf.Clamp(_moveSpeed - 0.2f, 10f, 15f); break;
        }

        // DiscreteActions[1] : 전방(0), 좌측(1), 우측(2)
        switch(DiscreteActions[1])
        {
            case 1: rotationAxis = Vector3.down; break;
            case 2: rotationAxis = Vector3.up; break;
        }

        _rigidbody.MovePosition(transform.position + _direction * _moveSpeed * Time.fixedDeltaTime);
        transform.Rotate(rotationAxis, Mathf.Clamp(_turnSpeed * Time.fixedDeltaTime, -45f, 45f));
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var DiscreteActionsOut = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.W))
        {
            DiscreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            DiscreteActionsOut[0] = 2;
        }
        if (Input.GetKey(KeyCode.A))
        {
            DiscreteActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            DiscreteActionsOut[1] = 2;
        }
    }
}
