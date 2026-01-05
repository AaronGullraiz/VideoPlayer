using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VideoOrderUIController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private Button itemButtonPrefab;
    [SerializeField] private Button upButton;
    [SerializeField] private Button downButton;

    private VideoConfig config;
    private int selectedIndex = -1;
    private readonly List<Button> buttons = new();

    private void Start()
    {
        upButton.gameObject.SetActive(false);
        downButton.gameObject.SetActive(false);

        Refresh();
    }

    private void Refresh()
    {
        foreach (var btn in buttons)
            Destroy(btn.gameObject);

        buttons.Clear();

        config = VideoConfigManager.Load();

        config.videos = config.videos
            .OrderBy(v => v.order)
            .ToList();

        for (int i = 0; i < config.videos.Count; i++)
        {
            int idx = i;
            var video = config.videos[i];

            Button btn = Instantiate(itemButtonPrefab, contentParent);
            btn.GetComponentInChildren<TMP_Text>().text = video.fileName;
            btn.onClick.AddListener(() => Select(idx));

            buttons.Add(btn);
        }

        NormalizeAndSave();
    }

    private void Select(int index)
    {
        selectedIndex = index;
        upButton.gameObject.SetActive(true);
        downButton.gameObject.SetActive(true);
    }

    public void MoveUp()
    {
        if (selectedIndex <= 0) return;

        Swap(selectedIndex, selectedIndex - 1);
        selectedIndex--;
        Refresh();
    }

    public void MoveDown()
    {
        if (selectedIndex < 0 || selectedIndex >= config.videos.Count - 1)
            return;

        Swap(selectedIndex, selectedIndex + 1);
        selectedIndex++;
        Refresh();
    }

    private void Swap(int a, int b)
    {
        var temp = config.videos[a];
        config.videos[a] = config.videos[b];
        config.videos[b] = temp;

        NormalizeAndSave();
    }

    private void NormalizeAndSave()
    {
        for (int i = 0; i < config.videos.Count; i++)
            config.videos[i].order = i;

        VideoConfigManager.Save(config);
    }
}