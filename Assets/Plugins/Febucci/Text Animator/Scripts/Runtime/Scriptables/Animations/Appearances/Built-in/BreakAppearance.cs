using Febucci.UI.Core;
using Febucci.UI.Effects;
using System.Collections.Generic;
using UnityEngine;

namespace Febucci.UI.Effects
{
    [UnityEngine.Scripting.Preserve]
    [CreateAssetMenu(fileName = "Break Appearance", menuName = "Text Animator/Animations/Appearances/Break")]
    [EffectInfo("break", EffectCategory.Appearances)]
    [DefaultValue(nameof(baseDuration), .7f)]
    public sealed class BreakAppearance : AppearanceScriptableBase
    {
        public float[] angles;
        public Vector2[] vectors;

        public string lastText;

        public override void ResetContext(TAnimCore animator)
        {
            base.ResetContext(animator);

            if (lastText == animator.textWithoutAnyTag) return;

            lastText = animator.textWithoutAnyTag;
            int length = animator.Characters.Length;

            angles = new float[length];
            vectors = new Vector2[length];
            for (int i = 0; i < length; i++)
            {
                vectors[i] = new Vector2(Random.Range(-25f, 25f), Random.Range(-150f, -180f));
                angles[i] = Random.Range(-180f, 180f);
            }
        }

        public override void ApplyEffectTo(ref CharacterData character, TAnimCore animator)
        {
            character.current.positions.RotateChar(
                Mathf.Lerp(
                    angles[character.index],
                    0,
                    Tween.EaseInOut(character.passedTime / duration)
                )
            );
            character.current.positions.MoveChar(vectors[character.index] * character.uniformIntensity * Tween.EaseIn(1 - character.passedTime / duration));
        }

        public override void SetModifier(ModifierInfo modifier)
        {
            switch (modifier.name)
            {
                default: base.SetModifier(modifier); break;
            }
        }
    }

}