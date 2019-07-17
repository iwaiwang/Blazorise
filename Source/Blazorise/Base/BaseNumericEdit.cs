﻿#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseNumericEdit<TValue> : BaseTextInput<TValue>
    //where TValue : struct 
    {
        #region Members

        #endregion

        #region Methods

        // implementation according to the response on https://github.com/aspnet/AspNetCore/issues/7898#issuecomment-479863699
        protected override void OnInit()
        {
            internalValue = Value;

            base.OnInit();
        }

        protected override async Task OnFirstAfterRenderAsync()
        {
            await JSRunner.InitializeNumericEdit( ElementId, ElementRef, Decimals, DecimalsSeparator, Step );

            await base.OnFirstAfterRenderAsync();
        }

        public override void Dispose()
        {
            JSRunner.DestroyNumericEdit( ElementId, ElementRef );

            base.Dispose();
        }

        public override Task SetParametersAsync( ParameterCollection parameters )
        {
            // This is needed for the two-way binding to work properly.
            // Otherwise the internal value would not be set.
            if ( parameters.TryGetValue<TValue>( nameof( Value ), out var newValue ) )
            {
                internalValue = newValue;
            }

            return base.SetParametersAsync( parameters );
        }

        protected override void HandleValue( object value )
        {
            if ( Converters.TryChangeType<TValue>( value, out var result ) )
            {
                // TODO: disabled until Blazor implements constraints for generic components!!!!
                //if ( Max != null && Comparers.Compare( result, Max ) > 0 )
                //    result = Max ?? default;
                //else if ( Min != null && Comparers.Compare( result, Min ) < 0 )
                //    result = Min ?? default;

                InternalValue = result;
            }
            else
                InternalValue = default;

            ValueChanged?.Invoke( InternalValue );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value inside the input field.
        /// </summary>
        [Parameter] protected TValue Value { get; set; }

        /// <summary>
        /// Occurs after the value has changed.
        /// </summary>
        /// <remarks>
        /// This will be converted to EventCallback once the Blazor team fix the error for generic components. see https://github.com/aspnet/AspNetCore/issues/8385
        /// </remarks>
        [Parameter] protected Action<TValue> ValueChanged { get; set; }

        /// <summary>
        /// Specifies the interval between valid values.
        /// </summary>
        [Parameter] protected decimal? Step { get; set; }

        /// <summary>
        /// Maximum number of decimal places after the decimal separator.
        /// </summary>
        [Parameter] protected int Decimals { get; set; } = 2;

        /// <summary>
        /// String to use as the decimal separator in numeric values.
        /// </summary>
        [Parameter] protected string DecimalsSeparator { get; set; } = ".";

        ///// <summary>
        ///// The minimum value to accept for this input.
        ///// </summary>
        //[Parameter] protected TValue? Min { get; set; }

        ///// <summary>
        ///// The maximum value to accept for this input.
        ///// </summary>
        //[Parameter] protected TValue? Max { get; set; }

        #endregion
    }
}
