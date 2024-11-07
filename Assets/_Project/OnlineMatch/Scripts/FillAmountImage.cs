using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class FillAmountImage : MonoBehaviour
{
	[SerializeField] private Image image;

	private void Start()
	{
		image.color = PhotonNetwork.LocalPlayer.GetTeam().TeamColor;
	}

	public void SetFillAmount(float fillAmount)
	{
		image.fillAmount = fillAmount;
	}
}
