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
    public void SetMaxHealth(float health)
    {
        healthBar.highValue = health;
        healthBar.value = health;
        healthBar.title = health.ToString("N0") + "/" + health.ToString("N0");
    }

    public void UpdateHealth(float currentHealth)
    {
        healthBar.value = currentHealth;
        healthBar.title = currentHealth.ToString("N0") + "/" + healthBar.highValue.ToString("N0");
    }
#endregion


}