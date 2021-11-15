using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Panel_Achievement : MonoBehaviour
{
    [SerializeField] private Image panelImageAchievement = null;
    [SerializeField] private Image imageThumbnail = null;
    [SerializeField] private TMP_Text textTitle = null;
    [SerializeField] private TMP_Text textDescription = null;

    public bool isUnlocked = false;

    [SerializeField] private Achievement achievement = null;

    public void DisplayAchievement(Achievement newAchievement)
    {
        achievement = newAchievement;
        imageThumbnail.sprite = achievement.thumbnail;
        textTitle.text = achievement.title;
        textDescription.text = achievement.description;

        CheckUnlockAchievement();
    }

    public void CheckUnlockAchievement()
    {
        // Check PlayerProgress Object if already Unlocked
        // Change isUnlocked
        PlayerProgress playerProgress = FindObjectOfType<PlayerProgress>();


        if (playerProgress.unlockedAchievements.Contains(achievement.Index))
        {
            panelImageAchievement.color = Color.white;
            imageThumbnail.color = Color.white;
        }
        else
        {
            panelImageAchievement.color = Color.grey;
            imageThumbnail.color = Color.grey;
        }
            
    }

}
