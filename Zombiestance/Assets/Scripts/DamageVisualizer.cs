using UnityEngine;
using UnityEngine.UI;

public class DamageVisualizer : MonoBehaviour
{
    public Image leftDamageImage;
    public Image rightDamageImage;
    public Image backDamageImage;
    public Image frontDamageImage;
    
    private bool _isTakingDamageLeft = true;
    private bool _isTakingDamageRight = true;
    private bool _isTakingDamageBack = true;
    private bool _isTakingDamageFront = true;
    private float _timeDelayMultiplier;
    private Color _healthyColor;
    private Color _damageColor;
    
    private void Start()
    {
        _isTakingDamageLeft = false;
        _isTakingDamageRight = false;
        _isTakingDamageBack = false;
        _isTakingDamageFront = false;
        _healthyColor = new Color(255f, 255f, 255f, 0f);
        _damageColor = new Color(255f, 255f, 255f, 255f);
        _timeDelayMultiplier = 6f;
    }

    private void Update()
    {
        var time = _timeDelayMultiplier * Time.deltaTime;
        
        if (_isTakingDamageLeft)
        {
            leftDamageImage.color = _damageColor;
            _isTakingDamageLeft = false;
        }
        else
        {
            leftDamageImage.color = Color.Lerp(leftDamageImage.color, _healthyColor, time);
            _isTakingDamageLeft = false;
        }
        
        if (_isTakingDamageRight)
        {
            rightDamageImage.color = _damageColor;
            _isTakingDamageRight = false;
        }
        else
        {
            rightDamageImage.color = Color.Lerp(rightDamageImage.color, _healthyColor, time);
        }
        
        if (_isTakingDamageBack)
        {
            backDamageImage.color = _damageColor;
            _isTakingDamageBack = false;
        }
        else
        {
            backDamageImage.color = Color.Lerp(backDamageImage.color, _healthyColor, time);
        }
        
        if (_isTakingDamageFront)
        {
            frontDamageImage.color = _damageColor;
            _isTakingDamageFront = false;
        }
        else
        {
            frontDamageImage.color = Color.Lerp(frontDamageImage.color, _healthyColor, time);
        }
    }
    
    public void DamageLeft()
    {
        _isTakingDamageLeft = true;
    }

    public void DamageRight()
    {
        _isTakingDamageRight = true;
    }
    
    public void DamageBack()
    {
        _isTakingDamageBack = true;
    }
    
    public void DamageFront()
    {
        _isTakingDamageFront = true;
    }
}
