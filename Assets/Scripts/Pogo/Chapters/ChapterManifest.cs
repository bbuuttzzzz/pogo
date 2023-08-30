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
    [CreateAssetMenu(fileName = "ChapterManifest", menuName = "ScriptableObjects/ChapterManifest", order = 1)]
    public class ChapterManifest : ScriptableObject
    {
        public ChapterDescriptor[] Chapters;

        public string DisplayName;
    }
}
