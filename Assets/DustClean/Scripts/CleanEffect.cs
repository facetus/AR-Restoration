using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace cleanDust
{

    public class CleanEffect : MonoBehaviour, IDragHandler, IEndDragHandler
    {


        /// <summary>
        /// Setup: We need our Texture (in this case the NewDust texture) to have the following settings:
        ///     -Texture Type: Default;
        ///     -Alpha Source: Input Texture Alpha;
        ///     -Alpha is transparent: enabled;
        ///     -Non-Power of 2: None;
        ///     -Read/Write: enabled;
        ///     -Format: RGBA 32 Bit;
        ///     
        /// This setup allows us to receive all the information we need from the texture we need make our calculation;
        /// 
        /// The following code implements a "Cleaning" effect in Unity where the user can erase parts of an image by dragging a brush over it;
        /// 
        /// Top Image and Mask Texture: The script uses a RawImage (topImage) and a Texture2D (maskTexture) to display the image to be cleaned.
        /// It creates a modifiable copy of the mask texture (cleanMask), which is used to track which parts of the image have been erased.
        /// 
        /// Cleaning Logic: The CleanUpPosition method determines which pixels of the cleanMask are affected by the brush, based on the brush size and position.
        /// It then sets these pixels' alpha values to 0 (fully transparent), simulating the cleaning effect. The cleanMask.Apply() method updates the texture after these changes.
        /// 
        /// Transparency Check: The IsTransparent method checks if the entire cleanMask texture is fully transparent by iterating over each pixel.
        /// The GetPixelsLeft method uses this check to determine whether the whole image has been cleaned.
        /// 
        /// 
        /// </summary>


        public RawImage topImage;
        public Texture2D maskTexture;
        public int brushSize = 20;

        private Texture2D cleanMask;
        [SerializeField] private GameObject currentBrush;

        [SerializeField] private GameObject dustImage;

        private bool canSpawnDust;
        [SerializeField] private float dustSpawnTimer;

        [SerializeField] private GameObject indicator;
        [SerializeField] private GameObject indicatorText;

        private bool allowInput;

        private bool paused;
        //Events;
        public static event Action<bool> OnWin;

        private void OnEnable()
        {
            SettingsController.OnPause += PauseGame;
        }

        private void OnDisable()
        {
            SettingsController.OnPause -= PauseGame;
        }

        public void PauseGame(bool isPause)
        {
            paused = isPause;
        }


        void Start()
        {
            canSpawnDust = true;
            cleanMask = Instantiate(maskTexture);
            topImage.material.SetTexture("_MaskTex", cleanMask);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (indicator.activeInHierarchy)
                StartCoroutine(RemoveIndicators());

            if (!paused)
            {
                GetPointPosition(eventData);
            }
            
            
            

        }
        IEnumerator RemoveIndicators()
        {
            indicator.GetComponent<CanvasGroup>().DOFade(0f, 0.5f);
            indicatorText.GetComponent<CanvasGroup>().DOFade(0f, 0.5f);

            yield return new WaitForSeconds(0.5f);

            indicator.SetActive(false);
            indicatorText.SetActive(false);
        }

        private void GetPointPosition(PointerEventData eventData)
        {
            Vector2 localPointerPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                topImage.rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localPointerPosition
            );
            Debug.Log(localPointerPosition);

            if (topImage.rectTransform.rect.Contains(localPointerPosition))
            {

                SpawnDust(localPointerPosition);
                SpawnBrush(localPointerPosition);
                CleanUpPosition(localPointerPosition);
            }
        }

        private void SpawnBrush(Vector2 localPointerPosition)
        {
            if (currentBrush != null)
            {
                currentBrush.SetActive(true);
                currentBrush.transform.SetParent(this.transform, false);
                currentBrush.GetComponent<RectTransform>().anchoredPosition = localPointerPosition;


            }
        }

        private void SpawnDust(Vector2 localPointerPosition)
        {
            //We spawn our dust image prefab;
            if (dustImage != null && canSpawnDust)
            {
                canSpawnDust = false;
                GameObject dustImageInstance = Instantiate(dustImage, this.transform);

                //We set the position;
                RectTransform dustImageRectTransform = dustImageInstance.GetComponent<RectTransform>();
                dustImageRectTransform.anchoredPosition = localPointerPosition;

                
                dustImageInstance.SetActive(true);


                StartCoroutine(DustFly(dustImageRectTransform));
                StartCoroutine(Timer(dustSpawnTimer));
            }
        }
        IEnumerator Timer(float timer)
        {
            yield return new WaitForSeconds(timer);
            canSpawnDust = true;
        }

        private IEnumerator DustFly(RectTransform dust)
        {
            // Define the duration and distance
            float flyDuration = 2f;
            float flyDistance = 150f;

            //We make the image fly upwards and slowly fade it;
            dust.DOAnchorPos(dust.anchoredPosition + new Vector2(0, flyDistance), flyDuration)
                .SetEase(Ease.OutCubic);
            dust.GetComponent<CanvasGroup>().DOFade(0f, flyDuration);

            // Wait for the animation to complete
            yield return new WaitForSeconds(flyDuration);

            // Destroy the dust image
            Destroy(dust.gameObject);
        }



        private void CleanUpPosition(Vector2 localPointerPosition)
        {
            int x = Mathf.Clamp((int)((localPointerPosition.x + topImage.rectTransform.rect.width * 0.5f) / topImage.rectTransform.rect.width * cleanMask.width), 0, cleanMask.width);
            int y = Mathf.Clamp((int)((localPointerPosition.y + topImage.rectTransform.rect.height * 0.5f) / topImage.rectTransform.rect.height * cleanMask.height), 0, cleanMask.height);

            for (int i = -brushSize; i <= brushSize; i++)
            {
                for (int j = -brushSize; j <= brushSize; j++)
                {
                    int nx = x + i;
                    int ny = y + j;
                    if (nx >= 0 && ny >= 0 && nx < cleanMask.width && ny < cleanMask.height)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), new Vector2(nx, ny));
                        if (distance <= brushSize)
                        {

                            cleanMask.SetPixel(nx, ny, new Color(0, 0, 0, 0)); // Clear the dirt by setting alpha to 0

                        }
                    }
                }
            }


            //We want to call .Apply(); because .Apply is an expensive method since it copies all the pixels no matter how many we changed;
            cleanMask.Apply();

        }


        private void GetPixelsLeft()
        {
            bool allTransparent = IsTransparent(cleanMask);
            Debug.Log("All pixels transparent: " + allTransparent);

            //Check if we won;
            if (allTransparent)
            {
                OnWin?.Invoke(true);
            }
        }



        bool IsTransparent(Texture2D tex)
        {
            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    //Check if the pixel is not fully transparent;
                    if (tex.GetPixel(x, y).a > 0f)
                    {
                        return false; //If we find any pixel that is not transparent, return false;
                    }
                }
            }
            return true; //If all pixels are fully transparent, return true;
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            if (currentBrush != null)
            {
                //We call our method here because they are very expensive(as they loop through all the pixels in our texture);
                //and we only need to get the information after our player releases the brush;

                //Count how many pixels are left
                GetPixelsLeft();
                currentBrush.SetActive(false);
            }
        }
    }

}