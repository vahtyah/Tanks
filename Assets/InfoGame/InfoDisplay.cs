using TMPro;
using UnityEngine;

namespace InfoGame
{
    public class InfoDisplay : MonoBehaviour
    {
        public static InfoDisplay Create(GameObject prefab, Transform parent, Info info)
        {
            var instance = Instantiate(prefab, parent).GetComponent<InfoDisplay>();
            instance.SetInfo(info.Title, info.Value);
            info.OnValueChanged(instance.SetValue);
            return instance;
        }
    
        public static void Remove(InfoDisplay infoDisplay)
        {
            Destroy(infoDisplay.gameObject);
        }
    
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI value;

        private void SetInfo(string title, string value)
        {
            this.title.text = title;
            this.value.text = value;
        }

        private void SetValue(string value)
        {
            this.value.text = value;
        }
    }
}