using System;
using UnityEngine;
using UnityEngine.UI;
using UnityProgressBar;

public class ParkingStructure : MonoBehaviour
{
    [Header("Data")]
    [SerializeField]
    private string _id = "Parking Structure";
    [SerializeField]
    private Texture2D _thumbnail;

    [Range(0f, 100f)]
    public float fillPercentage; // How full the structure is (0 to 100)

    // Serialize these
    [Header("UI Reference")]
    public Text nameText;
    public RawImage thumbnailDisplay;
    public Text percentageText;
    public ProgressBar progressBar;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"ParkingStructure {_id} started with fill: {fillPercentage}%");
        nameText.text = _id;
        thumbnailDisplay.texture = _thumbnail;
        UpdatePercentageText();
        UpdateProgressBarValue();
    }

    // Update is called once per frame
    void Update()
    {
        // You can simulate parking usage changes here, if needed
        UpdatePercentageText();
        UpdateProgressBarValue();
    }

    private void UpdateProgressBarValue()
    {
        progressBar.Value = fillPercentage / 100f;
    }

    void UpdatePercentageText()
    {
        percentageText.text = fillPercentage + "% full";
    }

    // This will automatically update the UI nameText in the Editor when _id is changed
    private void OnValidate()
    {
        if (nameText != null)
        {
            nameText.text = _id;
        }
        if (thumbnailDisplay != null)
        {
            thumbnailDisplay.texture = _thumbnail;
        }
    }

    // TODO: Assign fillPercentage from the API instead of setting it in-editor
    private void GetAPIPercentage() {}
}
