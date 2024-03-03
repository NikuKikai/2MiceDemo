using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.UI;
using Klak.TestTools;
using UltraFace;
using System.Linq;
using System;
using UnityEngine.Events;


public class FaceInputSource : MonoBehaviour
{
    [SerializeField] UnityEvent<Vector3> outputXYZ;
    [SerializeField] UnityEvent<Vector3> outputRotXYZ;

    [SerializeField] ImageSource imageSource = null;
    [SerializeField, Range(0, 1)] float detectThreshold = 0.5f;
    [SerializeField] ResourceSet detectResources = null;

    [SerializeField] NNModel markNNModel = null;
    [SerializeField] ComputeShader markCorpShader = null;

    [SerializeField] float zOffset = -0.45f;
    [SerializeField] float ryOffset = 0.04f;

    [SerializeField] bool visualize = false;
    [SerializeField] ComputeShader visShader = null;
    [SerializeField] RawImage visualizeDest;

    // WebCam
    FaceDetector _detector;
    FaceMarker _marker;
    RenderTexture _visTexture;
    (float x1, float y1, float x2, float y2) _lastFace = (0.3f, 0.2f, 0.7f, 0.8f);


    void Start()
    {
        _detector = new FaceDetector(detectResources);

        _marker = new FaceMarker(markNNModel, markCorpShader, visShader);

        _visTexture = new RenderTexture(640, 480, 0) {enableRandomWrite = true};
        _visTexture.Create();
    }

    void Update()
    {
        if (imageSource.AsTexture is null) return;

        // Detect face
        _detector.ProcessImage(imageSource.AsTexture, detectThreshold);
        var faces = _detector.Detections.ToArray();
        if (faces.Length == 0) return;
        var face = PostProcFaces(faces);

        // Detect landmarks
        var marks = _marker.RunModel(
            imageSource.AsTexture,
            face.x1,
            face.x2,
            face.y1,
            face.y2,
            visualize
        );

        // Visualize
        if (visualize) {
            Graphics.Blit(_marker.visTexture, _visTexture);
            visualizeDest.texture = _visTexture;
        }

        // Calc pose
        var pose = Marks2Pose(
            marks,
            imageSource.AsTexture.width,
            imageSource.AsTexture.height
        );
        // Debug.Log(pose);

        // Invoke events
        outputXYZ.Invoke(pose.pos);
        outputRotXYZ.Invoke(pose.rot);
    }

    void OnDestroy()
    {
        _detector?.Dispose();
    }

    (float x1, float y1, float x2, float y2) PostProcFaces(Detection[] faces) {
        // Best
        Detection face = faces[0];
        float score = face.score;
        foreach (var f in faces) {
            if (f.score > score) {
                score = f.score;
                face = f;
            }
        }

        // Smooth
        float k = 0.9f;
        _lastFace = (
            face.x1 * (1-k) + _lastFace.x1 * k,
            face.y1 * (1-k) + _lastFace.y1 * k,
            face.x2 * (1-k) + _lastFace.x2 * k,
            face.y2 * (1-k) + _lastFace.y2 * k
        );
        return _lastFace;
    }

    (Vector3 pos, Vector3 rot) Marks2Pose(Vector2[] marks, float imageW, float imageH) {
        // point definition https://github.com/google/mediapipe/blob/a908d668c730da128dfa8d9f6bd25d519d006692/mediapipe/modules/face_geometry/data/canonical_face_model_uv_visualization.png

        const int inose = 4;
        const int itop = 10;
        const int ibottom = 152;
        const int ileft = 93;
        const int iright = 323;

        var nose = marks[inose];
        var top = marks[itop];
        var bottom = marks[ibottom];
        var left = marks[ileft];
        var right = marks[iright];

        var size = new Vector2(Mathf.Abs(left.x - right.x), Mathf.Abs(top.y - bottom.y));
        var center = new Vector2((left.x + right.x) / 2, (top.y + bottom.y) / 2);

        var rx = - (nose.x * 2 - top.x - bottom.x) / size.x;
        var ry = - (nose.y * 2 - left.y - right.y) / size.y + ryOffset;
        var rz = (Mathf.Atan2(top.x-bottom.x, top.y-bottom.y) + Mathf.PI*5/2) % Mathf.PI - Mathf.PI/2;

        var px = - (center.x / imageW * 2 - 1);
        var py = - (center.y / imageH * 2 - 1);
        var pz = (size.y / imageH + zOffset) * 2f;
        return (
            new Vector3(px, py, pz),
            new Vector3(rx, ry, rz)
        );
    }
}
