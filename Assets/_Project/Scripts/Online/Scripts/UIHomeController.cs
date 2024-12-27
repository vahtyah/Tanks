using System;
using UnityEngine;
using UnityEngine.UI;

public class UIHomeController : MonoBehaviour
{
    [SerializeField] private CharacterSelector characterSelector;
    [SerializeField] private FindMatchUI findMatchUI;

    public CharacterSelector CharacterSelector => characterSelector;
}
