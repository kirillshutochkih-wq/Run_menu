using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("СКРИПТЫ")]
    public SettingsMenuCreator settingsCreator; // Ссылка на скрипт настроек

    [Header("КНОПКИ (перетащите сюда из Hierarchy)")]
    public Button startButton;
    public Button continueButton;
    public Button settingsButton;
    public Button exitButton;

    [Header("НАСТРОЙКИ МЕНЮ")]
    public string gameSceneName = "GameScene";
    public bool enableContinueButton = true;

    void Start()
    {
        SetupButtons();
        StyleMenu();
    }

    void SetupButtons()
    {
        // Проверяем кнопки
        if (startButton == null)
            Debug.LogError("❌ StartButton не назначен! Перетащите из Hierarchy");

        // Назначаем методы на кнопки
        if (startButton != null)
            startButton.onClick.AddListener(StartNewGame);

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ContinueGame);
            // Показываем/скрываем кнопку "Продолжить"
            continueButton.gameObject.SetActive(enableContinueButton && HasSaveGame());
        }

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettingsMenu);

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
    }

    void StyleMenu()
    {
        // Автоматически настраиваем внешний вид кнопок
        Button[] allButtons = GetComponentsInChildren<Button>();

        foreach (Button btn in allButtons)
        {
            SetupButtonStyle(btn);
        }
    }

    void SetupButtonStyle(Button button)
    {
        // Настраиваем цвета кнопок
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        colors.pressedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        colors.selectedColor = new Color(0.25f, 0.25f, 0.25f, 1f);
        colors.disabledColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        button.colors = colors;

        // Добавляем эффекты
        AddButtonEffects(button.gameObject);
    }

    void AddButtonEffects(GameObject buttonObj)
    {
        // Добавляем тень для объема
        Shadow shadow = buttonObj.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.5f);
        shadow.effectDistance = new Vector2(2, -2);

        // Добавляем анимацию при наведении
        ButtonHoverEffect hoverEffect = buttonObj.AddComponent<ButtonHoverEffect>();
    }

    // === МЕТОДЫ ДЛЯ КНОПОК ===

    void StartNewGame()
    {
        Debug.Log("🎮 Запуск новой игры...");

        // Здесь можно добавить анимацию или подтверждение
        ShowTransitionEffect(() => {
            SceneManager.LoadScene(gameSceneName);
        });
    }

    void ContinueGame()
    {
        Debug.Log("🔄 Продолжение сохраненной игры...");

        // Загружаем последнее сохранение
        string saveData = PlayerPrefs.GetString("GameSave", "");

        if (!string.IsNullOrEmpty(saveData))
        {
            // Здесь парсим сохранение и загружаем игру
            ShowTransitionEffect(() => {
                SceneManager.LoadScene(gameSceneName);
                // Дополнительно: LoadGameState(saveData);
            });
        }
        else
        {
            Debug.LogWarning("⚠️ Сохранений не найдено!");
            // Можно показать сообщение игроку
        }
    }

    void OpenSettingsMenu()
    {
        Debug.Log("⚙️ Открытие меню настроек...");

        // Вариант 1: Если используем SettingsMenuCreator
        if (settingsCreator != null)
        {
            settingsCreator.OpenSettings();
        }
        // Вариант 2: Создаем меню настроек на лету
        else
        {
            CreateSimpleSettingsMenu();
        }
    }

    void ExitGame()
    {
        Debug.Log("🚪 Выход из игры...");

        // Показываем подтверждение
        ShowExitConfirmation();
    }

    // === ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ===

    bool HasSaveGame()
    {
        // Проверяем наличие сохранений
        return PlayerPrefs.HasKey("GameSave") ||
               PlayerPrefs.HasKey("LastLevel") ||
               System.IO.File.Exists(Application.persistentDataPath + "/savegame.dat");
    }

    void ShowTransitionEffect(System.Action onComplete)
    {
        // Здесь можно добавить анимацию перехода
        // Например: fade out, эффект и т.д.

        // Пока просто вызываем коллбэк
        if (onComplete != null)
            onComplete.Invoke();
    }

    void ShowExitConfirmation()
    {
        // Создаем простое окно подтверждения
        GameObject confirmPanel = CreateConfirmationPanel(
            "Выйти из игры?",
            "Вы действительно хотите выйти?",
            () => {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
            },
            () => {
                Debug.Log("Выход отменен");
            }
        );
    }

    void CreateSimpleSettingsMenu()
    {
        // Быстрое создание простого меню настроек
        GameObject settingsPanel = new GameObject("SimpleSettingsPanel");
        settingsPanel.transform.SetParent(transform);
        settingsPanel.transform.SetAsLastSibling();

        // Фон
        Image bg = settingsPanel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.85f);

        // Растягиваем
        RectTransform rt = settingsPanel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // Окно настроек
        GameObject window = CreateSettingsWindow(settingsPanel.transform);

        // Кнопка закрытия
        CreateCloseButton(window.transform, settingsPanel);

        Debug.Log("✅ Создано простое меню настроек");
    }

    GameObject CreateSettingsWindow(Transform parent)
    {
        GameObject window = new GameObject("SettingsWindow");
        window.transform.SetParent(parent);

        Image img = window.AddComponent<Image>();
        img.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);

        RectTransform rt = window.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(500, 400);
        rt.anchoredPosition = Vector2.zero;

        // Заголовок
        GameObject title = new GameObject("Title");
        title.transform.SetParent(window.transform);

        TextMeshProUGUI titleText = title.AddComponent<TextMeshProUGUI>();
        titleText.text = "НАСТРОЙКИ";
        titleText.fontSize = 32;
        titleText.color = Color.yellow;
        titleText.alignment = TextAlignmentOptions.Center;

        RectTransform titleRt = title.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0.5f, 0.5f);
        titleRt.anchorMax = new Vector2(0.5f, 0.5f);
        titleRt.sizeDelta = new Vector2(300, 50);
        titleRt.anchoredPosition = new Vector2(0, 150);

        // Сообщение
        GameObject message = new GameObject("Message");
        message.transform.SetParent(window.transform);

        TextMeshProUGUI messageText = message.AddComponent<TextMeshProUGUI>();
        messageText.text = "Полное меню настроек\nбудет добавлено позже";
        messageText.fontSize = 20;
        messageText.color = Color.white;
        messageText.alignment = TextAlignmentOptions.Center;

        RectTransform msgRt = message.GetComponent<RectTransform>();
        msgRt.anchorMin = new Vector2(0.5f, 0.5f);
        msgRt.anchorMax = new Vector2(0.5f, 0.5f);
        msgRt.sizeDelta = new Vector2(400, 100);
        msgRt.anchoredPosition = Vector2.zero;

        return window;
    }

    void CreateCloseButton(Transform parent, GameObject panelToClose)
    {
        GameObject closeBtn = new GameObject("CloseButton");
        closeBtn.transform.SetParent(parent);

        Button btn = closeBtn.AddComponent<Button>();
        btn.onClick.AddListener(() => {
            Destroy(panelToClose);
            Debug.Log("Настройки закрыты");
        });

        Image img = closeBtn.AddComponent<Image>();
        img.color = new Color(0.3f, 0.1f, 0.1f, 1f);

        // Текст
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(closeBtn.transform);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "ЗАКРЫТЬ";
        text.fontSize = 18;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;

        // Растягиваем текст
        RectTransform textRt = textObj.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        // Позиция кнопки
        RectTransform btnRt = closeBtn.GetComponent<RectTransform>();
        btnRt.anchorMin = new Vector2(0.5f, 0.5f);
        btnRt.anchorMax = new Vector2(0.5f, 0.5f);
        btnRt.sizeDelta = new Vector2(150, 40);
        btnRt.anchoredPosition = new Vector2(0, -150);
    }

    GameObject CreateConfirmationPanel(string title, string message, System.Action onConfirm, System.Action onCancel)
    {
        GameObject panel = new GameObject("ConfirmationPanel");
        panel.transform.SetParent(transform);
        panel.transform.SetAsLastSibling();

        // Фон
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.9f);

        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // Создаем окно подтверждения
        // ... (можно добавить реализацию)

        return panel;
    }
}

// Дополнительный скрипт для эффектов кнопок
public class ButtonHoverEffect : MonoBehaviour
{
    private Vector3 originalScale;
    private Button button;

    void Start()
    {
        originalScale = transform.localScale;
        button = GetComponent<Button>();

        if (button != null)
        {
            // Добавляем события
            // (В реальном проекте лучше использовать EventTrigger)
        }
    }

    void Update()
    {
        // Простой эффект при наведении
        if (button != null && button.interactable)
        {
            // Можно добавить пульсацию или другие эффекты
        }
    }
}