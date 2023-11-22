using Pogo.Cosmetics;
using Pogo.MaterialTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WizardPhysics;
using WizardUtils;
using WizardUtils.Equipment;
using WizardUtils.Extensions;

namespace Pogo
{
    [RequireComponent(typeof(CollisionGroup))]
    public class FakePlayerController : MonoBehaviour, IPlayerModelControllerProvider
    {
        CollisionGroup collisionGroup;
        public AudioController AudioController;
        public float JumpForce = 6f;
        public float GravityScale = 1;
        [NonSerialized]
        public PlayerModelController CurrentModelController;
        [NonSerialized]
        public UnityEvent<PlayerModelController> OnModelControllerChanged;
        public EquipmentTypeDescriptor PlayerModelEquipmentType;
        public Vector3 Velocity;
        public float DefaultRotationDurationSeconds = 1;

        private void Awake()
        {
            loadSurfaceProperties();
            collisionGroup = GetComponent<CollisionGroup>();
            GetComponent<Equipper>().OnEquip.AddListener(Equipper_OnEquip);
            OnModelControllerChanged = new UnityEvent<PlayerModelController>();
            rotationOffset = transform.rotation.eulerAngles.y;
        }

        private void Update()
        {
            ApplyForce(GravityScale * Physics.gravity * Time.unscaledDeltaTime);
            Move();
        }

        private float rotationOffset;
        private float CurrentRawAngle
        {
            get => transform.localEulerAngles.y - rotationOffset;
            set
            {
                transform.localEulerAngles = new Vector3(0, value + rotationOffset, 0);
            }
        }
        public void SetRotationInstantly(float angle)
        {
            if (currentRotationCoroutine != null)
            {
                StopCoroutine(currentRotationCoroutine);
            }
            CurrentRawAngle = angle;
        }

        Coroutine currentRotationCoroutine;
        public void RotateTo(float angle) => RotateTo(angle, DefaultRotationDurationSeconds);
        public void RotateTo(float angle, float duration)
        {
            if (currentRotationCoroutine != null)
            {
                StopCoroutine(currentRotationCoroutine);
            }
            StartCoroutine(SmoothRotateTo(angle, duration));
        }

        private IEnumerator SmoothRotateTo(float targetAngle, float duration)
        {
            targetAngle = targetAngle.PositiveModulo(360);
            float initialAngle = CurrentRawAngle;
            float startTime = Time.unscaledTime;
            while (Time.unscaledTime < startTime + duration)
            {
                CurrentRawAngle = Mathf.LerpAngle(initialAngle, targetAngle, (Time.unscaledTime - startTime) / duration);
                yield return null;
            }
            CurrentRawAngle = targetAngle;
        }

        int lastJumpSoundIndex = -1;
        public void Jump(CollisionEventArgs args)
        {
            SurfaceConfig surfaceConfig = GetSurfacePropertyFromCollision(args.HitInfo);

            // play sound
            AudioClip sound;
            (sound, lastJumpSoundIndex) = surfaceConfig.NextRandomSound(lastJumpSoundIndex);
            if (sound != null) AudioController.PlayOneShot(sound);

            // jump up
            Accelerate(Vector3.up, JumpForce);
        }


        private void ApplyForce(Vector3 force)
        {
            Velocity += force;
        }

        /// <summary>
        /// add speed up to <paramref name="maxSpeed"/> along <paramref name="direction"/>
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="maxSpeed"></param>
        private void Accelerate(Vector3 direction, float maxSpeed)
        {
            float curSpeed = Vector3.Dot(Velocity, direction);
            float addSpeed = maxSpeed - curSpeed;
            if (addSpeed <= 0)
            {
                //already going too fast, so who cares
                return;
            }

            //since I'm not going too fast, make me go just the right speed in that direction
            Velocity += addSpeed * direction;
        }

