using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;


public class ModelTest : MonoBehaviour
{
    public NNModel faceDetOnnx;
    public NNModel faceMarkOnnx;

    private Model faceDetModel;
    private IWorker faceDetWorker;

    void Start()
    {
        faceDetModel = ModelLoader.Load(faceDetOnnx);
        Debug.Log("Inputs");
        foreach (var s in faceDetModel.inputs)
            Debug.Log(s);
        Debug.Log("Outputs");
        foreach (var s in faceDetModel.outputs)
            Debug.Log(s);
        faceDetWorker = WorkerFactory.CreateWorker(faceDetModel);

        Tensor input = new Tensor(1, 200, 200, 3);
        faceDetWorker.Execute(input);
        Tensor O = faceDetWorker.PeekOutput("446");
        input.Dispose();
        Debug.Log(O.DataToString());
    }

    void Update()
    {
        
    }
}
