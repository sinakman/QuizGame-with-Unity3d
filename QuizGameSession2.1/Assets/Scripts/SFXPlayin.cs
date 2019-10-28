using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayin : MonoBehaviour
{
    public AudioSource clicksound;
    public AudioSource incorrect;
    public AudioSource correct;

    public void PlayClickSound()
    {
        clicksound.Play();
    }

    public void EndRoundSound()
    {
        incorrect.Play();
    }

    public void TrueRoundSound()
    {
        correct.Play();
    }
}
