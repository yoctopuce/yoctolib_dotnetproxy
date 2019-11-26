/*********************************************************************
 *
 *  $Id: yocto_poweroutput_proxy.cs 38282 2019-11-21 14:50:25Z seb $
 *
 *  Implements YPowerOutputProxy, the Proxy API for PowerOutput
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
    //--- (YPowerOutput class start)
    static public partial class YoctoProxyManager
    {
        public static YPowerOutputProxy FindPowerOutput(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YPowerOutput func = null;
            YPowerOutputProxy res = (YPowerOutputProxy)YFunctionProxy.FindSimilarUnknownFunction("YPowerOutputProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YPowerOutputProxy)YFunctionProxy.FindSimilarKnownFunction("YPowerOutputProxy");
                if (res != null) return res;
                func = YPowerOutput.FirstPowerOutput();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YPowerOutputProxy)func.get_userData();
                    }
                }
            } else {
                func = YPowerOutput.FindPowerOutput(name);
                if (func.get_userData() != null) {
                    return (YPowerOutputProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YPowerOutputProxy(func, name);
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
 *   Yoctopuce application programming interface allows you to control
 *   the power output featured on some devices such as the Yocto-Serial.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YPowerOutputProxy : YFunctionProxy
    {
        //--- (end of YPowerOutput class start)
        //--- (YPowerOutput definitions)
        public const int _Voltage_INVALID = 0;
        public const int _Voltage_OFF = 1;
        public const int _Voltage_OUT3V3 = 2;
        public const int _Voltage_OUT5V = 3;
        public const int _Voltage_OUT4V7 = 4;
        public const int _Voltage_OUT1V8 = 5;

        // reference to real YoctoAPI object
        protected new YPowerOutput _func;
        // property cache
        protected int _voltage = _Voltage_INVALID;
        //--- (end of YPowerOutput definitions)

        //--- (YPowerOutput implementation)
        internal YPowerOutputProxy(YPowerOutput hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("PowerOutput " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YPowerOutput) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YPowerOutput hwd = YPowerOutput.FindPowerOutput(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YPowerOutput hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering PowerOutput callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YPowerOutput it = YPowerOutput.FirstPowerOutput();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextPowerOutput();
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
            _voltage = _func.get_voltage()+1;
        }

        /**
         * <summary>
         *   Returns the voltage on the power output featured by the module.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YPowerOutput.VOLTAGE_OFF</c>, <c>YPowerOutput.VOLTAGE_OUT3V3</c>,
         *   <c>YPowerOutput.VOLTAGE_OUT5V</c>, <c>YPowerOutput.VOLTAGE_OUT4V7</c> and
         *   <c>YPowerOutput.VOLTAGE_OUT1V8</c> corresponding to the voltage on the power output featured by the module
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPowerOutput.VOLTAGE_INVALID</c>.
         * </para>
         */
        public int get_voltage()
        {
            if (_func == null)
            {
                string msg = "No PowerOutput connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_voltage()+1;
        }

        /**
         * <summary>
         *   Changes the voltage on the power output provided by the
         *   module.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YPowerOutput.VOLTAGE_OFF</c>, <c>YPowerOutput.VOLTAGE_OUT3V3</c>,
         *   <c>YPowerOutput.VOLTAGE_OUT5V</c>, <c>YPowerOutput.VOLTAGE_OUT4V7</c> and
         *   <c>YPowerOutput.VOLTAGE_OUT1V8</c> corresponding to the voltage on the power output provided by the
         *   module
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
        public int set_voltage(int newval)
        {
            if (_func == null)
            {
                string msg = "No PowerOutput connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Voltage_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_voltage(newval-1);
        }


        // property with cached value for instant access (configuration)
        public int Voltage
        {
            get
            {
                if (_func == null) return _Voltage_INVALID;
                return (_online ? _voltage : _Voltage_INVALID);
            }
            set
            {
                setprop_voltage(value);
            }
        }

        // private helper for magic property
        private void setprop_voltage(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Voltage_INVALID) return;
            if (newval == _voltage) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_voltage(newval-1);
            _voltage = newval;
        }
    }
    //--- (end of YPowerOutput implementation)
}

