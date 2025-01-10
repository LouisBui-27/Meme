using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundButton : MonoBehaviour
{
    public AudioClip audioClip;
    public ImageEffect targetEffect;

    //public bool applyEffect = false;

    public void PlaySounnd()
    {
        GameManage.instance.PlaySound(audioClip);
        if (targetEffect != null)
        {
            targetEffect.TriggerEffect();
        }
    }
}
