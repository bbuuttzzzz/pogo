using Assets.Scripts.Player;
using Inputter;
using Pogo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WizardPhysics;
using WizardUtils;
using WizardUtils.SceneManagement;

[RequireComponent(typeof(CollisionGroup))]

public class PlayerController : MonoBehaviour
{
    public AudioController AudioController;
    public CollisionGroup CollisionGroup;
    public float AutoRespawnDelay;
    public PlayerJostler Jostler;

    private void Awake()
    {
        CollisionGroup = GetComponent<CollisionGroup>();    
    }

    void Start()
    {
        PogoGameManager.RegisterPlayer(this);
        PogoGameManager.GameInstance.OnPauseStateChanged += onPauseStateChanged;
        PogoGameManager.GameInstance.OnControlSceneChanged += onControlSceneChanged;
        loadSurfaceProperties();

        var sensitivitySetting = PogoGameManager.GameInstance.FindGameSetting(PogoGameManager.SETTINGKEY_SENSITIVITY);
        sensitivitySetting.OnChanged += onSensitivityChanged;
        SENSITIVITY = sensitivitySetting.Value;

        var invertYSetting = PogoGameManager.GameInstance.FindGameSetting(PogoGameManager.SETTINGKEY_INVERT);
        invertYSetting.OnChanged += onInvertYChanged;
        SENS_PITCH_SCALE = 0.8f * convertInvertYSetting(invertYSetting.Value);

        var respawnDelaySetting = PogoGameManager.GameInstance.FindGameSetting(PogoGameManager.SETTINGKEY_RESPAWNDELAY);
        respawnDelaySetting.OnChanged += onAutoRespawnDelayChanged;
        AutoRespawnDelay = respawnDelaySetting.Value;

        UpdateCursorLock(PogoGameManager.GameInstance.Paused);
        internalEyeAngles = new Vector3(0, transform.localRotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.identity;

        CollisionGroup.OnCollide.AddListener(onCollide);

        gameObject.SetActive(PogoGameManager.GameInstance.InControlScene);
        setControlSceneBehavior(PogoGameManager.GameInstance.InControlScene);
    }

    private static int convertInvertYSetting(float settingValue)
    {
        return settingValue == 1 ? -1 : 1;
    }

    private void OnDestroy()
    {
        PogoGameManager.GameInstance.OnPauseStateChanged -= onPauseStateChanged;
        PogoGameManager.GameInstance.OnControlSceneChanged -= onControlSceneChanged;
    }

    private void onAutoRespawnDelayChanged(object sender, GameSettingChangedEventArgs e)
    {
        AutoRespawnDelay = e.FinalValue;
    }

    private void onInvertYChanged(object sender, GameSettingChangedEventArgs e)
    {
        SENS_PITCH_SCALE = 0.8f * convertInvertYSetting(e.FinalValue);
    }

    private void onSensitivityChanged(object sender, GameSettingChangedEventArgs e)
    {
        SENSITIVITY = e.FinalValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentState == PlayerStates.Alive)
        {
            DoLook();
            UpdateDesiredModelPitch();
            ApplyForces();
            RotateAndMove();
        }
        else
        {
            CheckForRespawn();
        }
    }

    private void CheckForRespawn()
    {
        if (GameManager.GameInstance.Paused) return;

        if (InputManager.CheckKeyDown(KeyName.UIAdvance))
        {
            Spawn();
        }
    }

    #region Game Logic
    private PlayerStates currentState;
    public PlayerStates CurrentState
    {
        get => currentState;
        set
        {
            PlayerStateChangeEventArgs args = new PlayerStateChangeEventArgs(currentState, value);
            currentState = value;
            OnStateChanged?.Invoke(args);
        }
    }

    public UnityEvent<PlayerStateChangeEventArgs> OnStateChanged;

