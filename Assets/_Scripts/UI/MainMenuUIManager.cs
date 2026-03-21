using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("전체 화면 패널 (Full Screen Panels)")]
    public GameObject titlePanel;           // 게임 시작 화면 (타이틀)
    public GameObject mainMenuPanel;        // 메인 메뉴 화면
    public GameObject characterSelectPanel; // 캐릭터(체스세트) 선택 화면
    public GameObject glossaryPanel;        // 도감 화면

    [Header("팝업 화면 패널 (Popup Panels)")]
    public GameObject continuePopup;        // 계속하기 팝업 (진행 상황 + 시작 버튼)
    public GameObject optionsPopup;         // 옵션 팝업 (게임, 비디오, 그래픽, 오디오)

    private void Start()
    {
        // 게임을 켜면 가장 먼저 타이틀 화면만 보이게 초기화합니다.
        ShowPanel(titlePanel);
        
        // 팝업들은 모두 꺼둡니다.
        continuePopup.SetActive(false);
        optionsPopup.SetActive(false);
    }

    // ==========================================
    // 1. 타이틀 화면 버튼 이벤트
    // ==========================================
    public void OnTitleStartButtonClicked()
    {
        // 타이틀에서 '게임 시작'을 누르면 메인 메뉴로 넘어갑니다.
        ShowPanel(mainMenuPanel);
    }

    // ==========================================
    // 2. 메인 메뉴 버튼 이벤트 (좌측 4개, 우측 1개)
    // ==========================================
    public void OnNewGameClicked()
    {
        // 새 게임: 캐릭터 선택창으로 화면 전환
        ShowPanel(characterSelectPanel);
    }

    public void OnContinueClicked()
    {
        // 계속하기: 메인 메뉴를 유지한 채 위에 팝업을 띄움
        continuePopup.SetActive(true);
    }

    public void OnOptionsClicked()
    {
        // 옵션: 메인 메뉴 위에 설정 팝업을 띄움
        optionsPopup.SetActive(true);
    }

    public void OnGlossaryClicked()
    {
        // 도감: 도감 화면으로 전환
        ShowPanel(glossaryPanel);
    }

    public void OnQuitClicked()
    {
        // 종료: 게임 끄기
        Debug.Log("게임을 종료합니다.");
        Application.Quit();
    }

    // ==========================================
    // 3. 팝업 및 뒤로가기 버튼 이벤트
    // ==========================================
    public void CloseContinuePopup()
    {
        continuePopup.SetActive(false);
    }

    public void CloseOptionsPopup()
    {
        optionsPopup.SetActive(false);
    }

    public void BackToMainMenu()
    {
        // 도감이나 캐릭터 선택창에서 다시 메인 메뉴로 돌아갈 때 사용
        ShowPanel(mainMenuPanel);
    }

    // ==========================================
    // 유틸리티 함수: 원하는 패널만 켜고 나머지는 끄기
    // ==========================================
    private void ShowPanel(GameObject panelToShow)
    {
        titlePanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        characterSelectPanel.SetActive(false);
        glossaryPanel.SetActive(false);

        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
        }
    }
}