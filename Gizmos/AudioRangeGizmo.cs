using UnityEngine;

[ExecuteInEditMode]
public class AudioRangeGizmo : MonoBehaviour
{
    // Customizable fields
    public Color audioRangeColor = Color.magenta;
    public bool showGizmo = true;
    public float listenerRadius = 10f;

    private AudioSource audioSource;
    private AudioListener audioListener;

    void OnDrawGizmos()
    {
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (!audioListener)
        {
            audioListener = GetComponent<AudioListener>();
        }

        if (showGizmo)
        {
            Gizmos.color = audioRangeColor;

            if (audioSource != null)
            {
                // For audio sources, draw a wire sphere representing the max distance
                Gizmos.DrawWireSphere(transform.position, audioSource.maxDistance);
            }
            else if (audioListener != null)
            {
                // For audio listeners, draw a sphere representing the listener's radius
                Gizmos.DrawWireSphere(transform.position, listenerRadius);
            }
        }
    }
}