    public SurfaceConfig DefaultSurfaceConfig;
    public KillTypeDescriptor CollisionKillType;
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
            else if (hitInfo.collider is TerrainCollider)
            {
                var terrain = hitInfo.collider.GetComponent<Terrain>();
                material = terrain.materialTemplate;
                surfaceCache.MeshTriangleIndex = -1;
            }
            else
            {
                var specifier = hitInfo.collider.GetComponent<SurfaceConfigSpecifier>();
                if (specifier != null)
                {
                    surfaceCache.Collider = hitInfo.collider;
                    surfaceCache.SurfaceConfig = specifier.SurfaceConfig;
                    return specifier.SurfaceConfig;
                }

                renderer = hitInfo.collider.GetComponent<Renderer>();
                surfaceCache.Collider = hitInfo.collider;
                material = renderer.sharedMaterial;
            }
        }
        try
        {
            surfaceCache.SurfaceConfig = surfacePropertiesDict.ContainsKey(material) ? (surfacePropertiesDict[material] ?? DefaultSurfaceConfig)
                : DefaultSurfaceConfig;
        }
        catch(ArgumentNullException)
        {
            Debug.LogError($"Missing surfaceConfig for material {material}", material);
        }
        return surfaceCache.SurfaceConfig;
    }

    public void Respawn()
    {
        if (CurrentState != PlayerStates.Dead)
        {
            CurrentState = PlayerStates.Dead;
            PogoGameManager.PogoInstance?.OnPlayerDeath.Invoke();
        }

        Spawn();
    }

    public void DieFromSurface(CollisionEventArgs collision)
    {
        Die(new PlayerDeathData(CollisionKillType, collision.HitInfo.collider.transform, collision.HitInfo.point, collision.HitInfo.normal));
    }

    public void Die() => Die(new PlayerDeathData(CollisionKillType));

    public void Die(PlayerDeathData data)
    {
        if (CurrentState == PlayerStates.Dead) return;

        CurrentState = PlayerStates.Dead;
        PogoGameManager.PogoInstance?.OnPlayerDeath.Invoke();
        DelayedRespawnRoutine = StartCoroutine(DelayedRespawn(AutoRespawnDelay));
        if (data.KillType != null)
        {
            AudioController.PlayOneShot(data.KillType.RandomSound);
        }

        if (data.Position.HasValue && data.Normal.HasValue)
        {
            WizardEffects.EffectData effectData = new WizardEffects.EffectData()
            {
                parent = data.Parent,
                position = data.Position.Value,
                normal = data.Normal.Value
            };

            if (!string.IsNullOrEmpty(data.KillType.EffectName))
            {
                WizardEffects.EffectManager.CreateEffect(data.KillType.EffectName, effectData);
            }
        }
        Jostler.Play();
    }

    public void Spawn()
    {
        if (DelayedRespawnRoutine != null) StopCoroutine(DelayedRespawnRoutine);
        Jostler.Stop();
        CurrentState = PlayerStates.Alive;
        PogoGameManager.PogoInstance?.OnPlayerSpawn.Invoke();
        Reset();
    }

    public void Reset()
    {
        TeleportTo(PogoGameManager.PogoInstance.GetRespawnTransform());
    }

    private void ResetPhysics()
    {
        Velocity = Vector3.zero;
        PitchFrac = 0;
        Model.rotation = DesiredModelRotation;
    }

    public void TeleportTo(Transform target, bool preservePhysics = false)
    {
        transform.position = target.position;
        internalEyeAngles = new Vector3(0, target.rotation.eulerAngles.y, 0);
        if (!preservePhysics)
        {
            ResetPhysics();
        }
        Disjoint();
    }

    private void onControlSceneChanged(object sender, ControlSceneEventArgs e)
    {
        setControlSceneBehavior(e.InControlScene);
    }

    void setControlSceneBehavior(bool inControlScene)
    {
        Debug.Log($"Player setControlSceneBehavior {inControlScene}");
        gameObject.SetActive(!inControlScene);
        UpdateCursorLock(inControlScene);
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

    public Coroutine DelayedRespawnRoutine;
    public IEnumerator DelayedRespawn(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Spawn();
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
    public const float JumpForce = 6f;
    const float AIR_ACCELERATE = 0;
    public const float AIR_SPEED_MAX = 1f; //max tangent air speed, the lower this value the slower the airstrafe

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
    }

    int lastJumpSoundIndex = -1;
    public void Jump(CollisionEventArgs args)
    {
        SurfaceConfig surfaceConfig = GetSurfacePropertyFromCollision(args.HitInfo);
        AudioClip sound;
        (sound, lastJumpSoundIndex) = surfaceConfig.NextRandomSound(lastJumpSoundIndex);
        if (sound != null) AudioController.PlayOneShot(sound);


        if (args.HitInfo.collider != null
            && args.HitInfo.collider.TryGetComponent<ISpecialPlayerCollisionBehavior>(out var behavior)
            && behavior.TryOverrideCollisionBehavior(this, args, surfaceConfig))
        {
            // override default collision behavior
            return;
        }
        else
        {
            // perform default collision behavior

            // jump away from the surface
            Accelerate(args.HitInfo.normal, 2 * surfaceConfig.SurfaceRepelForceMultiplier);
            if (surfaceConfig.JumpForceMultiplier > 0)
            {
                // jump up based on the player's rotation
                Accelerate(DesiredModelRotation * Vector3.up, JumpForce * surfaceConfig.JumpForceMultiplier);
            }
        }
    }

    public void RotateAndMove()
    {
        if (Time.deltaTime == 0) return;
        CollisionGroup.RotateTo(DesiredModelRotation);
        CollisionGroup.Move(Velocity * Time.deltaTime);
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
        if (GameManager.GameInstance.Paused) return;

        //change eyeAngles based on mouse
        internalEyeAngles += InputManager.CheckAxisSet(AxisSetName.Aim).Scale(SENSITIVITY * SENS_PITCH_SCALE, SENSITIVITY, SENSITIVITY);
        //clamp pitch
        internalEyeAngles.x = Mathf.Clamp(internalEyeAngles.x, -89.9f, 89.9f);

        CameraSwivelPoint.transform.rotation = GetAimQuat();
    }
    #endregion
}
