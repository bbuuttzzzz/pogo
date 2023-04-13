using System;
using System.Collections;
using System.Drawing;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using WizardUtils;
using WizardUtils.Saving;

namespace Pogo
{
    [CreateAssetMenu(fileName = "WorldDescriptor", menuName = "ScriptableObjects/WorldDescriptor", order = 1)]
    public class WorldDescriptor : ScriptableObject
    {
        
        public LevelDescriptor[] Levels = new LevelDescriptor[4];

        public string DisplayName;

        void OnValidate()
        {
            if (Levels.Length != 4)
            {
                Debug.LogWarning("Don't change the 'Levels' field's array size!");
                Array.Resize(ref Levels, 4);
            }
        }
    }
}
