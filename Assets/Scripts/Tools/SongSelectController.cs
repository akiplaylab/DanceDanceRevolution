using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public sealed class SongSelectController : MonoBehaviour
{
    [SerializeField] SongLibrary library;

    [Header("UI")]
    [SerializeField] Transform listRoot;
    [SerializeField] SongRowView rowPrefab;

    readonly List<SongRowView> rows = new();
    int selectedIndex = 0;

    void Start()
    {
        BuildList();
        UpdateSelection();
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.upArrowKey.wasPressedThisFrame)
            MoveSelection(-1);

        if (kb.downArrowKey.wasPressedThisFrame)
            MoveSelection(+1);

        if (kb.enterKey.wasPressedThisFrame)
            SelectSong(selectedIndex);
    }

    void BuildList()
    {
        rows.Clear();

        for (int i = listRoot.childCount - 1; i >= 0; i--)
            Destroy(listRoot.GetChild(i).gameObject);

        for (int i = 0; i < library.Count; i++)
        {
            var row = Instantiate(rowPrefab, listRoot);
            row.Bind(this, i, library.Songs[i]);
            rows.Add(row);
        }
    }

    void MoveSelection(int delta)
    {
        selectedIndex = Mathf.Clamp(selectedIndex + delta, 0, rows.Count - 1);
        UpdateSelection();
    }

    void UpdateSelection()
    {
        for (int i = 0; i < rows.Count; i++)
            rows[i].SetSelected(i == selectedIndex);
    }

    public void OnRowClicked(int index)
    {
        selectedIndex = index;
        UpdateSelection();
        SelectSong(index);
    }

    void SelectSong(int index)
    {
        var song = library.Get(index);
        if (song == null) return;

        SelectedSong.Value = song;
        SceneManager.LoadScene("GameScene");
    }
}
