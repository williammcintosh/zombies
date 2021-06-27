using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LeaderScript
{
    public string leaderName;
    public int score;
    public string date;
    public LeaderScript(string newName, string newScore, string newDate)
    {
        leaderName = String.Copy(newName);
        score = int.Parse(newScore);
        date = String.Copy(newDate);
    }
    public LeaderScript(LeaderScript copyLeader)
    {
        leaderName = String.Copy(copyLeader.leaderName);
        score = copyLeader.score;
        date = String.Copy(copyLeader.date);
    }
}
