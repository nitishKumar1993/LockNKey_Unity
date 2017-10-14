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

    [SerializeField]
    private GameObject m_deathScreenGO;

    [SerializeField]
    private Text m_deathTimerText;

    [SerializeField]
    private GameObject m_skillCDGO;

    [SerializeField]
    private List<GameObject> m_runnersFrozenSlotsList;

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

    public void ShowDeathScreen(bool action)
    {
        m_deathScreenGO.SetActive(action);
    }

    public void UpdateDeathScreenText(int secs)
    {
        m_deathTimerText.text = "00:" + (secs > 9 ? secs.ToString() : "0" + secs.ToString());
    }

    public void ShowSkillCD(bool action)
    {
        m_skillCDGO.SetActive(action);
    }

    public void UpdateSkillCD(float amount)
    {
        m_skillCDGO.GetComponent<Image>().fillAmount = amount;
    }

    public GameObject GetUIRunnerFronzenSlot(string name)
    {
        GameObject tempGO = m_runnersFrozenSlotsList[0];
        tempGO.transform.Find("Text").GetComponent<Text>().text = name;
        tempGO.transform.Find("Image").GetComponent<Image>().color = Color.white;
        m_runnersFrozenSlotsList.RemoveAt(0);
        return tempGO;
    }

    public void ShowRunnerFrozenStatus(GameObject playerSlotGO,bool freeze)
    {
        playerSlotGO.transform.Find("Image").GetComponent<Image>().color = freeze ? (GameManager.Instance.CurrentPlayer.PlayerHeroData.m_heroType == HeroType.Chaser ? Color.green : Color.red): Color.white;
    }

    string FormateAmountToDisplay(int amount)
    {
        return amount < 10 ? "0" + amount.ToString() : amount.ToString();
    }
}
