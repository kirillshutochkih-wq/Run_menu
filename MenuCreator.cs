using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;

public class CompleteMenuCreator : MonoBehaviour
{
    [Header("НАСТРОЙКИ ИГРЫ")]
    public string gameSceneName = "GameScene";
    public bool showContinueButton = true;

    [Header("ЦВЕТА И СТИЛЬ")]
    public Color buttonColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
    public Color buttonHoverColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    public Color buttonTextColor = Color.white;
    public Color titleColor = Color.yellow;

    [Header("АУДИО")]
    public AudioMixer audioMixer; // Опционально

    // Приватные переменные
    private GameObject settingsPanel;
    private GameObject pausePanel;

    void Start()
    {
        CreateCompleteMenu();
    }

    void CreateCompleteMenu()
    {
        Debug.Log("🎮 Создаю главное меню...");

        // 1. Создаём тёмный фон для лучшей читаемости
        CreateDarkBackground();

        // 2. Создаём заголовок
        CreateTitle("МОЯ ИГРА", new Vector2(0, 250));

        // 3. Создаём кнопки главного меню
        CreateMenuButtons();

        Debug.Log("✅ Главное меню создано!");
    }

    void CreateDarkBackground()
    {
        GameObject darkBg = new GameObject("DarkBackground");
        darkBg.transform.SetParent(transform);
        darkBg.transform.SetAsFirstSibling();

        Image bgImage = darkBg.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.4f);

        // Растягиваем на весь экран
        RectTransform rect = darkBg.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    void CreateTitle(string text, Vector2 position)
    {
        GameObject title = new GameObject("GameTitle");
        title.transform.SetParent(transform);

        TextMeshProUGUI titleText = title.AddComponent<TextMeshProUGUI>();
        titleText.text = text;
        titleText.fontSize = 64;
        titleText.color = titleColor;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontStyle = FontStyles.Bold;

        // Добавляем эффекты
        Shadow shadow = title.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.7f);
        shadow.effectDistance = new Vector2(3, -3);

