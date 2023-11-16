using System;
using UnityEngine;
using UnityEngine.Events;

namespace WizardEffects
{
    public class Effect : MonoBehaviour
    {
        EffectTemplate Template;
        public string effectID => Template.name;
        public float lifetime;
        [HideInInspector] public bool idle = true;
        float finishTime;
        public bool IsLastingEffect => Template.IsLastingEffect;

        public EffectEvent OnPerform;
        public UnityEvent OnFinish;

        public bool MoveToPosition;

        public virtual void Perform(EffectData effectData)
        {
            idle = false;
            finishTime = Time.time + lifetime;
            if (MoveToPosition)
            {
                transform.position = effectData.position;
            }
            OnPerform?.Invoke(new EffectEventArgs(effectData));
        }

        public virtual void Update()
        {
            if (!idle && Time.time > finishTime)
            {
                idle = true;
                OnFinish?.Invoke();
                FinishEffect();
            }
        }

        protected virtual void FinishEffect()
        {
            EffectManager.EffectFinished(this);
        }
        public virtual void ApplyTemplate(EffectTemplate Template)
        {
            this.Template = Template;
        }
    }
}