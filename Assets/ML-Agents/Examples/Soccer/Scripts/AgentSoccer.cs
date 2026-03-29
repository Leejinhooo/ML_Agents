using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

public enum Team
{
    Blue=0,Purple=1
}
public class AgentSoccer : Agent
{
    public Team team;
    
    [HideInInspector] public Rigidbody agentRb; //에이전트 rigidbody 컴포넌트

    private float _lateralSpeed = 0.3f; //좌우 이동 속도 계수
    private float _forwardSpeed = 1.0f; //앞뒤 이동 속도 계수

    private float _kickPower;
    //에이전트 초기화:팀 설정, 포지션별 속도 설정, 컴포넌트 참조 획득
    public override void Initialize()
    {
        agentRb = GetComponent<Rigidbody>();
        agentRb.maxAngularVelocity = 500;
        
        GetComponent<BehaviorParameters>().TeamId = (int)team;
    }

    // 사용자가 직접 컨트롤 할 수 있게 함
    //휴리스틱(수동 조작) 입력 처리
    //사람이 키보드나 입력 장치로 에이전트를 직접 조작할 때 사용
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        base.Heuristic(in actionsOut);
    }

    //관측값 수집 함수
    //에이전트가 환경을 인식하기 위해 필요한 정보를 수집
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
    }

    //행동 수행 함수
    //신경망이 출력한 행동(Action)을 실제 게임 내 동작으로 변환
    public override void OnActionReceived(ActionBuffers actions)
    {
        MoveAgent(actions.DiscreteActions);
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        _kickPower = 0f;
        var forwardAxis = act[0]; //전후 이동 액선
        var rightAxis = act[1]; //좌우 이동
        var rotateAxis = act[2]; //회전

        switch (forwardAxis)
        {
            case 0:
                break; 
            case 1:
                dirToGo = transform.forward * _forwardSpeed;
                _kickPower = 1f;
                break;
            case 2:
                dirToGo = transform.forward * -_forwardSpeed;
                break;
        }

        switch (rightAxis)
        {
            case 0:
                break;
            case 1:
                dirToGo = transform.right * _lateralSpeed;
                break;
            case 2:
                dirToGo = transform.right * -_lateralSpeed;
                break;
        }

        switch (rotateAxis)
        {
            case 0:
                break;
            case 1: //왼쪽 이동
                rotateDir = transform.up * -1f;
                break;
            case 2: //오른쪽
                rotateDir = transform.up * 1f;
                break;
        }

        transform.Rotate(rotateDir, Time.fixedDeltaTime * 100f); //실제 물리 적용 : 회전 및 이동
        agentRb.AddForce(dirToGo,ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision other) //충돌 시 이벤트 함수
    {
        //공과 충돌했을 때 킥을 하는 기능
        var force = _kickPower *2000f;
        if (other.gameObject.CompareTag("ball"))
        {
            AddReward(0.2f);
            
            var dir = other.contacts[0].point - transform.position;
            dir = dir.normalized;
            other.gameObject.GetComponent<Rigidbody>().AddForce(dir * force);
        }
        
    }

    //에피소드 시작 시 호출
    //환경을 초기 상태로 리셋할 때 사용
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
    }
}
