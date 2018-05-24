using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
	public int ButtonIndex;
	private Button _butt;

	void Start()
	{
		//UnbreakableManager.Feesh
	}

	// Update is called once per frame
	public void ChooseAFish()
	{
		if (UnbreakableManager.OrangeFeesh == UnbreakableManager.OrangeFishChosen.Undecided)
		{
			if (ButtonIndex == 1)
			{
				UnbreakableManager.OrangeFeesh = UnbreakableManager.OrangeFishChosen.OrangeBrideChosen;
			}

			if (ButtonIndex == 2)
			{
				UnbreakableManager.OrangeFeesh = UnbreakableManager.OrangeFishChosen.OrangeGroomChosen;
			}
		}

		if (UnbreakableManager.GreenFeesh == UnbreakableManager.GreenFishChosen.Undecided)
		{
			if (ButtonIndex == 3)
			{
				UnbreakableManager.GreenFeesh = UnbreakableManager.GreenFishChosen.GreenBrideChosen;
			}

			if (ButtonIndex == 4)
			{
				UnbreakableManager.GreenFeesh = UnbreakableManager.GreenFishChosen.GreenGroomChosen;
			}
		}	
	}
}