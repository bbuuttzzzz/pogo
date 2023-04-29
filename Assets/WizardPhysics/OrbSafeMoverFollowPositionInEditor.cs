using System.Collections;
using UnityEngine;
using WizardPhysics;

namespace Assets.WizardPhysics
{
    [ExecuteAlways]
    public class OrbSafeMoverFollowPositionInEditor : MonoBehaviour
    {
        public OrbSafeMover Target;

        private void Update()
        {
            if (UnityEditor.EditorApplication.isPlaying) return;
            if (Target == null) return;

            if (Target.ShouldFollowTargetPosition)
            {
                Target.FollowTargetPosition();
            }
        }
    }
}