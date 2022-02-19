using Inputter;
using Pogo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WizardPhysics;
using WizardUtils;

[RequireComponent(typeof(CollisionGroup))]

public class PlayerController : MonoBehaviour
{
    public AudioController AudioController;
    CollisionGroup collisionGroup;

    private void Awake()
    {
        collisionGroup = GetComponent<CollisionGroup>();    
    }

    void Start()
    {
        PogoGameManager.RegisterPlayer(this);
        PogoGameManager.GameInstance.OnPauseStateChanged += onPauseStateChanged;
        loadSurfaceProperties();

        var sensitivitySetting = PogoGameManager.GameInstance.FindGameSetting(PogoGameManager.KEY_SENSITIVITY);
        sensitivitySetting.OnChanged += onSensitivityChanged;
        SENSITIVITY = sensitivitySetting.Value;

        var invertYSetting = PogoGameManager.GameInstance.FindGameSetting(PogoGameManager.KEY_INVERT);
        invertYSetting.OnChanged += onInvertYChanged;
        SENS_PITCH_SCALE = 0.8f * invertYSetting.Value;

        UpdateCursorLock(PogoGameManager.Paused);
        internalEyeAngles = new Vector3(0, transform.localRotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.identity;

        collisionGroup.OnCollide.AddListener(onCollide);
    }

    private void onInvertYChanged(object sender, GameSettingChangedEventArgs e)
    {
        SENS_PITCH_SCALE = 0.8f * e.FinalValue;
    }

    private void onSensitivityChanged(object sender, GameSettingChangedEventArgs e)
    {
        SENSITIVITY = e.FinalValue;
    }

    // Update is called once per frame
    void Update()
    {
        DoLook();
        UpdateDesiredModelPitch();
        ApplyForces();
        RotateAndMove();
    }

    #region Game Logic
    public UnityEvent OnDie;

    public SurfaceConfig DefaultSurfaceConfig;
    Dictionary<Material, SurfaceConfig> surfacePropertiesDict;
    void loadSurfaceProperties()
    {
        surfacePropertiesDict = new Dictionary<Material, SurfaceConfig>();
        var surfaceConfigs = Resources.LoadAll<SurfaceConfig>("Surfaces");
        foreach( var config in surfaceConfigs )
        {
            foreach( var material in config.Materials )
            {
                try
                {
                    surfacePropertiesDict.Add(material, config);
                }
                catch(ArgumentException e)
                {
                    // throw a pretty error for duplicate materials
                    var existingSurfaceConfigName = surfacePropertiesDict[material].name;
                    throw new ArgumentException($"Duplicate Surface Definition for material {material.name}: {existingSurfaceConfigName} & {config.name}", e);
                }
            }
        }
    }

    SurfaceConfigCacheEntry surfaceCache = new SurfaceConfigCacheEntry();
    SurfaceConfig GetSurfacePropertyFromCollision(RaycastHit hitInfo)
    {
        Material material = null;
        if (hitInfo.collider is MeshCollider)
        {
            MeshRenderer renderer;
            if (hitInfo.collider == surfaceCache.Collider)
            {
                if (surfaceCache.TriangleIndex == hitInfo.triangleIndex)
                {
                    return surfaceCache.SurfaceConfig;
                }
                renderer = surfaceCache.Renderer as MeshRenderer;
                surfaceCache.TriangleIndex = hitInfo.triangleIndex;
            }
            else
            {
                renderer = hitInfo.collider.GetComponent<MeshRenderer>();
                surfaceCache.Collider = hitInfo.collider;
                surfaceCache.Renderer = renderer;
                surfaceCache.TriangleIndex = hitInfo.triangleIndex;
            }
            if (renderer != null)
            {
                material = (hitInfo.collider as MeshCollider).sharedMesh.GetMaterialAtTriangle(renderer, hitInfo.triangleIndex);
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
                surfaceCache.Renderer = renderer;
                surfaceCache.TriangleIndex = -1;
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

    public void DieFromSurface(CollisionEventArgs collision)
    {
        WizardEffects.EffectManager.CreateEffect("DeathMark", new WizardEffects.EffectData()
        {
            position = collision.HitInfo.point,
            normal = collision.HitInfo.normal
        });
        Die();
    }
    public void Die(KillType killType = null)
    {
        transform.position = PogoGameManager.PogoInstance.RespawnPoint.position;
        internalEyeAngles = new Vector3(0, PogoGameManager.PogoInstance.RespawnPoint.rotation.eulerAngles.y, 0);
        Model.rotation = DesiredModelRotation;

        Velocity = Vector3.zero;
        PitchFrac = 0;
        OnDie.Invoke();
        PogoGameManager.PogoInstance?.OnPlayerDeath.Invoke();
        if (killType != null)
        {
            AudioController.PlayOneShot(killType.RandomSound);
        }
    }

    private void onPauseStateChanged(object sender, bool e)
    {
        UpdateCursorLock(e);
    }

    private static void UpdateCursorLock(bool isPaused)
    {
#if UNITY_WEBGL
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
#else
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
#endif
    }
    #endregion

    #region Model maneuvering
    public Transform Model;
    public Transform Camera;
    public float PitchFrac = 0;
    const float ModelPitchMul = 1.5f;
    const float PitchFracSpeed = 10;


    public Quaternion DesiredModelRotation => Quaternion.Euler(PitchFrac * ModelPitchMul * EyeAngles.x, EyeAngles.y, EyeAngles.z);
    public Quaternion CameraRotation => Quaternion.Euler(PitchFrac * EyeAngles.x, EyeAngles.y, EyeAngles.z);

    void UpdateDesiredModelPitch()
    {
        float targetPitchFrac = InputManager.CheckKey(KeyName.Jump) ? 0 : 1;
        PitchFrac = Mathf.MoveTowards(PitchFrac, targetPitchFrac, PitchFracSpeed * Time.deltaTime);
    }

#endregion

#region Movement

    public Vector3 Velocity;

    const float JumpMaxSideSpeed = 6f;
    const float JumpForce = 6f;
    const float AIR_ACCELERATE = 0;
    const float AIR_SPEED_MAX = 1f; //max tangent air speed, the lower this value the slower the airstrafe

    public void ApplyForce(Vector3 force)
    {
        Velocity += force;
    }

    public void ApplyForces()
    {
        Vector3 movement = InputManager.CheckAxisSet(AxisSetName.Movement);
        Vector3 airMove = GetYawQuat() * new Vector3(movement.x, 0, movement.z);
        Velocity = AirAccelerate(Velocity, airMove);

        ApplyForce(Physics.gravity * Time.deltaTime);
        Debug.Log($"{Velocity.y:N3}\t{Time.deltaTime:N3}");
    }

    public void Jump(CollisionEventArgs args)
    {
        var surfaceConfig = GetSurfacePropertyFromCollision(args.HitInfo);
        var sound = surfaceConfig.RandomSound;
        if (sound != null) AudioController.PlayOneShot(sound);

        Accelerate(args.HitInfo.normal, 2 * surfaceConfig.SurfaceRepelForceMultiplier);
        if (surfaceConfig.JumpForceMultiplier > 0)
        {
            Accelerate(DesiredModelRotation * Vector3.up, JumpForce * surfaceConfig.JumpForceMultiplier);
        }
        //Decelerate(ModelRotation * Vector3.up, JumpMaxSideSpeed, 1);
    }

    public void RotateAndMove()
    {
        if (Time.deltaTime == 0) return;
        collisionGroup.RotateTo(DesiredModelRotation);
        collisionGroup.Move(Velocity * Time.deltaTime);
    }

    private void onCollide(CollisionEventArgs e)
    {
        // if we have any speed into the surface, remove it
        var normalVelocity = Velocity.GetNormalComponent(e.HitInfo.normal);
        if (normalVelocity.magnitude < 0)
        {
            Velocity -= normalVelocity;
        }
    }

    public UnityEvent OnDisjoint;
    public void Disjoint()
    {
        OnDisjoint?.Invoke();
    }
#endregion

#region Physics
    /// <summary>
    /// Airstrafing function from quake 3 movement code
    /// </summary>
    /// <param name="vel">Initial Velocity</param>
    /// <param name="mov">player's movement input</param>
    /// <returns>New velocity</returns>
    Vector3 AirAccelerate(Vector3 vel, Vector3 mov)
    {
        float ACCEL = AIR_ACCELERATE;

        Vector3 flatVel = vel.Flatten();
        float currentspeed = Vector3.Dot(flatVel, mov); //gets velocity along movement
                                                        //weird step here from the quake guys
                                                        //really, i want to see if AIR_SPEED_MAX is bigger than currentspeed
                                                        //then, if it is, add the amount it is bigger by the player's speed along mov
                                                        //this just cuts out a step because lol
        float addspeed = AIR_SPEED_MAX - currentspeed; //finds how far from max your current speed is

        if (addspeed <= 0) //if you're already accelerating over max, don't change anything
            return vel;

        float accelspeed = ACCEL * Time.deltaTime * AIR_SPEED_MAX; //finds the acceleration to add
        if (accelspeed > addspeed) //if the acceleration would put you over max, cap it at max.
            accelspeed = addspeed;

        return vel + accelspeed * mov;
    }

    /// <summary>
    /// Removes speed over <paramref name="maxSpeed"/> that's not along <paramref name="direction"/>
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="maxSpeed"></param>
    public void Decelerate(Vector3 direction, float maxSpeed, float maxSpeedReduction = float.MaxValue)
    {
        Vector3 tangentVel = Velocity.GetTangentComponent(direction);
        float curSpeed = tangentVel.magnitude;
        float removeSpeed = curSpeed - maxSpeed;
        if (removeSpeed > 0)
        {
            if (removeSpeed > maxSpeedReduction)
            {
                Debug.Log($"{curSpeed} -> {curSpeed - Mathf.Min(removeSpeed, maxSpeedReduction)} (-{Mathf.Min(removeSpeed, maxSpeedReduction)})");
            }
            Velocity -= tangentVel * Mathf.Min(removeSpeed, maxSpeedReduction) / curSpeed;
        }
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
#endregion

#region Aiming
    public Transform CameraSwivelPoint;

    private float SENSITIVITY = 0.1f; //default: 0.5. mouselook speed
    private float SENS_PITCH_SCALE = 0.8f; //fraction of sensitivity to apply for pitch
    private Vector3 internalEyeAngles; // player's aim direction before camera effects
    public Vector3 EyeAngles => internalEyeAngles;

    /// <summary>
    /// Return a rotation along player's aim direction
    /// </summary>
    /// <returns></returns>
    public Quaternion GetAimQuat()
    {
        return Quaternion.Euler(EyeAngles);
    }
    /// <summary>
    /// Return a rotation of the player's aim direction, flattened
    /// </summary>
    /// <returns></returns>
    public Quaternion GetYawQuat()
    {
        return Quaternion.Euler(0, EyeAngles.y, 0);
    }
    /// <summary>
    /// Returns a vector along player's aim direction
    /// </summary>
    /// <returns name = aimDir></returns>
    public Vector3 GetAimDir()
    {
        return GetAimQuat() * Vector3.forward;
    }
    void DoLook()
    {
        if (PogoGameManager.Paused) return;

        //change eyeAngles based on mouse
        internalEyeAngles += InputManager.CheckAxisSet(AxisSetName.Aim).Scale(SENSITIVITY * SENS_PITCH_SCALE, SENSITIVITY, SENSITIVITY);
        //clamp pitch
        internalEyeAngles.x = Mathf.Clamp(internalEyeAngles.x, -89.9f, 89.9f);

        CameraSwivelPoint.transform.rotation = GetAimQuat();
    }
#endregion
}
