using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotRegion : MonoBehaviour
{
    [SerializeField]
    private GameObject axisModel;

    private GameObject xAxisObject, yAxisObject, zAxisObject;
    private Axis xAxis, yAxis, zAxis;

    [SerializeField]
    private GameObject dataSpawnerObject;

    [SerializeField]
    private float axisPadding = .1f;

    public float size;

    [SerializeField]
    private GameObject colliderPrefab;

    [HideInInspector]
    public string[] chosenParameters = new string[3];

    // Start is called before the first frame update
    public void SetupPlotRegion()
    {
        // Instantiate axes and data spawner
        xAxisObject = Instantiate(axisModel, transform);
        yAxisObject = Instantiate(axisModel, transform);
        zAxisObject = Instantiate(axisModel, transform);
        GameObject spawner = Instantiate(dataSpawnerObject, transform);

        xAxis = xAxisObject.GetComponent<Axis>();
        yAxis = yAxisObject.GetComponent<Axis>();
        zAxis = zAxisObject.GetComponent<Axis>();

        // Position data spawner
        spawner.transform.position = new Vector3(
            -size / 2 + axisPadding * size,
            axisPadding * size,
            -size / 2 + axisPadding * size);
        DataSpawner spawnerScript = spawner.GetComponent<DataSpawner>();

        // Plot data on the appropriate region
        spawnerScript.regionSize = size - size * axisPadding * 2;
        Array.Copy(chosenParameters, spawnerScript.paramLst, chosenParameters.Length);
        spawnerScript.PlotData();

        // Label axes
        xAxis.UpdateAxis(
            size,
            spawnerScript.minValues[0].ToString(),
            spawnerScript.maxValues[0].ToString(),
            ToLabel(chosenParameters[0]),
            axisPadding);
        yAxis.UpdateAxis(
            size,
            spawnerScript.minValues[1].ToString(),
            spawnerScript.maxValues[1].ToString(),
            ToLabel(chosenParameters[1]),
            axisPadding);
        zAxis.UpdateAxis(
            size,
            spawnerScript.minValues[2].ToString(),
            spawnerScript.maxValues[2].ToString(),
            ToLabel(chosenParameters[2]),
            axisPadding);

        // Rotate axes
        xAxisObject.transform.rotation = Quaternion.Euler(0, -90, 0);
        yAxisObject.transform.rotation = Quaternion.Euler(90, 45, 0);
        zAxisObject.transform.rotation = Quaternion.Euler(0, 180, 0);

        // Create trigger zones to dynamically reposition the axes
        CreateCollider(new Vector3(-1, 0, -1), "xNzNQuad");
        CreateCollider(new Vector3(-1, 0, 1), "xNzPQuad");
        CreateCollider(new Vector3(1, 0, -1), "xPzNQuad");
        CreateCollider(new Vector3(1, 0, 1), "xPzPQuad");
    }

    // Retruns string properly formatted for a label
    private string ToLabel(string str)
    {
        switch (str)
        {
            case "Class":
                return "Class";
            case "Alcohol":
                return "Alcohol";
            case "MalicAcid":
                return "Malic Acid";
            case "Ash":
                return "Ash";
            case "AshAlcalinity":
                return "Ash Alcalinity";
            case "Magnesium":
                return "Magnesium";
            case "TotalPhenols":
                return "Total Phenols";
            case "Flavanoids":
                return "Flavanoids";
            case "NonflavanoidPhenols":
                return "Non flav. Phen.";
            case "Proanthocyanins":
                return "Proanthocyanins";
            case "ColorIntensity":
                return "Color Intensity";
            case "Hue":
                return "Hue";
            case "OD280/OD315":
                return "OD280/OD315";
            case "Proline":
                return "Proline";
            default:
                return "Unkown";
        }
    }

    public void MoveAxes(Vector3 axisPos)
    {
        // Translate x axis ----------------------------------------------
        xAxisObject.transform.position = new Vector3(size / 2,
                                                    0,
                                                    size / 2 * axisPos.z);

        // Translate y axis ----------------------------------------------
        yAxisObject.transform.position = new Vector3(size / 2 * axisPos.x,
                                                    size,
                                                    size / 2 * axisPos.z);

        // Calculate rotation for y axis
        float targetRot = Mathf.Abs(Mathf.Max(0, axisPos.x) * 3 -
                                    Mathf.Max(0, axisPos.z)) * 90;
        
        // Rotate y axis
        yAxisObject.transform.localRotation = Quaternion.Euler(
            yAxisObject.transform.localEulerAngles.x,
            45 + targetRot,
            0);

        // Translate z axis ----------------------------------------------
        zAxisObject.transform.position = new Vector3(size / 2 * axisPos.x,
                                                    0,
                                                    size / 2);
    }

    private GameObject CreateCollider(Vector3 colliderDirection, string objectName)
    {
        // Instantiate collider
        GameObject collider = Instantiate(colliderPrefab, transform);

        // Set collider center, so scaling happens in the correct direction
        collider.GetComponent<BoxCollider>().center = colliderDirection * .5f;

        // Scale and name colliders
        collider.transform.localScale = new Vector3(1000, 1000, 1000);
        collider.name = objectName;

        return collider;
    }
}
