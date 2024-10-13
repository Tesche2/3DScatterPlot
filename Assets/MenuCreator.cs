using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuCreator : MonoBehaviour
{
    private string[] labelsArr =
        {"Alcohol",
        "Malic Acid",
        "Ash",
        "Ash Alcalinity",
        "Magnesium",
        "Total Phenols",
        "Flavanoids",
        "Non Flav. Phen.",
        "Proanthocyanins",
        "Color Intensity",
        "Hue",
        "OD280/OD315",
        "Proline" };

    private string[] paramsArr =
        {"Alcohol",
        "MalicAcid",
        "Ash",
        "AshAlcalinity",
        "Magnesium",
        "TotalPhenols",
        "Flavanoids",
        "NonflavanoidPhenols",
        "Proanthocyanins",
        "ColorIntensity",
        "Hue",
        "OD280/OD315",
        "Proline" };

    [SerializeField]
    private GameObject parameterSelectLabel, axisButtonObject, plotRegionObject;
    private GameObject plotRegion;
    private PlotRegion plotRegionScript;

    [SerializeField]
    private InputActionReference closeMenu;

    [SerializeField]
    private CameraController cameraController;

    private readonly float yStep = 23.0f;
    private readonly float xStep = 20.0f;

    private GameObject[][] buttonMatrix;

    private string[] chosenParams = new string[3];

    private void OnEnable()
    {
        // Enable crosshair, to make aiming easier
        GameObject.Find("Crosshair").GetComponent<Canvas>().enabled = false;

        closeMenu.action.performed += CloseMenu;
    }

    private void OnDisable()
    {
        GameObject.Find("Crosshair").GetComponent<Canvas>().enabled = true;

        closeMenu.action.performed -= CloseMenu;
    }

    private void CloseMenu(InputAction.CallbackContext obj)
    {
        Cursor.lockState = CursorLockMode.Locked;
        CancelMenu();
    }

    public void CancelMenu()
    {
        // Give controller back to the camera
        cameraController.enabled = true;
        gameObject.SetActive(false);
    }

    public void CloseApplication()
    {
        // Close application, or leave play mode
        #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateLabelList();
        CreateButtons();

        // Select Alcohol, Ash and Magnesium as default parameters
        OnButtonClick(0, 0);
        OnButtonClick(2, 1);
        OnButtonClick(4, 2);

        OnSelectClick();
    }

    private void CreateLabelList()
    {
        for (int i = 0; i < labelsArr.Length; i++)
        {
            // Instantiate, define text, and position labels on the menu
            GameObject newLabel = Instantiate(parameterSelectLabel, transform);
            newLabel.GetComponent<TextMeshProUGUI>().text = labelsArr[i];
            newLabel.transform.localPosition = new Vector3(
                newLabel.transform.localPosition.x,
                newLabel.transform.localPosition.y - yStep * i);
            newLabel.name =  paramsArr[i] + "Label";
        }
    }

    private void CreateButtons()
    {
        buttonMatrix = new GameObject[paramsArr.Length][];
        for (int i = 0; i < paramsArr.Length; i++)
        {
            buttonMatrix[i] = new GameObject[3];
            for(int j = 0; j < 3; j++)
            {
                // get X, Y or Z for 0, 1 or 2
                char axisCoord = (char)('X'+j);

                // Instantiate and position each button
                GameObject newButton = Instantiate(axisButtonObject, transform);
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = axisCoord.ToString();
                newButton.transform.localPosition = new Vector3(
                    newButton.transform.localPosition.x + xStep * j,
                    newButton.transform.localPosition.y - yStep * i);
                newButton.name = paramsArr[i] + axisCoord.ToString();

                int capI = i;
                int capJ = j;

                // Add button functionality
                newButton.GetComponent<Button>().onClick.AddListener(
                    () => OnButtonClick(capI, capJ));

                // Add button to matrix, for later referencing
                buttonMatrix[i][j] = newButton;
            }
        }
    }

    private void OnButtonClick(int param, int axis)
    {
        Color unselectedColor = new Color(1, 1, 1);
        Color selectedColor = new Color(.5f, .5f, .5f);

        // Resets all buttons to the base unselected color
        for(int i = 0; i < paramsArr.Length; i++)
        {
            buttonMatrix[i][axis].GetComponent<Button>().image.color = unselectedColor;
        }

        // Sets selected button to have the correct color
        buttonMatrix[param][axis].GetComponent<Button>().image.color = selectedColor;

        // Update chosen parameters array
        chosenParams[axis] = paramsArr[param];

    }

    public void OnSelectClick()
    {
        // Destroy current plot region, so a new one can be created
        if (GameObject.Find("PlotRegion"))
        {
            Destroy(GameObject.Find("PlotRegion"));
        }

        // Create new plot region with new parameters
        plotRegion = Instantiate(plotRegionObject);
        plotRegion.transform.position = new Vector3(0, 0, 0);
        plotRegion.name = "PlotRegion";
        plotRegionScript = plotRegion.GetComponent<PlotRegion>();

        // Plot data on the new plot region
        plotRegionScript.chosenParameters = chosenParams;
        plotRegionScript.SetupPlotRegion();

        // Give controller back to the camera
        cameraController.enabled = true;
        gameObject.SetActive(false);
    }
}
