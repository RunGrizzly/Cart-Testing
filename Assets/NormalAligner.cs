using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAligner : MonoBehaviour
{

    public LayerMask layerMask;
    GameObject normalMarker;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 65, layerMask))
        {
            Destroy(normalMarker);

            Debug.Log("We hit the floot");
            normalMarker = GameObject.CreatePrimitive(PrimitiveType.Plane);
            normalMarker.transform.position = hit.point + hit.normal * 2;

            normalMarker.GetComponent<Renderer>().material.color = Color.green;

            //Find the angle about y.
            //Current transform.y (euler angles.)

            GameObject orientation = new GameObject();
            orientation.transform.up = hit.normal;
            //Find the angle about y.
            //Current transform.y (euler angles.)
            orientation.transform.rotation *= Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));

            normalMarker.transform.rotation = orientation.transform.rotation;

            Debug.Log(hit.normal);
        }

    }

    private void OnDrawGizmos()
    {

    }
}