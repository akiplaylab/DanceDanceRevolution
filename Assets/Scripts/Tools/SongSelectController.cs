using UnityEngine;
using UnityEngine.SceneManagement;

public class SongSelectController : MonoBehaviour
{
    [SerializeField] SongDefinition[] songs;

    public void SelectSong(int index)
    {
        SelectedSong.value = songs[index];
        SceneManager.LoadScene("GameScene");
    }
}
