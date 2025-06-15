using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Vector3 teleportTargetPosition; // Set this in the Inspector or via script
    public string wallTag;
    public float boostForce = 5000f; // Adjust as needed
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object collided with has the correct tag
        if (collision.gameObject.CompareTag(wallTag))
        {
            if (wallTag == "Portal A")
            {
                // Teleport this object to the target position
                transform.position = teleportTargetPosition;
                transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y + 180, transform.rotation.z);
                Vector3 boostDirection = -1 * transform.forward;
                rb.AddForce(boostDirection * boostForce, ForceMode.Impulse);
            }

            if (wallTag == "Portal B")
            {
                // 1. Move to target position
                transform.position = teleportTargetPosition;

                // 2. Reset rotation to face forward (world Z+ direction)
                // This must happen BEFORE calculating boostDirection
                transform.rotation = Quaternion.Euler(0, 0, 0);

                // 3. Calculate boost direction based on the new rotation
                Vector3 boostDirection = transform.forward; // Or use Vector3.back if always global

                // 4. Clear any previous velocity
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero; // Also clear angular momentum

                // 5. Freeze rotation (prevents future physics-based rotation)
                rb.constraints = RigidbodyConstraints.FreezeRotation;

                // 6. Apply boost forces
                rb.AddForce(boostDirection * boostForce / 13f, ForceMode.Impulse);
                rb.AddForce(Vector3.up * boostForce / 17f, ForceMode.Impulse);
            }
        }
    }
}