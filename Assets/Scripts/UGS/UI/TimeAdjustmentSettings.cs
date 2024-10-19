using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeAdjustmentSettings : MonoBehaviour
{
    [SerializeField] private Button hourUpBtn;
    [SerializeField] private Button hourDownBtn;
    [SerializeField] private Button minutesUpBtn;
    [SerializeField] private Button minutesDownBtn;
    [SerializeField] private Button meridiemBtn;
    [SerializeField] private InputField hourInput;
    [SerializeField] private InputField minutesInput;
    [SerializeField] private Text hourText;
    [SerializeField] private Text minutesText;
    [SerializeField] private TextMeshProUGUI meridiemText;

    [SerializeField] private int hourChangeStep = 1;
    [SerializeField] private int minuteChangeStep = 30;

    [SerializeField] private int hourMinLimit = 1;
    [SerializeField] private int hourMaxLimit = 12;
    [SerializeField] private int minuteMinLimit = 0;
    [SerializeField] private int minuteMaxLimit = 59;

    private bool isAM;
    private bool IsAM
    {
        get
        {
            return isAM;
        }
        set
        {
            isAM = value;
            meridiemText.text = isAM ? "AM" : "PM";
        }
    }

    private void Awake()
    {
        hourInput.characterLimit = 2;
        minutesInput.characterLimit = 2;
        hourInput.text = "12";
        minutesInput.text = "00";
    }
    private void OnEnable()
    {
        hourUpBtn.onClick.AddListener(() => AdjustTime(hourInput, -hourChangeStep, hourMinLimit, hourMaxLimit));
        hourDownBtn.onClick.AddListener(() => AdjustTime(hourInput, hourChangeStep, hourMinLimit, hourMaxLimit));
        minutesUpBtn.onClick.AddListener(() => AdjustTime(minutesInput, -minuteChangeStep, minuteMinLimit, minuteMaxLimit));
        minutesDownBtn.onClick.AddListener(() => AdjustTime(minutesInput, minuteChangeStep, minuteMinLimit, minuteMaxLimit));
        meridiemBtn.onClick.AddListener(() => IsAM = !IsAM);

        hourInput.onEndEdit.AddListener(OnInputFieldEndEdit);
        minutesInput.onEndEdit.AddListener(OnInputFieldEndEdit);
    }
    private void OnDisable()
    {
        hourUpBtn.onClick.RemoveAllListeners();
        hourDownBtn.onClick.RemoveAllListeners();
        minutesUpBtn.onClick.RemoveAllListeners();
        minutesDownBtn.onClick.RemoveAllListeners();
        meridiemBtn.onClick.RemoveAllListeners();

        hourInput.onEndEdit.RemoveAllListeners();
        minutesInput.onEndEdit.RemoveAllListeners();
    }
    private void OnInputFieldEndEdit(string input)
    {
        InputField currentInputField = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<InputField>();
        if (currentInputField == hourInput)
        {
            AdjustTime(hourInput, 0, hourMinLimit, hourMaxLimit);
        }
        else if (currentInputField == minutesInput)
        {
            AdjustTime(minutesInput, 0, minuteMinLimit, minuteMaxLimit);
        }
    }
    private void AdjustTime(InputField timeInput, int adjustment, int min, int max)
    {
        timeInput.text = GetTime(timeInput.text, adjustment, min, max);
    }

    private string GetTime(string time, int adjustment, int min, int max)
    {
        if (int.TryParse(time, out int timeValue))
        {
            timeValue += adjustment;
            // Adjust for the min-max format
            if (timeValue < min)
            {
                timeValue = max;
            }
            else if (timeValue > max)
            {
                timeValue = min;
            }
            // Formats the number to have at least two digits
            return timeValue.ToString("D2");
        }
        return string.Empty;
    }
    public string ReturnTime()
    {
        return $"{hourText.text}:{minutesText.text} {meridiemText.text}";
    }
    public void SetTime(DateTime dateTime)
    {
        //Set the currentRaceCheckins time to the text fields
        hourInput.text = dateTime.ToString("hh");
        minutesInput.text = dateTime.ToString("mm");
        IsAM = dateTime.ToString("tt").Equals("AM", StringComparison.OrdinalIgnoreCase);
    }
}
