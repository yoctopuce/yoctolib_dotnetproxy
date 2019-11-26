/*********************************************************************
 *
 *  $Id: yocto_current_proxy.cs 38282 2019-11-21 14:50:25Z seb $
 *
 *  Implements YCurrentProxy, the Proxy API for Current
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
    //--- (YCurrent class start)
    static public partial class YoctoProxyManager
    {
        public static YCurrentProxy FindCurrent(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YCurrent func = null;
            YCurrentProxy res = (YCurrentProxy)YFunctionProxy.FindSimilarUnknownFunction("YCurrentProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YCurrentProxy)YFunctionProxy.FindSimilarKnownFunction("YCurrentProxy");
                if (res != null) return res;
                func = YCurrent.FirstCurrent();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YCurrentProxy)func.get_userData();
                    }
                }
            } else {
                func = YCurrent.FindCurrent(name);
                if (func.get_userData() != null) {
                    return (YCurrentProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YCurrentProxy(func, name);
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
 *   The YCurrent class allows you to read and configure Yoctopuce current
 *   sensors, for instance using a Yocto-Watt, a Yocto-Amp or a Yocto-Motor-DC.
 * <para>
 *   It inherits from YSensor class the core functions to read measurements,
 *   to register callback functions, to access the autonomous datalogger.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YCurrentProxy : YSensorProxy
    {
        //--- (end of YCurrent class start)
        //--- (YCurrent definitions)
        public const int _Enabled_INVALID = 0;
        public const int _Enabled_FALSE = 1;
        public const int _Enabled_TRUE = 2;

        // reference to real YoctoAPI object
        protected new YCurrent _func;
        // property cache
        protected int _enabled = _Enabled_INVALID;
        //--- (end of YCurrent definitions)

        //--- (YCurrent implementation)
        internal YCurrentProxy(YCurrent hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Current " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YCurrent) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YCurrent hwd = YCurrent.FindCurrent(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YCurrent hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Current callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YCurrent it = YCurrent.FirstCurrent();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextCurrent();
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
            _enabled = _func.get_enabled()+1;
        }

        /**
         * <summary>
         *   Returns the activation state of this input.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YCurrent.ENABLED_FALSE</c> or <c>YCurrent.ENABLED_TRUE</c>, according to the activation
         *   state of this input
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YCurrent.ENABLED_INVALID</c>.
         * </para>
         */
        public int get_enabled()
        {
            if (_func == null)
            {
                string msg = "No Current connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_enabled()+1;
        }

        /**
         * <summary>
         *   Changes the activation state of this voltage input.
         * <para>
         *   When AC measurements are disabled,
         *   the device will always assume a DC signal, and vice-versa. When both AC and DC measurements
         *   are active, the device switches between AC and DC mode based on the relative amplitude
         *   of variations compared to the average value.
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YCurrent.ENABLED_FALSE</c> or <c>YCurrent.ENABLED_TRUE</c>, according to the activation
         *   state of this voltage input
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
        public int set_enabled(int newval)
        {
            if (_func == null)
            {
                string msg = "No Current connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Enabled_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_enabled(newval-1);
        }


        // property with cached value for instant access (configuration)
        public int Enabled
        {
            get
            {
                if (_func == null) return _Enabled_INVALID;
                return (_online ? _enabled : _Enabled_INVALID);
            }
            set
            {
                setprop_enabled(value);
            }
        }

        // private helper for magic property
        private void setprop_enabled(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Enabled_INVALID) return;
            if (newval == _enabled) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_enabled(newval-1);
            _enabled = newval;
        }
    }
    //--- (end of YCurrent implementation)
}

