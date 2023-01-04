using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI gasText;
    public TextMeshProUGUI steerText;
    public TextMeshProUGUI brakeText;
    public TextMeshProUGUI rewardsText;
    public TextMeshProUGUI episodeCount;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUI(float gas, float steer, int brake,float rewards)
    {
        gasText.text = gas.ToString("F2");
        steerText.text = steer.ToString("F2");
        brakeText.text = brake.ToString();
        rewardsText.text = rewards.ToString("F2");


    }
    public void UpdateEpisodeCount(int count)
    {
        episodeCount.text = count.ToString();
    }
}
