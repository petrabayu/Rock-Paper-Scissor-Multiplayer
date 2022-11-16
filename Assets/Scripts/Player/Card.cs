using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Attack AttackValue;
    public CardPlayer player;
    public Transform atkPosRef;
    public Vector2 originalPosition;
    Vector2 originalScale;
    Color originalColor;

    bool isClickable = true;


    private void Start()
    {
        originalPosition = this.transform.position;
        originalScale = this.transform.localScale;
        originalColor = GetComponent<Image>().color;
    }
    public void Onclick()
    {
        if (isClickable)
            player.SetChoosenCard(this);
    }

    internal void Reset()
    {
        transform.position = originalPosition;
        transform.localScale = originalScale;
        GetComponent<Image>().color = originalColor;
    }
    internal void AnimateAttack()
    {
        transform.DOMove(atkPosRef.position, 1);
    }

    public void setClickable(bool value)
    {
        isClickable = value;
    }

}
