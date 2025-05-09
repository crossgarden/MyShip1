using UnityEngine;

public class CameraBorderBuilder : MonoBehaviour
{
    public float wallThickness = 1f;  // 벽의 두께

    void Start()
    {
        Camera cam = Camera.main;

        // 카메라의 세로 크기 계산
        float camHeight = cam.orthographicSize * 2f;

        // 카메라의 가로 크기 계산
        float camWidth = camHeight * cam.aspect;

        // 카메라의 z 위치 (z 값은 벽이 카메라와 일치하도록 설정)
        float camZ = cam.transform.position.z;

        // 벽 생성
        CreateWall("Wall_Top", new Vector2(0, camHeight / 2f + wallThickness / 2f), new Vector2(camWidth, wallThickness), camZ);
        CreateWall("Wall_Bottom", new Vector2(0, -camHeight / 2f - wallThickness / 2f), new Vector2(camWidth, wallThickness), camZ);
        CreateWall("Wall_Left", new Vector2(-camWidth / 2f - wallThickness / 2f, 0), new Vector2(wallThickness, camHeight), camZ);
        CreateWall("Wall_Right", new Vector2(camWidth / 2f + wallThickness / 2f, 0), new Vector2(wallThickness, camHeight), camZ);
    }

    void CreateWall(string name, Vector2 position, Vector2 size, float zPos)
    {
        // 벽 생성
        GameObject wall = new GameObject(name);
        wall.transform.position = new Vector3(position.x, position.y, zPos);  // 벽 위치 설정

        // SpriteRenderer 추가
        SpriteRenderer sr = wall.AddComponent<SpriteRenderer>();
        sr.sprite = GenerateGraySprite();  // 회색 스프라이트 생성
        sr.color = Color.gray;  // 회색으로 색상 설정
        sr.sortingOrder = 0;  // 정렬 순서 설정

        wall.transform.localScale = size;  // 벽 크기 설정
        wall.AddComponent<BoxCollider2D>();  // 충돌 영역 추가
        wall.transform.parent = this.transform;  // 부모 객체로 지정
    }

    // 회색 스프라이트 생성 함수
    Sprite GenerateGraySprite()
    {
        // 1x1 픽셀의 회색 텍스처 생성
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.gray);  // 회색 색 설정
        tex.Apply();

        // 텍스처를 스프라이트로 변환
        Rect rect = new Rect(0, 0, 1, 1);
        return Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f), 1f);
    }
}
