using Pogo;
using Pogo.MaterialTypes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WizardPhysics.PhysicsTime;
using WizardUtils;

namespace WizardPhysics
{
    [RequireComponent(typeof(Collider))]
    public class OrbSafeMover : MonoBehaviour, ISpecialPlayerCollisionBehavior
    {
        private List<CollisionGroup> Subscribers = new List<CollisionGroup>();
        Collider self;
        private Vector3 lastVelocity;
        public bool ShouldFollowTargetPosition;
        public bool TargetPositionIsLocal;
        public Vector3 TargetPosition;

        public Transform RendererTransform;

        private bool _subscribedToPhysicsUpdates;

        [System.Serializable]
        public enum PlayerCollisionBehavior
        {
            AlwaysInheritVelocity,
            NeverInheritVelocity,
            SmartAccelerate
        }
        public PlayerCollisionBehavior CollisionBehavior;

        private void Awake()
        {
            self = GetComponent<Collider>();
            if (ShouldFollowTargetPosition && RendererTransformExists())
            {

                _subscribedToPhysicsUpdates = true;

                PogoGameManager.PogoInstance.TimeManager.OnPhysicsUpdate.AddListener(OnPhysicsUpdate);
                PogoGameManager.PogoInstance.TimeManager.OnRenderUpdate.AddListener(OnRenderUpdate);
            }
        }

        private bool RendererTransformExists()
        {
            if (RendererTransform == null)
            {
                Debug.LogError($"OrbSafeMover DISABLED!!! RendererTransform missing for {name}", gameObject);
                return false;
            }
            return true;
        }

        private void OnDestroy()
        {
            if (_subscribedToPhysicsUpdates)
            {
                PogoGameManager.PogoInstance.TimeManager.OnPhysicsUpdate.RemoveListener(OnPhysicsUpdate);
                PogoGameManager.PogoInstance.TimeManager.OnRenderUpdate.RemoveListener(OnRenderUpdate);
            }
        }

        #region Following
        private Vector3 lastPhysicsPosition;
        private Vector3 currentPhysicsPosition;

        private void OnPhysicsUpdate()
        {
            lastPhysicsPosition = currentPhysicsPosition;
            currentPhysicsPosition = TargetPositionIsLocal ? transform.parent.TransformPoint(TargetPosition) : TargetPosition;
            PhysicsMoveTo(currentPhysicsPosition);
        }

        private void OnRenderUpdate(RenderArgs arg0)
        {
            RendererMoveTo(Vector3.Lerp(lastPhysicsPosition, currentPhysicsPosition, arg0.FrameInterpolator));
        }

#if UNITY_EDITOR
        public void FollowTargetPositionInEditor()
        {
            Vector3 targetPosition = TargetPositionIsLocal ? transform.parent.TransformPoint(TargetPosition) : TargetPosition;
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                transform.position = targetPosition;
                return;
            }

            PhysicsMoveTo(targetPosition);
            RendererMoveTo(targetPosition);
        }
#endif

        #endregion

        public void Subscribe(CollisionGroup group)
        {
            Subscribers.Add(group);
        }

        public void Unsubscribe(CollisionGroup group)
        {
            Subscribers.Remove(group);
        }

        public void PhysicsMoveTo(Vector3 finalPosition)
        {
            lastVelocity = (finalPosition - transform.position) / Time.fixedDeltaTime;

            if (transform.position == finalPosition) { return; }

            foreach(var collisionGroup in Subscribers)
            {
                collisionGroup.TestAgainst(self, finalPosition - transform.position);
            }

            transform.position = finalPosition;
        }

        public void RendererMoveTo(Vector3 finalPosition) => RendererTransform.position = finalPosition;

        /// <summary>
        /// Check if this object has special player collision behavior given this data, and then perform that behavior
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        /// <returns>true if the behavior has been overridden</returns>
        /// <param name="surfaceConfig"></param>
        bool ISpecialPlayerCollisionBehavior.TryOverrideCollisionBehavior(PlayerController target, CollisionEventArgs args, SurfaceConfig surfaceConfig)
        {
            switch (CollisionBehavior)
            {
                case PlayerCollisionBehavior.AlwaysInheritVelocity:
                    target.Velocity += lastVelocity;
                    return false;
                case PlayerCollisionBehavior.SmartAccelerate:
                    SmartAccelerate(target, args, surfaceConfig);
                    return true;
                case PlayerCollisionBehavior.NeverInheritVelocity:
                    return false;
                default:
                    throw new ArgumentException();
            }
        }

        private void SmartAccelerate(PlayerController target, CollisionEventArgs args, SurfaceConfig surfaceConfig)
        {
            // enter the frame of reference of the mover
            Vector3 localVelocity = target.Velocity - lastVelocity;

            // perform the "normal" jump behavior in this frame of reference
            // jump away from the surface
            Accelerate(ref localVelocity, args.HitInfo.normal, 2 * surfaceConfig.SurfaceRepelForceMultiplier);
            if (surfaceConfig.JumpForceMultiplier > 0)
            {
                // jump up based on the player's rotation
                Accelerate(ref localVelocity, target.DesiredModelRotation * Vector3.up, PlayerController.JumpForce * surfaceConfig.JumpForceMultiplier);
            }

            // leave the frame of reference of the mover
            target.Velocity = localVelocity + lastVelocity;
        }

        private void Accelerate(ref Vector3 velocity, Vector3 direction, float maxSpeed)
        {
            float curSpeed = Vector3.Dot(velocity, direction);
            float addSpeed = maxSpeed - curSpeed;
            if (addSpeed <= 0)
            {
                return;
            }

            //since I'm not going too fast, make me go just the right speed in that direction
            velocity += addSpeed * direction;
        }
    }
}
