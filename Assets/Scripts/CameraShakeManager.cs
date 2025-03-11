using Assets.Scripts;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager instance;

    [SerializeField]
    private float globalShakeForce = 1f;

    [SerializeField]
    private CinemachineImpulseListener _impulseListener;

    private CinemachineImpulseDefinition _impulseDefinition;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void Start()
    {
        _impulseListener = GetComponent<CinemachineImpulseListener>();
    }

    public void CameraShake(CinemachineImpulseSource impulseSource)
    {
        impulseSource.GenerateImpulse(globalShakeForce);
    }

    public void ScreenShakeFromProfile(
        ScreenShakeProfile profile,
        CinemachineImpulseSource impulseSource
    )
    {
        SetupScreenShakeSettings(profile, impulseSource);
        impulseSource.GenerateImpulseWithForce(profile.impactForce);
    }

    private void SetupScreenShakeSettings(
        ScreenShakeProfile profile,
        CinemachineImpulseSource impulseSource
    )
    {
        //globalShakeForce = profile.impactForce;
        _impulseDefinition = impulseSource.ImpulseDefinition;

        _impulseDefinition.ImpulseDuration = profile.impactTime;
        impulseSource.DefaultVelocity = profile.defaultVelocity;
        _impulseDefinition.CustomImpulseShape = profile.impulseCurve;

        _impulseListener.ReactionSettings.AmplitudeGain = profile.listenerAmplitude;
        _impulseListener.ReactionSettings.FrequencyGain = profile.listenerFrequency;
        _impulseListener.ReactionSettings.Duration = profile.listenerDuration;
    }
}
