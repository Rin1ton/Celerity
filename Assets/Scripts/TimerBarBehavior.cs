using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerBarBehavior : MonoBehaviour
{

	Slider mySlider;
	public GameObject myFillBar;
	public GameObject myBackGround;
	public bool isRacing => myFillBar.activeSelf;

	private void Awake()
	{
		References.theTimerBar = this;
		mySlider = GetComponent<Slider>();
	}

	// Start is called before the first frame update
	void Start()
	{
		Hide();
	}

	public void SetValue(float val)
	{
		mySlider.value = val;
	}

	public void Show()
	{
		myFillBar.SetActive(true);
		myBackGround.SetActive(true);
	}

	public void Hide()
	{
		myFillBar.SetActive(false);
		myBackGround.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
