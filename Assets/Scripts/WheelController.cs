using UnityEngine;

public class WheelController : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // ░░░░░░░░░░ WHEEL SETTINGS ░░░░░░░░░░
    // ─────────────────────────────────────────────
    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] WheelCollider backLeft;

    // ─────────────────────────────────────────────
    // ░░░░░░░░░░ DRIVING SETTINGS ░░░░░░░░░░
    // ─────────────────────────────────────────────
    public float acceleration = 1f;
    public float brakingForce = 10f;
    public float downForce = 30f;
    public Vector3 respawnPosition = new Vector3(-70, 0, 122.25f);

    // ─────────────────────────────────────────────
    // ░░░░░░░░░░ INTERNAL STATE ░░░░░░░░░░
    // ─────────────────────────────────────────────
    private float currentAcceleration = 0f;
    private float currentBrakeForce = 0f;
    private float currentTurnAngle = 0f;
    private float maxTurnAngle = 30f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Press R to reset/respawn
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCar();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            UprightCar();
        }
    }

    private void FixedUpdate()
    {
        ApplyDownforce();
        AdjustFrictionBasedOnSpeed();
        HandleInput();
        ApplyMotorTorque();
        ApplyBrakeTorque();
        ApplySteering();
    }

    // ─────────────────────────────────────────────
    // ░░░░░░░░░░ INPUT HANDLING ░░░░░░░░░░
    // ─────────────────────────────────────────────

    private void HandleInput()
    {
        currentAcceleration = Mathf.Clamp(acceleration * Input.GetAxis("Vertical"), -10f, 10f);
        currentBrakeForce = Input.GetKey(KeyCode.Space) ? brakingForce : 0f;
        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
    }

    // ─────────────────────────────────────────────
    // ░░░░░░░░░░ PHYSICS ░░░░░░░░░░
    // ─────────────────────────────────────────────

    private void ApplyDownforce()
    {
        rb.AddForce(Vector3.down * downForce, ForceMode.Force);
    }

    private void ApplyMotorTorque()
    {
        float torque = currentAcceleration * 1000f;
        frontRight.motorTorque = torque;
        frontLeft.motorTorque = torque;
        backRight.motorTorque = torque;
        backLeft.motorTorque = torque;
    }

    private void ApplyBrakeTorque()
    {
        frontRight.brakeTorque = currentBrakeForce;
        frontLeft.brakeTorque = currentBrakeForce;
        backRight.brakeTorque = currentBrakeForce;
        backLeft.brakeTorque = currentBrakeForce;
    }

    private void ApplySteering()
    {
        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;
    }

    private void ResetCar()
    {
        // Disable physics temporarily
        rb.isKinematic = true;

        // Reset position and rotation
        transform.position = respawnPosition;
        transform.rotation = Quaternion.Euler(0, -90f, 0);

        // Clear motion
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Clear all torque/brakes
        SetAllWheelsTorque(0f);
        SetAllWheelsBrake(0f);
        SetAllWheelsSteering(0f);

        // Reset timer (if exists)
        var timer = GameObject.Find("TimeManager")?.GetComponent<TimerUI>();
        if (timer != null) timer.ResetTimer();

        // Re-enable physics and stop motion artifacts
        rb.isKinematic = false;
        rb.Sleep(); // Puts rigidbody to rest — no jitters or sliding
    }

    private void UprightCar()
    {
        // Disable physics temporarily
        rb.isKinematic = true;

        // Get the current Euler angles.
        Vector3 eulerAngles = transform.eulerAngles;

        // Set the z-axis rotation to 0.
        eulerAngles.z = 0f;

        // Apply the modified Euler angles back to the transform.
        transform.rotation = Quaternion.Euler(eulerAngles);

        // Clear motion
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Clear all torque/brakes
        SetAllWheelsTorque(0f);
        SetAllWheelsBrake(0f);
        SetAllWheelsSteering(0f);

        // Re-enable physics and stop motion artifacts
        rb.isKinematic = false;
        rb.Sleep(); // Puts rigidbody to rest — no jitters or sliding
    }

    // ─────────────────────────────────────────────
    // ░░░░░░░░░░ HELPERS ░░░░░░░░░░
    // ─────────────────────────────────────────────

    private void SetAllWheelsTorque(float torque)
    {
        frontRight.motorTorque = torque;
        frontLeft.motorTorque = torque;
        backRight.motorTorque = torque;
        backLeft.motorTorque = torque;
    }

    private void SetAllWheelsBrake(float brake)
    {
        frontRight.brakeTorque = brake;
        frontLeft.brakeTorque = brake;
        backRight.brakeTorque = brake;
        backLeft.brakeTorque = brake;
    }

    private void SetAllWheelsSteering(float angle)
    {
        frontLeft.steerAngle = angle;
        frontRight.steerAngle = angle;
    }

    private void AdjustFrictionBasedOnSpeed()
    {
        float speed = rb.linearVelocity.magnitude;

        // Adjust sideways friction (grip) depending on speed
        float sidewaysStiffness = Mathf.Lerp(1.5f, 3.5f, speed / 50f);  // More grip at high speed
        float forwardStiffness = Mathf.Lerp(1.0f, 2.5f, speed / 50f);   // Prevents sliding when accelerating

        ApplyFrictionCurve(frontLeft, forwardStiffness, sidewaysStiffness);
        ApplyFrictionCurve(frontRight, forwardStiffness, sidewaysStiffness);
        ApplyFrictionCurve(backLeft, forwardStiffness, sidewaysStiffness);
        ApplyFrictionCurve(backRight, forwardStiffness, sidewaysStiffness);
    }

    private void ApplyFrictionCurve(WheelCollider wheel, float forwardStiffness, float sidewaysStiffness)
    {
        WheelFrictionCurve forwardFriction = wheel.forwardFriction;
        forwardFriction.stiffness = forwardStiffness;
        wheel.forwardFriction = forwardFriction;

        WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
        sidewaysFriction.stiffness = sidewaysStiffness;
        wheel.sidewaysFriction = sidewaysFriction;
    }

}
