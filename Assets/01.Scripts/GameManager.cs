using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public List<RacerAgent> RacerList = new List<RacerAgent>();
    public List<GameObject> CheckPoints = new List<GameObject>();
    public Dictionary<RacerAgent, float> Ranking = new Dictionary<RacerAgent, float>();

    private void Update()
    {
        foreach(var racer in RacerList)
        {
            Ranking.Add(racer, Vector3.Distance(CheckPoints[0].transform.position, racer.transform.position));
        }
        Ranking = Ranking.OrderByDescending(item => item.Value).ToDictionary(item => item.Key, item => item.Value);
    }

    public void RemoveCheckPoint()
    {

    }
}
