using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;
using UltraFace;
using System.Linq;

public sealed class FaceMarker : System.IDisposable
{
    NNModel _nnmodel;
    ComputeShader _preproc;
    ComputeShader _vis;
    IWorker _worker;

    (ComputeBuffer preprocess, ComputeBuffer marks, RenderTexture vis) _buffers;

    int _model_input_w = 0;
    int _model_input_h = 0;
    int _model_input_pixels => _model_input_w * _model_input_h * 3;

    public RenderTexture visTexture => _buffers.vis;


    public FaceMarker(NNModel nnmodel, ComputeShader preproc, ComputeShader vis) {
        _nnmodel = nnmodel;
        _preproc = preproc;
        _vis = vis;

        // Init Model
        var model = ModelLoader.Load(nnmodel);
        _worker = model.CreateWorker();
        _model_input_w = model.inputs[0].shape[6];
        _model_input_h = model.inputs[0].shape[5];
        PrintModelDims(model);

        // Buffer
        _buffers.preprocess = new ComputeBuffer(_model_input_pixels, sizeof(float));
        _buffers.marks = new ComputeBuffer(468*3, sizeof(float));
        // _buffers.marks = new RenderTexture(468, 1, 0, RenderTextureFormat.Default);
        _buffers.vis = new RenderTexture(640, 480, 0, RenderTextureFormat.ARGB32)
        {
            enableRandomWrite = true
        };
        _buffers.vis.Create();
    }

    public void Dispose() {
        _worker?.Dispose();
        _worker = null;

        _buffers.preprocess?.Dispose();
        _buffers.preprocess = null;
        _buffers.marks?.Dispose();
        _buffers.marks = null;

        if (Application.isPlaying) {
            UnityEngine.Object.Destroy(_buffers.vis);
        }
        else {
            UnityEngine.Object.DestroyImmediate(_buffers.vis);
        }
    }

    public Vector2[] RunModel(Texture source, float x1, float x2, float y1, float y2, bool vis = false) {
        x1 -= 10f/source.width;
        y1 -= 10f/source.height;
        x2 += 10f/source.width;
        y2 += 10f/source.height;
        // Preprocessing
        _preproc.SetInts("ModelInputSize", _model_input_w, _model_input_h);
        _preproc.SetTexture(0, "Input", source);
        _preproc.SetBuffer(0, "Output", _buffers.preprocess);
        _preproc.SetFloats("P1", x1, y1);
        _preproc.SetFloats("P2", x2, y2);
        _preproc.DispatchThreads(0, _model_input_w, _model_input_h, 1);

        // NNworker invocation
        using var t = new Tensor(new TensorShape(1, _model_input_h, _model_input_w, 3), _buffers.preprocess);
        using var roi_x =  new Tensor(new TensorShape(1,1,1,1), new float[]{source.width*x1});
        using var roi_y = new Tensor(new TensorShape(1,1,1,1), new float[]{source.height*y1});
        using var roi_w = new Tensor(new TensorShape(1,1,1,1), new float[]{source.width*(x2-x1)});
        using var roi_h = new Tensor(new TensorShape(1,1,1,1), new float[]{source.height*(y2-y1)});
        var inputs = new Dictionary<string, Tensor>() {
            {"input", t},
            {"crop_x1", roi_x},
            {"crop_y1", roi_y},
            {"crop_width", roi_w},
            {"crop_height", roi_h},
        };
        _worker.Execute(inputs);

        //  NN output retrieval
        using var marksTensor = _worker.PeekOutput("final_landmarks");  // dense_1 for face_landmark.onnx
        // using var scoreTensor = _worker.PeekOutput("score");
        // Debug.Log(scoreTensor.ToReadOnlyArray()[0]);

        // Vis
        if (vis) {
            _buffers.marks.SetData(marksTensor.ToReadOnlyArray());

            _vis.SetTexture(0, "Input", source);
            _vis.SetBuffer(0, "Marks", _buffers.marks);
            _vis.SetTexture(0, "Output", _buffers.vis);
            _vis.SetBool("IsBG", false);
            _vis.SetFloats("P1", x1, y1);
            _vis.SetFloats("P2", x2, y2);
            _vis.DispatchThreads(0, 640, 480, 1);
        }

        Vector2[] marks = new Vector2[468];
        var fullMarks = marksTensor.ToReadOnlyArray();
        for (int i=0; i< 468; i++) {
            marks[i] = new Vector2(fullMarks[i*3], fullMarks[i*3+1]);
        }
        return marks;
    }

    public static (float x1, float x2, float y1, float y2) FitRect(Detection face)
    {
        var roi_w = face.x2 - face.x1;
        var roi_h = face.y2 - face.y1;
        var roi_cx = (face.x1 + face.x2) /2;
        var roi_cy = (face.y1 + face.y2) /2;
        roi_w = roi_w > roi_h ? roi_w: roi_h;
        return (roi_cx - roi_w/2, roi_cx + roi_w/2, roi_cy - roi_w/2, roi_cy + roi_w/2);
    }

    public static void PrintModelDims(Model model) {
        string res = "==== Inputs ====\n";
        foreach (var input in model.inputs) {
            res += "  " + input.name;
            res += " [" + string.Join(", ", input.shape.ToArray()) + "]\n";
        }
        res += "==== Outputs ====\n";
        foreach (var output in model.outputs) {
            res += "  " + output;
            var shape = model.GetShapeByName(model.outputs[0]).Value;
            res += " " + shape + "\n";
        }
        Debug.Log(res);
    }
}

