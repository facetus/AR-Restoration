using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace cleanDust
{
    public class CleanEffectManager : MiniGame
    {
        public static event Action OnStart;

        [SerializeField] private GameObject startScreen;
        [SerializeField] private GameObject gamePlay;
        [SerializeField] private GameObject endScreen;

        private bool hasWon;


        private void OnEnable()
        {
            CleanEffect.OnWin += EndGame;
            
        }

        private void OnDisable()
        {
            CleanEffect.OnWin -= EndGame;
        }


        private void Start()
        {
            //achievementManager = FindObjectOfType<AchievementManager>();
            //bagdeCaseHandler = FindObjectOfType<BagdeCaseHandler>();
            EnableStartScreen();
        }

        public override void CalculateScore()
        {
            throw new System.NotImplementedException();
        }

        public override void EndGame(bool won)
        {
            hasWon = won;
            


            //gamePlay.GetComponent<FadingComponent>().Disable();
            EndScreen();

            //Gamehunting script
            
        }

        public override void EndScreen()
        {

            StartCoroutine(Timer());
            
        }

        public override void GainAchievement()
        {
            endScreen.SetActive(false);
            //Gamehunting script first;
            //GameHunting gameHunting = FindObjectOfType<GameHunting>();
            //gameHunting.FinishCurrentGame(gameHunting.FindPrefabIndex(gameObject.name), hasWon);

            //Achievement after;
            //achievementManager.Unlock(achievementKey);
            //bagdeCaseHandler.UnlockBudge(budgeIndex);

        }

        public override void StartGame()
        {
            startScreen.SetActive(false);
            gamePlay.GetComponent<CanvasGroup>().DOFade(1f, 1f);
            OnStart?.Invoke();
        }

        public void EnableStartScreen()
        {
            startScreen.SetActive(true);
            startScreen.GetComponent<CanvasGroup>().DOFade(1f, 1f);
        }

        IEnumerator Timer()
        {
            yield return new WaitForSeconds(1f);
            gamePlay.GetComponent<CanvasGroup>().DOFade(0f, 1f);
            gamePlay.SetActive(false);
            endScreen.SetActive(true);
            endScreen.GetComponent<CanvasGroup>().DOFade(1f, 1f);

        }
    }

}