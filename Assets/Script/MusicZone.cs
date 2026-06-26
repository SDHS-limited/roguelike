using UnityEngine;

public class MusicZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private string targetTag = "Player";

    private AudioSource audioSource;
    private bool playerInRange = false;
    private float targetVolume = 0f;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;
        audioSource.Play();
    }

    private bool hasBeenTriggered = false;

    void Update()
    {
        if (!hasBeenTriggered)
        {
            CheckPlayerInRange();
        }
        HandleVolumeFade();
    }

    private void CheckPlayerInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(targetTag))
            {
                hasBeenTriggered = true;
                targetVolume = 1f;
                break;
            }
        }
    }

    private void HandleVolumeFade()
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
            
            if (hasBeenTriggered && !audioSource.isPlaying)
            {
                audioSource.UnPause();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
