using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance;

    [SerializeField]
    private Text m_timerText;

    private void Start()
    {
        Instance = this;
    }

    public void OnQuitBtnCicked()
    {
        if (NDiscovery.Instance.running)
            NDiscovery.Instance.StopBroadcasting();

        NetworkManager.singleton.StopHost();
    }

    public void UpdateTimerUI(int secs)
    {
        if(m_timerText)
        {
            int currentMins = (int)(secs / 60);
            int currentSecs = secs - currentMins * 60;
            m_timerText.text = string.Format("{0}:{1}", FormateAmountToDisplay(currentMins), FormateAmountToDisplay(currentSecs));
        }
    }

    string FormateAmountToDisplay(int amount)
    {
        return amount < 10 ? "0" + amount.ToString() : amount.ToString();
    }
}
