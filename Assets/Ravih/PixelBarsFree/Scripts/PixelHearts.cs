using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HeyHeyThere.PixelBarsFree
{
    /// <summary>
    /// A row of hearts, shields or stars showing value of maxValue, perIcon points to an icon: at 4 a
    /// heart empties by quarters, at 2 by halves (shields and stars by halves at most). When the value
    /// drops, the icons it emptied blink. It sizes its RectTransform to the row, its pivot kept, and
    /// makes its icons as hidden children, in the editor too.
    /// </summary>
    [ExecuteAlways, RequireComponent(typeof(RectTransform))]
    public class PixelHearts : MonoBehaviour
    {
        public enum Icon { Heart, Shield, Star, MiniHeart }

        /// <summary>Each icon's pictures from full to empty, by the share of it left full.</summary>
        static readonly (float share, string name)[][] States =
        {
            new[] { (1f, "heart"), (0.75f, "heart_3q"), (0.5f, "heart_half"), (0.25f, "heart_1q"), (0f, "heart_empty") },
            new[] { (1f, "shield"), (0.5f, "shield_half"), (0f, "shield_empty") },
            new[] { (1f, "star"), (0.5f, "star_half"), (0f, "star_empty") },
            new[] { (1f, "mini_heart"), (0.5f, "mini_heart_half"), (0f, "mini_heart_empty") },
        };
        const float BlinkTime = 0.5f;

        public Icon icon;
        public float maxValue = 12f;
        public float value = 12f;
        [Tooltip("Points an icon holds"), Range(1, 4)]
        public int perIcon = 4;
        [Tooltip("Pixels between icons")]
        public int gap;
        [Tooltip("Icons to a row before the next (0: one row)")]
        public int columns;

        readonly List<(Image picture, Image flash)> icons = new List<(Image, Image)>();
        readonly Dictionary<int, float> blink = new Dictionary<int, float>();
        float last;

        public int Count => Mathf.CeilToInt(maxValue / perIcon);
        Vector2 IconSize => icon == Icon.MiniHeart ? new Vector2(9f, 8f) : new Vector2(16f, 16f);

        /// <summary>The row's size in art pixels.</summary>
        public Vector2 Size
        {
            get
            {
                int n = Count, cols = columns > 0 ? Mathf.Min(n, columns) : n, rows = Mathf.CeilToInt(n / (float)Mathf.Max(1, cols));
                var s = IconSize;
                return new Vector2(cols * s.x + Mathf.Max(0, cols - 1) * gap, rows * s.y + Mathf.Max(0, rows - 1) * gap);
            }
        }

        /// <summary>The share of icon i left full, rounded down to a picture there is.</summary>
        public float Part(int i)
        {
            float s = Mathf.Clamp01((value - i * perIcon) / perIcon);
            foreach (var state in States[(int)icon])
                if (s >= state.share - 0.001f)
                    return state.share;
            return 0f;
        }

        string Picture(float share)
        {
            foreach (var state in States[(int)icon])
                if (Mathf.Approximately(state.share, share))
                    return state.name;
            return null;
        }

        public bool Blinking(int i) => blink.ContainsKey(i);

        void OnEnable()
        {
            if (icons.Count == 0)
                foreach (Transform child in transform)  // an earlier one's, left by a script reload
                    if (child.gameObject.hideFlags == HideFlags.HideAndDontSave)
                        PixelStatBar.Drop(child.gameObject);
            last = value;
            Tick(0f);
        }

        void OnDisable()
        {
            foreach (var (picture, flash) in icons)
                picture.enabled = flash.enabled = false;
        }

        void OnDestroy()
        {
            foreach (var (picture, _) in icons)
                if (picture != null)
                    PixelStatBar.Drop(picture.gameObject);
            icons.Clear();
        }

        void Update() => Tick(Application.isPlaying ? Time.unscaledDeltaTime : 0f);

        /// <summary>Advances the blinks by dt seconds; Update calls it.</summary>
        public void Tick(float dt)
        {
            value = Mathf.Clamp(value, 0f, maxValue);
            int n = Count;
            if (value < last && Application.isPlaying)
                for (int i = 0; i < n; i++)
                    if (Mathf.Clamp01((value - i * perIcon) / perIcon) < Mathf.Clamp01((last - i * perIcon) / perIcon))
                        blink[i] = BlinkTime;
            last = value;
            foreach (var i in new List<int>(blink.Keys))
                if ((blink[i] -= dt) <= 0f)
                    blink.Remove(i);

            var rt = (RectTransform)transform;
            var size = Size;
            if (rt.rect.size != size)
            {
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            }
            while (icons.Count < n)
                icons.Add((Make("Icon"), null));
            for (int i = 0; i < icons.Count; i++)
                if (icons[i].flash == null)
                {
                    var flash = Make("Flash", icons[i].picture.transform);
                    icons[i] = (icons[i].picture, flash);
                }
            var s = IconSize;
            int cols = columns > 0 ? Mathf.Min(n, columns) : n;
            for (int i = 0; i < icons.Count; i++)
            {
                var (picture, flash) = icons[i];
                picture.enabled = i < n && enabled;
                flash.enabled = picture.enabled && blink.TryGetValue(i, out var left) && left % 0.16f > 0.08f;  // lit every other 0.08 s
                if (i >= n)
                    continue;
                var name = Picture(Part(i));
                picture.sprite = PixelStatBar.Load($"Pips/{name}");
                flash.sprite = PixelStatBar.Load($"Pips/flash/{name}");
                picture.rectTransform.anchoredPosition = new Vector2(i % cols * (s.x + gap), -(i / cols) * (s.y + gap));
                picture.rectTransform.sizeDelta = flash.rectTransform.sizeDelta = s;
            }
        }

        Image Make(string name, Transform parent = null)
        {
            var go = new GameObject(name, typeof(RectTransform)) { hideFlags = HideFlags.HideAndDontSave, layer = gameObject.layer };
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent ? parent : transform, false);
            rt.anchorMin = rt.anchorMax = rt.pivot = Vector2.up;
            var image = go.AddComponent<Image>();
            image.raycastTarget = false;
            image.color = parent ? new Color(1f, 1f, 1f, 0.85f) : Color.white;
            return image;
        }
    }
}
