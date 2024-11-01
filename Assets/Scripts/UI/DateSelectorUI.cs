using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DateSelectorUI : MonoBehaviour
{
    [SerializeField] private Button dayUpBtn;
    [SerializeField] private Button dayDownBtn;
    [SerializeField] private Button monthUpBtn;
    [SerializeField] private Button monthDownBtn;
    [SerializeField] private Button yearUpBtn;
    [SerializeField] private Button yearDownBtn;

    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI monthText;
    [SerializeField] private TextMeshProUGUI yearText;

    [SerializeField] private int dayChangeStep = 1;
    [SerializeField] private int monthChangeStep = 1;
    [SerializeField] private int yearChangeStep = 1;

    [SerializeField] private int dayMinLimit = 1;
    [SerializeField] private int dayMaxLimit = 31;
    [SerializeField] private int monthMinLimit = 1;
    [SerializeField] private int monthMaxLimit = 12; 
    [SerializeField] private int yearMinLimit = 2024;
    [SerializeField] private int yearMaxLimit = 3000;

    private void OnEnable()
    {
        dayUpBtn.onClick.AddListener(() => AdjustDate(dayText,"D2", -dayChangeStep,dayMinLimit, dayMaxLimit));
        dayDownBtn.onClick.AddListener(() => AdjustDate(dayText, "D2", dayChangeStep, dayMinLimit, dayMaxLimit));
        monthUpBtn.onClick.AddListener(() => AdjustDate(monthText, "D2", -monthChangeStep, monthMinLimit, monthMaxLimit));
        monthDownBtn.onClick.AddListener(() => AdjustDate(monthText, "D2", monthChangeStep, monthMinLimit, monthMaxLimit));
        yearUpBtn.onClick.AddListener(() => AdjustDate(yearText, "D4", yearChangeStep, yearMinLimit, yearMaxLimit));
        yearDownBtn.onClick.AddListener(() => AdjustDate(yearText, "D4", yearChangeStep, yearMinLimit, yearMaxLimit));
    }
    private void OnDisable()
    {
        dayUpBtn.onClick.RemoveAllListeners();
        dayDownBtn.onClick.RemoveAllListeners();
        monthUpBtn.onClick.RemoveAllListeners();
        monthDownBtn.onClick.RemoveAllListeners();
        yearUpBtn.onClick.RemoveAllListeners();
        yearDownBtn.onClick.RemoveAllListeners();
    }
    private void AdjustDate(TextMeshProUGUI text,string digitFormat, int adjustment, int min, int max)
    {
        int timeValue = int.Parse(text.text);
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
        text.text = timeValue.ToString(digitFormat);
    }
    public void SetDate(DateTime dateTime)
    {
        //Set the currentRaceCheckins date to the text fields
        dayText.text = dateTime.Day.ToString("D2");
        monthText.text = dateTime.Month.ToString("D2");
        yearText.text = dateTime.Year.ToString("D4");
    }
    public string ReturnDate()
    {
        return $"{dayText.text}-{monthText.text}-{yearText.text}";
    }
}
