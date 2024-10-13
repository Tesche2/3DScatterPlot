using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject dataCube;

    [SerializeField]
    private Material[] materialsArray = new Material[3];
    [SerializeField]
    private Material[] highlightedMaterialsArray = new Material[3];

    [SerializeField]
    private CSVParser parser;

    [HideInInspector]
    public float regionSize;

    [HideInInspector]
    public string[] paramLst = new string[3];


    // Define min and max value of each parameter
    [HideInInspector]
    public float[] minValues = new float[3];
    [HideInInspector]
    public float[] maxValues = new float[3];

    private CSVParser.Parameter StrToPar(string str)
    {
        switch (str)
        {
            case "Class":
                return CSVParser.Parameter.Class;
            case "Alcohol":
                return CSVParser.Parameter.Alcohol;
            case "MalicAcid":
                return CSVParser.Parameter.MalicAcid;
            case "Ash":
                return CSVParser.Parameter.Ash;
            case "AshAlcalinity":
                return CSVParser.Parameter.AshAlcalinity;
            case "Magnesium":
                return CSVParser.Parameter.Magnesium;
            case "TotalPhenols":
                return CSVParser.Parameter.TotalPhenols;
            case "Flavanoids":
                return CSVParser.Parameter.Flavanoids;
            case "NonflavanoidPhenols":
                return CSVParser.Parameter.NonflavanoidPhenols;
            case "Proanthocyanins":
                return CSVParser.Parameter.Proanthocyanins;
            case "ColorIntensity":
                return CSVParser.Parameter.ColorIntensity;
            case "Hue":
                return CSVParser.Parameter.Hue;
            case "OD280/OD315":
                return CSVParser.Parameter.Protein;
            case "Proline":
                return CSVParser.Parameter.Proline;
            default:
                return CSVParser.Parameter.Class;
        }
    }

    private string SimplifyStr(string str)
    {
        switch (str)
        {
            case "Class":
                return "Class";
            case "Alcohol":
                return "Alcohol";
            case "MalicAcid":
                return "M. Acid";
            case "Ash":
                return "Ash";
            case "AshAlcalinity":
                return "Ash Alc.";
            case "Magnesium":
                return "Mg";
            case "TotalPhenols":
                return "T. Phen.";
            case "Flavanoids":
                return "Flav.";
            case "NonflavanoidPhenols":
                return "N.F.P.";
            case "Proanthocyanins":
                return "Proantho.";
            case "ColorIntensity":
                return "Col. Int.";
            case "Hue":
                return "Hue";
            case "OD280/OD315":
                return "Proteins";
            case "Proline":
                return "Proline";
            default:
                return "???";
        }
    }

    private void DetermineParamLimits(float[][] dataMatrix)
    {
        // Fill min and max values arrays with positive and negative infinity
        minValues = Enumerable.Repeat(float.PositiveInfinity, dataMatrix[0].Length - 1).ToArray();
        maxValues = Enumerable.Repeat(float.NegativeInfinity, dataMatrix[0].Length - 1).ToArray();

        for (int i = 0; i < minValues.Length; i++)
        {
            for(int j = 0; j < dataMatrix.Length; j++)
            {
                // Replace min and max values with the ones from dataMatrix
                minValues[i] = Mathf.Min(minValues[i], dataMatrix[j][i]);
                maxValues[i] = Mathf.Max(maxValues[i], dataMatrix[j][i]);
            }
        }
    }

    // Get a value between 0 and 1, based on min and max values of the parameter
    private Vector3 NormalizeCoordinates(float[] dataPoint)
    {
        Vector3 normalizedVector = new Vector3();

        for(int i = 0; i < minValues.Length; i++)
        {
            normalizedVector[i] = (dataPoint[i] - minValues[i]) / (maxValues[i] - minValues[i]);
        }

        return normalizedVector;
    }

    public void PlotData()
    {
        // Read wine.csv file
        parser.ReadCSV();

        // Get dataMatrix
        parser.RequestLists(
            StrToPar(paramLst[0]),
            StrToPar(paramLst[1]),
            StrToPar(paramLst[2]),
            out float[][] dataMatrix);

        // Determine min and max value arrays
        DetermineParamLimits(dataMatrix);

        for (int i = 0; i < dataMatrix.Length - 1; i++)
        {
            // Instantiate and resize data point
            GameObject data = Instantiate(dataCube, transform);
            data.transform.localScale *= regionSize * 0.02f;
            for (int j = 0; j < minValues.Length; j++)
            {
                // Normalize coordinates of data point, so it fits in the plot region
                Vector3 dataPos = NormalizeCoordinates(dataMatrix[i]);
                data.transform.localPosition = dataPos * regionSize;
                data.GetComponent<MeshRenderer>().material =
                    materialsArray[(int)dataMatrix[i][3] - 1];
                DataCube cubeScript = data.GetComponent<DataCube>();
                
                // Define cube script variables
                cubeScript.cubeMaterial =
                    materialsArray[(int)dataMatrix[i][3] - 1];
                cubeScript.highlightedMaterial =
                    highlightedMaterialsArray[(int)dataMatrix[i][3] - 1];
                cubeScript.cubeClass = "Class " + (int)dataMatrix[i][3];
                cubeScript.paramLst = new string[]
                {
                    SimplifyStr(paramLst[0]),
                    SimplifyStr(paramLst[1]),
                    SimplifyStr(paramLst[2]),
                };
                cubeScript.coords = new float[]
                {
                    dataMatrix[i][0],
                    dataMatrix[i][1],
                    dataMatrix[i][2]
                };
            }
        }
    }
}
