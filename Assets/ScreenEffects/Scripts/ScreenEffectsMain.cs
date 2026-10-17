using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScreenEffectsMain : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private Image whiteImage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        FrameFreeze(0.1f);
        ScreenFlash(0.2f);
        ScreenShake(new Vector3(0f, -3f, 0f), 0.5f, CinemachineImpulseDefinition.ImpulseShapes.Explosion);
    }

    public void ScreenShake(Vector3? velocity = null, float duration = 0.1f, CinemachineImpulseDefinition.ImpulseShapes shape = CinemachineImpulseDefinition.ImpulseShapes.Bump)
    {
        //impulseSource.ImpulseDefinition.ImpulseShape = shape;
        //impulseSource.ImpulseDefinition.ImpulseDuration = duration;

        //Vector3 finalVelocity = velocity ?? new Vector3(0f, -1f, 0f);

        //impulseSource.GenerateImpulse(finalVelocity);

        Camera.main.DOShakePosition(0.5f, 3, 10);
    }

    public void FrameFreeze(float duration = 0.1f)
    {
        StartCoroutine(HitStop(duration));
    }

    public void ScreenFlash(float duration = 0.1f)
    {
        whiteImage.gameObject.SetActive(true);
        Color color = whiteImage.color;
        color.a = 0.7f; 
        whiteImage.color = color;

        whiteImage.DOFade(0f, duration)
            .SetUpdate(true)
            .OnComplete(() => {
                whiteImage.gameObject.SetActive(false);
            });
    }

    IEnumerator HitStop(float duration) 
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = originalTimeScale;
    }   
}