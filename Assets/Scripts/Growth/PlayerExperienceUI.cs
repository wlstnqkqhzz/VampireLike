using UnityEngine;

namespace VampireLike.Growth
{
    /// <summary>
    /// 화면 최상단에 뱀서라이크 스타일의 경험치 게이지와 현재 레벨을 표시한다.
    /// </summary>
    public class PlayerExperienceUI : MonoBehaviour
    {
        // 표시할 경험치 데이터다. 비어 있으면 같은 Player 오브젝트에서 찾는다.
        [SerializeField]
        private PlayerExperience playerExperience;

        // 화면 위쪽에서 얼마나 내려올지 정한다.
        [SerializeField]
        private float topMargin = 8f;

        // 화면 좌우 여백이다.
        [SerializeField]
        private float sideMargin = 72f;

        // 경험치 바 높이다.
        [SerializeField]
        private float barHeight = 18f;

        // HUD 표시 여부다. 테스트 중 Inspector에서 끌 수 있다.
        [SerializeField]
        private bool drawHud = true;

        private Texture2D whiteTexture;
        private GUIStyle levelStyle;

        private void Awake()
        {
            if (playerExperience == null)
                playerExperience = GetComponent<PlayerExperience>();

            EnsureTexture();
        }

        private void OnDestroy()
        {
            if (whiteTexture != null)
                Destroy(whiteTexture);
        }

        private void OnValidate()
        {
            topMargin = Mathf.Max(0f, topMargin);
            sideMargin = Mathf.Max(0f, sideMargin);
            barHeight = Mathf.Max(8f, barHeight);
        }

        private void OnGUI()
        {
            // 현재 프로젝트에서는 간단한 학습용 HUD라 OnGUI로 고정 화면 UI를 그린다.
            if (!drawHud)
                return;

            if (playerExperience == null)
                playerExperience = GetComponent<PlayerExperience>();

            if (playerExperience == null)
                return;

            EnsureTexture();
            EnsureStyles();

            GUI.depth = -1000;
            DrawHud();
        }

        private void DrawHud()
        {
            // 현재 Game 뷰 해상도에 맞춰 상단 경험치 바의 폭을 계산한다.
            const float levelWidth = 92f;
            const float levelGap = 8f;
            float barWidth = Mathf.Max(240f, Screen.width - sideMargin * 2f - levelWidth - levelGap);

            Rect borderRect = new Rect(sideMargin, topMargin, barWidth, barHeight + 6f);
            Rect backgroundRect = new Rect(borderRect.x + 3f, borderRect.y + 3f, borderRect.width - 6f, borderRect.height - 6f);
            Rect fillRect = new Rect(backgroundRect.x, backgroundRect.y, backgroundRect.width * playerExperience.ExperienceProgress, backgroundRect.height);
            Rect levelRect = new Rect(borderRect.xMax + levelGap, borderRect.y - 1f, levelWidth, borderRect.height + 2f);

            Color previousColor = GUI.color;

            GUI.color = new Color(0.03f, 0.025f, 0.02f, 0.78f);
            GUI.DrawTexture(borderRect, whiteTexture);
            GUI.DrawTexture(levelRect, whiteTexture);

            GUI.color = new Color(0.01f, 0.012f, 0.012f, 0.78f);
            GUI.DrawTexture(backgroundRect, whiteTexture);

            GUI.color = new Color(0.16f, 0.45f, 0.95f, 0.96f);
            GUI.DrawTexture(fillRect, whiteTexture);

            GUI.color = Color.white;
            GUI.Label(levelRect, $"레벨 {playerExperience.CurrentLevel}", levelStyle);

            GUI.color = previousColor;
        }

        private void EnsureTexture()
        {
            // GUI.DrawTexture에 사용할 1x1 흰색 텍스처를 만들어 색상만 바꿔 재사용한다.
            if (whiteTexture != null)
                return;

            whiteTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            whiteTexture.SetPixel(0, 0, Color.white);
            whiteTexture.Apply();
        }

        private void EnsureStyles()
        {
            // 레벨 배지 텍스트 스타일을 한 번만 만든다.
            if (levelStyle != null)
                return;

            levelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16,
                fontStyle = FontStyle.Bold
            };
            levelStyle.normal.textColor = Color.white;
        }
    }
}
