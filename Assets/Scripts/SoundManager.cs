using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource sword_slice_meat_01;
    public AudioSource sword_slice_meat_02;
    public AudioSource sword_slice_metal_01;
    public AudioSource slash_01;
    public AudioSource slash_02;
    public AudioSource electric_effect_01;

    public void PlaySound(AudioSource audio)
    {
        audio.PlayOneShot(audio.clip, 0.2f);
    }

    public void PlaySlash()
    {
        Debug.Log("Playing Slash Sound");
        slash_01.PlayOneShot(slash_01.clip, 0.2f);
    }

    public void PlaySlash2()
    {
        Debug.Log("Playing Slash Sound 2");
        slash_02.PlayOneShot(slash_02.clip, 0.2f);
    }

}
