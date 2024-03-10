using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardPhysics;
using WizardPhysics.PhysicsTime;
using WizardUtils.Extensions;

namespace Pogo.Trains
{
    public class TrainCar : MonoBehaviour
    {
        private PogoGameManager gameManager;
        public OrbSafeMover Mover;
        public TrainTrack Track;
        public MeshRenderer RenderMeshRenderer;
        public MeshCollider PhysicsMeshCollider;
        public BoxCollider SubscribeTriggerBoxCollider;

        public float SubscribeTriggerSkinWidth = 4f;
        public float MaxFrameDistance = 1f;

        private float CurrentTrackTime;

        private Vector3 LastPhysicsPosition;
        private Vector3 CurrentPhysicsPosition;

        public void CopyVisuals(MeshRenderer renderer)
        {
            Mesh newMesh = renderer.GetComponent<MeshFilter>().mesh;
            RenderMeshRenderer.GetComponent<MeshFilter>().mesh = newMesh;
            RenderMeshRenderer.materials = renderer.materials;
            PhysicsMeshCollider.sharedMesh = newMesh;

            SubscribeTriggerBoxCollider.center = renderer.bounds.center;
            SubscribeTriggerBoxCollider.size = 2 * (renderer.bounds.extents + Vector3.one * SubscribeTriggerSkinWidth);
        }

        private void Awake()
        {
            gameManager = PogoGameManager.PogoInstance;
            gameManager.TimeManager.OnPhysicsUpdate.AddListener(OnPhysicsUpdate);
            gameManager.TimeManager.OnRenderUpdate.AddListener(OnRenderUpdate);
        }

        public void Offset(float offsetFraction)
        {
            CurrentTrackTime += offsetFraction * Track.TotalDuration;
        }

        private void OnPhysicsUpdate()
        {
            CurrentTrackTime += Time.fixedDeltaTime;
            LastPhysicsPosition = CurrentPhysicsPosition;
            CurrentPhysicsPosition = Track.Sample(CurrentTrackTime);

            // if we want to move too far in a single frame, do it without a physics update.
            if (Vector3.SqrMagnitude(CurrentPhysicsPosition - LastPhysicsPosition) > MaxFrameDistance * MaxFrameDistance)
            {
                LastPhysicsPosition = CurrentPhysicsPosition;
                Mover.transform.position = CurrentPhysicsPosition;
            }
            else
            {
                Mover.PhysicsMoveTo(CurrentPhysicsPosition);
            }
        }

        private void OnRenderUpdate(RenderArgs arg0)
        {
            Mover.RendererMoveTo(Vector3.Lerp(LastPhysicsPosition, CurrentPhysicsPosition, arg0.FrameInterpolator));
        }
    }
}
