using UnityEngine;
using UnityEngine.UI;

public class InfoPopup : MonoBehaviour
{
   private Animator _animator;
   public Text text;
   private void Start()
   {
      _animator = GetComponent<Animator>();
      _animator.Play("WavePopup");
   }

   public void NewWave(int wave)
   {
      _animator.Play("WavePopup");
      text.text = "WAVE " + wave;
   }
   
}
