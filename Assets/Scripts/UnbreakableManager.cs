 using System;
 using UnityEngine;
using System.Collections;

public class UnbreakableManager : MonoBehaviour
{
 private static UnbreakableManager _manager;

 public enum OrangeFishChosen
 {
  Undecided,
  OrangeBrideChosen,
  OrangeGroomChosen,
 };

 public enum GreenFishChosen
 {
  Undecided,
  GreenBrideChosen,
  GreenGroomChosen
 };

 public static OrangeFishChosen OrangeFeesh;
 public static GreenFishChosen GreenFeesh;

 void Awake()
 {
  if (_manager == null)
  {
   DontDestroyOnLoad(this);
  }
  else
  {
   Destroy(gameObject);
  }
 }
}
  
    
