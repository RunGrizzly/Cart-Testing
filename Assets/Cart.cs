using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class Cart : MonoBehaviour
{

    public float maxEnergy;
    public float currEnergy;

    public float energyRegen;

    public Drivable drivable;
    public AudioSource audioSource;
    public AudioClip revNoise;

    public float carVolume;
    public float carPitch;

    // Start is called before the first frame update
    public virtual void Start()
    {
        StartCoroutine(CarSounds());
        currEnergy = maxEnergy;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (currEnergy < maxEnergy)
        {
            currEnergy += energyRegen;
        }
    }

    public virtual void SetRegen(float v)
    {
        energyRegen = v;
    }

    public virtual IEnumerator CarSounds()
    {

        while (this.enabled)
        {

            float rev = Mathf.Abs(drivable.currThrust - drivable.currBrake);

            if (rev > 0)
            {

                audioSource.PlayOneShot(revNoise);
            }

            audioSource.pitch = rev / 1000 * carPitch;
            audioSource.volume = rev / 1000 * carVolume;

            yield return new WaitForSeconds(0.05f);
        }
    }
}