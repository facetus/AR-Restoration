using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;

namespace cleanDust
{
    public class Indicator : MonoBehaviour
    {

        public static event Action OnIndicatorRemove;


        [SerializeField] private int moveDistance;
        [SerializeField] private float moveDuration;

        RectTransform uiElement;
        private Vector2 originalPos;

        [SerializeField] private GameObject textIndicator;

        private void Start()
        {
            uiElement = this.GetComponent<RectTransform>();
            originalPos = uiElement.anchoredPosition;
        }
        private void OnEnable()
        {
            CleanEffectManager.OnStart += RevealIndicator;
        }

        private void OnDisable()
        {
            CleanEffectManager.OnStart -= RevealIndicator;
        }

        public void RevealIndicator()
        {
            //Small Delay
            StartCoroutine(Timer());
        }

        IEnumerator Timer()
        {
            yield return new WaitForSeconds(1f);

            this.GetComponent<CanvasGroup>().DOFade(1f, 2f);
            textIndicator.GetComponent<CanvasGroup>().DOFade(1f, 2f);

            //Start left right animation;
            StartCoroutine(StartAnimation());
        }

        IEnumerator StartAnimation()
        {
            yield return new WaitForSeconds(3f);

            // Move left
            uiElement.DOAnchorPosX(originalPos.x - moveDistance, moveDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                // Move right
                uiElement.DOAnchorPosX(originalPos.x + moveDistance * 2, moveDuration * 2).SetEase(Ease.Linear).OnComplete(() =>
                {
                    // Move back to the original position
                    uiElement.DOAnchorPos(originalPos, moveDuration).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        // Fade out after returning to the original position
                        StartCoroutine(FinishAnimations());
                    });

                    
                });
            });
        }

        private IEnumerator FinishAnimations()
        {
            yield return new WaitForSeconds(2f);
            textIndicator.GetComponent<CanvasGroup>().DOFade(0f, 1f);

            this.GetComponent<CanvasGroup>().DOFade(0f, 1f).OnComplete(() =>
            {
                this.gameObject.SetActive(false);
                textIndicator.gameObject.SetActive(false);


                OnIndicatorRemove?.Invoke();
            });
        }
    }

}