using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WizardEffects
{
    public class EffectManager : MonoBehaviour
    {
        public static EffectManager self;

        public int MaxFinishedEffects = 20;
        List<Effect> FinishedEffects;

        public int MaxFinishedLastingEffects = 20;
        List<Effect> FinishedLastingEffects;

        Dictionary<string, EffectTemplate> effectLookupTable;

        // Messages
        private void Awake()
        {
            self = this;
            FinishedEffects = new List<Effect>();
            FinishedLastingEffects = new List<Effect>();

            LoadEffects();
        }

        void LoadEffects()
        {
            effectLookupTable = new Dictionary<string, EffectTemplate>();
            EffectTemplate[] effectTemplates = Resources.LoadAll<EffectTemplate>("Effects");
            foreach (EffectTemplate template in effectTemplates)
            {
                if (template.RegisterEffect) effectLookupTable[template.name] = template;
            }
        }

        // Internal Functions
        
        void EnqueueLastingEffect(Effect effect)
        {
            FinishedLastingEffects.Add(effect);

            // if we have more unused effects than we should have, destroy the oldest one
            if (self.FinishedLastingEffects.Count > self.MaxFinishedLastingEffects)
            {
                Effect oldestEffect = FinishedLastingEffects[0];
                FinishedLastingEffects.RemoveAt(0);
                Destroy(oldestEffect.gameObject);
            }
        }

        void EnqueueEffect(Effect effect)
        {
            FinishedEffects.Add(effect);

            // if we have more unused effects than we should have, destroy the oldest one
            if (self.FinishedEffects.Count > self.MaxFinishedEffects)
            {
                Effect oldestEffect = FinishedEffects[0];
                FinishedEffects.RemoveAt(0);
                Destroy(oldestEffect.gameObject);
            }
        }
        /// <summary>
        /// removes and returns the first effect from FinishedEffects of the same ID, else returns null
        /// </summary>
        /// <param name="effectName"></param>
        /// <returns></returns>
        Effect findCachedEffect(string effectID)
        {
            for (int n = 0; n < FinishedEffects.Count; n++)
            {
                if (FinishedEffects[n].effectID == effectID)
                {
                    Effect found = FinishedEffects[n];
                    if (!found.IsLastingEffect)
                    {
                        FinishedEffects.RemoveAt(n);
                        return found;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }
        Effect makeEffect(string effectID)
        {
            if (!effectLookupTable.ContainsKey(effectID))
            {
                throw new System.Exception($"Attempted to create unregistered effect \"{effectID}\". Is it registered?");
            }

            EffectTemplate template = effectLookupTable[effectID];
            GameObject g = Instantiate(template.Prefab);
            g.transform.parent = self.transform;
            g.name = effectID + (int)Time.time;
            Effect e = g.GetComponent<Effect>();
            
            try
            {
                e.ApplyTemplate(template);
            }
            catch(System.NullReferenceException ex)
            {
                throw new MissingComponentException($"Could not find Effect component for effect \"{effectID}\"", ex);
            }
            return e;
        }


        // Global Functions
        public static void EffectFinished(Effect effect)
        {
            if (effect.IsLastingEffect)
                self.EnqueueLastingEffect(effect);
            else
                self.EnqueueEffect(effect);
        }

        public static void CreateEffect(string effectID, EffectData data)
        {
            Effect e = self.findCachedEffect(effectID);

            if (e == null)
            {
                e = self.makeEffect(effectID);
            }

            e.ApplyTemplate(self.effectLookupTable[effectID]);
            e.Perform(data);
        }
        public static void RegisterEffect(string effectID, EffectTemplate effectTemplate)
        {
            self.effectLookupTable[effectID] = effectTemplate;
        }
    }

    [System.Serializable]
    public struct EffectPair
    {
        public string Name;
        public EffectTemplate Template;
    }
}