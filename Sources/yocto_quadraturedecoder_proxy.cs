/*********************************************************************
 *
 *  $Id: yocto_quadraturedecoder_proxy.cs 38282 2019-11-21 14:50:25Z seb $
 *
 *  Implements YQuadratureDecoderProxy, the Proxy API for QuadratureDecoder
 *
 *  - - - - - - - - - License information: - - - - - - - - -
 *
 *  Copyright (C) 2011 and beyond by Yoctopuce Sarl, Switzerland.
 *
 *  Yoctopuce Sarl (hereafter Licensor) grants to you a perpetual
 *  non-exclusive license to use, modify, copy and integrate this
 *  file into your software for the sole purpose of interfacing
 *  with Yoctopuce products.
 *
 *  You may reproduce and distribute copies of this file in
 *  source or object form, as long as the sole purpose of this
 *  code is to interface with Yoctopuce products. You must retain
 *  this notice in the distributed source file.
 *
 *  You should refer to Yoctopuce General Terms and Conditions
 *  for additional information regarding your rights and
 *  obligations.
 *
 *  THE SOFTWARE AND DOCUMENTATION ARE PROVIDED 'AS IS' WITHOUT
 *  WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING
 *  WITHOUT LIMITATION, ANY WARRANTY OF MERCHANTABILITY, FITNESS
 *  FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO
 *  EVENT SHALL LICENSOR BE LIABLE FOR ANY INCIDENTAL, SPECIAL,
 *  INDIRECT OR CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA,
 *  COST OF PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY OR
 *  SERVICES, ANY CLAIMS BY THIRD PARTIES (INCLUDING BUT NOT
 *  LIMITED TO ANY DEFENSE THEREOF), ANY CLAIMS FOR INDEMNITY OR
 *  CONTRIBUTION, OR OTHER SIMILAR COSTS, WHETHER ASSERTED ON THE
 *  BASIS OF CONTRACT, TORT (INCLUDING NEGLIGENCE), BREACH OF
 *  WARRANTY, OR OTHERWISE.
 *
 *********************************************************************/


using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Timers;
using System.Globalization;
using System.Text.RegularExpressions;

namespace YoctoProxyAPI
{
    //--- (YQuadratureDecoder class start)
    static public partial class YoctoProxyManager
    {
        public static YQuadratureDecoderProxy FindQuadratureDecoder(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YQuadratureDecoder func = null;
            YQuadratureDecoderProxy res = (YQuadratureDecoderProxy)YFunctionProxy.FindSimilarUnknownFunction("YQuadratureDecoderProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YQuadratureDecoderProxy)YFunctionProxy.FindSimilarKnownFunction("YQuadratureDecoderProxy");
                if (res != null) return res;
                func = YQuadratureDecoder.FirstQuadratureDecoder();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YQuadratureDecoderProxy)func.get_userData();
                    }
                }
            } else {
                func = YQuadratureDecoder.FindQuadratureDecoder(name);
                if (func.get_userData() != null) {
                    return (YQuadratureDecoderProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YQuadratureDecoderProxy(func, name);
            }
            if (func != null) {
                res.linkToHardware(name);
                if(func.isOnline()) res.arrival();
            }
            return res;
        }
    }

/**
 * <summary>
 *   The YQuadratureDecoder class allows you to decode a two-wire signal produced by a
 *   quadrature encoder, for instance using a Yocto-PWM-Rx.
 * <para>
 *   It inherits from YSensor class the core functions to read measurements,
 *   to register callback functions, to access the autonomous datalogger.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YQuadratureDecoderProxy : YSensorProxy
    {
        //--- (end of YQuadratureDecoder class start)
        //--- (YQuadratureDecoder definitions)
        public const double _Speed_INVALID = Double.NaN;
        public const int _Decoding_INVALID = 0;
        public const int _Decoding_OFF = 1;
        public const int _Decoding_ON = 2;

        // reference to real YoctoAPI object
        protected new YQuadratureDecoder _func;
        // property cache
        protected int _decoding = _Decoding_INVALID;
        //--- (end of YQuadratureDecoder definitions)

        //--- (YQuadratureDecoder implementation)
        internal YQuadratureDecoderProxy(YQuadratureDecoder hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("QuadratureDecoder " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YQuadratureDecoder) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YQuadratureDecoder hwd = YQuadratureDecoder.FindQuadratureDecoder(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YQuadratureDecoder hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering QuadratureDecoder callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YQuadratureDecoder it = YQuadratureDecoder.FirstQuadratureDecoder();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextQuadratureDecoder();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _decoding = _func.get_decoding()+1;
        }

        /**
         * <summary>
         *   Changes the current expected position of the quadrature decoder.
         * <para>
         *   Invoking this function implicitly activates the quadrature decoder.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the current expected position of the quadrature decoder
         * </param>
         * <para>
         * </para>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_currentValue(double newval)
        {
            if (_func == null)
            {
                string msg = "No QuadratureDecoder connected";
                throw new YoctoApiProxyException(msg);
            }
            if (Double.IsNaN(newval)) return YAPI.SUCCESS;
            return _func.set_currentValue(newval);
        }


        // property with cached value for instant access (advertised value)
        public new double CurrentValue
        {
            get
            {
                if (_func == null) return _CurrentValue_INVALID;
                return (_online ? _currentValue : _CurrentValue_INVALID);
            }
            set
            {
                setprop_currentValue(value);
            }
        }

        // private helper for magic property
        private void setprop_currentValue(double newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (Double.IsNaN(newval)) return;
            if (newval == _currentValue) return;
            _func.set_currentValue(newval);
            _currentValue = newval;
        }

        /**
         * <summary>
         *   Returns the increments frequency, in Hz.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the increments frequency, in Hz
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YQuadratureDecoder.SPEED_INVALID</c>.
         * </para>
         */
        public double get_speed()
        {
            if (_func == null)
            {
                string msg = "No QuadratureDecoder connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_speed();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the current activation state of the quadrature decoder.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YQuadratureDecoder.DECODING_OFF</c> or <c>YQuadratureDecoder.DECODING_ON</c>, according
         *   to the current activation state of the quadrature decoder
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YQuadratureDecoder.DECODING_INVALID</c>.
         * </para>
         */
        public int get_decoding()
        {
            if (_func == null)
            {
                string msg = "No QuadratureDecoder connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_decoding()+1;
        }

        /**
         * <summary>
         *   Changes the activation state of the quadrature decoder.
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YQuadratureDecoder.DECODING_OFF</c> or <c>YQuadratureDecoder.DECODING_ON</c>, according
         *   to the activation state of the quadrature decoder
         * </param>
         * <para>
         * </para>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_decoding(int newval)
        {
            if (_func == null)
            {
                string msg = "No QuadratureDecoder connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Decoding_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_decoding(newval-1);
        }


        // property with cached value for instant access (configuration)
        public int Decoding
        {
            get
            {
                if (_func == null) return _Decoding_INVALID;
                return (_online ? _decoding : _Decoding_INVALID);
            }
            set
            {
                setprop_decoding(value);
            }
        }

        // private helper for magic property
        private void setprop_decoding(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Decoding_INVALID) return;
            if (newval == _decoding) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_decoding(newval-1);
            _decoding = newval;
        }
    }
    //--- (end of YQuadratureDecoder implementation)
}

