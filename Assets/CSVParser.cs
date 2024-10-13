using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVParser : MonoBehaviour
{
    public enum Parameter
    {
        Class,
        Alcohol,
        MalicAcid,
        Ash,
        AshAlcalinity,
        Magnesium,
        TotalPhenols,
        Flavanoids,
        NonflavanoidPhenols,
        Proanthocyanins,
        ColorIntensity,
        Hue,
        Protein, // OD280/OD315
        Proline
    }

    private string[] labels;
    private float[][] items;

    // Start is called before the first frame update
    public void ReadCSV()
    {
        TextAsset wineText = Resources.Load<TextAsset>("wine");

        if(wineText != null)
        {
            // Split file text into an array of rows
            string[] rows = wineText.text.Split('\n');

            // Split firt row into string of labels
            labels = rows[0].Split(',');
            // Define items matrix with one row per wine
            items = new float[rows.Length - 2][];

            for(int i = 1; i < rows.Length - 1; i++)
            {
                // Split items into a list
                string[] itemLst = rows[i].Split(',');
                items[i - 1] = new float[labels.Length];

                for(int j = 0; j < labels.Length; j++)
                {
                    // Add to items matrix while converting str to float
                    float.TryParse(itemLst[j], out items[i - 1][j]);
                }
            }
        }
        else
        {
            Debug.LogError("Failed to open CSV wine file");
        }
    }

    public void RequestLists(Parameter par1, Parameter par2, Parameter par3, out float[][] itemsMatrix)
    {
        
        // Create list of wine parameters
        Parameter[] paramLst = {par1, par2, par3, Parameter.Class};
        // Define size of items matrix
        itemsMatrix = new float[items.Length][];

        for (int i = 0; i < items.Length; i++)
        {
            // Define size of items matrix rows
            itemsMatrix[i] = new float[paramLst.Length];
            for(int j = 0; j < paramLst.Length; j++)
            {
                // Add wine to the matrix
                itemsMatrix[i][j] = items[i][(int)paramLst[j]];
            }
        }
    }
}
