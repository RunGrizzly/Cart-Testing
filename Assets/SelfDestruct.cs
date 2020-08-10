using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{

    public float destroyTime;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyGO());
    }
    public IEnumerator DestroyGO()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}