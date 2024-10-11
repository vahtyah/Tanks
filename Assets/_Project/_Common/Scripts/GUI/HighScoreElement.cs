using TMPro;
using UnityEngine;

public class HighScoreElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    public void SetData(int i, string username, int score)
    {
        text.text = $"{i}. {username} - {score}";
    }
}
