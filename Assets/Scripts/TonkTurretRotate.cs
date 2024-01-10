using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TonkTurretRotate : MonoBehaviour
{
    [SerializeField] float rotSpeed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RotateTurret();
    }

    private void RotateTurret()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            Vector3 newDir = Vector3.Lerp(transform.forward, -transform.right, rotSpeed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(newDir, transform.up);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            Vector3 newDir = Vector3.Lerp(transform.forward, transform.right, rotSpeed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(newDir, transform.up);
        }
    }
}
