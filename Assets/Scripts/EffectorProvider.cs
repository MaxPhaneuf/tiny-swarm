using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectorProvider : MonoBehaviour
{
    public Clicker clicker;
   
    public List<EffectorSelector> selectors = new();

    private int _currentSelector;
    private void Start()
    {
        clicker.onClick.AddListener(Create);
    }

    private void Select(EffectorSelector selector)
    {
        selectors[_currentSelector].Deselect();
        _currentSelector = selector.index;
        selector.Select();
    }

    public void Init(int startingAmount)
    {
        for (int i = 0; i < selectors.Count; i++)
        {
            var selector = selectors[i];
            selectors[i].Init(startingAmount, i, () => Select(selector));  
        }
        selectors[_currentSelector].Select();
    }

    public static void RemoveFood()
    {
        var toRemove = new List<Effector>();
        foreach (var effector in BoidProvider.Effectors)
        {
            if (effector.type == EffectorType.Food)
            {
                toRemove.Add(effector);
            }
        }
        foreach (var effector in toRemove)
        {
            BoidProvider.Effectors.Remove(effector);
            Destroy(effector.gameObject);
        }
        toRemove.Clear();
    }
    
    private void Create(Vector3 position)
    {
        if (selectors[_currentSelector].GetCount() <= 0)
        {
            return;
        }
        selectors[_currentSelector].Decrement();
        if(!CheckInventory())
            StartCoroutine(nameof(WaitToEnd));
        var obj = Instantiate(selectors[_currentSelector].prefab, position, Quaternion.identity);
        BoidProvider.Effectors.Add(obj);
    }

    public IEnumerator WaitToEnd()
    {
        float current = 0;
        float duration = 5;
        while (current < duration)
        {
            current += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        GameManager.Instance.FinishedLevel();
    }
    
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1) || Input.GetKeyUp(KeyCode.Keypad1))
        {
            Select(selectors[0]);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2) || Input.GetKeyUp(KeyCode.Keypad2))
        {
            Select(selectors[1]);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3) || Input.GetKeyUp(KeyCode.Keypad3))
        {
            Select(selectors[2]);
        }
    }

    private bool CheckInventory()
    {
        var has = false;
        foreach (var selector in selectors)
        {
            if (selector.GetCount() > 0)
                has = true;
        }

        return has;
    }
}
