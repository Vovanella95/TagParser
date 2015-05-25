﻿namespace AngleSharp.Dom.Css
{
    using AngleSharp.Css;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// More information available at:
    /// https://developer.mozilla.org/en-US/docs/Web/CSS/border-bottom
    /// </summary>
    sealed class CssBorderBottomProperty : CssShorthandProperty
    {
        #region ctor

        internal CssBorderBottomProperty(CssStyleDeclaration rule)
            : base(PropertyNames.BorderBottom, rule, PropertyFlags.Animatable)
        {
        }

        #endregion

        #region Methods

        protected override Boolean IsValid(ICssValue value)
        {
            return CssBorderProperty.Converter.TryConvert(value, m =>
            {
                Get<CssBorderBottomWidthProperty>().TrySetValue(m.Item1);
                Get<CssBorderBottomStyleProperty>().TrySetValue(m.Item2);
                Get<CssBorderBottomColorProperty>().TrySetValue(m.Item3);
            });
        }

        internal override String SerializeValue(IEnumerable<CssProperty> properties)
        {
            var color = properties.OfType<CssBorderBottomColorProperty>().FirstOrDefault();
            var width = properties.OfType<CssBorderBottomWidthProperty>().FirstOrDefault();
            var style = properties.OfType<CssBorderBottomStyleProperty>().FirstOrDefault();

            if (color == null || width == null || style == null)
                return String.Empty;

            return CssBorderProperty.SerializeValue(width, style, color);
        }

        #endregion
    }
}
