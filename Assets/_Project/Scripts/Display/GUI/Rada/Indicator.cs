using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    #region Static Methods

    private static IndicatorManager manager;

    private static void EnsureManagerExists()
    {
        if (manager == null)
        {
            manager = IndicatorManager.Instance ?? new GameObject("IndicatorManager").AddComponent<IndicatorManager>();
        }
    }

    public static Indicator Create(Transform target, Sprite sprite)
    {
        EnsureManagerExists();
        var indicatorGo = Pool.Spawn(manager.indicatorPrefab, manager.indicatorContainer);
        var indicator = indicatorGo.GetComponent<Indicator>();
        indicator.Initialize(target, sprite);
        manager.Register(indicator);
        return indicator;
    }

    public static Indicator UpdateTarget(Transform oldTarget, Transform newTarget)
    {
        EnsureManagerExists();
        return manager.UpdateTarget(oldTarget, newTarget);
    }

    public static Indicator GetIndicator(Transform target)
    {
        EnsureManagerExists();
        return manager.GetIndicator(target);
    }

    public static void Destroy(Indicator indicator)
    {
        Pool.Despawn(indicator.gameObject);
    }

    #endregion

    public RectTransform indicatorRectTransform;
    public Image iconImage;
    private RectTransform iconRectTransform;
    
    [SerializeField] private GameObject movingEffect;
    [SerializeField] private GameObject ownerMovingEffect;
    
    
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI stateText;


    public float edgePadding = 50f;

    private Camera mainCamera;
    private Transform target;

    private bool isTopEdge;
    private bool isBottomEdge;
    private bool isLeftEdge;
    private bool isRightEdge;

    private void Initialize(Transform target, Sprite sprite)
    {
        iconImage.sprite = sprite;
        this.target = target;
    }

    void Start()
    {
        iconRectTransform = iconImage.GetComponent<RectTransform>();
        mainCamera = Camera.main;
        SetCountdownTextVisibility(false);
        SetStateTextVisibility(false);
        SetMovingEffectVisibility(false);
        SetOwnerMovingEffectVisibility(false);
    }

    public void UpdateIndicatorPosition(float screenWidth, float screenHeight)
    {
        if (target == null || mainCamera == null) return;
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(target.position);

        if (screenPosition.z < 0)
        {
            screenPosition *= -1;
        }

        bool isTargetOnScreen = screenPosition.z > 0 && screenPosition.x > 0 && screenPosition.x < screenWidth &&
                                screenPosition.y > 0 && screenPosition.y < screenHeight;
        indicatorRectTransform.gameObject.SetActive(!isTargetOnScreen);

        if (!isTargetOnScreen)
        {
            screenPosition.x = Mathf.Clamp(screenPosition.x, edgePadding, screenWidth - edgePadding);
            screenPosition.y = Mathf.Clamp(screenPosition.y, edgePadding, screenHeight - edgePadding);
            indicatorRectTransform.position = screenPosition;

            HandleRotateIcon();
            HandleStateText();
        }
    }

    private void HandleRotateIcon()
    {
        Vector3 directionToTarget = (target.position - mainCamera.transform.position).normalized;
        float rotationAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
        iconRectTransform.rotation = Quaternion.Euler(0, 0, -rotationAngle);
    }

    private void HandleStateText()
    {
        if (!stateText.gameObject.activeSelf) return;
        CheckEdges();
        SetStateTextPosition();
    }

    private void CheckEdges()
    {
        var topEdge = indicatorRectTransform.position.y >= Screen.height - edgePadding;
        var bottomEdge = indicatorRectTransform.position.y <= edgePadding;
        var leftEdge = indicatorRectTransform.position.x <= edgePadding;
        var rightEdge = indicatorRectTransform.position.x >= Screen.width - edgePadding;
        SetEdgeFlags(topEdge, bottomEdge, leftEdge, rightEdge);
    }

    private void SetEdgeFlags(bool top, bool bottom, bool left, bool right)
    {
        isTopEdge = top && !(left || right);
        isBottomEdge = bottom && !(left || right);
        isLeftEdge = left;
        isRightEdge = right;
    }

    public void StartReturningEffect(float remaining)
    {
        SetCountdownText(remaining);
        SetCountdownTextVisibility(true);
        SetStateTextVisibility(true);
        SetStateText("Returning");
        StopMovingEffect();
    }

    public void StopReturningEffect()
    {
        SetCountdownTextVisibility(false);
        SetStateTextVisibility(false);
    }

    public void StartMovingEffect()
    {
        SetMovingEffectVisibility(true);
        StopReturningEffect();
    }
    
    public void StartOwnerMovingEffect()
    {
        SetOwnerMovingEffectVisibility(true);
        StopReturningEffect();
    }

    public void StopMovingEffect()
    {
        SetMovingEffectVisibility(false);
        SetOwnerMovingEffectVisibility(false);
    }

    private void SetStateTextPosition()
    {
        if (isTopEdge)
        {
            stateText.rectTransform.position = new Vector3(indicatorRectTransform.position.x,
                iconImage.rectTransform.position.y - 55f, 0);
            stateText.alignment = TextAlignmentOptions.Midline;
        }
        else if (isBottomEdge)
        {
            stateText.rectTransform.position = new Vector3(indicatorRectTransform.position.x,
                iconImage.rectTransform.position.y + 55f, 0);
            stateText.alignment = TextAlignmentOptions.Midline;
        }
        else if (isLeftEdge)
        {
            stateText.rectTransform.position = new Vector3(iconImage.rectTransform.position.x + 55f,
                indicatorRectTransform.position.y, 0);
            stateText.alignment = TextAlignmentOptions.MidlineLeft;
        }
        else if (isRightEdge)
        {
            stateText.rectTransform.position = new Vector3(iconImage.rectTransform.position.x - 55f,
                indicatorRectTransform.position.y, 0);
            stateText.alignment = TextAlignmentOptions.MidlineRight;
        }
    }

    public void UpdateTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetCountdownText(float remaining)
    {
        countdownText.text = remaining.ToString("N0");
    }

    public void SetCountdownTextVisibility(bool visible)
    {
        countdownText.gameObject.SetActive(visible);
    }

    public void SetStateTextVisibility(bool visible)
    {
        stateText.gameObject.SetActive(visible);
    }

    private void SetStateText(string text)
    {
        stateText.text = text;
    }

    public void SetMovingEffectVisibility(bool visible)
    {
        movingEffect.SetActive(visible);
    }

    private void SetOwnerMovingEffectVisibility(bool b)
    {
        ownerMovingEffect.SetActive(b);
    }

    public Transform Target => target;
}