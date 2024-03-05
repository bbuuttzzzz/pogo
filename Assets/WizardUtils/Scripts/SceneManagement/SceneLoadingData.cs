using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils.SceneManagement
{
    public class SceneLoadingData
    {
        public ControlSceneDescriptor InitialScene;
        public ControlSceneDescriptor TargetControlScene;
        public int[] TargetSceneBuildIds;
        public Action Callback;
        public float StartTime;
        public float MinimumLoadTime;
        public Coroutine DelayedFinishLoadCoroutine;

        public override string ToString()
        {
            string startName = InitialScene != null ? InitialScene.name : "NON-CONTROL";
            string endName = TargetControlScene != null ? TargetControlScene.name : BuildIdsToString();
            return $"[{startName} -> {endName}]";
        }

        private string BuildIdsToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < TargetSceneBuildIds.Length; i++)
            {
                int buildId = TargetSceneBuildIds[i];
                sb.Append(buildId);
                if (i != TargetSceneBuildIds.Length - 1)
                {
                    sb.Append(" ");
                }
            }

            return sb.ToString();
        }
    }
}
