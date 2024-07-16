using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 rotationAxis = Vector3.up; // Arah sumbu putaran
    public float rotationSpeed = 20.0f; // Kecepatan putaran dalam derajat per detik

    // Update is called once per frame
    void Update()
    {
        // Menggunakan Time.deltaTime untuk membuat rotasi menjadi smooth terlepas dari frame rate
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }

}
