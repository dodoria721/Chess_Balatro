using UnityEngine;

public class OptionsTabManager : MonoBehaviour
{
    [Header("옵션 탭 패널들 (Tab Contents)")]
    public GameObject gameTabPanel;         // 게임 설정 화면
    public GameObject videoTabPanel;        // 비디오 설정 화면
    public GameObject graphicsTabPanel;     // 그래픽 설정 화면
    public GameObject audioTabPanel;        // 오디오 설정 화면

    private void Start()
    {
        ShowTab(gameTabPanel);
    }

    public void OnGameTabClicked()
    {
        ShowTab(gameTabPanel);
    }

    public void OnVideoTabClicked()
    {
        ShowTab(videoTabPanel);
    }

    public void OnGraphicsTabClicked()
    {
        ShowTab(graphicsTabPanel);
    }

    public void OnAudioTabClicked()
    {
        ShowTab(audioTabPanel);
    }

    private void ShowTab(GameObject tabToShow)
    {
        gameTabPanel.SetActive(false);
        videoTabPanel.SetActive(false);
        graphicsTabPanel.SetActive(false);
        audioTabPanel.SetActive(false);

        if (tabToShow != null)
        {
            tabToShow.SetActive(true);
        }
    }
}
