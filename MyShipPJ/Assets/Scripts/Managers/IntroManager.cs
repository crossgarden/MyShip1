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

    void Awake() // Start() ��� Awake()���� �̺�Ʈ ��� �õ�
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
            // 0���� 1���� ���� ���� ���� (���� ����������)
            fadeOutCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeOutDuration);
            yield return null; // ���� �����ӱ��� ���
        }
        fadeOutCanvasGroup.alpha = 1; // Ȯ���ϰ� �������ϰ� ���� (ȭ���� ������ �˰� ��)

        // ���̵� �ƿ� �Ϸ� �� ���� �� �ε�
        PlayerPrefs.SetInt("Intro", 0); // PlayerPrefs ���� ����
        SceneManager.LoadScene("MainScene"); // ������ ���� ������ �ε�
    }
}