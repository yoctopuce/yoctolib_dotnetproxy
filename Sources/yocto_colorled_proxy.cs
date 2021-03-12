/*********************************************************************
 *
 *  $Id: yocto_colorled_proxy.cs 43619 2021-01-29 09:14:45Z mvuilleu $
 *
 *  Implements YColorLedProxy, the Proxy API for ColorLed
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
using YoctoLib;

namespace YoctoProxyAPI
{
    //--- (YColorLed class start)
    static public partial class YoctoProxyManager
    {
        public static YColorLedProxy FindColorLed(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YColorLed func = null;
            YColorLedProxy res = (YColorLedProxy)YFunctionProxy.FindSimilarUnknownFunction("YColorLedProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YColorLedProxy)YFunctionProxy.FindSimilarKnownFunction("YColorLedProxy");
                if (res != null) return res;
                func = YColorLed.FirstColorLed();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YColorLedProxy)func.get_userData();
                    }
                }
            } else {
                func = YColorLed.FindColorLed(name);
                if (func.get_userData() != null) {
                    return (YColorLedProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YColorLedProxy(func, name);
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
 *   The <c>ColorLed</c> class allows you to drive a color LED.
 * <para>
 *   The color can be specified using RGB coordinates as well as HSL coordinates.
 *   The module performs all conversions form RGB to HSL automatically. It is then
 *   self-evident to turn on a LED with a given hue and to progressively vary its
 *   saturation or lightness. If needed, you can find more information on the
 *   difference between RGB and HSL in the section following this one.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YColorLedProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves an RGB LED for a given identifier.
         * <para>
         *   The identifier can be specified using several formats:
         * </para>
         * <para>
         * </para>
         * <para>
         *   - FunctionLogicalName
         * </para>
         * <para>
         *   - ModuleSerialNumber.FunctionIdentifier
         * </para>
         * <para>
         *   - ModuleSerialNumber.FunctionLogicalName
         * </para>
         * <para>
         *   - ModuleLogicalName.FunctionIdentifier
         * </para>
         * <para>
         *   - ModuleLogicalName.FunctionLogicalName
         * </para>
         * <para>
         * </para>
         * <para>
         *   This function does not require that the RGB LED is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YColorLed.isOnline()</c> to test if the RGB LED is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   an RGB LED by logical name, no error is notified: the first instance
         *   found is returned. The search is performed first by hardware name,
         *   then by logical name.
         * </para>
         * <para>
         *   If a call to this object's is_online() method returns FALSE although
         *   you are certain that the matching device is plugged, make sure that you did
         *   call registerHub() at application initialization time.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="func">
         *   a string that uniquely characterizes the RGB LED, for instance
         *   <c>YRGBLED2.colorLed1</c>.
         * </param>
         * <returns>
         *   a <c>YColorLed</c> object allowing you to drive the RGB LED.
         * </returns>
         */
        public static YColorLedProxy FindColorLed(string func)
        {
            return YoctoProxyManager.FindColorLed(func);
        }
        //--- (end of YColorLed class start)
        //--- (YColorLed definitions)
        public const int _RgbColor_INVALID = -1;
        public const int _HslColor_INVALID = -1;
        public const int _RgbColorAtPowerOn_INVALID = -1;
        public const int _BlinkSeqSize_INVALID = -1;
        public const int _BlinkSeqMaxSize_INVALID = -1;
        public const int _BlinkSeqSignature_INVALID = -1;
        public const string _Command_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YColorLed _func;
        // property cache
        protected int _rgbColor = _RgbColor_INVALID;
        protected int _rgbColorAtPowerOn = _RgbColorAtPowerOn_INVALID;
        protected int _blinkSeqMaxSize = _BlinkSeqMaxSize_INVALID;
        protected int _hslColor = _HslColor_INVALID;
        //--- (end of YColorLed definitions)

        //--- (YColorLed implementation)
        internal YColorLedProxy(YColorLed hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("ColorLed " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YColorLed) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YColorLed hwd = YColorLed.FindColorLed(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YColorLed hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering ColorLed callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type ColorLed available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YColorLed.FindColorLed</c> to obtain an object that can control the
         *   corresponding device.
         * </para>
         * </summary>
         * <returns>
         *   an array of strings, each string containing the unique hardwareId
         *   of a device function currently connected.
         * </returns>
         */
        public static new string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YColorLed it = YColorLed.FirstColorLed();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextColorLed();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
            _hslColor = _func.get_hslColor();
            _blinkSeqMaxSize = _func.get_blinkSeqMaxSize();
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            _rgbColorAtPowerOn = _func.get_rgbColorAtPowerOn();
        }

        /**
         * <summary>
         *   Returns the current RGB color of the LED.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current RGB color of the LED
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorLed.RGBCOLOR_INVALID</c>.
         * </para>
         */
        public int get_rgbColor()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLed connected");
            }
            res = _func.get_rgbColor();
            if (res == YAPI.INVALID_INT) {
                res = _RgbColor_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the current color of the LED, using an RGB color.
         * <para>
         *   Encoding is done as follows: 0xRRGGBB.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the current color of the LED, using an RGB color
         * </param>
         * <para>
         * </para>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_rgbColor(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLed connected");
            }
            if (newval == _RgbColor_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_rgbColor(newval);
        }

        // property with cached value for instant access (advertised value)
        /// <value>Current RGB color of the LED.</value>
        public int RgbColor
        {
            get
            {
                if (_func == null) {
                    return _RgbColor_INVALID;
                }
                if (_online) {
                    return _rgbColor;
                }
                return _RgbColor_INVALID;
            }
            set
            {
                setprop_rgbColor(value);
            }
        }

        // private helper for magic property
        private void setprop_rgbColor(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _RgbColor_INVALID) {
                return;
            }
            if (newval == _rgbColor) {
                return;
            }
            _func.set_rgbColor(newval);
            _rgbColor = newval;
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            _rgbColor = YAPI._hexStrToInt(value);
            _hslColor = InternalStuff.rgb2hsl(_rgbColor, _hslColor);
        }

        // property with cached value for instant access (derived from advertised value)
        /// <value>Current HSL color of the LED.</value>
        public int HslColor
        {
            get
            {
                if (_func == null) {
                    return _HslColor_INVALID;
                }
                if (_online) {
                    return _hslColor;
                }
                return _HslColor_INVALID;
            }
            set
            {
                setprop_hslColor(value);
            }
        }

        // private helper for magic property
        private void setprop_hslColor(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _HslColor_INVALID) {
                return;
            }
            if (newval == _hslColor) {
                return;
            }
            _func.set_hslColor(newval);
            _hslColor = newval;
        }

        /**
         * <summary>
         *   Returns the current HSL color of the LED.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current HSL color of the LED
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorLed.HSLCOLOR_INVALID</c>.
         * </para>
         */
        public int get_hslColor()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLed connected");
            }
            res = _func.get_hslColor();
            if (res == YAPI.INVALID_INT) {
                res = _HslColor_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the current color of the LED, using a color HSL.
         * <para>
         *   Encoding is done as follows: 0xHHSSLL.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the current color of the LED, using a color HSL
         * </param>
         * <para>
         * </para>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_hslColor(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLed connected");
            }
            if (newval == _HslColor_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_hslColor(newval);
        }

        /**
         * <summary>
         *   Performs a smooth transition in the RGB color space between the current color and a target color.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="rgb_target">
         *   desired RGB color at the end of the transition
         * </param>
         * <param name="ms_duration">
         *   duration of the transition, in millisecond
         * </param>
         * <para>
         * </para>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int rgbMove(int rgb_target,int ms_duration)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLed connected");
            }
            return _func.rgbMove(rgb_target, ms_duration);
        }

        /**
         * <summary>
         *   Performs a smooth transition in the HSL color space between the current color and a target color.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="hsl_target">
         *   desired HSL color at the end of the transition
         * </param>
         * <param name="ms_duration">
         *   duration of the transition, in millisecond
         * </param>
         * <para>
         * </para>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int hslMove(int hsl_target,int ms_duration)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLed connected");
            }
            return _func.hslMove(hsl_target, ms_duration);
        }

        /**
         * <summary>
         *   Returns the configured color to be displayed when the module is turned on.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the configured color to be displayed when the module is turned on
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorLed.RGBCOLORATPOWERON_INVALID</c>.
         * </para>
         */
        public int get_rgbColorAtPowerOn()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLed connected");
            }
            res = _func.get_rgbColorAtPowerOn();
            if (res == YAPI.INVALID_INT) {
                res = _RgbColorAtPowerOn_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the color that the LED displays by default when the module is turned on.
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the color that the LED displays by default when the module is turned on
         * </param>
         * <para>
         * </para>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_rgbColorAtPowerOn(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLed connected");
            }
            if (newval == _RgbColorAtPowerOn_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_rgbColorAtPowerOn(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Configured color to be displayed when the module is turned on.</value>
        public int RgbColorAtPowerOn
        {
            get
            {
                if (_func == null) {
                    return _RgbColorAtPowerOn_INVALID;
                }
                if (_online) {
                    return _rgbColorAtPowerOn;
                }
                return _RgbColorAtPowerOn_INVALID;
            }
            set
            {
                setprop_rgbColorAtPowerOn(value);
            }
        }

        // private helper for magic property
        private void setprop_rgbColorAtPowerOn(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _RgbColorAtPowerOn_INVALID) {
                return;
            }
            if (newval == _rgbColorAtPowerOn) {
                return;
            }
            _func.set_rgbColorAtPowerOn(newval);
            _rgbColorAtPowerOn = newval;
        }

        /**
         * <summary>
         *   Returns the current length of the blinking sequence.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current length of the blinking sequence
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorLed.BLINKSEQSIZE_INVALID</c>.
         * </para>
         */
        public int get_blinkSeqSize()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLed connected");
            }
            res = _func.get_blinkSeqSize();
            if (res == YAPI.INVALID_INT) {
                res = _BlinkSeqSize_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the maximum length of the blinking sequence.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the maximum length of the blinking sequence
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorLed.BLINKSEQMAXSIZE_INVALID</c>.
         * </para>
         */
        public int get_blinkSeqMaxSize()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLed connected");
            }
            res = _func.get_blinkSeqMaxSize();
            if (res == YAPI.INVALID_INT) {
                res = _BlinkSeqMaxSize_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (constant value)
        /// <value>Maximum length of the blinking sequence.</value>
        public int BlinkSeqMaxSize
        {
            get
            {
                if (_func == null) {
                    return _BlinkSeqMaxSize_INVALID;
                }
                if (_online) {
                    return _blinkSeqMaxSize;
                }
                return _BlinkSeqMaxSize_INVALID;
            }
        }

        /**
         * <summary>
         *   Return the blinking sequence signature.
         * <para>
         *   Since blinking
         *   sequences cannot be read from the device, this can be used
         *   to detect if a specific blinking sequence is already
         *   programmed.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorLed.BLINKSEQSIGNATURE_INVALID</c>.
         * </para>
         */
        public int get_blinkSeqSignature()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLed connected");
            }
            res = _func.get_blinkSeqSignature();
            if (res == YAPI.INVALID_INT) {
                res = _BlinkSeqSignature_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Add a new transition to the blinking sequence, the move will
         *   be performed in the HSL space.
         * <para>
         * </para>
         * </summary>
         * <param name="HSLcolor">
         *   desired HSL color when the transition is completed
         * </param>
         * <param name="msDelay">
         *   duration of the color transition, in milliseconds.
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int addHslMoveToBlinkSeq(int HSLcolor, int msDelay)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLed connected");
            }
            return _func.addHslMoveToBlinkSeq(HSLcolor, msDelay);
        }

        /**
         * <summary>
         *   Adds a new transition to the blinking sequence, the move is
         *   performed in the RGB space.
         * <para>
         * </para>
         * </summary>
         * <param name="RGBcolor">
         *   desired RGB color when the transition is completed
         * </param>
         * <param name="msDelay">
         *   duration of the color transition, in milliseconds.
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int addRgbMoveToBlinkSeq(int RGBcolor, int msDelay)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLed connected");
            }
            return _func.addRgbMoveToBlinkSeq(RGBcolor, msDelay);
        }

        /**
         * <summary>
         *   Starts the preprogrammed blinking sequence.
         * <para>
         *   The sequence is
         *   run in a loop until it is stopped by stopBlinkSeq or an explicit
         *   change.
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int startBlinkSeq()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLed connected");
            }
            return _func.startBlinkSeq();
        }

        /**
         * <summary>
         *   Stops the preprogrammed blinking sequence.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int stopBlinkSeq()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLed connected");
            }
            return _func.stopBlinkSeq();
        }

        /**
         * <summary>
         *   Resets the preprogrammed blinking sequence.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int resetBlinkSeq()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLed connected");
            }
            return _func.resetBlinkSeq();
        }
    }
    //--- (end of YColorLed implementation)
}

