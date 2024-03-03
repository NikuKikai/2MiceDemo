using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Police : MonoBehaviour
{
    public Transform shoulder;
    public Transform player;
    public Camera playerDetectCam;
    public float moveSpeed = 1;
    public float handsUpTime = 0.5f;

    bool detected;
    bool seePlayer;
    bool aimed;

    bool handsup;

    void Start()
    {
        detected = CheckDetect();
        seePlayer = CheckSeePlayer();
        aimed = CheckAimed();
        handsup = seePlayer && detected;
        if (handsup) {
            StartCoroutine(HandleDetected());
        }
        else {
            StartCoroutine(HandleUndetected());
        }
    }

    void Update()
    {
        // Look at player
        transform.LookAt(player);

        // Move to player
        aimed = CheckAimed();
        if ( !aimed && handsup) {
            var dpos = player.position - transform.position;
            if (dpos.magnitude > 1)
                transform.position = transform.position + dpos.normalized * moveSpeed * Time.deltaTime;
        }

        // Detection
        detected = CheckDetect();
        Decide();
    }

    void FixedUpdate()
    {
        seePlayer = CheckSeePlayer();
        Decide();
    }

    void Decide()
    {
        var newHandsup = seePlayer && detected;
        if (!handsup && newHandsup) {
            StopAllCoroutines();
            StartCoroutine(HandleDetected());
        }
        else if (handsup && !newHandsup) {
            StopAllCoroutines();
            StartCoroutine(HandleUndetected());
        }
        handsup = newHandsup;
    }

    bool CheckAimed() {
        var v1 = player.GetComponentInChildren<ShoulderControl>().transform.forward;
        var v2 = transform.position-player.position;
        v1.y /= 3; v2.y /= 3;
        var angle = Vector3.Angle(v1, v2);
        return Math.Abs(angle) < 10;
    }
    bool CheckDetect() {
        var planes = GeometryUtility.CalculateFrustumPlanes(playerDetectCam);
        var objCollider =  GetComponentInChildren<Collider>();
        return GeometryUtility.TestPlanesAABB(planes, objCollider.bounds);
    }
    bool CheckSeePlayer() {
        var start = transform.Find("Head").position;
        var target = player.position;
        var hits = Physics.RaycastAll(start, target-start, 100);
        var hitList = new List<RaycastHit>();
        hitList.AddRange(hits);
        hitList.Sort((a, b) => Math.Sign(a.distance - b.distance));
        return hits.Length > 0 && hitList[0].transform.tag == "Player";
    }

    IEnumerator HandleDetected() {
        float delta = 80 / (handsUpTime/0.05f);
        while ((shoulder.localEulerAngles.x+180) % 360 -180 > -80) {
            shoulder.localEulerAngles -= new Vector3(delta, 0, 0);
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator HandleUndetected() {
        float delta = 80 / (handsUpTime/0.05f);
        while (Math.Abs(shoulder.localEulerAngles.x) > delta) {
            shoulder.localEulerAngles += new Vector3(delta, 0, 0);
            yield return new WaitForSeconds(0.05f);
        }
        shoulder.localEulerAngles = new Vector3(0, 0, 0);

        // Shoot
        while (true) {
            if (seePlayer)
                GetComponentInChildren<Gun>().Fire();
            yield return new WaitForSeconds(0.5f);
        }
    }

}
