using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectShape : MonoBehaviour, IPointerClickHandler
{

    public GrideCell grideCell;

    public Action<GrideCell> onMouseDownAction;
    private Image image;
    private Text text;
    void Awake()
    {
        image = GetComponent<Image>();
        text = GetComponentInChildren<Text>();
    }

    public bool HaveValue
    {
        get { return string.IsNullOrEmpty(text.text); }
    }

    public void SetSelctedState()
    {
        image.color = Color.green;
    }

    public void SetNotSelectedState()
    {
        image.color = Color.white;
    }

    public void SetValue(string value)
    {
        text.text = value;
    }

    public string GetValue()
    {
        return text.text;
    }

    public bool CompareValue(ObjectShape other)
    {
        return string.Equals(GetValue(), other.GetValue());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onMouseDownAction != null)
        {
            onMouseDownAction(grideCell);
        }
    }

    public void PairHandler()
    {
        gameObject.SetActive(false);
    }
}
