using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Axis : MonoBehaviour
{
    public GameObject cameraObject;

    [SerializeField]
    private LineRenderer axisLine;

    [SerializeField]
    private TextMeshPro minText, maxText, label;

    public void UpdateAxis(float lineSize, string minStr, string maxStr, string labelStr, float padding)
    {
        // Set axis scale and texts
        axisLine.transform.localScale = new Vector3(1, 1, lineSize);
        minText.text = minStr;
        maxText.text = maxStr;
        label.text = labelStr;

        // Position axis' labels
        minText.transform.localPosition = new Vector3(0, -0.4f, lineSize*(1-padding));
        maxText.transform.localPosition = new Vector3(0, -0.4f, lineSize*padding);
        label.transform.localPosition = new Vector3(0, -0.4f, lineSize/2);

        // Get camera GameObject
        cameraObject = GameObject.Find("CameraControls");
    }

    void Update()
    {
        // Labels always face the camera
        minText.transform.LookAt(cameraObject.transform);
        maxText.transform.LookAt(cameraObject.transform);
        label.transform.LookAt(cameraObject.transform);
    }
}