        // Позиционируем
        RectTransform rect = title.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(800, 100);
        rect.anchoredPosition = position;
    }

    void CreateMenuButtons()
    {
        // Создаём контейнер для кнопок
        GameObject buttonContainer = new GameObject("ButtonContainer");
        buttonContainer.transform.SetParent(transform);

        RectTransform containerRect = buttonContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(400, 400);
        containerRect.anchoredPosition = new Vector2(0, 0);

        // 1. Кнопка "НОВАЯ ИГРА"
        CreateButton(buttonContainer.transform, "НОВАЯ ИГРА", new Vector2(0, 100), () => {
            Debug.Log("🎮 Начало новой игры!");
            SceneManager.LoadScene(gameSceneName);
        });

        // 2. Кнопка "ПРОДОЛЖИТЬ" (опционально)
        if (showContinueButton)
        {
            CreateButton(buttonContainer.transform, "ПРОДОЛЖИТЬ", new Vector2(0, 20), () => {
                Debug.Log("🔄 Продолжение игры...");
                if (PlayerPrefs.HasKey("GameSaved"))
                {
                    SceneManager.LoadScene(PlayerPrefs.GetString("LastScene", gameSceneName));
                }
                else
                {
                    Debug.Log("⚠️ Сохранений не найдено!");
                    // Можно показать сообщение игроку
                }
            });
        }

        // 3. Кнопка "НАСТРОЙКИ"
        CreateButton(buttonContainer.transform, "НАСТРОЙКИ", new Vector2(0, -60), () => {
            Debug.Log("⚙️ Открытие настроек...");
            OpenSettingsMenu();
        });

        // 4. Кнопка "ВЫХОД"
        CreateButton(buttonContainer.transform, "ВЫХОД", new Vector2(0, -140), () => {
            Debug.Log("🚪 Выход из игры...");
            ShowExitConfirmation();
        });
    }

    GameObject CreateButton(Transform parent, string text, Vector2 position, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObj = new GameObject(text + "Button");
        buttonObj.transform.SetParent(parent);

        // 1. Добавляем компоненты
        Image image = buttonObj.AddComponent<Image>();
        Button button = buttonObj.AddComponent<Button>();

        // 2. Настраиваем цвета кнопки
        ColorBlock colors = button.colors;
        colors.normalColor = buttonColor;
        colors.highlightedColor = buttonHoverColor;
        colors.pressedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        colors.disabledColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        colors.colorMultiplier = 1f;
        button.colors = colors;

        // 3. Добавляем тень для объёма
        Shadow shadow = buttonObj.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.5f);
        shadow.effectDistance = new Vector2(2, -2);

        // 4. Создаём текст кнопки
        GameObject textObj = new GameObject("ButtonText");
        textObj.transform.SetParent(buttonObj.transform);

        TextMeshProUGUI textComp = textObj.AddComponent<TextMeshProUGUI>();
        textComp.text = text;
        textComp.fontSize = 28;
        textComp.color = buttonTextColor;
        textComp.alignment = TextAlignmentOptions.Center;
        textComp.fontStyle = FontStyles.Bold;

        // 5. Растягиваем текст на всю кнопку
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(5, 5);
        textRect.offsetMax = new Vector2(-5, -5);

        // 6. Настраиваем размер и позицию кнопки
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = new Vector2(350, 70);
        buttonRect.anchoredPosition = position;

        // 7. Добавляем эффект при наведении
        ButtonAnimator animator = buttonObj.AddComponent<ButtonAnimator>();

        // 8. Назначаем действие
        button.onClick.AddListener(action);

        return buttonObj;
    }

    // ===== МЕНЮ НАСТРОЕК =====

    void OpenSettingsMenu()
    {
        if (settingsPanel == null)
        {
            CreateSettingsMenu();
        }
        settingsPanel.SetActive(true);
    }

    void CreateSettingsMenu()
    {
        Debug.Log("⚙️ Создаю меню настроек...");

        // 1. Панель настроек
        settingsPanel = new GameObject("SettingsPanel");
        settingsPanel.transform.SetParent(transform);
        settingsPanel.transform.SetAsLastSibling();

        Image panelImage = settingsPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.85f);

        // Растягиваем на весь экран
        RectTransform panelRect = settingsPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // 2. Окно настроек
        GameObject settingsWindow = CreateSettingsWindow(settingsPanel.transform);

        // 3. Заголовок
        CreateSettingsTitle(settingsWindow.transform, "НАСТРОЙКИ", new Vector2(0, 230));

        // 4. Кнопка закрытия
        CreateCloseButton(settingsWindow.transform, new Vector2(280, 230), () => {
            settingsPanel.SetActive(false);
        });

        // 5. Настройки звука
        CreateSoundSettings(settingsWindow.transform);

        // 6. Настройки графики
        CreateGraphicsSettings(settingsWindow.transform);

        // 7. Кнопки
        CreateSettingsButtons(settingsWindow.transform);

        Debug.Log("✅ Меню настроек создано!");
    }

    GameObject CreateSettingsWindow(Transform parent)
    {
        GameObject window = new GameObject("SettingsWindow");
        window.transform.SetParent(parent);

        Image windowImage = window.AddComponent<Image>();
        windowImage.color = new Color(0.12f, 0.12f, 0.12f, 0.95f);

        // Добавляем тень
        Shadow shadow = window.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.5f);
        shadow.effectDistance = new Vector2(5, -5);

        // Размер и позиция
        RectTransform rect = window.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(700, 550);
        rect.anchoredPosition = Vector2.zero;

        return window;
    }

    void CreateSettingsTitle(Transform parent, string text, Vector2 position)
    {
        GameObject title = new GameObject("SettingsTitle");
        title.transform.SetParent(parent);

        TextMeshProUGUI titleText = title.AddComponent<TextMeshProUGUI>();
        titleText.text = text;
        titleText.fontSize = 36;
        titleText.color = Color.yellow;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontStyle = FontStyles.Bold;

        RectTransform rect = title.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(400, 60);
        rect.anchoredPosition = position;
    }

    void CreateCloseButton(Transform parent, Vector2 position, UnityEngine.Events.UnityAction action)
    {
        GameObject closeBtn = new GameObject("CloseButton");
        closeBtn.transform.SetParent(parent);

        Button button = closeBtn.AddComponent<Button>();
        button.onClick.AddListener(action);

        Image image = closeBtn.AddComponent<Image>();
        image.color = new Color(0.3f, 0.1f, 0.1f, 1f);

        // Текст
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(closeBtn.transform);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "✕";
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;

        // Растягиваем текст
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        // Позиция кнопки
        RectTransform btnRect = closeBtn.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = new Vector2(40, 40);
        btnRect.anchoredPosition = position;
    }

    void CreateSoundSettings(Transform parent)
    {
        // Заголовок секции
        CreateSectionTitle(parent, "ЗВУК", new Vector2(-250, 150));

        // Громкость музыки
        CreateVolumeSlider(parent, "Музыка", "MusicVolume", new Vector2(0, 100));

        // Громкость звуков
        CreateVolumeSlider(parent, "Звуки", "SFXVolume", new Vector2(0, 40));

        // Общая громкость
        CreateVolumeSlider(parent, "Общая", "MasterVolume", new Vector2(0, -20));
    }

    void CreateGraphicsSettings(Transform parent)
    {
        // Заголовок секции
        CreateSectionTitle(parent, "ГРАФИКА", new Vector2(-250, -80));

        // Dropdown разрешений
        CreateResolutionDropdown(parent, new Vector2(0, -140));

        // Dropdown качества
        CreateQualityDropdown(parent, new Vector2(0, -200));
    }

    void CreateSectionTitle(Transform parent, string text, Vector2 position)
    {
        GameObject section = new GameObject("Section_" + text);
        section.transform.SetParent(parent);

        TextMeshProUGUI sectionText = section.AddComponent<TextMeshProUGUI>();
        sectionText.text = text;
        sectionText.fontSize = 24;
        sectionText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        sectionText.alignment = TextAlignmentOptions.Left;

        RectTransform rect = section.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(400, 30);
        rect.anchoredPosition = position;
        rect.pivot = new Vector2(0, 0.5f);
    }

    void CreateVolumeSlider(Transform parent, string label, string prefsKey, Vector2 position)
    {
        GameObject container = new GameObject(label + "SliderContainer");
        container.transform.SetParent(parent);

        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(500, 50);
        containerRect.anchoredPosition = position;

        // Метка
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(container.transform);

        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label + ":";
        labelText.fontSize = 20;
        labelText.color = Color.white;
        labelText.alignment = TextAlignmentOptions.Left;

        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0.5f);
        labelRect.anchorMax = new Vector2(0, 0.5f);
        labelRect.sizeDelta = new Vector2(100, 30);
        labelRect.anchoredPosition = new Vector2(0, 0);
        labelRect.pivot = new Vector2(0, 0.5f);

        // Слайдер (упрощённая версия)
        GameObject sliderObj = new GameObject("Slider");
        sliderObj.transform.SetParent(container.transform);

        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = 0.0001f;
        slider.maxValue = 1f;
        slider.value = PlayerPrefs.GetFloat(prefsKey, 0.7f);

        // Простой внешний вид
        Image bg = sliderObj.AddComponent<Image>();
        bg.color = new Color(0.3f, 0.3f, 0.3f, 1f);

        // Ползунок
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(sliderObj.transform);

        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;

        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(20, 30);

        slider.targetGraphic = handleImage;

        // Позиция слайдера
        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0, 0.5f);
        sliderRect.anchorMax = new Vector2(1, 0.5f);
        sliderRect.sizeDelta = new Vector2(-120, 20);
        sliderRect.anchoredPosition = new Vector2(60, 0);
        sliderRect.pivot = new Vector2(0.5f, 0.5f);

        // Значение
        GameObject valueObj = new GameObject("Value");
        valueObj.transform.SetParent(container.transform);

        TextMeshProUGUI valueText = valueObj.AddComponent<TextMeshProUGUI>();
        valueText.text = $"{slider.value:F1}";
        valueText.fontSize = 18;
        valueText.color = new Color(0.7f, 0.7f, 0.7f, 1f);

        RectTransform valueRect = valueObj.GetComponent<RectTransform>();
        valueRect.anchorMin = new Vector2(1, 0.5f);
        valueRect.anchorMax = new Vector2(1, 0.5f);
        valueRect.sizeDelta = new Vector2(40, 30);
        valueRect.anchoredPosition = new Vector2(0, 0);
        valueRect.pivot = new Vector2(1, 0.5f);

        // Обработчик
        slider.onValueChanged.AddListener((value) => {
            valueText.text = $"{value:F1}";
            PlayerPrefs.SetFloat(prefsKey, value);
            PlayerPrefs.Save();

            // Если есть AudioMixer
            if (audioMixer != null)
            {
                float volumeDB = Mathf.Log10(value) * 20;
                audioMixer.SetFloat(prefsKey, volumeDB);
            }
        });

        // Инициализируем
        slider.onValueChanged.Invoke(slider.value);
    }

    void CreateResolutionDropdown(Transform parent, Vector2 position)
    {
        GameObject container = new GameObject("ResolutionContainer");
        container.transform.SetParent(parent);

        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(500, 50);
        containerRect.anchoredPosition = position;

        // Метка
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(container.transform);

        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = "Разрешение:";
        labelText.fontSize = 20;
        labelText.color = Color.white;
        labelText.alignment = TextAlignmentOptions.Left;

        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0.5f);
        labelRect.anchorMax = new Vector2(0, 0.5f);
        labelRect.sizeDelta = new Vector2(150, 30);
        labelRect.anchoredPosition = new Vector2(0, 0);
        labelRect.pivot = new Vector2(0, 0.5f);

        // Кнопка для смены разрешения (упрощённо)
        GameObject btnObj = new GameObject("ResolutionButton");
        btnObj.transform.SetParent(container.transform);

        Button button = btnObj.AddComponent<Button>();
        button.onClick.AddListener(() => {
            ToggleResolution();
        });

        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = buttonColor;

        // Текст кнопки
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform);

        TextMeshProUGUI btnText = textObj.AddComponent<TextMeshProUGUI>();
        btnText.text = $"{Screen.width} × {Screen.height}";
        btnText.fontSize = 18;
        btnText.color = Color.white;
        btnText.alignment = TextAlignmentOptions.Center;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        // Позиция кнопки
        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0, 0.5f);
        btnRect.anchorMax = new Vector2(1, 0.5f);
        btnRect.sizeDelta = new Vector2(-160, 35);
        btnRect.anchoredPosition = new Vector2(80, 0);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
    }

    void CreateQualityDropdown(Transform parent, Vector2 position)
    {
        GameObject container = new GameObject("QualityContainer");
        container.transform.SetParent(parent);

        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(500, 50);
        containerRect.anchoredPosition = position;

        // Метка
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(container.transform);

        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = "Качество:";
        labelText.fontSize = 20;
        labelText.color = Color.white;
        labelText.alignment = TextAlignmentOptions.Left;

        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0.5f);
        labelRect.anchorMax = new Vector2(0, 0.5f);
        labelRect.sizeDelta = new Vector2(150, 30);
        labelRect.anchoredPosition = new Vector2(0, 0);
        labelRect.pivot = new Vector2(0, 0.5f);

        // Кнопка для смены качества
        GameObject btnObj = new GameObject("QualityButton");
        btnObj.transform.SetParent(container.transform);

        Button button = btnObj.AddComponent<Button>();
        button.onClick.AddListener(() => {
            CycleQuality();
        });

        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = buttonColor;

        // Текст кнопки
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform);

        TextMeshProUGUI btnText = textObj.AddComponent<TextMeshProUGUI>();
        btnText.text = QualitySettings.names[QualitySettings.GetQualityLevel()];
        btnText.fontSize = 18;
        btnText.color = Color.white;
        btnText.alignment = TextAlignmentOptions.Center;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        // Позиция кнопки
        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0, 0.5f);
        btnRect.anchorMax = new Vector2(1, 0.5f);
        btnRect.sizeDelta = new Vector2(-160, 35);
        btnRect.anchoredPosition = new Vector2(80, 0);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
    }

    void CreateSettingsButtons(Transform parent)
    {
        // Кнопка "Применить"
        CreateSettingsButton(parent, "ПРИМЕНИТЬ", new Vector2(100, -260), () => {
            Debug.Log("✅ Настройки применены");
            settingsPanel.SetActive(false);
        });

        // Кнопка "Сброс"
        CreateSettingsButton(parent, "СБРОС", new Vector2(-100, -260), () => {
            Debug.Log("🔄 Сброс настроек");
            // Сброс громкости
            PlayerPrefs.DeleteKey("MusicVolume");
            PlayerPrefs.DeleteKey("SFXVolume");
            PlayerPrefs.DeleteKey("MasterVolume");
            // Перезагружаем настройки
            settingsPanel.SetActive(false);
            CreateSettingsMenu();
        });
    }

    void CreateSettingsButton(Transform parent, string text, Vector2 position, UnityEngine.Events.UnityAction action)
    {
        GameObject btn = CreateButton(parent, text, position, action);
        RectTransform rect = btn.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(180, 45);
    }

    // ===== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ =====

    void ToggleResolution()
    {
        // Простой переключатель разрешений
        if (Screen.width == 1920)
            Screen.SetResolution(1280, 720, Screen.fullScreen);
        else if (Screen.width == 1280)
            Screen.SetResolution(1366, 768, Screen.fullScreen);
        else if (Screen.width == 1366)
            Screen.SetResolution(1600, 900, Screen.fullScreen);
        else
            Screen.SetResolution(1920, 1080, Screen.fullScreen);

        Debug.Log($"📐 Изменено разрешение: {Screen.width}×{Screen.height}");
    }

    void CycleQuality()
    {
        int currentLevel = QualitySettings.GetQualityLevel();
        int nextLevel = (currentLevel + 1) % QualitySettings.names.Length;
        QualitySettings.SetQualityLevel(nextLevel);

        Debug.Log($"🎨 Установлено качество: {QualitySettings.names[nextLevel]}");
    }

    void ShowExitConfirmation()
    {
        // Простое подтверждение выхода
#if UNITY_EDITOR
        Debug.Log("🚪 Выход из редактора Unity");
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Debug.Log("🚪 Выход из игры");
            Application.Quit();
#endif
    }
}

// Дополнительный скрипт для анимации кнопок
public class ButtonAnimator : MonoBehaviour
{
    private Vector3 originalScale;
    private Button button;

    void Start()
    {
        originalScale = transform.localScale;
        button = GetComponent<Button>();
    }

    void Update()
    {
        // Простая анимация при наведении
        if (button != null && button.interactable)
        {
            // Можно добавить пульсацию или другие эффекты
            if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == gameObject)
            {
                transform.localScale = originalScale * 1.05f;
            }
            else
            {
                transform.localScale = originalScale;
            }
        }
    }
}