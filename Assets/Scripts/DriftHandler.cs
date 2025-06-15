using UnityEngine;

public class DriftHandler : MonoBehaviour
{
    [System.Serializable]
    public class WheelEffects
    {
        public WheelCollider wheelCollider;
        public TrailRenderer skidTrail;
        public ParticleSystem smoke;
        public AudioSource skidAudio;

        public bool isDrifting = false;

        public void StartDrift()
        {
            if (isDrifting) return;
            isDrifting = true;

            if (skidTrail != null)
                skidTrail.emitting = true;

            if (smoke != null && !smoke.isPlaying)
                smoke.Play();

            if (skidAudio != null && !skidAudio.isPlaying)
                skidAudio.Play();
        }

        public void StopDrift()
        {
            if (!isDrifting) return;
            isDrifting = false;

            if (skidTrail != null)
                skidTrail.emitting = false;

            if (smoke != null && smoke.isPlaying)
                smoke.Stop();

            if (skidAudio != null && skidAudio.isPlaying)
                skidAudio.Stop();
        }
    }

    public WheelEffects[] wheels;

    [Header("Drift Settings")]
    public float driftSlipThreshold = 0.4f; // Higher = more forgiving
    public bool useHandbrakeDrift = true;

    private void Update()
    {
        foreach (var wheel in wheels)
        {
            WheelHit hit;
            if (wheel.wheelCollider.GetGroundHit(out hit))
            {
                bool isSliding = Mathf.Abs(hit.sidewaysSlip) > driftSlipThreshold;
                bool isHandbrake = useHandbrakeDrift && Input.GetKey(KeyCode.Space);

                if (isSliding || isHandbrake)
                    wheel.StartDrift();
                else
                    wheel.StopDrift();
            }
            else
            {
                wheel.StopDrift();
            }
        }
    }
}
