using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MiniGame : MonoBehaviour
{
    //REQUIRE THE 2 SCRIPTS TO RUN;
    //[HideInInspector] public AchievementManager achievementManager;
    //[HideInInspector] public BagdeCaseHandler bagdeCaseHandler;

    public string achievementKey;
    public int budgeIndex;

    public abstract void StartGame();
    public abstract void EndScreen();

    public abstract void GainAchievement();
    public abstract void CalculateScore();

    public abstract void EndGame(bool won);



}
