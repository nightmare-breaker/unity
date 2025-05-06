using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BtnManage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite originalSprite;
    [SerializeField] private Sprite hoverSprite;
    public GameObject arrowIcon;

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetImage.sprite = hoverSprite;
        arrowIcon.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetImage.sprite = originalSprite;
        arrowIcon.SetActive(false);
    }
}