using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public List<RacerAgent> RacerList = new List<RacerAgent>();
    public List<GameObject> CheckPointList = new List<GameObject>();
    public Dictionary<RacerAgent, float> Ranking = new Dictionary<RacerAgent, float>();
    public Dictionary<string, GameObject> GoalInRacer = new Dictionary<string, GameObject>();

    public List<CinemachineVirtualCamera> VCamList = new List<CinemachineVirtualCamera>();
    private int _camIdx = 0;

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            VCamList[_camIdx++].Priority = 10;
            _camIdx = _camIdx % VCamList.Count;
            VCamList[_camIdx].Priority = 20;
        }

        foreach (var racer in RacerList)
        {
            Ranking[racer] = Vector3.Distance(CheckPointList[0].transform.position, racer.transform.position);
        }
        Ranking.OrderBy(item => item.Value).ToDictionary(item => item.Key, item => item.Value);
    }

    public float GetRewardByRanking(RacerAgent agent)
    {
        int idx = 0;
        foreach(var racer in RacerList)
        {
            idx++;
            if(racer == agent)
            {
                return 11 - idx;
            }
        }

        return 0;
    }

    public void EndEpisodeAllRacer()
    {
        foreach(var racer in RacerList)
        {
            racer.EndEpisode();
        }
    }

    public float GetDistanceToNextCheckPoint(RacerAgent racer)
    {
        return Vector3.Distance(CheckPointList[racer._checkPointCnt].transform.position, racer.transform.position);
    }
}
