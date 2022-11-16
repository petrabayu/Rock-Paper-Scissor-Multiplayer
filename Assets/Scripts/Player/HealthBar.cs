using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
    public Image image;
    public void UpadateBar(float fillAmount)
    {
        image.DOFillAmount(fillAmount, 0.25f);
        if (fillAmount > 0.6)
        {
            image.DOColor(Color.green, 0.25f);
        }
        else if (fillAmount > 0.4)
        {
            image.DOColor(Color.yellow, 0.25f);
        }
        else
        {
            image.DOColor(Color.red, 0.25f);
        }
    }
}
