using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAnimationManager : MonoBehaviour
{
    [SerializeField] List<GameObject> _lightList;
    [SerializeField] Material whiteLightmat;
    [SerializeField] Material blueLightmat;
    [SerializeField] Light dirLight;
    [SerializeField] float rotSpeed = 20f;
    public static LightAnimationManager Instance;

    void Awake()
    {
        Instance = this;
    }
       
    public void RotateUpLightOn()
    {
        dirLight.intensity = Lerp(dirLight.intensity, 0.25f, Time.deltaTime);

        foreach (var light in _lightList)
        {
            light.transform.rotation = Quaternion.Euler(-90, light.transform.localEulerAngles.y, light.transform.localEulerAngles.z);
        }
    }

    public void RotateDownLightOff()
    {
        dirLight.intensity = Lerp(dirLight.intensity, 0.75f, Time.deltaTime);

        foreach (var light in _lightList)
        {
            light.transform.Rotate(Vector3.right * Time.deltaTime * rotSpeed);
        }
         
    }

    public void ChangeLightColor(bool setBlue)
    {
        foreach (var light in _lightList)
        {
            light.transform.GetChild(0).GetComponent<MeshRenderer>().material = setBlue ? blueLightmat : whiteLightmat;
        }
    }

    float Lerp(float a, float b, float t)
    {
        return (1 -t) * a + t * b;
    }
}
