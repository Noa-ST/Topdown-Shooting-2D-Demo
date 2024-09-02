using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CineController : Singleton<CineController>
{
    [SerializeField] private float m_shakeDuration = 0.3f;
    [SerializeField] private float m_shakeAmplitude = 1.2f;
    [SerializeField] private float m_shakeFrequency = 2.0f;

    private float m_shakeElapsedTime = 0f;

    [SerializeField] private CinemachineVirtualCamera m_virtualCamera;
    private CinemachineBasicMultiChannelPerlin m_virtualCameraNoise;

    protected override void Awake()
    {
        MakeSingleton(false);
    }

    private void Start()
    {
        if (m_virtualCamera != null)
            m_virtualCameraNoise = m_virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    void Update()
    {
        ShakeListener();
    }

    void ShakeListener()
    {
        if (m_virtualCamera == null || m_virtualCameraNoise == null) return;

        if (m_shakeElapsedTime > 0)
        {
            m_virtualCameraNoise.m_AmplitudeGain = m_shakeAmplitude;
            m_virtualCameraNoise.m_FrequencyGain = m_shakeFrequency;

            m_shakeElapsedTime -= Time.deltaTime;
            return;
        }

        m_virtualCameraNoise.m_AmplitudeGain = 0f;
        m_shakeElapsedTime = 0f;
    }

    public void ShakeTrigger()
    {
        m_shakeElapsedTime = m_shakeDuration;
    }
}
