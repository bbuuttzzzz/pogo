using Assets.Scripts.Player;
using Pogo.MaterialTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WizardPhysics;
using WizardUtils;
using WizardUtils.Equipment;

namespace Pogo
{
    [RequireComponent(typeof(CollisionGroup))]
    public class FakePlayerController : MonoBehaviour
    {
        CollisionGroup collisionGroup;
        public AudioController AudioController;
        public float JumpForce = 6f;
        public float GravityScale = 1;
        [NonSerialized]
        public PlayerModelController CurrentModelController;
        public EquipmentTypeDescriptor PlayerModelEquipmentType;

        private void Awake()
        {
            loadSurfaceProperties();
            collisionGroup = GetComponent<CollisionGroup>();
            GetComponent<Equipper>().OnEquip.AddListener(Equipper_OnEquip);
        }

        private void Update()
        {
            ApplyForce(GravityScale * Physics.gravity * Time.unscaledDeltaTime);
            Move();
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
            }
        }

        private void Move()
        {
            collisionGroup.Move(Velocity * Time.deltaTime);
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

        public Vector3 Velocity;

        public void ApplyForce(Vector3 force)
        {
            Velocity += force;
        }

        /// <summary>
        /// add speed up to <paramref name="maxSpeed"/> along <paramref name="direction"/>
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="maxSpeed"></param>
        public void Accelerate(Vector3 direction, float maxSpeed)
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
