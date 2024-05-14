using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Horse"))
        {
            GameManager.Instance.RemoveCheckPoint(this);
        }
    }
}
