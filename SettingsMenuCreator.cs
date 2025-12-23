using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.Rendering;

public class SettingsMenuCreator : MonoBehaviour
{
    [Header("Настройки звука")]
    public AudioMixer audioMixer; // Если используете Audio Mixer

    [Header("Доступные разрешения")]
    public Vector2Int[] resolutions = {
        new Vector2Int(1920, 1080),
        new Vector2Int(1280, 720),
        new Vector2Int(1366, 768),
        new Vector2Int(1600, 900)
    };

    [Header("Стиль")]
    public Color panelColor = new Color(0.12f, 0.12f, 0.12f, 0.95f);
    public Color buttonColor = new Color(0.25f, 0.25f, 0.25f, 1f);
    public Color sliderColor = Color.gray;

    private GameObject settingsPanel;
    private GameObject settingsWindow;

    void Start()
    {
        // Создаём панель настроек, но скрываем её
        CreateSettingsPanel();
        settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
        else
        {
            CreateSettingsPanel();
        }
    }

    void CreateSettingsPanel()
    {
        // === 1. СОЗДАЁМ ПАНЕЛЬ НАСТРОЕК ===
        settingsPanel = new GameObject("SettingsPanel");
        settingsPanel.transform.SetParent(transform);
        settingsPanel.transform.SetAsLastSibling(); // Поверх всего

        // Добавляем Image для затемнения фона
        Image panelImage = settingsPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.7f);

        // Растягиваем на весь экран
        RectTransform panelRect = settingsPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // === 2. СОЗДАЁМ ОКНО НАСТРОЕК ===
        settingsWindow = CreateSettingsWindow(settingsPanel.transform);

        // === 3. ДОБАВЛЯЕМ ЭЛЕМЕНТЫ В ОКНО ===
        AddTitle(settingsWindow.transform, "НАСТРОЙКИ", new Vector2(0, 230));
        AddCloseButton(settingsWindow.transform);

        // Секция звука
        AddSectionTitle(settingsWindow.transform, "ЗВУК", new Vector2(-180, 150));
        AddVolumeControl(settingsWindow.transform, "Музыка", "MusicVolume", new Vector2(0, 100));
        AddVolumeControl(settingsWindow.transform, "Звуки", "SFXVolume", new Vector2(0, 40));

        // Секция графики
        AddSectionTitle(settingsWindow.transform, "ГРАФИКА", new Vector2(-180, -20));
        AddResolutionDropdown(settingsWindow.transform, new Vector2(0, -60));
        AddQualityDropdown(settingsWindow.transform, new Vector2(0, -120));

        // Кнопки
        AddApplyButton(settingsWindow.transform, new Vector2(100, -220));
        AddCancelButton(settingsWindow.transform, new Vector2(-100, -220));

