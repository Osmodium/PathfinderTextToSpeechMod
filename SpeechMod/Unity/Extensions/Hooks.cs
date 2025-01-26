using System;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace SpeechMod.Unity.Extensions;

public static class Hooks
{
	public static void HookUpTextToSpeechOnTransformWithPath(string path, bool force = false)
	{
		var transform = UIHelper.TryFind(path);
		if (transform == null)
		{
			Debug.LogWarning($"GameObject on '{path}' was not found!");
			return;
		}

		HookupTextToSpeechOnTransform(transform, force);
	}

	public static void HookupTextToSpeechOnTransform(this Transform transform, bool force = false)
	{
		if (transform == null)
		{
			Debug.LogWarning("Can't hook up text to speech on null transform!");
			return;
		}

		var path = transform.GetGameObjectPath();

#if DEBUG
		//Debug.Log($"Attempting to get TextMeshProUGUI in children on '{path}'...");
#endif

		var allTexts = transform.GetComponentsInChildren<TextMeshProUGUI>(true);
		if (allTexts?.Length == 0)
		{
			Debug.LogWarning($"No TextMeshProUGUI found in children on '{path}'!");
			return;
		}

#if DEBUG
		//Debug.Log($"Found {allTexts?.Length} TextMeshProUGUIs on '{path}'!");
#endif

		allTexts.HookupTextToSpeech(force);
	}

	public static void HookupTextToSpeech(this TextMeshProUGUI[] textMeshPros, bool force = false)
	{
		if (textMeshPros == null)
		{
			Debug.LogWarning("No TextMeshProUGUIs to hook up!");
			return;
		}

		foreach (var textMeshPro in textMeshPros)
		{
			textMeshPro.HookupTextToSpeech(force);
		}
	}

	public static void HookupTextToSpeech(this TextMeshProUGUI textMeshPro, bool force = false)
	{
		if (textMeshPro == null)
		{
			Debug.LogWarning("No TextMeshProUGUI!");
			return;
		}

		var textMeshProTransform = textMeshPro.transform;
		if (textMeshProTransform == null)
		{
			Debug.LogWarning("Transform on TextMeshProUGUI is null!");
			return;
		}

		// If the parent is clickable, don't hook up the text to speech, unless forced.
		if (!force && textMeshProTransform.IsParentClickable())
		{
			return;
		}

		// Add the hook data to the text mesh pro. If it already exists, return.
		if (!textMeshProTransform.gameObject.TryAddComponent<TextMeshProHookData>(out var hookData))
			return;

		// Save the text style, color and extra padding
		hookData.FontStyles = textMeshPro.fontStyle;
		hookData.Color = textMeshPro.color;
		hookData.ExtraPadding = textMeshPro.extraPadding;

		// Make the text mesh pro clickable
		textMeshPro.raycastTarget = true;

		// Subscribe to the pointer enter, and add it to the disposables for ensuring it gets cleaned up.
		textMeshPro.OnPointerEnterAsObservable()
			.Subscribe(
			_ =>
			{
				// Update the text style, color and extra padding
				hookData.FontStyles = textMeshPro.fontStyle;
				hookData.Color = textMeshPro.color;
				hookData.ExtraPadding = textMeshPro.extraPadding;

				if (Main.Settings!.FontStyleOnHover)
				{
					for (var i = 0; i < Main.Settings.FontStyles!.Length; i++)
					{
						if (Main.Settings.FontStyles[i])
						{
							textMeshPro.fontStyle ^=
								(FontStyles)Enum.Parse(typeof(FontStyles), Main.FontStyleNames![i]!, true);
						}
					}

					textMeshPro.extraPadding = false;
				}

				if (Main.Settings.ColorOnHover)
				{
					textMeshPro.color = UIHelper.HoverColor;
				}
			}
		).AddTo(hookData.Disposables);

		// Subscribe to the pointer exit, and add it to the disposables for ensuring it gets cleaned up.
		textMeshPro.OnPointerExitAsObservable().Subscribe(
			_ =>
			{
				textMeshPro.fontStyle = hookData.FontStyles;
				textMeshPro.color = hookData.Color;
				textMeshPro.extraPadding = hookData.ExtraPadding;
			}
		).AddTo(hookData.Disposables);

		// Subscribe to the pointer click, and add it to the disposables for ensuring it gets cleaned up.
		textMeshPro.OnPointerClickAsObservable().Subscribe(
			clickEvent =>
			{
				if (clickEvent?.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left)
				{
					Main.Speech?.Speak(textMeshPro.text);
				}
			}
		).AddTo(hookData.Disposables);
	}
}