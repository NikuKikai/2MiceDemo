using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Target") ||
            collision.gameObject.CompareTag("Player")) {
            print("hit " + collision.gameObject.name);
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Player")) {
            GameObject.Find("Damage").GetComponent<Damage>().Trigger();
        }
    }
}