        Debug.Log("✅ Меню настроек создано!");
    }

    GameObject CreateSettingsWindow(Transform parent)
    {
        GameObject window = new GameObject("SettingsWindow");
        window.transform.SetParent(parent);

        // Внешний вид окна
        Image windowImage = window.AddComponent<Image>();
        windowImage.color = panelColor;

        // Добавляем тень для объёма
        Shadow shadow = window.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.5f);
        shadow.effectDistance = new Vector2(5, -5);

        // Размер и позиция
        RectTransform rect = window.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(650, 600);
        rect.anchoredPosition = Vector2.zero;

        return window;
    }

    void AddTitle(Transform parent, string text, Vector2 position)
    {
        GameObject title = new GameObject("Title");
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

    void AddSectionTitle(Transform parent, string text, Vector2 position)
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

    void AddCloseButton(Transform parent)
    {
        GameObject closeBtn = CreateButton(parent, "X", new Vector2(280, 230), new Vector2(50, 50));
        closeBtn.name = "CloseButton";

        Button button = closeBtn.GetComponent<Button>();
        button.onClick.AddListener(() => {
            settingsPanel.SetActive(false);
            Debug.Log("⚙️ Настройки закрыты");
        });

        // Делаем текст больше
        TextMeshProUGUI text = closeBtn.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.fontSize = 28;
            text.fontStyle = FontStyles.Bold;
        }
    }

    void AddVolumeControl(Transform parent, string label, string playerPrefKey, Vector2 position)
    {
        // Создаём контейнер для слайдера
        GameObject container = new GameObject(label + "_Container");
        container.transform.SetParent(parent);

        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(400, 60);
        containerRect.anchoredPosition = position;

        // Текст с названием
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
        labelRect.sizeDelta = new Vector2(150, 30);
        labelRect.anchoredPosition = new Vector2(0, 0);
        labelRect.pivot = new Vector2(0, 0.5f);

        // Слайдер
        GameObject sliderObj = new GameObject("Slider");
        sliderObj.transform.SetParent(container.transform);

        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = 0.0001f; // 0.0001 = -80dB (почти тишина)
        slider.maxValue = 1f;
        slider.value = PlayerPrefs.GetFloat(playerPrefKey, 0.5f);

        // Настраиваем слайдер
        slider.targetGraphic = sliderObj.AddComponent<Image>();
        slider.targetGraphic.color = sliderColor;

        // Заполнение слайдера
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform);

        RectTransform fillRect = fillArea.AddComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0, 0.25f);
        fillRect.anchorMax = new Vector2(1, 0.75f);
        fillRect.offsetMin = new Vector2(5, 0);
        fillRect.offsetMax = new Vector2(-5, 0);

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform);

        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = Color.green;

        RectTransform fillImageRect = fill.GetComponent<RectTransform>();
        fillImageRect.anchorMin = Vector2.zero;
        fillImageRect.anchorMax = Vector2.one;
        fillImageRect.sizeDelta = Vector2.zero;

        // Ползунок
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObj.transform);

        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.sizeDelta = Vector2.zero;

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform);

        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;

        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.anchorMin = new Vector2(0, 0);
        handleRect.anchorMax = new Vector2(0, 1);
        handleRect.sizeDelta = new Vector2(20, 0);

        // Назначаем слайдеру компоненты
        slider.fillRect = fillImageRect;
        slider.handleRect = handleRect;

        // Позиция слайдера
        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0, 0.5f);
        sliderRect.anchorMax = new Vector2(1, 0.5f);
        sliderRect.sizeDelta = new Vector2(-160, 20);
        sliderRect.anchoredPosition = new Vector2(80, 0);
        sliderRect.pivot = new Vector2(0.5f, 0.5f);

        // Сохранение значения
        slider.onValueChanged.AddListener((value) => {
            PlayerPrefs.SetFloat(playerPrefKey, value);
            PlayerPrefs.Save();

            // Если используете Audio Mixer
            if (audioMixer != null)
            {
                // Конвертируем линейное значение в децибелы
                float volumeDB = Mathf.Log10(value) * 20;
                audioMixer.SetFloat(playerPrefKey, volumeDB);
            }

            Debug.Log($"🔊 {label}: {value:F2}");
        });

        // Текущее значение
        GameObject valueObj = new GameObject("ValueText");
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

        // Обновление текста при изменении слайдера
        slider.onValueChanged.AddListener((value) => {
            valueText.text = $"{value:F1}";
        });
    }

    void AddResolutionDropdown(Transform parent, Vector2 position)
    {
        GameObject container = new GameObject("ResolutionContainer");
        container.transform.SetParent(parent);

        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(400, 60);
        containerRect.anchoredPosition = position;

        // Текст
        GameObject label = new GameObject("Label");
        label.transform.SetParent(container.transform);

        TextMeshProUGUI labelText = label.AddComponent<TextMeshProUGUI>();
        labelText.text = "Разрешение:";
        labelText.fontSize = 20;
        labelText.color = Color.white;
        labelText.alignment = TextAlignmentOptions.Left;

        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0.5f);
        labelRect.anchorMax = new Vector2(0, 0.5f);
        labelRect.sizeDelta = new Vector2(150, 30);
        labelRect.anchoredPosition = new Vector2(0, 0);
        labelRect.pivot = new Vector2(0, 0.5f);

        // Dropdown
        GameObject dropdownObj = new GameObject("ResolutionDropdown");
        dropdownObj.transform.SetParent(container.transform);

        TMP_Dropdown dropdown = dropdownObj.AddComponent<TMP_Dropdown>();

        // Наполняем опциями
        dropdown.options.Clear();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string optionText = $"{resolutions[i].x} × {resolutions[i].y}";
            dropdown.options.Add(new TMP_Dropdown.OptionData(optionText));

            // Проверяем текущее разрешение
            if (Screen.width == resolutions[i].x && Screen.height == resolutions[i].y)
            {
                currentResolutionIndex = i;
            }
        }

        dropdown.value = currentResolutionIndex;

        // Настраиваем внешний вид
        Image dropdownImage = dropdownObj.AddComponent<Image>();
        dropdownImage.color = buttonColor;

        // Позиция
        RectTransform dropdownRect = dropdownObj.GetComponent<RectTransform>();
        dropdownRect.anchorMin = new Vector2(0, 0.5f);
        dropdownRect.anchorMax = new Vector2(1, 0.5f);
        dropdownRect.sizeDelta = new Vector2(-160, 35);
        dropdownRect.anchoredPosition = new Vector2(80, 0);
        dropdownRect.pivot = new Vector2(0.5f, 0.5f);

        // Обработчик изменения
        dropdown.onValueChanged.AddListener((index) => {
            Debug.Log($"📐 Выбрано разрешение: {resolutions[index].x}x{resolutions[index].y}");
        });
    }

    void AddQualityDropdown(Transform parent, Vector2 position)
    {
        GameObject container = new GameObject("QualityContainer");
        container.transform.SetParent(parent);

        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(400, 60);
        containerRect.anchoredPosition = position;

        // Текст
        GameObject label = new GameObject("Label");
        label.transform.SetParent(container.transform);

        TextMeshProUGUI labelText = label.AddComponent<TextMeshProUGUI>();
        labelText.text = "Качество:";
        labelText.fontSize = 20;
        labelText.color = Color.white;
        labelText.alignment = TextAlignmentOptions.Left;

        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0.5f);
        labelRect.anchorMax = new Vector2(0, 0.5f);
        labelRect.sizeDelta = new Vector2(150, 30);
        labelRect.anchoredPosition = new Vector2(0, 0);
        labelRect.pivot = new Vector2(0, 0.5f);

        // Dropdown
        GameObject dropdownObj = new GameObject("QualityDropdown");
        dropdownObj.transform.SetParent(container.transform);

        TMP_Dropdown dropdown = dropdownObj.AddComponent<TMP_Dropdown>();

        // Получаем названия качеств из Unity
        string[] qualityNames = QualitySettings.names;
        dropdown.options.Clear();

        foreach (string name in qualityNames)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(name));
        }

        dropdown.value = QualitySettings.GetQualityLevel();

        // Настраиваем внешний вид
        Image dropdownImage = dropdownObj.AddComponent<Image>();
        dropdownImage.color = buttonColor;

        // Позиция
        RectTransform dropdownRect = dropdownObj.GetComponent<RectTransform>();
        dropdownRect.anchorMin = new Vector2(0, 0.5f);
        dropdownRect.anchorMax = new Vector2(1, 0.5f);
        dropdownRect.sizeDelta = new Vector2(-160, 35);
        dropdownRect.anchoredPosition = new Vector2(80, 0);
        dropdownRect.pivot = new Vector2(0.5f, 0.5f);

        // Обработчик изменения
        dropdown.onValueChanged.AddListener((index) => {
            QualitySettings.SetQualityLevel(index);
            Debug.Log($"🎨 Установлено качество: {qualityNames[index]}");
        });
    }

    GameObject CreateButton(Transform parent, string text, Vector2 position, Vector2 size)
    {
        GameObject buttonObj = new GameObject(text + "_Button");
        buttonObj.transform.SetParent(parent);

        // Добавляем компоненты
        Image image = buttonObj.AddComponent<Image>();
        Button button = buttonObj.AddComponent<Button>();

        // Настраиваем цвета
        ColorBlock colors = button.colors;
        colors.normalColor = buttonColor;
        colors.highlightedColor = new Color(0.35f, 0.35f, 0.35f, 1f);
        colors.pressedColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        button.colors = colors;

        // Добавляем текст
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);

        TextMeshProUGUI textComp = textObj.AddComponent<TextMeshProUGUI>();
        textComp.text = text;
        textComp.fontSize = 22;
        textComp.color = Color.white;
        textComp.alignment = TextAlignmentOptions.Center;

        // Растягиваем текст
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        // Настраиваем кнопку
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = size;
        buttonRect.anchoredPosition = position;

        return buttonObj;
    }

    void AddApplyButton(Transform parent, Vector2 position)
    {
        GameObject applyBtn = CreateButton(parent, "ПРИМЕНИТЬ", position, new Vector2(180, 50));
        applyBtn.name = "ApplyButton";

        Button button = applyBtn.GetComponent<Button>();
        button.onClick.AddListener(() => {
            Debug.Log("✅ Настройки применены");
            // Здесь можно применить все настройки
            settingsPanel.SetActive(false);
        });
    }

    void AddCancelButton(Transform parent, Vector2 position)
    {
        GameObject cancelBtn = CreateButton(parent, "ОТМЕНА", position, new Vector2(180, 50));
        cancelBtn.name = "CancelButton";

        Button button = cancelBtn.GetComponent<Button>();
        button.onClick.AddListener(() => {
            Debug.Log("❌ Настройки отменены");
            settingsPanel.SetActive(false);
        });
    }
}