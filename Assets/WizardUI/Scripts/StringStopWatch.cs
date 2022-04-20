using UnityEngine;
using UnityEngine.Events;

namespace WizardUI
{
    public class StringStopWatch : MonoBehaviour
    {
        public float StartTime;

        public string StringFormat;

        [SerializeField]
        private bool calculateOnUpdate;
        public bool CalculateOnUpdate { get => calculateOnUpdate; set => calculateOnUpdate = value; }


        public UnityEvent<string> OnThink;


        public virtual void Update()
        {
            if (CalculateOnUpdate) OnThink?.Invoke(CalculateValue());
        }

        public void SetToValue(float value)
        {
            OnThink?.Invoke(StringFormat == null || StringFormat == "" ? value.ToString() : value.ToString(StringFormat));
        }

        public void Set()
        {
            StartTime = Time.time;
        }

        public string CalculateValue()
        {
            float value = Time.time - StartTime;
            return StringFormat == null || StringFormat == "" ? value.ToString() : value.ToString(StringFormat);
        }
    }
}