        private void Equipper_OnEquip(EquipmentSlot arg0)
        {
            if (arg0.EquipmentType == PlayerModelEquipmentType)
            {
                CurrentModelController = GetComponent<Equipper>()
                    .FindSlot(PlayerModelEquipmentType)
                    .ObjectInstance
                    .GetComponent<PlayerModelController>();
                CurrentModelController.OnLoadAsDisplayModel.Invoke();
                OnModelControllerChanged.Invoke(CurrentModelController);
            }
        }


        private void Move()
        {
            collisionGroup.Move(Velocity * Time.deltaTime);
        }


        #region IPlayerModelControllerProvider
        PlayerModelController IPlayerModelControllerProvider.PlayerModelController => CurrentModelController;

        UnityEvent<PlayerModelController> IPlayerModelControllerProvider.OnModelControllerChanged => OnModelControllerChanged;

        #endregion

        // TODO pull this up
        #region Surfaces
        SurfaceConfigCacheEntry surfaceCache = new SurfaceConfigCacheEntry();
        SurfaceConfig GetSurfacePropertyFromCollision(RaycastHit hitInfo)
        {
            Material material = null;
            if (hitInfo.collider is MeshCollider)
            {
                MeshRenderer renderer;
                if (hitInfo.collider == surfaceCache.Collider)
                {
                    if (surfaceCache.MeshTriangleIndex == hitInfo.triangleIndex)
                    {
                        return surfaceCache.SurfaceConfig;
                    }
                    renderer = surfaceCache.MeshRenderer as MeshRenderer;
                    surfaceCache.MeshTriangleIndex = hitInfo.triangleIndex;
                }
                else
                {
                    renderer = hitInfo.collider.GetComponent<MeshRenderer>();
                    surfaceCache.Collider = hitInfo.collider;
                    surfaceCache.MeshRenderer = renderer;
                    surfaceCache.MeshTriangleIndex = hitInfo.triangleIndex;
                }
                if (renderer != null)
                {
                    if (hitInfo.collider is MeshCollider meshCollider && meshCollider.convex)
                    {
                        material = renderer.materials[0];
                    }
                    else
                    {
                        material = (hitInfo.collider as MeshCollider).sharedMesh.GetMaterialAtTriangle(renderer, hitInfo.triangleIndex);
                    }
                }
            }
            else
            {
                Renderer renderer;
                if (surfaceCache.Collider == hitInfo.collider)
                {
                    return surfaceCache.SurfaceConfig;
                }
                else
                {
                    renderer = hitInfo.collider.GetComponent<Renderer>();
                    surfaceCache.Collider = hitInfo.collider;
                    surfaceCache.MeshRenderer = renderer;
                    surfaceCache.MeshTriangleIndex = -1;
                }

                if (renderer != null)
                {
                    material = renderer.sharedMaterial;
                }
            }

            surfaceCache.SurfaceConfig = surfacePropertiesDict.ContainsKey(material) ? (surfacePropertiesDict[material] ?? DefaultSurfaceConfig)
                : DefaultSurfaceConfig;
            return surfaceCache.SurfaceConfig;
        }


        public SurfaceConfig DefaultSurfaceConfig;
        Dictionary<Material, SurfaceConfig> surfacePropertiesDict;
        void loadSurfaceProperties()
        {
            surfacePropertiesDict = new Dictionary<Material, SurfaceConfig>();
            var surfaceConfigs = Resources.LoadAll<SurfaceConfig>("Surfaces");
            foreach (var config in surfaceConfigs)
            {
                foreach (var material in config.Materials)
                {
                    try
                    {
                        surfacePropertiesDict.Add(material, config);
                    }
                    catch (ArgumentNullException)
                    {
                        Debug.LogError($"NULL Material for surface definition: {config.name}", config);
                    }
                    catch (ArgumentException)
                    {
                        // throw a pretty error for duplicate materials
                        var existingSurfaceConfigName = surfacePropertiesDict[material].name;
                        Debug.LogError($"Duplicate Surface Definition for material {material.name}: {existingSurfaceConfigName} & {config.name}", config);
                    }
                }
            }
        }
        #endregion
    }
}
