using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SignalMatchingUI : MonoBehaviour
{
    public SignalEngine signalEngine;
    public Slider wavelengthSlider;
    public Slider frequencySlider;
    public TextMeshProUGUI wavelengthValueText;
    public TextMeshProUGUI frequencyValueText;
    public TextMeshProUGUI matchStatusText;
    public Button confirmMatchButton;

    private Signal currentSignal;
    private float targetWavelength;
    private float targetFrequency;
    private float matchTolerance = 0.05f; // 5% tolerance

    public bool isAutomationUnlocked = false; // Set to true when automation upgrade is purchased

    void Start()
    {
        wavelengthSlider.onValueChanged.AddListener(OnWavelengthChanged);
        frequencySlider.onValueChanged.AddListener(OnFrequencyChanged);
        confirmMatchButton.onClick.AddListener(OnConfirmMatch);

        // Initially hide if automation is unlocked
        UpdateAutomationStatus();
    }

    void Update()
    {
        UpdateAutomationStatus();
    }

    void UpdateAutomationStatus()
    {
        if (signalEngine != null)
        {
            isAutomationUnlocked = signalEngine.signalMatchingAutomationUnlocked;
            gameObject.SetActive(!isAutomationUnlocked);
        }
    }

    public void SetSignal(Signal signal)
    {
        currentSignal = signal;
        targetWavelength = signal.wavelength;
        targetFrequency = signal.frequency;

        // Set slider ranges based on target values with some margin
        wavelengthSlider.minValue = Mathf.Max(300f, targetWavelength - 100f);
        wavelengthSlider.maxValue = targetWavelength + 100f;
        frequencySlider.minValue = Mathf.Max(0.1f, targetFrequency - 20f);
        frequencySlider.maxValue = targetFrequency + 20f;

        // Reset sliders to random positions within range
        wavelengthSlider.value = UnityEngine.Random.Range(wavelengthSlider.minValue, wavelengthSlider.maxValue);
        frequencySlider.value = UnityEngine.Random.Range(frequencySlider.minValue, frequencySlider.maxValue);

        UpdateUI();
        gameObject.SetActive(!isAutomationUnlocked);
    }

    void OnWavelengthChanged(float value)
    {
        wavelengthValueText.text = value.ToString("F1") + " nm";
        CheckMatch();
    }

    void OnFrequencyChanged(float value)
    {
        frequencyValueText.text = value.ToString("F1") + " Hz";
        CheckMatch();
    }

    void CheckMatch()
    {
        if (currentSignal == null) return;

        float wavelengthDiff = Mathf.Abs(wavelengthSlider.value - targetWavelength) / targetWavelength;
        float frequencyDiff = Mathf.Abs(frequencySlider.value - targetFrequency) / targetFrequency;

        bool isMatched = wavelengthDiff <= matchTolerance && frequencyDiff <= matchTolerance;
        matchStatusText.text = isMatched ? "MATCHED!" : "Adjusting...";
        matchStatusText.color = isMatched ? Color.green : Color.red;
        confirmMatchButton.interactable = isMatched;
    }

    void OnConfirmMatch()
    {
        if (currentSignal == null) return;

        // Apply matching bonus to signal processing
        currentSignal.patternMultiplier *= 1.5f; // 50% bonus for manual matching

        // Hide UI and proceed with processing
        gameObject.SetActive(false);

        // Notify SignalEngine to process the matched signal
        signalEngine.ProcessMatchedSignal(currentSignal);
    }

    void UpdateUI()
    {
        OnWavelengthChanged(wavelengthSlider.value);
        OnFrequencyChanged(frequencySlider.value);
    }
}