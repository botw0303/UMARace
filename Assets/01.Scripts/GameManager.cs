using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public List<RacerAgent> RacerList = new List<RacerAgent>();
    public List<CheckPoint> CheckPointList = new List<CheckPoint>();
    public Dictionary<RacerAgent, float> Ranking = new Dictionary<RacerAgent, float>();

    private void Update()
    {
        foreach(var racer in RacerList)
        {
            Ranking.Add(racer, Vector3.Distance(CheckPointList[0].transform.position, racer.transform.position));
        }
        Ranking = Ranking.OrderByDescending(item => item.Value).ToDictionary(item => item.Key, item => item.Value);
    }

    public void GameSetting()
    {
        CheckPointList = FindObjectsByType<CheckPoint>(FindObjectsSortMode.None).ToList();
        Ranking.Clear();
    }

    public void RemoveCheckPoint(CheckPoint checkPoint)
    {
        CheckPointList.Remove(checkPoint);
        Destroy(checkPoint);
    }

    public void GetRewardByRanking(RacerAgent agent)
    {
        // Dictionary의 검색해서 인덱스를 가져오고 그 인덱스에 따라서 점수 반환하도록
    }
}
