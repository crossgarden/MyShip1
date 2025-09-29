using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;  // �߰�

public class EyeTracking : MonoBehaviour
{
    [Header("Eye Components")]
    public SpriteRenderer bodySpriteRenderer;
    public GameObject eyelessBody;
    public GameObject eyes;

    [Header("Eye Movement Settings")]
    public float eyeMovementRange = 0.2f;
    public float followSpeed = 10f;

    private Vector3 eyesDefaultPos;
    private bool isTracking = false;
    private Animator animator;

    void Start()
    {
        eyesDefaultPos = eyes.transform.localPosition;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // MainScene üũ �߰�
        if (SceneManager.GetActiveScene().name != "MainScene")
            return;

        // Sleep ���� Ȯ��
        bool isSleeping = animator != null && animator.GetBool("IsSleeping");

        // ��ġ ���� (Sleep ���� �ƴ� ����)
        if (Input.GetMouseButtonDown(0) && !isSleeping)
        {
            StartEyeTracking();
        }

        // ��ġ ��
        if (Input.GetMouseButton(0) && isTracking)
        {
            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchWorldPos.z = 0;
            UpdateEyeDirection(touchWorldPos);
        }

        // ��ġ ��
        if (Input.GetMouseButtonUp(0))
        {
            StopEyeTracking();
        }
    }

    void StartEyeTracking()
    {
        isTracking = true;
        bodySpriteRenderer.enabled = false;
        eyelessBody.SetActive(true);
        eyes.SetActive(true);
    }

    void StopEyeTracking()
    {
        isTracking = false;
        StartCoroutine(ReturnToDefault());
    }

    void UpdateEyeDirection(Vector3 targetPos)
    {
        Vector3 faceCenter = transform.position;
        Vector3 direction = (targetPos - faceCenter).normalized;
        Vector3 eyeOffset = direction * eyeMovementRange;
        Vector3 newPos = eyesDefaultPos + eyeOffset;

        eyes.transform.localPosition = Vector3.Lerp(
            eyes.transform.localPosition,
            newPos,
            followSpeed * Time.deltaTime
        );
    }

    IEnumerator ReturnToDefault()
    {
        while (Vector3.Distance(eyes.transform.localPosition, eyesDefaultPos) > 0.01f)
        {
            eyes.transform.localPosition = Vector3.Lerp(
                eyes.transform.localPosition,
                eyesDefaultPos,
                followSpeed * Time.deltaTime
            );
            yield return null;
        }

        eyes.transform.localPosition = eyesDefaultPos;
        eyes.SetActive(false);
        eyelessBody.SetActive(false);
        bodySpriteRenderer.enabled = true;
    }
}