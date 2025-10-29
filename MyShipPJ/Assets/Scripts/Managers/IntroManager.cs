using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public Image fadeOutImage;
    public CanvasGroup fadeOutCanvasGroup;
    public float fadeOutDuration = 1.0f;

    void Awake() // Start() 대신 Awake()에서 이벤트 등록 시도
    {
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
    void OnVideoFinished(VideoPlayer vp)
    {
        StartCoroutine(FadeOutAndLoadScene());
    }

    IEnumerator FadeOutAndLoadScene()
    {

        float timer = 0f;
        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            // 0에서 1까지 알파 값을 보간 (점점 불투명해짐)
            fadeOutCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeOutDuration);
            yield return null; // 다음 프레임까지 대기
        }
        fadeOutCanvasGroup.alpha = 1; // 확실하게 불투명하게 설정 (화면이 완전히 검게 됨)

        // 페이드 아웃 완료 후 다음 씬 로드
        PlayerPrefs.SetInt("Intro", 0); // PlayerPrefs 설정 유지
        SceneManager.LoadScene("MainScene"); // 지정된 다음 씬으로 로드
    }
}