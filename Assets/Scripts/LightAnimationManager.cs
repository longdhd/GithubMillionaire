using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAnimationManager : MonoBehaviour
{
    [SerializeField] List<GameObject> _lightList;
    [SerializeField] Material whiteLightmat;
    [SerializeField] Material blueLightmat;
    [SerializeField] Light dirLight;
    public static LightAnimationManager Instance;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public void RotateUpLightOn()
    {
        dirLight.intensity = 0.4f;
        foreach (var light in _lightList)
        {
            Vector3 newDirection = Vector3.RotateTowards(light.transform.forward, transform.up, Time.fixedDeltaTime, 0.0f);
            light.transform.rotation = Quaternion.LookRotation(newDirection);

            float angleDiff = Vector3.Angle(light.transform.forward, transform.up);
            if (angleDiff <= 2f)
                light.SetActive(true);
        }
    }

    public void RotateDownLightOff()
    {
        dirLight.intensity = 0.8f;
        foreach (var light in _lightList)
        {
            Vector3 newDirection = Vector3.RotateTowards(light.transform.forward, transform.forward, Time.fixedDeltaTime * 0.125f, 0.0f);
            light.transform.rotation = Quaternion.LookRotation(newDirection);
            
            float angleDiff = Vector3.Angle(light.transform.forward, transform.forward);
            if (angleDiff <= 2f)
                light.SetActive(false);
        }
         
    }

    public void ChangeLightColor(bool setBlue)
    {
        foreach (var light in _lightList)
        {
            light.GetComponent<MeshRenderer>().material = setBlue ? blueLightmat : whiteLightmat;
        }
    }
}
