using UnityEngine;
using UnityEngine.Rendering;

public class Underwater : MonoBehaviour
{
    public enum DepthZone
    {
        Surface,
        Shallow,
        Deep,
        Abyss
    }

    private Transform cameraTrans;
    private int waterLine = -2;
    private int deepLine = -40;
    private int abyssLine = -100;
    private float fogChangeSpeed = 2f;
    [SerializeField] private Volume currentVolume;
    [SerializeField] private VolumeProfile underwaterProfile;
    [SerializeField] private VolumeProfile surfaceProfile;
    [SerializeField] private VolumeProfile abyssProfile;
    private DepthZone currentZone;
    private Color targetFogColor;

    void Start()
    {
        cameraTrans = Camera.main.transform;
    }

    void Update()
    {
        DepthZone newZone = GetDepthZone(cameraTrans.position.y);
        
        if(newZone != currentZone)
        {
            currentZone = newZone;
            ApplyDepthEffects(currentZone);
        }

        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, targetFogColor, Time.deltaTime*fogChangeSpeed);
    }

    DepthZone GetDepthZone(float depth)
    {
        if (depth < abyssLine) return DepthZone.Abyss;
        if (depth < deepLine) return DepthZone.Deep;
        if (depth < waterLine) return DepthZone.Shallow;
        return DepthZone.Surface;
    }

    void ApplyDepthEffects(DepthZone zone)
    {
        switch(zone)
        {
            case DepthZone.Abyss:
                RenderSettings.fog = true;
                currentVolume.profile = abyssProfile;
                targetFogColor = Color.black;
                // Apply abyss effects
                break;
            case DepthZone.Deep:
                RenderSettings.fog = true;
                targetFogColor = new Color(0.1f, 0.3f, 0.5f); 
                currentVolume.profile = underwaterProfile;
                // Apply deep water effects
                break;
            case DepthZone.Shallow:
                RenderSettings.fog = true;
                currentVolume.profile = underwaterProfile;
                targetFogColor = new Color(0.2f, 0.5f, 0.7f); 
                // Apply shallow water effects
                break;
            case DepthZone.Surface:
                RenderSettings.fog = false;
                currentVolume.profile = surfaceProfile;
                break;
        }
    }
}
