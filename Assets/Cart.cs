using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class Cart : MonoBehaviour
{

    public Drivable drivable;
    public AudioSource audioSource;
    public AudioClip revNoise;

    public float carVolume;

    // Start is called before the first frame update
    public virtual void Start()
    {
        StartCoroutine(CarSounds());
    }

    // Update is called once per frame
    public virtual void Update()
    {

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

            audioSource.pitch = rev / 800;
            audioSource.volume = rev / 800 * carVolume;

            yield return new WaitForSeconds(0.05f);
        }
    }
}