using Collision;
using Inputter;
using Pogo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{

    void Start()
    {
        PogoGameManager.RegisterPlayer(this);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        internalEyeAngles = new Vector3(0, transform.localRotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        DoLook();
        UpdateModelPitch();
        RotateModel();
        ApplyForces();
        Move();
    }

    #region Game Logic
    public UnityEvent OnDie;

    public void DieFrom(CollisionEventArgs collision)
    {
        WizardEffects.EffectManager.CreateEffect("DeathMark", new WizardEffects.EffectData()
        {
            position = collision.HitInfo.point,
            normal = collision.HitInfo.normal
        });
        Die();
    }
    public void Die()
    {
        transform.position = PogoGameManager.PogoInstance.RespawnPoint.position;
        internalEyeAngles = new Vector3(0, PogoGameManager.PogoInstance.RespawnPoint.rotation.eulerAngles.y, 0);
        Velocity = Vector3.zero;
        PitchFrac = 0;
        OnDie.Invoke();
        PogoGameManager.PogoInstance?.OnPlayerDeath.Invoke();
    }
    #endregion

    #region Model maneuvering
    public Transform Model;
    public Transform Camera;
    public float PitchFrac = 0;
    const float ModelPitchMul = 1.5f;
    const float PitchFracSpeed = 10;


    public Quaternion ModelRotation => Quaternion.Euler(PitchFrac * ModelPitchMul * EyeAngles.x, EyeAngles.y, EyeAngles.z);
    public Quaternion CameraRotation => Quaternion.Euler(PitchFrac * EyeAngles.x, EyeAngles.y, EyeAngles.z);

    void UpdateModelPitch()
    {
        float targetPitchFrac = InputManager.CheckKey(KeyName.Jump) ? 0 : 1;
        PitchFrac = Mathf.MoveTowards(PitchFrac, targetPitchFrac, PitchFracSpeed * Time.deltaTime);
    }

    void RotateModel()
    {
        Model.rotation = ModelRotation;
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
    }

    public void Jump(CollisionEventArgs args)
    {
        Accelerate(args.HitInfo.normal, 2);
        Accelerate(ModelRotation * Vector3.up, JumpForce);
        Decelerate(ModelRotation * Vector3.up, JumpMaxSideSpeed, 1);
    }

    public void Move()
    {
        transform.position += Velocity * Time.deltaTime;
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
        //change eyeAngles based on mouse
        internalEyeAngles += InputManager.CheckAxisSet(AxisSetName.Aim).Scale(SENSITIVITY * SENS_PITCH_SCALE, SENSITIVITY, SENSITIVITY);
        //clamp pitch
        internalEyeAngles.x = Mathf.Clamp(internalEyeAngles.x, -89.9f, 89.9f);

        CameraSwivelPoint.transform.rotation = GetAimQuat();
    }
    #endregion
}
