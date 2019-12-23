/*********************************************************************
 *
 *  $Id: yocto_led_proxy.cs 38913 2019-12-20 18:59:49Z mvuilleu $
 *
 *  Implements YLedProxy, the Proxy API for Led
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
    //--- (YLed class start)
    static public partial class YoctoProxyManager
    {
        public static YLedProxy FindLed(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YLed func = null;
            YLedProxy res = (YLedProxy)YFunctionProxy.FindSimilarUnknownFunction("YLedProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YLedProxy)YFunctionProxy.FindSimilarKnownFunction("YLedProxy");
                if (res != null) return res;
                func = YLed.FirstLed();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YLedProxy)func.get_userData();
                    }
                }
            } else {
                func = YLed.FindLed(name);
                if (func.get_userData() != null) {
                    return (YLedProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YLedProxy(func, name);
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
 *   The <c>YLed</c> class allows you to drive a monocolor LED.
 * <para>
 *   You can not only to drive the intensity of the LED, but also to
 *   have it blink at various preset frequencies.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YLedProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a monochrome LED for a given identifier.
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
         *   This function does not require that the monochrome LED is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YLed.isOnline()</c> to test if the monochrome LED is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a monochrome LED by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the monochrome LED, for instance
         *   <c>YBUZZER2.led1</c>.
         * </param>
         * <returns>
         *   a <c>YLed</c> object allowing you to drive the monochrome LED.
         * </returns>
         */
        public static YLedProxy FindLed(string func)
        {
            return YoctoProxyManager.FindLed(func);
        }
        //--- (end of YLed class start)
        //--- (YLed definitions)
        public const int _Power_INVALID = 0;
        public const int _Power_OFF = 1;
        public const int _Power_ON = 2;
        public const int _Luminosity_INVALID = -1;
        public const int _Blinking_INVALID = 0;
        public const int _Blinking_STILL = 1;
        public const int _Blinking_RELAX = 2;
        public const int _Blinking_AWARE = 3;
        public const int _Blinking_RUN = 4;
        public const int _Blinking_CALL = 5;
        public const int _Blinking_PANIC = 6;

        // reference to real YoctoAPI object
        protected new YLed _func;
        // property cache
        protected int _luminosity = _Luminosity_INVALID;
        protected int _blinking = _Blinking_INVALID;
        protected int _power = _Power_INVALID;
        //--- (end of YLed definitions)

        //--- (YLed implementation)
        internal YLedProxy(YLed hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Led " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YLed) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YLed hwd = YLed.FindLed(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YLed hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Led callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type Led available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YLed.FindLed</c> to obtain an object that can control the
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
            YLed it = YLed.FirstLed();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextLed();
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
            _luminosity = _func.get_luminosity();
        }

        /**
         * <summary>
         *   Returns the current LED state.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>led._Power_OFF</c> or <c>led._Power_ON</c>, according to the current LED state
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>led._Power_INVALID</c>.
         * </para>
         */
        public int get_power()
        {
            if (_func == null)
            {
                string msg = "No Led connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_power()+1;
        }

        /**
         * <summary>
         *   Changes the state of the LED.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>led._Power_OFF</c> or <c>led._Power_ON</c>, according to the state of the LED
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
        public int set_power(int newval)
        {
            if (_func == null)
            {
                string msg = "No Led connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Power_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_power(newval-1);
        }


        // private helper for magic property
        private void setprop_power(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Power_INVALID) return;
            if (newval == _power) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_power(newval-1);
            _power = newval;
        }

        /**
         * <summary>
         *   Returns the current LED intensity (in per cent).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current LED intensity (in per cent)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>led._Luminosity_INVALID</c>.
         * </para>
         */
        public int get_luminosity()
        {
            if (_func == null)
            {
                string msg = "No Led connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_luminosity();
            if (res == YAPI.INVALID_INT) res = _Luminosity_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the current LED intensity (in per cent).
         * <para>
         *   Remember to call the
         *   <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the current LED intensity (in per cent)
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
        public int set_luminosity(int newval)
        {
            if (_func == null)
            {
                string msg = "No Led connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Luminosity_INVALID) return YAPI.SUCCESS;
            return _func.set_luminosity(newval);
        }


        // property with cached value for instant access (configuration)
        public int Luminosity
        {
            get
            {
                if (_func == null) return _Luminosity_INVALID;
                return (_online ? _luminosity : _Luminosity_INVALID);
            }
            set
            {
                setprop_luminosity(value);
            }
        }

        // private helper for magic property
        private void setprop_luminosity(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Luminosity_INVALID) return;
            if (newval == _luminosity) return;
            _func.set_luminosity(newval);
            _luminosity = newval;
        }

        // property with cached value for instant access (advertised value)
        public int Blinking
        {
            get
            {
                if (_func == null) return _Blinking_INVALID;
                return (_online ? _blinking : _Blinking_INVALID);
            }
            set
            {
                setprop_blinking(value);
            }
        }

        // property with cached value for instant access (derived from advertised value)
        public int Power
        {
            get
            {
                if (_func == null) return _Power_INVALID;
                return (_online ? _power : _Power_INVALID);
            }
            set
            {
                setprop_power(value);
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            if (value == "STILL") _blinking = 1;
            if (value == "RELAX") _blinking = 2;
            if (value == "AWARE") _blinking = 3;
            if (value == "RUN") _blinking = 4;
            if (value == "CALL") _blinking = 5;
            if (value == "PANIC") _blinking = 6;
            if (value == "ON") _blinking = 1;
            if (value == "OFF") _power = 0;
            else if (value != "") _power = 1;
        }

        /**
         * <summary>
         *   Returns the current LED signaling mode.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>led._Blinking_STILL</c>, <c>led._Blinking_RELAX</c>, <c>led._Blinking_AWARE</c>,
         *   <c>led._Blinking_RUN</c>, <c>led._Blinking_CALL</c> and <c>led._Blinking_PANIC</c> corresponding to
         *   the current LED signaling mode
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>led._Blinking_INVALID</c>.
         * </para>
         */
        public int get_blinking()
        {
            if (_func == null)
            {
                string msg = "No Led connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_blinking()+1;
        }

        /**
         * <summary>
         *   Changes the current LED signaling mode.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>led._Blinking_STILL</c>, <c>led._Blinking_RELAX</c>, <c>led._Blinking_AWARE</c>,
         *   <c>led._Blinking_RUN</c>, <c>led._Blinking_CALL</c> and <c>led._Blinking_PANIC</c> corresponding to
         *   the current LED signaling mode
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
        public int set_blinking(int newval)
        {
            if (_func == null)
            {
                string msg = "No Led connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Blinking_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_blinking(newval-1);
        }


        // private helper for magic property
        private void setprop_blinking(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Blinking_INVALID) return;
            if (newval == _blinking) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_blinking(newval-1);
            _blinking = newval;
        }
    }
    //--- (end of YLed implementation)
}

