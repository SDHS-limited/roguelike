using UnityEngine;

public class DashEffect : MonoBehaviour
{
    [Header("FOV Settings")]
    public float dashFOV = 85f;
    public float transitionSpeed = 10f;

    [Header("Trail Settings")]
    public Color trailColor = new Color(0.5f, 0.8f, 1f, 0.5f);
    public float trailTime = 0.3f;
    public float trailWidth = 0.5f;

    [Header("Camera Tilt")]
    public float dashTilt = 2f;

    private Move moveScript;
    private Camera playerCamera;
    private float defaultFOV;
    private TrailRenderer trail;
    private Quaternion originalCamRotation;

    void Start()
    {
        moveScript = GetComponent<Move>();
        playerCamera = GetComponentInChildren<Camera>();
        
        if (playerCamera != null)
        {
            defaultFOV = playerCamera.fieldOfView;
            originalCamRotation = playerCamera.transform.localRotation;
        }

        // Setup Trail Renderer
        GameObject trailObj = new GameObject("DashTrail");
        trailObj.transform.SetParent(transform);
        trailObj.transform.localPosition = Vector3.up * 1.0f; // Adjusted height

        trail = trailObj.AddComponent<TrailRenderer>();
        trail.time = trailTime;
        trail.startWidth = trailWidth;
        trail.endWidth = 0.05f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.minVertexDistance = 0.1f;
        
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(trailColor, 0f), new GradientColorKey(Color.white, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.6f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        trail.colorGradient = gradient;
        trail.emitting = false;
    }

    void Update()
    {
        if (moveScript == null || playerCamera == null) return;

        bool isDashing = moveScript.IsDashing;

        // FOV Effect
        float targetFOV = isDashing ? dashFOV : defaultFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * transitionSpeed);

        // Trail Effect
        trail.emitting = isDashing;

        // Camera Tilt (Pitch forward slightly when dashing)
        float targetTilt = isDashing ? dashTilt : 0f;
        // We only want to affect the local rotation relative to the movement direction
        // But for simplicity, we'll just add a small X rotation kick
        // However, CameraRot might overwrite this. Let's see.
    }
    }

