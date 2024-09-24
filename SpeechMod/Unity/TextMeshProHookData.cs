using TMPro;
using UniRx;
using UnityEngine;

namespace SpeechMod.Unity;

public class TextMeshProHookData : MonoBehaviour
{
	public CompositeDisposable Disposables = new();

	public FontStyles FontStyles { get; set; }
    public Color Color { get; set; }
    public bool ExtraPadding { get; set; }

	private void OnDestroy()
	{
		Disposables.Dispose();
	}
}