using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EffectorSelector : MonoBehaviour
{
    private Button _button;
    public Effector prefab;
    public TextMeshProUGUI count;
    public GameObject selectedPanel;

    private int _count;
    public int index;

    public void Init(int startingAmount, int idx, UnityAction callback)
    {
        _button = GetComponent<Button>();
        _button.onClick.RemoveAllListeners();
        index = idx;
        _count = startingAmount;
        count.text = startingAmount.ToString();
        _button.onClick.AddListener(callback);
    }

    public void Decrement()
    {
        _count -= 1;
        count.text = _count.ToString();
    }

    public void Increment()
    {
        _count += 1;
        count.text = _count.ToString();
    }

    public int GetCount()
    {
        return _count;
    }
    
    public void Select()
    {
        selectedPanel.SetActive(true);
    }

    public void Deselect()
    {
        selectedPanel.SetActive(false);
    }
}
