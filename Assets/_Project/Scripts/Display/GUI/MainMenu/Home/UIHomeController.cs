using System;
using UnityEngine;
using UnityEngine.UI;

public class UIHomeController : MonoBehaviour
{
    [SerializeField] private CharacterSelector characterSelector;
    public CharacterSelector CharacterSelector => characterSelector;
}
