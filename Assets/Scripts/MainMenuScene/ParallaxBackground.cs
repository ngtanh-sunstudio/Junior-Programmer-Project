using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = -50f;

    private RectTransform original;
    private RectTransform duplicate;
    private float imageHeight;
    private float startY;

    private void Awake()
    {
        original = GetComponent<RectTransform>();
    }

    private void Start()
    {
        Canvas.ForceUpdateCanvases();

        imageHeight = original.rect.height;
        if (imageHeight <= 0f)
        {
            return;
        }

        startY = original.anchoredPosition.y;
        duplicate = CreateDuplicateAbove();
    }

    private void Update()
    {
        if (duplicate == null)
        {
            return;
        }

        Vector2 movement = Vector2.up * scrollSpeed * Time.deltaTime;

        original.anchoredPosition += movement;
        duplicate.anchoredPosition += movement;

        WrapIfPastHeight(original, duplicate);
        WrapIfPastHeight(duplicate, original);
    }

    private RectTransform CreateDuplicateAbove()
    {
        Image sourceImage = GetComponent<Image>();
        GameObject clone = new GameObject(name + "_Loop", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        clone.transform.SetParent(transform.parent, false);
        clone.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);

        RectTransform cloneRect = clone.GetComponent<RectTransform>();
        SetupCloneRectTransform(cloneRect);

        Image cloneImage = clone.GetComponent<Image>();
        SetupCloneImage(sourceImage, cloneImage);

        return cloneRect;
    }

    private void SetupCloneRectTransform(RectTransform cloneRect)
    {
        cloneRect.anchorMin = original.anchorMin;
        cloneRect.anchorMax = original.anchorMax;
        cloneRect.pivot = original.pivot;
        cloneRect.sizeDelta = original.sizeDelta;
        cloneRect.localScale = original.localScale;
        cloneRect.localRotation = original.localRotation;
        cloneRect.anchoredPosition = original.anchoredPosition + Vector2.up * imageHeight;
    }

    private static void SetupCloneImage(Image sourceImage, Image cloneImage)
    {
        cloneImage.sprite = sourceImage.sprite;
        cloneImage.color = sourceImage.color;
        cloneImage.material = sourceImage.material;
        cloneImage.raycastTarget = sourceImage.raycastTarget;
        cloneImage.preserveAspect = sourceImage.preserveAspect;
    }

    private void WrapIfPastHeight(RectTransform tile, RectTransform otherTile)
    {
        if (scrollSpeed < 0f && tile.anchoredPosition.y <= startY - imageHeight)
        {
            tile.anchoredPosition = otherTile.anchoredPosition + Vector2.up * imageHeight;
        }
        else if (scrollSpeed > 0f && tile.anchoredPosition.y >= startY + imageHeight)
        {
            tile.anchoredPosition = otherTile.anchoredPosition - Vector2.up * imageHeight;
        }
    }

}
