using TMPro;
using UnityEngine;

public class Line : MonoBehaviour, IPoolable
{
    public LineData data;

    public TextMeshProUGUI textContainer;
    public Transform textRotationPoint;
    public Transform rotationPoint;
    public Transform lineImageTransform;
    public Transform textTransform;

    public bool isClickable;
    public bool isForSection;
    public void SetOptions(LineData data)
    {
        this.data = data;

        isClickable = data.isClickable;
        isForSection = data.isForSection;

        if (isClickable)
        {
            textContainer.fontStyle = FontStyles.Italic;
        }
        else
        {
            textContainer.fontStyle = FontStyles.Normal;
        }
        textContainer.text = data.name;
        transform.localPosition = data.localPosition;

        int coefficientX = data.xReverse ? -1 : 1;

        int coefficientY = data.yReverse ? -1 : 1;

        lineImageTransform.localPosition = new Vector3(0, 5f * coefficientX, 0);
        textRotationPoint.localPosition = new Vector3(0, 5f * coefficientX, 0);
        textTransform.localPosition = new Vector3(25f * coefficientY, 2.5f * coefficientX, 0);

        rotationPoint.rotation = Quaternion.Euler(0, 0, -data.rotation * coefficientX * coefficientY);
        textRotationPoint.localRotation = Quaternion.Euler(0, 0, data.rotation * coefficientX * coefficientY);

        if (data.yReverse) textContainer.alignment = TextAlignmentOptions.MidlineRight;
        else textContainer.alignment = TextAlignmentOptions.MidlineLeft;

        transform.localScale = Vector3.one * PamsManager.Instance.currentDistance / 300.0f;

        gameObject.SetActive(!isForSection || PamsManager.Instance.showSectionToggle);

        LookAtCamera(Camera.main.transform);
    }

    public void OnClickToVisualizeChild()
    {
        if (isClickable)
        {
            PamsManager.Instance.VisualizeChildByName(textContainer.text);
        }
    }

    public void LookAtCamera(Transform cameraTransform)
    {
        transform.rotation = Quaternion.LookRotation(-cameraTransform.position);
    }

    public void Show(bool isLinesVisible, bool isSectionVisible)
    {
        gameObject.SetActive(
            isLinesVisible &&
            (!isForSection || isSectionVisible)
        );
    }

    public void OnSpawn()
    {
        PamsManager.Instance.lookLinesAtMainCamera += LookAtCamera;
        PamsManager.Instance.showLines += Show;
    }

    public void OnDespawn()
    {
        PamsManager.Instance.lookLinesAtMainCamera -= LookAtCamera;
        PamsManager.Instance.showLines -= Show;
    }
}
