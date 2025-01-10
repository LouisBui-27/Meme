using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEffect : MonoBehaviour
{
    public EffectType EffectType;
    public LeanTweenType tweenType;
    public float rotationSpeed; 
    public float duration;      
    private bool isRunning = false;
    public void TriggerEffect()
    {
        switch (EffectType)
        {
            case EffectType.ScaleBounce:
                ScaleBoundEffect();
                break;
            case EffectType.RotateY:
                RotateYBoundEffect();
                break;
            case EffectType.RotateZ:
                RotateZBoundEffect();
                break;
            case EffectType.MoveY:
                MoveUpDownEffect(7.5f);
                break;
            case EffectType.ShakeRotationZ:
                ShakeRotationZ(15f,-15f);
                break;
            case EffectType.None:
                break;
        }
    }
    private void ScaleBoundEffect()
    {
        if (isRunning)
        {
            return;
        }
        isRunning = true;
        LeanTween.scale(gameObject, new Vector3(1.4f, 1.4f, 1.4f), 0.3f).setEase(tweenType).setOnComplete(() =>
        {
            LeanTween.scale(gameObject, Vector3.one, 0.5f).setEase(tweenType).setOnComplete(()=>
            {
                isRunning = false;
            });
        });
    }
    private void RotateYBoundEffect()
    {
   
        LeanTween.rotateY(gameObject, gameObject.transform.eulerAngles.y + rotationSpeed * duration, duration)
            .setEase(LeanTweenType.linear) // Xoay đều liên tục
            .setOnComplete(() =>
            {
                LeanTween.rotateY(gameObject, 0f, duration).setEase(tweenType);
              //  gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                
            });

    }
    private void RotateZBoundEffect()
    {
   
        LeanTween.rotateZ(gameObject, gameObject.transform.eulerAngles.z + rotationSpeed * duration, duration)
            .setEase(LeanTweenType.linear) // Xoay đều liên tục
            .setOnComplete(() =>
            {
                LeanTween.rotateZ(gameObject, 0f, duration).setEase(tweenType);
              //  gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
              
            });
    }
    private void MoveUpDownEffect(float distance)
    {
        if (isRunning)
        {
            return;
        }
        isRunning = true;
        Vector3 startPos = gameObject.transform.position;
        LeanTween.moveY(gameObject, startPos.y + distance, duration).
        setEase(LeanTweenType.easeInOutSine).setLoopPingPong(6).setOnComplete(()=>{
            isRunning = false;
            LeanTween.moveY(gameObject, startPos.y, duration);
        });
    }
    private void ShakeRotationZ(float angle, float angle1)
    {
        if (isRunning)
        {
            return;
        }
        isRunning = true;
        LeanTween.rotateZ(gameObject, angle, duration/4f)
       .setEase(LeanTweenType.easeInOutSine)
       .setOnComplete(() =>
       {
           
           // Quay sang góc angle1
           LeanTween.rotateZ(gameObject, angle1, duration/4f)
               .setEase(LeanTweenType.easeInOutSine)
               .setOnComplete(() =>
               {
                   isRunning = false;
                   // Quay về góc ban đầu (0 độ)
                   LeanTween.rotateZ(gameObject, 0f, duration / 4f)
                       .setEase(LeanTweenType.easeInOutSine);
               });
       });
    }
}
