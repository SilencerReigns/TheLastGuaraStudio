using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HeyHeyThere.PixelBarsFree
{
    /// <summary>
    /// The demo: the kit in a game's HUD, as the Godot demo has it. Hearts, health, mana and stamina
    /// bars, a boss bar, a party panel, status columns, two orbs, cooldown rings and a segmented XP bar.
    /// The buttons hit, heal, cast and strike; the row under them swaps the frame style. It builds its
    /// canvas on Start, at the largest whole scale that fits 240 by 420 art pixels on screen.
    /// </summary>
    public class BarsDemo : MonoBehaviour
    {
        public static readonly string[] AllStyles = { "wood", "stone", "sky", "forest", "royal", "ember", "gilded", "cyan" };
        public static readonly string[] AllFills = { "health", "mana", "stamina", "xp", "shield", "rage", "magic", "poison" };
        public const int ArtHeight = 240, ArtWidth = 420;  // narrower, the boss bar meets the party panel
        const float Cooldown = 2.5f;  // seconds a ring takes to refill
        static readonly Color Light = new Color(0.94f, 0.92f, 0.86f);

        [Tooltip("The kit's pixel font, Font/; the built-in font if none")]
        public Font font;
        public Canvas canvas;
        public PixelHearts hearts;
        public PixelStatBar hp, mp, stamina, boss, hpOrb, mpOrb, xp;
        public readonly List<PixelStatBar> bars = new List<PixelStatBar>(), rings = new List<PixelStatBar>(), mobs = new List<PixelStatBar>();
        public string style;

        readonly List<(string name, Image image)> styleButtons = new List<(string, Image)>();
        readonly List<Image> buttons = new List<Image>();
        Vector2Int seen;  // the screen's size, to build afresh when it changes

        /// <summary>The largest whole number that fits ArtWidth by ArtHeight art pixels in a screen this size.</summary>
        public static int Scale(int width, int height) => Mathf.Max(1, Mathf.Min(width / ArtWidth, height / ArtHeight));

        /// <summary>Those of the names this kit has: the free sampler has one style and three fills.</summary>
        public static string[] Styles => AllStyles.Where(s => PixelStatBar.Load($"Frames/{s}/bar") != null).ToArray();
        public static string[] Fills => AllFills.Where(f => PixelStatBar.Load($"Fills/{f}/bar") != null).ToArray();
        static string SomeFill(string name) => Fills.Contains(name) ? name : Fills[0];

        void Start()
        {
            DemoInput.Ensure();
            seen = new Vector2Int(Screen.width, Screen.height);
            Build(seen.x, seen.y);
        }

        /// <summary>The HUD for a screen of this size, afresh.</summary>
        public void Build(int width, int height)
        {
            if (canvas)
                DestroyImmediate(canvas.gameObject);
            bars.Clear();
            rings.Clear();
            mobs.Clear();
            styleButtons.Clear();
            buttons.Clear();
            if (!font)
                font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
            canvas.transform.SetParent(transform, false);
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = true;
            var scaler = canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.referencePixelsPerUnit = 100;
            scaler.scaleFactor = Scale(width, height);
            float w = Mathf.Floor(width / (float)Scale(width, height));
            style = Styles[0];
            Player();
            Boss(w);
            Party(w);
            Status();
            Mobs(w);
            Bottom(w);
            Buttons(w);
            foreach (var b in bars)  // laid as set, for a HUD built outside Play mode
                b.Tick(0f);
            hearts.Tick(0f);
        }

        void Update()
        {
            if (seen != new Vector2Int(Screen.width, Screen.height))
            {
                seen = new Vector2Int(Screen.width, Screen.height);
                Build(seen.x, seen.y);
            }
            float dt = Time.deltaTime;
            foreach (var r in rings)
                r.value += dt * 100f / Cooldown;
            stamina.value += dt * 12f;
            mp.value += dt * 3f;
            mpOrb.value = mp.value;
        }

        public void Hit()
        {
            hp.value -= 22f;
            hpOrb.value = hp.value;
            hearts.value -= 3f;
        }

        public void Heal()
        {
            hp.value += 30f;
            hpOrb.value = hp.value;
            hearts.value += 4f;
        }

        public void Cast()
        {
            foreach (var r in rings)
                if (r.value >= r.maxValue)
                {
                    r.value = 0f;
                    mp.value -= 20f;
                    mpOrb.value = mp.value;
                    stamina.value -= 30f;
                    xp.value = (xp.value + 7f) % 100f;
                    return;
                }
        }

        public void Strike()
        {
            boss.value = boss.value > 9f ? boss.value - 9f : 100f;
            foreach (var m in mobs)
                m.value = m.value > 15f ? m.value - 15f : 100f;
        }

        public void SetStyle(string name)
        {
            style = name;
            foreach (var b in bars)
                b.style = name;
            var frame = PixelStatBar.Load($"Frames/{name}/bar");
            foreach (var b in buttons)
                b.sprite = frame;
            foreach (var (n, image) in styleButtons)
                image.color = n == name ? Color.white : new Color(0.6f, 0.6f, 0.6f);
        }

        void Player()
        {
            hearts = Add<PixelHearts>("Hearts", 6, 6, 0, 0);
            hearts.maxValue = 20f;
            hearts.value = 14f;
            hp = Bar(PixelStatBar.Shape.Bar, "health", 6, 26, 100, 80);
            hp.low = 0.3f;
            mp = Bar(PixelStatBar.Shape.Bar, "mana", 6, 40, 80, 65);
            stamina = Bar(PixelStatBar.Shape.Thin, "stamina", 6, 54, 64, 100);
        }

        void Boss(float w)
        {
            boss = Bar(PixelStatBar.Shape.Boss, "health", Mathf.Round(w / 2 - 89), 8, 176, 90);
            boss.segments = 4;
            Label("THE HOLLOW KING", w / 2 + 8, 25, TextAnchor.UpperCenter);
        }

        void Party(float w)
        {
            var members = new[] { ("Ayla", 90f, 40f), ("Bram", 35f, 70f), ("Cyn", 60f, 100f) };
            for (int i = 0; i < members.Length; i++)
            {
                var (name, health, mana) = members[i];
                float y = 6 + i * 14;
                Label(name, w - 120, y - 1, TextAnchor.UpperLeft);
                Bar(PixelStatBar.Shape.Bar, "health", w - 92, y, 52, health);
                Bar(PixelStatBar.Shape.Bar, "mana", w - 38, y, 36, mana);
            }
        }

        void Status()
        {
            var shown = new[] { "rage", "shield", "magic", "poison" }.Where(Fills.Contains).ToArray();
            if (shown.Length == 0)  // the sampler's
                shown = Fills;
            float x = 6;
            for (int i = 0; i < shown.Length; i++)
            {
                var b = Bar(PixelStatBar.Shape.Vertical, shown[i], x, 86, 56, new[] { 70f, 45f, 90f, 25f }[i % 4]);
                b.segments = 4;
                Label(shown[i].Substring(0, 1).ToUpper(), x + 6, 142, TextAnchor.UpperCenter);
                x += 16;
            }
        }

        /// <summary>Bars over the enemies' heads: thin ones, a shield or poison under two.</summary>
        void Mobs(float w)
        {
            float x = Mathf.Round(w / 2 - 120);
            foreach (var (name, fill, value, extra) in new[] { ("Slime", "health", 100f, ""), ("Knight", "health", 70f, "shield"), ("Wisp", "magic", 45f, "poison") })
            {
                Label(name, x + 24, 105, TextAnchor.UpperCenter);
                mobs.Add(Bar(PixelStatBar.Shape.Thin, fill, x, 118, 48, value));
                if (extra != "" && Fills.Contains(extra))
                    mobs.Add(Bar(PixelStatBar.Shape.Thin, extra, x, 126, 48, 60));
                x += 96;
            }
        }

        void Bottom(float w)
        {
            hpOrb = Bar(PixelStatBar.Shape.Orb, "health", 6, 168, 0, 80);
            mpOrb = Bar(PixelStatBar.Shape.Orb, "mana", w - 70, 168, 0, 65);
            hpOrb.transform.localScale = mpOrb.transform.localScale = new Vector3(2f, 2f, 1f);
            var fills = new[] { "magic", "mana", "rage", "shield" };
            for (int i = 0; i < 4; i++)
            {
                var r = Bar(PixelStatBar.Shape.Ring, fills[i], Mathf.Round(w / 2 - 52 + i * 28), 196, 0, 100);
                r.trail = r.flash = false;
                rings.Add(r);
                Label($"{i + 1}", Mathf.Round(w / 2 - 52 + i * 28) + 10, 200, TextAnchor.UpperCenter);
            }
            xp = Bar(PixelStatBar.Shape.Bar, "xp", Mathf.Round(w / 2 - 110), 222, 220, 40);
            xp.segments = 10;
            xp.trail = xp.flash = false;
        }

        void Buttons(float w)
        {
            Row(w / 2, 44, 4, new (string, UnityAction)[] { ("Hit", Hit), ("Heal", Heal), ("Cast", Cast), ("Strike boss", Strike) });
            var styles = Styles;
            var made = Row(w / 2, 66, 2, styles.Select(s => (char.ToUpper(s[0]) + s.Substring(1), (UnityAction)(() => SetStyle(s)))).ToArray());
            for (int i = 0; i < styles.Length; i++)
                styleButtons.Add((styles[i], made[i]));
            SetStyle(style);
        }

        /// <summary>Buttons cut from the style's bar frame, in a row centred on x.</summary>
        List<Image> Row(float centre, float y, float gap, (string text, UnityAction click)[] items)
        {
            var made = items.Select(item =>
            {
                var image = Add<Image>(item.text, 0, y, 0, 14);
                image.type = Image.Type.Sliced;
                var button = image.gameObject.AddComponent<Button>();
                button.onClick.AddListener(item.click);
                var colors = button.colors;
                colors.normalColor = colors.selectedColor = new Color(0.88f, 0.88f, 0.88f);
                colors.highlightedColor = Color.white;
                colors.pressedColor = new Color(0.65f, 0.65f, 0.65f);
                button.colors = colors;
                var text = MakeText(item.text, image.transform, TextAnchor.MiddleCenter);
                Full(text.rectTransform);
                image.rectTransform.sizeDelta = new Vector2(Mathf.Ceil(text.preferredWidth) + 10, 14);
                buttons.Add(image);
                return image;
            }).ToList();
            float x = Mathf.Round(centre - (made.Sum(b => b.rectTransform.sizeDelta.x) + gap * (made.Count - 1)) / 2);
            foreach (var b in made)
            {
                b.rectTransform.anchoredPosition = new Vector2(x, -y);
                x += b.rectTransform.sizeDelta.x + gap;
            }
            return made;
        }

        PixelStatBar Bar(PixelStatBar.Shape shape, string fill, float x, float y, float length, float value)
        {
            var size = shape switch
            {
                PixelStatBar.Shape.Vertical => new Vector2(12, length),
                PixelStatBar.Shape.Thin => new Vector2(length, 7),
                PixelStatBar.Shape.Boss => new Vector2(length, 16),
                PixelStatBar.Shape.Orb => new Vector2(32, 32),
                PixelStatBar.Shape.Ring => new Vector2(20, 20),
                _ => new Vector2(length, 12),
            };
            var b = Add<PixelStatBar>($"{fill} {shape}", x, y, size.x, size.y);
            b.shape = shape;
            b.style = style;
            b.fill = SomeFill(fill);
            b.value = value;
            b.Settle();
            bars.Add(b);
            return b;
        }

        Text Label(string text, float x, float y, TextAnchor anchor)
        {
            var t = MakeText(text, canvas.transform, anchor);
            var rt = t.rectTransform;
            rt.anchorMin = rt.anchorMax = Vector2.up;
            rt.pivot = new Vector2(anchor == TextAnchor.UpperCenter ? 0.5f : 0f, 1f);
            rt.anchoredPosition = new Vector2(x, -y);
            rt.sizeDelta = new Vector2(Mathf.Ceil(t.preferredWidth), 12);
            var shadow = t.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.8f);
            shadow.effectDistance = new Vector2(1f, -1f);
            return t;
        }

        Text MakeText(string text, Transform parent, TextAnchor anchor)
        {
            var t = new GameObject(text, typeof(RectTransform)).AddComponent<Text>();
            t.transform.SetParent(parent, false);
            t.font = font;
            t.fontSize = font.dynamic ? 10 : font.fontSize;  // a bitmap font at its own size
            t.color = Light;
            t.alignment = anchor;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.raycastTarget = false;
            t.text = text;
            return t;
        }

        /// <summary>A component on a new object under the canvas, its top left at x, y art pixels down from the top left.</summary>
        T Add<T>(string name, float x, float y, float w, float h) where T : Component
        {
            var rt = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rt.SetParent(canvas.transform, false);
            rt.anchorMin = rt.anchorMax = rt.pivot = Vector2.up;
            rt.anchoredPosition = new Vector2(x, -y);
            rt.sizeDelta = new Vector2(w, h);
            return rt.gameObject.AddComponent<T>();
        }

        static void Full(RectTransform rt)
        {
            rt.anchorMin = rt.offsetMin = rt.offsetMax = Vector2.zero;
            rt.anchorMax = Vector2.one;
        }
    }
}
