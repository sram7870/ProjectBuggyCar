using UnityEngine;

public class BounceBoost : MonoBehaviour
{
    public float boostForce = 5000f; // Adjust as needed

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bounce"))
        {
            // Boost in the car's local Z direction (backward)
            Vector3 boostDirection = transform.forward * -1;
            rb.AddForce(boostDirection * boostForce, ForceMode.Impulse);
            transform.rotation = Quaternion.identity;
        }
    }
}