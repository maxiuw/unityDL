using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneController : MonoBehaviour
{
    // synth is going to be a camera
    public ImageSynthesis synth;
    public GameObject[] prefabs;
    int maxObjects = 50;
    float maxX = 5;
    float maxZ = 5; 
    float minX = -5; 
    float minZ= -5;
    float maxY = 3;
    float minY = 1;
    // Start is called before the first frame update
    private GameObject[] created;
    private shapePool pool;
    private int frameCount = 0;
    private int valimages;
    private int trainimages = 10000;
    void Start()
    {
        pool = shapePool.Create(prefabs);

        // created = new GameObject[maxObjects];
    }

    // Update is called once per frame
    void Update()
    {
        if (frameCount % 30 == 0)
        {

            GenerateRandom();
            // it will synth on the scene chance
            // frameCount = 0;
        }
        frameCount++;
        // string filename = $"image_{frameCount.ToString().PadLeft(5,'0')}";
        // we just wamt tp save display 1 and 2         
        // synth.Save(filename, 512,512, "Assets/captures/training", 2);

        
        
        // GenerateRandom();
        // synth.OnSceneChange(); // it will synth on the scene chance
    }

    void GenerateRandom() {
        // first it will destroy all the objects in the scene 
        // for (int j = 0; j < created.Length; j++){
        //     if(created[j] != null) {
        //         Destroy(created[j]);
        //     }
        // }
        pool.ReclaimAll();
        // and now generate new ones 
        int objnumber = Random.Range(5, maxObjects);
        for (int i = 0; i < objnumber; i++) {
            // Debug.Log(objnumber);

            int prefabIdx = Random.Range(0, prefabs.Length); //);
            GameObject prefab = prefabs[prefabIdx]; 
            // random position, rotation and scale 
            float newX, newY, newZ;
            newX = Random.Range(minX, maxX);
            newY = Random.Range(minY, maxY);
            newZ = Random.Range(minZ, maxZ);

            Vector3 newPos = new Vector3(newX, newY, newZ);
            Quaternion newRot = Random.rotation;

            var newObj = pool.Get((shapeLabel)prefabIdx);
            newObj.obj.transform.position = newPos;
            newObj.obj.transform.rotation = newRot;
            // GameObject newObj = Instantiate(prefab, newPos, newRot);
            float scaleFactor = Random.Range(0.2f, 1);
            Vector3 newScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            newObj.obj.transform.localScale = newScale;
            float newR, newG, newB;
            newR = Random.Range(0.0f, 1.0f);
            newG = Random.Range(0.0f, 1.0f);
            newB = Random.Range(0.0f, 1.0f);
            Color newColor = new Color(newR, newG, newB);
            newObj.obj.GetComponent<Renderer>().material.color = newColor;

            // created[i] = newObj; // this commented out creates a cool waterfall (if we dont use shape pool ) :)
        }
        synth.OnSceneChange();
    }
}
