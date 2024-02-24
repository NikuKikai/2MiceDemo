using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ViewControlFace
{
    public TextAsset faceModel;
    public TextAsset eyeModel;
    public TextAsset landmarkModel;

    public UnityEvent<float> outputX;
    public UnityEvent<float> outputY;
    public UnityEvent<float, float> outputXY;
    public UnityEvent<float> outputRotX;
    public UnityEvent<float> outputRotY;
    public UnityEvent<float, float> outputRotXY;

    // private FaceProcessorLive<WebCamTexture> processor;
    // private DetectedFace face;
    private float rx = 0f;
    private float ry = 0f;
#if MY
    protected override void Awake()
    {
        base.Awake();
        base.forceFrontalCamera = true; // we work with frontal cams here, let's force it for macOS s MacBook doesn't state frontal cam correctly

        byte[] shapeDat = landmarkModel.bytes;
        if (shapeDat.Length == 0)
            Debug.LogError("Landmark model not found!");

        processor = new FaceProcessorLive<WebCamTexture>();
        processor.Initialize(faceModel.text, eyeModel.text, landmarkModel.bytes);

        // data stabilizer - affects face rects, face landmarks etc.
        processor.DataStabilizer.Enabled = true;        // enable stabilizer
        processor.DataStabilizer.Threshold = 2.0;       // threshold value in pixels
        processor.DataStabilizer.SamplesCount = 2;      // how many samples do we need to compute stable data

        // performance data - some tricks to make it work faster
        processor.Performance.Downscale = 256;          // processed image is pre-scaled down to N px by long side
        processor.Performance.SkipRate = 0;             // we actually process only each Nth frame (and every frame for skipRate = 0)
    }

    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        processor.ProcessTexture(input, TextureParameters);

        if (processor.Faces.Count > 0)
            this.face = processor.Faces[0];
        var w = processor.Image.Width;
        var h = processor.Image.Height;
        var x = (float)this.face.Region.Center.X / w - 0.5f;
        var y = (float)this.face.Region.Center.Y / h - 0.5f;

        if (this.face.Marks.Length == 69) {

            var leftEye = this.face.Elements[(int)DetectedFace.FaceElements.LeftEye].Marks[0]; // original 36
            var rightEye = this.face.Elements[(int)DetectedFace.FaceElements.RightEye].Marks[3]; // 45
            var midEye = (leftEye + rightEye) / 2;
            var nose = this.face.Elements[(int)DetectedFace.FaceElements.Nose].Marks[3]; // 33
            var lip = this.face.Elements[(int)DetectedFace.FaceElements.OuterLip].Marks[9]; // 57

            rx = (float)(nose.X - leftEye.X) / (rightEye.X - leftEye.X) - 0.5f;
            ry = (float)(nose.Y - midEye.Y) / (lip.Y - midEye.Y) - 0.6f;
        }

        x *= 10; y*= -10;
        rx *= 10; ry *= 10;
        this.outputX?.Invoke(x);
        this.outputY?.Invoke(y);
        this.outputXY?.Invoke(x, y);
        this.outputRotX?.Invoke(rx);
        this.outputRotY?.Invoke(ry);
        this.outputRotXY?.Invoke(rx, ry);

        return true;
    }
#endif
}
