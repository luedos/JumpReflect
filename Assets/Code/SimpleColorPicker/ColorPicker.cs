﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.SimpleColorPicker.Scripts
{
	/// <summary>
	/// Color picker window representation.
	/// </summary>
	public class ColorPicker : MonoBehaviour
	{
		public Color Color;
		public ColorMode ColorMode;
		public ColorJoystick ColorJoystick;
		public Image Gradient;
		public RectTransform RectTransform;
		public Slider Hue;
		public ColorSlider R, G, B, H, S, V, A;
		public InputField Hex;
		public Image CompareLook; // [0] is old color, [1] is new color.
		public Image TransparencyLook;
		public Text Mode;
		public GameObject RgbSliders;
		public GameObject HsvSliders;
		public bool Locked;

        LevelCreator.ITextureDataHolder myTextureHolder;

        [HideInInspector] public Texture2D Texture;


		/// <summary>
		/// Called on app start if script is enabled.
		/// </summary>
		public void Start()
		{
            if (Texture != null)
                return;
			Texture = new Texture2D(128, 128) { filterMode = FilterMode.Point };
			Gradient.sprite = Sprite.Create(Texture, new Rect(0f, 0f, Texture.width, Texture.height), new Vector2(0.5f, 0.5f), 100f);
		}


        public void OpenOnTextureHolder(LevelCreator.ITextureDataHolder inTextureHolder)
        {
            if (Texture == null)
            {
                Texture = new Texture2D(128, 128) { filterMode = FilterMode.Point };
                Gradient.sprite = Sprite.Create(Texture, new Rect(0f, 0f, Texture.width, Texture.height), new Vector2(0.5f, 0.5f), 100f);
            }
            if (inTextureHolder == null)
                return;
            myTextureHolder = inTextureHolder;
            SetColor(inTextureHolder.GetColor());
        }


		/// <summary>
		/// Set color picker RGB color.
		/// </summary>
		public void SetColor(Color color, bool picker = true, bool sliders = true, bool hex = true, bool hue = true)
		{
			float h, s, v;

			Color.RGBToHSV(color, out h, out s, out v);
			SetColor(h, s, v, color.a, picker, sliders, hex, hue);
		}

		/// <summary>
		/// Set color picker HSV color.
		/// </summary>
		public void SetColor(float h, float s, float v, float a, bool picker = true, bool sliders = true, bool hex = true, bool hue = true)
		{
			var color = Color.HSVToRGB(h, s, v);

			color.a = a;

			Color = TransparencyLook.color = CompareLook.color = color;

            if (myTextureHolder != null)
                myTextureHolder.SetColor(Color);

			ColorJoystick.Center.color = new Color(Color.r, Color.g, Color.b);
			Locked = true;

			if (sliders || ColorMode == ColorMode.Hsv)
			{
				R.Set(Color.r);
				G.Set(Color.g);
				B.Set(Color.b);
			}

			if (sliders || ColorMode == ColorMode.Rgb)
			{
				H.Set(h);
				S.Set(s);
				V.Set(v);
			}

			A.Set(Color.a);

			if (hue) Hue.value = h;
			if (hex) Hex.text = ColorUtility.ToHtmlStringRGBA(Color);
			if (picker) ColorJoystick.transform.localPosition = new Vector2(s * Texture.width / Texture.width * RectTransform.rect.width, v * Texture.height / Texture.height * RectTransform.rect.height);

			Locked = false;
			UpdateGradient();
		}

		/// <summary>
		/// Called when HUE changed.
		/// </summary>
		public void OnHueShanged(float value)
		{
			if (Locked) return;

			float h, s, v;

			Color.RGBToHSV(Color, out h, out s, out v);

			h = value;
			SetColor(h, s, v, A.Value, hue: false);
		}

		/// <summary>
		/// Called whed slider changed.
		/// </summary>
		public void OnSliderChanged()
		{
			if (Locked) return;

			if (ColorMode == ColorMode.Rgb)
			{
				SetColor(new Color(R.Value, G.Value, B.Value, A.Value), sliders: false);
			}
			else
			{
				SetColor(H.Value, S.Value, V.Value, A.Value, sliders: false);
			}
		}

		/// <summary>
		/// Called when HEX code changed.
		/// </summary>
		public void OnHexValueChanged(string value)
		{
			if (Locked) return;

			value = Regex.Replace(value.ToUpper(), "[^0-9A-F]", "");

			Hex.text = value;

			Color color;

			if (ColorUtility.TryParseHtmlString("#" + value, out color))
			{
				SetColor(color, hex: false);
			}
		}

		/// <summary>
		/// Switch mode RGB/HSV.
		/// </summary>
		public void SwitchMode()
		{
			ColorMode = ColorMode == ColorMode.Rgb ? ColorMode.Hsv : ColorMode.Rgb;
			SetMode(ColorMode);
		}

		/// <summary>
		/// Set mode RGB/HSV.
		/// </summary>
		public void SetMode(ColorMode mode)
		{
			RgbSliders.SetActive(mode == ColorMode.Rgb);
			HsvSliders.SetActive(mode == ColorMode.Hsv);
			Mode.text = mode == ColorMode.Rgb ? "HSV" : "RGB";
		}

		private void UpdateGradient()
		{
			var pixels = new List<Color>();

			for (var y = 0; y < Texture.height; y++)
			{
				for (var x = 0; x < Texture.width; x++)
				{
					pixels.Add(Color.HSVToRGB(Hue.value, (float) x / Texture.width, (float) y / Texture.height));
				}
			}

			Texture.SetPixels(pixels.ToArray());
			Texture.Apply();
		}
	}
}