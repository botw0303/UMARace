using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RacerAgent : Agent
{
    public float MoveSpeed = 10f;
    public float TurnSpeed = 25f;

    private float _testValue;
    private Material _testmaterial;
    private SkinnedMeshRenderer _renderer;

    public float Stamina = 100f;
    public float StaminaConsumptionRate = 1f;
    private float _staminaTimer = 0f;
    private float _consumptionTime = 1f;

    private Rigidbody _rigidbody;
    private Vector3 _startPos;
    private Quaternion _startRotation;

    public int _checkPointCnt = 0;
    private Dictionary<string, GameObject> CheckedPointDictionary;

    private bool _isGoalIn = false;

    private float _distanceToNextCheckPoint = 0f;
    private float _lastDistanceToNextCheckPoint = 0f;

    private float _goalTimer = 0f;

    public override void Initialize()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _startPos = transform.localPosition;
        _startRotation = transform.localRotation;
        CheckedPointDictionary = new Dictionary<string, GameObject>();
    }

    public override void OnEpisodeBegin()
    {
        MoveSpeed = 12.5f;
        Stamina = 100f;
        StaminaConsumptionRate = 1f;
        _checkPointCnt = 0;
        CheckedPointDictionary.Clear();

        _rigidbody.velocity = _rigidbody.angularVelocity = Vector3.zero;
        transform.localPosition = _startPos;
        transform.localRotation = _startRotation;

        _isGoalIn = false;

        _distanceToNextCheckPoint = 0f;
        _lastDistanceToNextCheckPoint = 0f;

        _goalTimer = 0f;

        GameManager.Instance.GoalInRacer.Clear();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_goalTimer);
        sensor.AddObservation(transform.forward);
        sensor.AddObservation(_distanceToNextCheckPoint);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!_isGoalIn)
        {
            var DiscreteActions = actions.DiscreteActions;

            Vector3 rotationAxis = Vector3.zero;

            _lastDistanceToNextCheckPoint = GameManager.Instance.GetDistanceToNextCheckPoint(this);

            // DiscreteActions[0] : 지속(0), 가속(1), 감속(2)
            if (!(Stamina < 0))
            {
                switch (DiscreteActions[0])
                {
                    case 1:
                        {
                            MoveSpeed = Mathf.Clamp(MoveSpeed + 0.15f, 10f, 25f);
                            TurnSpeed = Mathf.Clamp(TurnSpeed - 0.25f, 10f, 35f);
                            StaminaConsumptionRate = Mathf.Clamp(StaminaConsumptionRate + 0.03f, 1f, 4f);
                            break;
                        }
                    case 2:
                        {
                            MoveSpeed = Mathf.Clamp(MoveSpeed - 0.15f, 10f, 25f);
                            TurnSpeed = Mathf.Clamp(TurnSpeed + 0.25f, 10f, 35f);
                            StaminaConsumptionRate = Mathf.Clamp(StaminaConsumptionRate - 0.03f, 1f, 4f);
                            break;
                        }
                }
            }
            else
            {
                MoveSpeed = 10f;
                TurnSpeed = 10f;
            }

            // DiscreteActions[1] : 전방(0), 좌측(1), 우측(2)
            switch (DiscreteActions[1])
            {
                case 1: rotationAxis = Vector3.down; break;
                case 2: rotationAxis = Vector3.up; break;
            }

            _rigidbody.MovePosition(transform.position + transform.forward * MoveSpeed * Time.fixedDeltaTime);
            transform.Rotate(rotationAxis, Mathf.Clamp(TurnSpeed * Time.fixedDeltaTime, -45f, 45f));

            _distanceToNextCheckPoint = GameManager.Instance.GetDistanceToNextCheckPoint(this);

            if (_distanceToNextCheckPoint > _lastDistanceToNextCheckPoint)
            {
                AddReward(-0.1f);
            }
            else if (_distanceToNextCheckPoint < _lastDistanceToNextCheckPoint)
            {
                AddReward(0.2f);
            }

            _staminaTimer += Time.deltaTime;
            if (_staminaTimer > _consumptionTime)
            {
                Stamina -= StaminaConsumptionRate;
                _staminaTimer = 0f;

                if (Stamina < 0)
                {
                    AddReward(-0.01f / MaxStep);
                }
            }

            _goalTimer = Time.deltaTime;
        }
        else
        {
            _rigidbody.velocity = Vector3.zero;
        }
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Horse"))
        {
            AddReward(-1f);
        }
        else if (collision.transform.CompareTag("Fence"))
        {
            AddReward(-1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("CheckPoint"))
        {
            if (!CheckedPointDictionary.ContainsKey(other.gameObject.name))
            {
                if (other == GameManager.Instance.CheckPointList[CheckedPointDictionary.Count])
                {
                    AddReward(0.1f);
                }
                else
                {
                    AddReward(-1f);
                }
                IncreaseCheckPointCnt();
                CheckedPointDictionary.Add(other.gameObject.name, other.gameObject);
            }
        }
        else if (other.transform.CompareTag("GoalLine"))
        {
            Debug.Log("일단 충돌은 됨");
            if (_checkPointCnt >= 21)
            {
                Debug.Log("체크 포인트도 다 돌았음");
                if (!GameManager.Instance.GoalInRacer.ContainsKey(gameObject.name))
                {
                    Debug.Log("골인해서 골인한 레이서 목록에 넣어주고 리워드 줌");
                    GameManager.Instance.GoalInRacer.Add(gameObject.name, gameObject);
                    GameManager.Instance.GetRewardByRanking(this);
                    _isGoalIn = true;

                }
                if (GameManager.Instance.GoalInRacer.Count >= GameManager.Instance.RacerList.Count - 1)
                {
                    Debug.Log("EndEpisode");
                    GameManager.Instance.EndEpisodeAllRacer();
                }
            }
        }
    }

    public void IncreaseCheckPointCnt()
    {
        _checkPointCnt++;
    }
}
