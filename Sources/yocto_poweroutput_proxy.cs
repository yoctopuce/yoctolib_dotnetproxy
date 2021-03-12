/*********************************************************************
 *
 *  $Id: yocto_poweroutput_proxy.cs 43619 2021-01-29 09:14:45Z mvuilleu $
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
using YoctoLib;

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
 *   The <c>YPowerOutput</c> class allows you to control
 *   the power output featured on some Yoctopuce devices.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YPowerOutputProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a power output for a given identifier.
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
         *   This function does not require that the power output is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YPowerOutput.isOnline()</c> to test if the power output is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a power output by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the power output, for instance
         *   <c>YI2CMK01.powerOutput</c>.
         * </param>
         * <returns>
         *   a <c>YPowerOutput</c> object allowing you to drive the power output.
         * </returns>
         */
        public static YPowerOutputProxy FindPowerOutput(string func)
        {
            return YoctoProxyManager.FindPowerOutput(func);
        }
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

        /**
         * <summary>
         *   Enumerates all functions of type PowerOutput available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YPowerOutput.FindPowerOutput</c> to obtain an object that can control the
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
            if (_func == null) {
                throw new YoctoApiProxyException("No PowerOutput connected");
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
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_voltage(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PowerOutput connected");
            }
            if (newval == _Voltage_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_voltage(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>Voltage on the power output featured by the module.</value>
        public int Voltage
        {
            get
            {
                if (_func == null) {
                    return _Voltage_INVALID;
                }
                if (_online) {
                    return _voltage;
                }
                return _Voltage_INVALID;
            }
            set
            {
                setprop_voltage(value);
            }
        }

        // private helper for magic property
        private void setprop_voltage(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Voltage_INVALID) {
                return;
            }
            if (newval == _voltage) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_voltage(newval-1);
            _voltage = newval;
        }
    }
    //--- (end of YPowerOutput implementation)
}

