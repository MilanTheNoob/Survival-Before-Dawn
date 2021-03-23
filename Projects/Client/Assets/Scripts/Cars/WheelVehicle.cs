using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WheelVehicle : InteractableItem
{
    #region Singleton

    public static WheelVehicle instance;
    void Awake() { instance = this; }

    #endregion

    public WheelCollider[] driveWheel;
    public WheelCollider[] turnWheel;

    public AnimationCurve motorTorque = new AnimationCurve(new Keyframe(0, 200), new Keyframe(50, 300), new Keyframe(200, 0));

    [Range(2, 16)]
    public float diffGearing = 4.0f;

    public float brakeForce = 1500.0f;

    [Range(0f, 50.0f)]
    public float steerAngle = 30.0f;
    [Range(0.001f, 1.0f)]
    public float steerSpeed = 0.2f;

    [Range(0.0f, 2f)]
    public float driftIntensity = 1f;

    public Transform centerOfMass;

    [Range(0.5f, 10f)]
    public float downforce = 1.0f;
    public float Downforce { get { return downforce; } set { downforce = Mathf.Clamp(value, 0, 5); } }

    [HideInInspector]
    public float steering;
    [HideInInspector]
    public float throttle;

    public bool Handbrake;

    bool drift;

    float speed = 0.0f;
    public float Speed { get { return speed; } }

    public bool jumping = false;

    Rigidbody _rb;
    WheelCollider[] wheels;

    Vector2 chunk;

    void Start()
    {
        chunk = TerrainGenerator.GetNearestChunk(transform.position);

        isInteractable = true;
        interactTxt = "Drive";

        _rb = GetComponent<Rigidbody>();

        if (_rb != null && centerOfMass != null)
        {
            _rb.centerOfMass = centerOfMass.localPosition;
        }

        wheels = GetComponentsInChildren<WheelCollider>();

        foreach (WheelCollider wheel in wheels)
        {
            wheel.motorTorque = 0.0001f;
        }
    }

    public override void OnInteract() { CarManager.EnterCar(this); }

    void FixedUpdate()
    {
        if (Handbrake)
        {
            foreach (WheelCollider wheel in wheels)
            {
                wheel.motorTorque = 0.0001f;
                wheel.brakeTorque = brakeForce;
            }
        }
        else if (Mathf.Abs(speed) < 4 || Mathf.Sign(speed) == Mathf.Sign(throttle))
        {
            if (chunk != TerrainGenerator.GetNearestChunk(transform.position))
            {
                if (TerrainGenerator.AddToNearestChunk(gameObject, TerrainGenerator.ChildType.Vehicle))
                    chunk = TerrainGenerator.GetNearestChunk(transform.position);
            }

            speed = transform.InverseTransformDirection(_rb.velocity).z * 3.6f;

            foreach (WheelCollider wheel in turnWheel)
            {
                wheel.steerAngle = Mathf.Lerp(wheel.steerAngle, steering, steerSpeed);
            }

            foreach (WheelCollider wheel in wheels)
            {
                wheel.brakeTorque = 0;
            }

            foreach (WheelCollider wheel in driveWheel)
            {
                wheel.motorTorque = throttle * motorTorque.Evaluate(speed) * diffGearing / driveWheel.Length;
            }
        }
        else
        {
            foreach (WheelCollider wheel in wheels)
            {
                wheel.brakeTorque = Mathf.Abs(throttle) * brakeForce;
            }
        }

        if (drift)
        {
            Vector3 driftForce = -transform.right;
            driftForce.y = 0.0f;
            driftForce.Normalize();

            if (steering != 0)
                driftForce *= _rb.mass * speed / 7f * throttle * steering / steerAngle;
            Vector3 driftTorque = transform.up * 0.1f * steering / steerAngle;


            _rb.AddForce(driftForce * driftIntensity, ForceMode.Force);
            _rb.AddTorque(driftTorque * driftIntensity, ForceMode.VelocityChange);
        }

        _rb.AddForce(-transform.up * speed * downforce);
    }
}
