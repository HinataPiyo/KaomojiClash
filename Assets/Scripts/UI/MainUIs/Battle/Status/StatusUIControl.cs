using System;
using UnityEngine;
using UnityEngine.UIElements;

public class StatusUIControl : MonoBehaviour 
{
    UIDocument uiDoc;

    VisualElement[] mentalIcons;
    ProgressBar healthBar;

    void Awake()
    {
        uiDoc = GetComponent<UIDocument>();

        var box = uiDoc.rootVisualElement.Q("status-resource");
        mentalIcons = box.Q("mental").Query<VisualElement>().ToList().ToArray();
        healthBar = box.Q<ProgressBar>("health");
    }
    
#region Mental
    public void UpdateMental(int currentMental)
    {
        for (int i = 0; i < mentalIcons.Length; i++)
        {
            if (i <= currentMental)
            {
                mentalIcons[i].style.visibility = Visibility.Visible;
            }
            else
            {
                mentalIcons[i].style.visibility = Visibility.Hidden;
            }
        }
    }
#endregion

#region Health
    public void SetMaxHealth(float maxHealth)
    {
        healthBar.highValue = maxHealth;
        healthBar.value = maxHealth;
        healthBar.title = $"{maxHealth}/{maxHealth}";
    }

    public void UpdateHealth(float currentHealth)
    {
        healthBar.value = currentHealth;
        healthBar.title = $"{currentHealth}/{healthBar.highValue}";
    }
#endregion


}