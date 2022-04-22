using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using System.IO;

public class mlRunner : MonoBehaviour {
    
    // [SerializeField]
    // private Texture2D inputImage;
    private RenderTexture intputImageRendered;
    // [SerializeField]
    // private Texture2D outputImage;
    [SerializeField]
    private NNModel Vea;
    private Model runtimeModel;
    private IWorker worker;
    private string outputlayername;
    [SerializeField]
    private Camera cam;
    private const int IMAGE_MEAN = 0;
    private const float IMAGE_STD = 1f;
    // Start is called before the first frame update
    private int framerate = 0;
    private int height = 64; //512;
    private int width = 64; //512;
    void Start()
    {
        // load the model, takes the model (onnx) and creates runtime model 
        // cam = GetComponent<Camera>();
        runtimeModel = ModelLoader.Load(Vea);
        // create the worker - specifies which device is used for running the model
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, runtimeModel);
        // we take the last layer   
        outputlayername = runtimeModel.outputs[runtimeModel.outputs.Count - 1];
        
    }

    public void Update() {
        if (framerate % 100 == 0) {
            // // Predict();
            // Texture2D tex = readImage(cam, width, height);
            // // Debug.Log(tex);
            // // Texture2D prediction = 
            // Predict(tex);
            // Object.Destroy(tex);
            // // Object.Destroy(prediction);
        }
        // Predict();
        Texture2D tex = readImage(cam, width, height);
        // Debug.Log(tex);
        // Texture2D prediction = 
        Predict(tex);
        Object.Destroy(tex);
        // Object.Destroy(prediction);
        framerate++;
    }

    // Update is called once per frame
    public void Predict(Texture2D img)
    {
     
        // Get camera (if the script is attached to the camera object)
        
        // Debug.Log(cam.targetTexture);
        // if (cam != null)
        // {
        //     Debug.Log("Cam is ok" + cam.targetTexture.width + cam.targetTexture.height);
        // }

        // RenderTexture currentRT = RenderTexture.active;
        // RenderTexture.active = cam.targetTexture;

        // // Create render
        // cam.Render();

        // Texture2D img = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
        // img.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        // img.Apply();

        // Convert to Color32
        // var input = img.GetPixels32();

        // RenderTexture.active = currentRT;

        Debug.Log("Original render is: w: " + img.width + " h:" + img.height);
        
        // image to tensor 
        // Tensor tensor = TransformInput(input, height, width); // this gives a shitty input
        Tensor tensor = new Tensor(img, 3);
        Debug.Log(tensor.ArgMax());
        // Execute it 
        worker.Execute(tensor);

        // Get result
        // Tensor output = worker.PeekOutput();
        // or 
        var output = worker.PeekOutput(outputlayername);

        var renderTexture = new RenderTexture(160, 160, 24, RenderTextureFormat.ARGB32);
 
        // var item = new Image();
        // item.image = renderTexture;
        // m_MainCamera.targetTexture = renderTexture;

        // saving prediction/original/tesor for verification 
        // // Convert to predicted RenderTexture
        // var predictedTextureRender = output.ToRenderTexture();
        // var predictedTexture2d = toTexture2D(predictedTextureRender);
        // var bytes = predictedTexture2d.EncodeToPNG();
        // int rand = Random.Range(0,10000);
        // string filename = $"prediction_{rand.ToString().PadLeft(5,'0')}.png";
		// File.WriteAllBytes(filename, bytes);	
        
        // // original image tensor 
        // predictedTextureRender = tensor.ToRenderTexture();
        // predictedTexture2d = toTexture2D(predictedTextureRender);
        // bytes = predictedTexture2d.EncodeToPNG();
        // filename = $"originalTensor_{rand.ToString().PadLeft(5,'0')}.png";
		// File.WriteAllBytes(filename, bytes);	

        // // image origninal 
        // bytes = img.EncodeToPNG();
        // filename = $"originalIm_{rand.ToString().PadLeft(5,'0')}.png";
		// File.WriteAllBytes(filename, bytes);

    }

    public Tensor TransformInput(Color32[] pic, int width, int height)
    {
        float[] floatValues = new float[width * height * 3];
        for (int i = 0; i < pic.Length; ++i)
        {
            var color = pic[i];
            // Debug.Log(color);
            floatValues[i * 3 + 0] = color.r/255; //- IMAGE_MEAN) / IMAGE_STD;
            floatValues[i * 3 + 1] = color.g/255; //- IMAGE_MEAN) / IMAGE_STD;
            floatValues[i * 3 + 2] = color.b/255; //- IMAGE_MEAN) / IMAGE_STD;
        }
        return new Tensor(1, height, width, 3, floatValues);
    }

    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    private Texture2D readImage(Camera cam, int width, int height) {
		
		var depth = 24;
		var format = RenderTextureFormat.Default;
		var readWrite = RenderTextureReadWrite.Default;
		var finalRT = RenderTexture.GetTemporary(width, height, depth, format, readWrite);
		var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        // this seems to be resizing texture 
        // RenderTexture.GetTemporary(mainCamera.pixelWidth, mainCamera.pixelHeight, depth, format, readWrite, antiAliasing);

		var prevActiveRT = RenderTexture.active;
		var prevCameraRT = cam.targetTexture;

		// render to offscreen texture (readonly from CPU side)
		RenderTexture.active = finalRT;
		cam.targetTexture = finalRT;

		cam.Render();

		// read offsreen texture contents into the CPU readable texture
		tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
		tex.Apply();
        
        return tex;

		// encode texture into PNG
		// var bytes = tex.EncodeToPNG();
		// File.WriteAllBytes(filename, bytes);					

		// // restore state and cleanup
		// cam.targetTexture = prevCameraRT;
		// RenderTexture.active = prevActiveRT;

		// Object.Destroy(tex);
		// RenderTexture.ReleaseTemporary(finalRT);
	}

}
