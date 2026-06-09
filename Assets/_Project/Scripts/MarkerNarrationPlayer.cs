using UnityEngine;
using Vuforia;

[RequireComponent(typeof(AudioSource))]
public class MarkerNarrationPlayer : MonoBehaviour
{
    public ObserverBehaviour observerBehaviour;
    public AudioClip narrationClip;

    [Header("Settings")]
    public bool playOnlyOnce = true;
    public bool stopWhenMarkerLost = false;

    private AudioSource audioSource;
    private bool hasPlayed = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (observerBehaviour == null)
        {
            observerBehaviour = GetComponent<ObserverBehaviour>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void OnEnable()
    {
        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    private void OnDisable()
    {
        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        bool markerDetected =
            status.Status == Status.TRACKED ||
            status.Status == Status.EXTENDED_TRACKED;

        if (markerDetected)
        {
            PlayNarration();
        }
        else
        {
            if (stopWhenMarkerLost && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private void PlayNarration()
    {
        if (narrationClip == null) return;

        if (playOnlyOnce && hasPlayed) return;

        audioSource.clip = narrationClip;
        audioSource.Play();

        hasPlayed = true;
    }

    public void ResetNarration()
    {
        hasPlayed = false;
    }
}