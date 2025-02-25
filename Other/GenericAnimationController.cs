using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GenericAnimationController : MonoBehaviour
{
    [Tooltip("List of GameObjects that can be disabled.")]
    public List<GameObject> gameObjectsToControl;

    [Tooltip("List of AudioClips that can be played.")]
    public List<AudioClip> audioClips;

    public Animator[] corrospondingAnimators;
    public AnimationClip[] animations;

    [Tooltip("Audio Source to play clips from.")]
    public AudioSource audioSource;
    
    public MonoBehaviour[] scriptsToControl;
    
    public void DisableScript(int index)
    {
        if (index >= 0 && index < scriptsToControl.Length)
        {
            scriptsToControl[index].enabled = false;
        }
        else
        {
            Debug.LogWarning("Index out of range for disabling scripts.");
        }
    }
    public void EnableScript(int index)
    {
        if (index >= 0 && index < scriptsToControl.Length)
        {
            scriptsToControl[index].enabled = true;
        }
        else
        {
            Debug.LogWarning("Index out of range for enabling scripts.");
        }
    }


    public void Start()
    {
        if (GetComponent<AudioSource>())
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void DisableGameObject(int index)
    {
        if (index >= 0 && index < gameObjectsToControl.Count)
        {
            gameObjectsToControl[index].SetActive(false);
        }
        else
        {
            Debug.LogWarning("Index out of range for disabling GameObjects.");
        }
    }
    
    public void EnableGameObject(int index)
    {
        if (index >= 0 && index < gameObjectsToControl.Count)
        {
            gameObjectsToControl[index].SetActive(true);
        }
        else
        {
            Debug.LogWarning("Index out of range for Enabling GameObjects.");
        }
    }
    public void PlayAudioClip(int index)
    {
        if (audioSource != null && index >= 0 && index < audioClips.Count)
        {
            audioSource.clip = audioClips[index];
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Index out of range or audio source not set for playing audio.");
        }
    }
    
    public void PlayAnimation(int index)
    {
        if (index >= 0 && index < animations.Length)
        {
            if (corrospondingAnimators[index].name.Equals("Default-Lighter"))
            {
                if (corrospondingAnimators[index].GetBool("Lighter") == false)
                    return;
            }
            
            corrospondingAnimators[index].Play(animations[index].name);
        }
        else
        {
            Debug.LogWarning("Index out of range for playing animations.");
        }
    }
}
