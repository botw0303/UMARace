using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoSingleton<UIManager>
{
    public int CurrentMoney = 1000000;
    public int SelectedRacerIdx;
    public int BettingMoney = 0;

    public TMP_InputField Ipfd;

    public void Select1()
    {
        SelectedRacerIdx = 0;
    }

    public void Select2()
    {
        SelectedRacerIdx = 1;
    }

    public void Select3()
    {
        SelectedRacerIdx = 2;
    }

    public void Select4()
    {
        SelectedRacerIdx = 3;
    }

    public void Select5()
    {
        SelectedRacerIdx = 4;
    }

    public void Select6()
    {
        SelectedRacerIdx = 5;
    }

    public void Select7()
    {
        SelectedRacerIdx = 6;
    }

    public void Select8()
    {
        SelectedRacerIdx = 7;
    }

    public void Select9()
    {
        SelectedRacerIdx = 8;
    }

    public void Select10()
    {
        SelectedRacerIdx = 9;
    }

    public void GameStart()
    {
        BettingMoney = int.Parse(Ipfd.text);
        if(BettingMoney < 1)
        {
            return;
        }
        SceneManager.LoadScene(1);
    }

    public void GameEnd()
    {
        foreach(RacerAgent racer in GameManager.Instance.RacerList)
        {
            if(racer != GameManager.Instance.RacerList[SelectedRacerIdx])
            {

            }
        }
    }
}
