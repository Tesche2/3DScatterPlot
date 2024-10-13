using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DataCube : MonoBehaviour
{
    [HideInInspector]
    public Material highlightedMaterial, cubeMaterial;

    [HideInInspector]
    public string cubeClass;
    [HideInInspector]
    public string[] paramLst = new string[3];
    [HideInInspector]
    public float[] coords = new float[3];


    [SerializeField]
    private GameObject textHolder;
    [SerializeField]
    private TextMeshPro classText, paramsText;

    private float classFontSize, paramsFontSize, distThresh = 3.5f;

    private GameObject cameraObject;

    // Start is called before the first frame update
    void Start()
    {
        classFontSize = classText.fontSize;
        paramsFontSize = paramsText.fontSize;
    }

    // Update is called once per frame
    void Update()
    {
        // Tags always face the camera
        textHolder.transform.LookAt(cameraObject.transform);

        // Resize font based on camera distance, so it can be seen from further away
        float dist = Vector3.Distance(transform.position, cameraObject.transform.position);
        if (dist >= distThresh)
        {
            classText.fontSize = classFontSize + dist - distThresh;
            paramsText.fontSize = paramsFontSize + dist - distThresh;
        }
    }

    private void OnEnable()
    {
        // Highlights, enables text, and defines text of data node aimed at
        gameObject.GetComponent<MeshRenderer>().material = highlightedMaterial;
        textHolder.SetActive(true);
        classText.text = cubeClass;
        paramsText.text =
            $"{paramLst[0]}: {coords[0]}\n" +
            $"{paramLst[1]}: {coords[1]}\n" +
            $"{paramLst[2]}: {coords[2]}\n";

        // Get camera object, so the tags can always face it
        cameraObject = GameObject.Find("CameraControls");
    }

    private void OnDisable()
    {
        // Returns material to its original one, and disables text
        gameObject.GetComponent<MeshRenderer>().material = cubeMaterial;
        textHolder.SetActive(false);
    }
}
