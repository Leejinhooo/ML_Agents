using Unity.AppUI.Redux;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class RollerAgent : Agent
{
    private Rigidbody _rigidbody;

    public Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        
    }

    public override void OnEpisodeBegin() //에피소드가 실행됐을 때 어떻게 초기화시켜줄지.
    {
        if (transform.localPosition.y < 0)
        {
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.linearVelocity = Vector3.zero;
            transform.localPosition = new Vector3(0, 0.05f, 0);
        }
        target.localPosition = new Vector3(Random.value *8-4,0.5f, Random.value *8-4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(_rigidbody.linearVelocity.x);
        sensor.AddObservation(_rigidbody.linearVelocity.y);
    }
    public float forceMultiplier = 10f;

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        _rigidbody.AddForce(controlSignal * forceMultiplier);
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }
        else if(transform.localPosition.y < 0) EndEpisode();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
