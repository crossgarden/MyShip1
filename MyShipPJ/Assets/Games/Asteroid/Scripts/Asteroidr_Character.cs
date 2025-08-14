using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Asteroid_Character : MonoBehaviour
{
    Rigidbody2D rigid;
    [SerializeField] float moveSpeed = 5f; // 캐릭터 이동 속도

    private float minX, maxX, minY, maxY; // 화면 경계 좌표
    private float characterHalfWidth, characterHalfHeight; // 캐릭터 크기의 반
    private int characterDirection = -1; // 캐릭터 방향 (-1: 오른쪽, 1: 왼쪽)

    // 조이스틱 관련 변수
    [SerializeField] private GameObject joystickPrefab; // 조이스틱 프리팹
    private GameObject currentJoystick; // 현재 조이스틱 객체
    private Image joystickBackground; // 조이스틱 배경 이미지
    private Image joystickHandle; // 조이스틱 핸들 이미지
    private Vector2 joystickTouchStartPosition; // 조이스틱 터치 시작 위치
    private bool isJoystickActive = false; // 조이스틱 활성화 상태
    private const float joystickRadius = 100f; // 조이스틱 반경
    private const float handleMoveRadius = 50f; // 핸들 이동 반경

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        CalculateScreenBounds(); // 화면 경계 계산
    }

    void Update()
    {
        HandleJoystickInput(); // 조이스틱 입력 처리
        ClampPosition(); // 화면 경계 내로 위치 제한
    }

    void ClampPosition()
    {
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        transform.position = clampedPosition;
    }

    void CalculateScreenBounds()
    {
        // 캐릭터 콜라이더 크기 계산
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            characterHalfWidth = collider.bounds.extents.x;
            characterHalfHeight = collider.bounds.extents.y;
        }
        else
        {
            characterHalfWidth = 0.5f;
            characterHalfHeight = 0.5f;
        }

        // 카메라 뷰포트 계산
        float cameraHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float cameraHalfHeight = Camera.main.orthographicSize;

        // 화면 경계 계산 (캐릭터가 화면 밖으로 나가지 않도록)
        minX = Camera.main.transform.position.x - cameraHalfWidth + characterHalfWidth;
        maxX = Camera.main.transform.position.x + cameraHalfWidth - characterHalfWidth;
        minY = Camera.main.transform.position.y - cameraHalfHeight + characterHalfHeight;
        maxY = Camera.main.transform.position.y + cameraHalfHeight - characterHalfHeight;
    }

    void HandleJoystickInput()
    {
        // 게임이 일시정지 상태면 조이스틱 입력 무시
        if (Time.timeScale == 0)
            return;

        // 터치 입력 처리 (모바일)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // 터치 시작
            if (touch.phase == TouchPhase.Began && !IsPointerOverUI(touch.position))
            {
                CreateJoystick(touch.position);
                isJoystickActive = true;
                joystickTouchStartPosition = touch.position;
            }
            // 터치 이동
            else if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isJoystickActive)
            {
                Vector2 currentTouchPosition = touch.position;
                Vector2 direction = currentTouchPosition - joystickTouchStartPosition;
                float distance = direction.magnitude;

                // 조이스틱 핸들 이동
                Vector2 handlePosition = direction.normalized * Mathf.Min(distance, handleMoveRadius);
                if (joystickHandle != null)
                    joystickHandle.rectTransform.anchoredPosition = handlePosition;

                // 캐릭터 이동
                Vector2 movement = direction.normalized * moveSpeed;
                rigid.velocity = movement;

                // 캐릭터 방향 전환
                if (movement.x > 0 && characterDirection != -1)
                {
                    characterDirection = -1;
                    FlipCharacter();
                }
                else if (movement.x < 0 && characterDirection != 1)
                {
                    characterDirection = 1;
                    FlipCharacter();
                }
            }
            // 터치 종료
            else if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && isJoystickActive)
            {
                DestroyJoystick();
                rigid.velocity = Vector2.zero;
                isJoystickActive = false;
            }
        }
        // 마우스 입력 처리 (에디터/시뮬레이터)
        else
        {
            // 마우스 클릭 시작 (조이스틱 생성)
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUI(Input.mousePosition))
            {
                CreateJoystick(Input.mousePosition);
                isJoystickActive = true;
                joystickTouchStartPosition = Input.mousePosition;
            }
            // 마우스 드래그 (조이스틱 조작)
            else if (Input.GetMouseButton(0) && isJoystickActive)
            {
                Vector2 currentTouchPosition = Input.mousePosition;
                Vector2 direction = currentTouchPosition - joystickTouchStartPosition;
                float distance = direction.magnitude;

                // 조이스틱 핸들 이동
                Vector2 handlePosition = direction.normalized * Mathf.Min(distance, handleMoveRadius);
                if (joystickHandle != null)
                    joystickHandle.rectTransform.anchoredPosition = handlePosition;

                // 캐릭터 이동
                Vector2 movement = direction.normalized * moveSpeed;
                rigid.velocity = movement;

                // 캐릭터 방향 전환
                if (movement.x > 0 && characterDirection != -1)
                {
                    characterDirection = -1;
                    FlipCharacter();
                }
                else if (movement.x < 0 && characterDirection != 1)
                {
                    characterDirection = 1;
                    FlipCharacter();
                }
            }
            // 마우스 클릭 종료 (조이스틱 제거)
            else if (Input.GetMouseButtonUp(0) && isJoystickActive)
            {
                DestroyJoystick();
                rigid.velocity = Vector2.zero;
                isJoystickActive = false;
            }
        }
    }

    // UI 위에 포인터가 있는지 확인하는 헬퍼 메서드
    private bool IsPointerOverUI(Vector2 screenPosition)
    {
        // EventSystem이 없으면 false 반환
        if (EventSystem.current == null)
            return false;

        // 터치 입력용 포인터 데이터 생성
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = screenPosition;

        // 레이캐스트 결과 리스트
        List<RaycastResult> results = new List<RaycastResult>();

        // UI 레이캐스트 수행
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }

    void CreateJoystick(Vector2 screenPosition)
    {
        if (joystickPrefab != null)
        {
            // 캔버스 참조 가져오기
            RectTransform canvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();

            // 스크린 좌표를 캔버스 로컬 좌표로 변환
            Vector2 localPointer;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, Camera.main, out localPointer))
            {
                currentJoystick = Instantiate(joystickPrefab, canvasRect); // 부모를 먼저 설정하여 스케일 문제 방지
                currentJoystick.GetComponent<RectTransform>().anchoredPosition = localPointer; // 캔버스 로컬 좌표 설정

                // 조이스틱 컴포넌트 가져오기
                joystickBackground = currentJoystick.GetComponent<Image>();
                joystickHandle = currentJoystick.transform.GetChild(0).GetComponent<Image>();

                // 초기 위치 설정
                joystickHandle.rectTransform.anchoredPosition = Vector2.zero;
            }
        }
    }

    void DestroyJoystick()
    {
        if (currentJoystick != null)
        {
            Destroy(currentJoystick);
            currentJoystick = null;
            joystickBackground = null;
            joystickHandle = null;
        }
    }

    void FlipCharacter()
    {
        Vector3 newScale = transform.localScale;
        newScale.x = Mathf.Abs(newScale.x) * characterDirection;
        transform.localScale = newScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Obstacle")
        {
            Asteroid_Manager.instance.GameOver(isOver: true);
        }

        if (other.tag == "Coin")
        {
            AudioManager.instance.PlaySFX(AudioManager.SFXClip.SUCCESS);
            Destroy(other.gameObject);
            Asteroid_Manager.instance.GetCoin();
        }
    }

    // 게임오버 시 조이스틱 강제 제거용 public 메서드
    public void ForceDestroyJoystick()
    {
        if (isJoystickActive)
        {
            DestroyJoystick();
            rigid.velocity = Vector2.zero;
            isJoystickActive = false;
        }
    }
}