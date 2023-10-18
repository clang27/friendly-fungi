using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Handle: MonoBehaviour {
    private Tweener _handleColorTween, _handleSizeTween;
    private Color _handleColor;
    private Image _handleImage;
    
    private void Awake() {
        _handleImage = GetComponent<Image>();
        _handleColor = _handleImage.color;
    }

    private void OnEnable() {
        PulsateDropdownHandle();
    }

    private void OnDisable() {
        _handleColorTween.Kill();
        _handleSizeTween.Kill();
    }

    private void PulsateDropdownHandle() {
        _handleImage.color = _handleColor;
        _handleImage.transform.localScale = Vector3.one;
			
        Color.RGBToHSV(_handleColor, out var h, out var s, out var v);
			
        _handleColorTween = DOVirtual.Float(s, 0.7f, 1f, 
            (f) => _handleImage.color = Color.HSVToRGB(h, f, v)).SetLoops(-1, LoopType.Yoyo);

        _handleSizeTween = _handleImage.transform.DOScaleX(1.4f, 1f).SetLoops(-1, LoopType.Yoyo);
    }
}
