using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Pogo.Cosmetics
{
    public class VendingMachineButtonController : MonoBehaviour
    {
        public string NoMoreUnlocksText;
        public Sprite NoMoreUnlocksSprite;

        [Range(0f, 1f)]
        public float HighlightPower;

        public Color HighlightColor1;
        public Color HighlightColor2;

        public float HighlightWidth1;
        public float HighlightWidth2;

        private Color NextUnlockBaseColor;

        public Outline outline;
        public Image CosmeticImage;
        public TextMeshProUGUI QuarterCountText;
        public TextMeshProUGUI NextUnlockText;

        public UnityEvent OnUnlockTriggered;

        public bool Highlighted;

        private bool buttonCanBeActive;
        public bool ButtonCanBeActive
        {
            get => buttonCanBeActive;
            set
            {
                buttonCanBeActive = value;
                UpdateDisplay();
            }
        }

        [NonSerialized]
        private VendingMachineUnlockData nextReward;
        public VendingMachineUnlockData NextReward
        {
            get => nextReward;
            set
            {
                nextReward = value;
                UpdateDisplay();
            }
        }
        public bool RewardAvailable => NextReward.CoinsNeeded <= 0 && nextReward.Cosmetic != null;

        [SerializeField]
        private int quarterCount;
        public int QuarterCount
        {
            get => quarterCount;
            set
            {
                quarterCount = value;
                UpdateDisplay();
            }
        }

        private void Awake()
        {
            NextUnlockBaseColor = NextUnlockText.color;
            UpdateDisplay();
        }

        private void LateUpdate()
        {
            if (Highlighted)
            {
                outline.OutlineWidth = Mathf.Lerp(HighlightWidth1, HighlightWidth2, HighlightPower);
                outline.OutlineColor = Color.Lerp(HighlightColor2 , HighlightColor1, HighlightPower);
            }
            else
            {
                outline.OutlineWidth = 0;
                outline.OutlineColor = Color.gray;
            }
        }

        [ContextMenu("Update Display Now")]
        public void UpdateDisplay()
        {
            GetComponent<Button>().interactable = ButtonCanBeActive && RewardAvailable;
            if (NextReward.Cosmetic != null)
            {
                CosmeticImage.enabled = true;
                CosmeticImage.sprite = NextReward.Cosmetic.Icon;
                if (RewardAvailable)
                {
                    NextUnlockText.text = "Click Me!";
                }
                else
                {
                    NextUnlockText.text = $"{NextReward.CoinsNeeded} More!";
                }
            }
            else
            {
                NextUnlockText.text = "";
                if (NoMoreUnlocksSprite != null)
                {
                    CosmeticImage.enabled = true;
                    CosmeticImage.sprite = NoMoreUnlocksSprite;
                }
                else
                {
                    CosmeticImage.enabled = false;
                }
                NextUnlockText.text = NoMoreUnlocksText;
            }
            QuarterCountText.text = GetQuarterCountText(QuarterCount);
        }

        Coroutine flashHideCoroutine;
        public void TriggerUnlock()
        {
            OnUnlockTriggered.Invoke();
            GetComponent<Animator>().SetTrigger("Unlock");
            if (flashHideCoroutine != null) StopCoroutine(flashHideCoroutine);
            StartCoroutine(DoUnlockAnimationStuff(1, 1));
        }

        public IEnumerator DoUnlockAnimationStuff(float disappearDuration, float fadeInDuration)
        {
            float startTime = Time.unscaledTime;
            CosmeticImage.gameObject.SetActive(false);
            Color invisColor = NextUnlockBaseColor;
            invisColor.a = 0f;
            NextUnlockText.color = invisColor;

            float t = 0;
            while (t < 1)
            {
                t = (Time.unscaledTime - startTime) / disappearDuration;
                yield return null;
            }
            
            CosmeticImage.gameObject.SetActive(true);
            startTime = Time.unscaledTime;
            t = 0;
            while (t < 1)
            {
                t = (Time.unscaledTime - startTime) / fadeInDuration;
                CosmeticImage.color = new Color(1, 1, 1, t);
                NextUnlockText.color = Color.Lerp(invisColor, NextUnlockBaseColor, t);
                yield return null;
            }

            CosmeticImage.color = Color.white;
            NextUnlockText.color = NextUnlockBaseColor;
        }

        private static string GetQuarterCountText(int count)
        {
            decimal money = count * 0.25m;
            return $"{money:0.00}";
        }
    }
}
