/*********************************************************************
 *
 *  $Id: yocto_powersupply_proxy.cs 55576 2023-07-25 06:26:34Z mvuilleu $
 *
 *  Implements YPowerSupplyProxy, the Proxy API for PowerSupply
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
    //--- (YPowerSupply class start)
    static public partial class YoctoProxyManager
    {
        public static YPowerSupplyProxy FindPowerSupply(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YPowerSupply func = null;
            YPowerSupplyProxy res = (YPowerSupplyProxy)YFunctionProxy.FindSimilarUnknownFunction("YPowerSupplyProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YPowerSupplyProxy)YFunctionProxy.FindSimilarKnownFunction("YPowerSupplyProxy");
                if (res != null) return res;
                func = YPowerSupply.FirstPowerSupply();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YPowerSupplyProxy)func.get_userData();
                    }
                }
            } else {
                func = YPowerSupply.FindPowerSupply(name);
                if (func.get_userData() != null) {
                    return (YPowerSupplyProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YPowerSupplyProxy(func, name);
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
 *   The <c>YPowerSupply</c> class allows you to drive a Yoctopuce power supply.
 * <para>
 *   It can be use to change the voltage and current limits, and to enable/disable
 *   the output.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YPowerSupplyProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a regulated power supply for a given identifier.
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
         *   This function does not require that the regulated power supply is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YPowerSupply.isOnline()</c> to test if the regulated power supply is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a regulated power supply by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the regulated power supply, for instance
         *   <c>MyDevice.powerSupply</c>.
         * </param>
         * <returns>
         *   a <c>YPowerSupply</c> object allowing you to drive the regulated power supply.
         * </returns>
         */
        public static YPowerSupplyProxy FindPowerSupply(string func)
        {
            return YoctoProxyManager.FindPowerSupply(func);
        }
        //--- (end of YPowerSupply class start)
        //--- (YPowerSupply definitions)
        public const double _VoltageLimit_INVALID = Double.NaN;
        public const double _CurrentLimit_INVALID = Double.NaN;
        public const int _PowerOutput_INVALID = 0;
        public const int _PowerOutput_OFF = 1;
        public const int _PowerOutput_ON = 2;
        public const double _MeasuredVoltage_INVALID = Double.NaN;
        public const double _MeasuredCurrent_INVALID = Double.NaN;
        public const double _InputVoltage_INVALID = Double.NaN;
        public const string _VoltageTransition_INVALID = YAPI.INVALID_STRING;
        public const double _VoltageLimitAtStartUp_INVALID = Double.NaN;
        public const double _CurrentLimitAtStartUp_INVALID = Double.NaN;
        public const int _PowerOutputAtStartUp_INVALID = 0;
        public const int _PowerOutputAtStartUp_OFF = 1;
        public const int _PowerOutputAtStartUp_ON = 2;
        public const string _Command_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YPowerSupply _func;
        // property cache
        protected double _voltageLimitAtStartUp = _VoltageLimitAtStartUp_INVALID;
        protected double _currentLimitAtStartUp = _CurrentLimitAtStartUp_INVALID;
        protected int _powerOutputAtStartUp = _PowerOutputAtStartUp_INVALID;
        //--- (end of YPowerSupply definitions)

        //--- (YPowerSupply implementation)
        internal YPowerSupplyProxy(YPowerSupply hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("PowerSupply " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YPowerSupply) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YPowerSupply hwd = YPowerSupply.FindPowerSupply(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YPowerSupply hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering PowerSupply callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type PowerSupply available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YPowerSupply.FindPowerSupply</c> to obtain an object that can control the
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
            YPowerSupply it = YPowerSupply.FirstPowerSupply();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextPowerSupply();
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
            _voltageLimitAtStartUp = _func.get_voltageLimitAtStartUp();
            _currentLimitAtStartUp = _func.get_currentLimitAtStartUp();
            _powerOutputAtStartUp = _func.get_powerOutputAtStartUp()+1;
        }

        /**
         * <summary>
         *   Changes the voltage limit, in V.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the voltage limit, in V
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
        public int set_voltageLimit(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PowerSupply connected");
            }
            if (newval == _VoltageLimit_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_voltageLimit(newval);
        }

        /**
         * <summary>
         *   Returns the voltage limit, in V.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the voltage limit, in V
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPowerSupply.VOLTAGELIMIT_INVALID</c>.
         * </para>
         */
        public double get_voltageLimit()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No PowerSupply connected");
            }
            res = _func.get_voltageLimit();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the current limit, in mA.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the current limit, in mA
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
        public int set_currentLimit(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PowerSupply connected");
            }
            if (newval == _CurrentLimit_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_currentLimit(newval);
        }

        /**
         * <summary>
         *   Returns the current limit, in mA.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the current limit, in mA
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPowerSupply.CURRENTLIMIT_INVALID</c>.
         * </para>
         */
        public double get_currentLimit()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No PowerSupply connected");
            }
            res = _func.get_currentLimit();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the power supply output switch state.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YPowerSupply.POWEROUTPUT_OFF</c> or <c>YPowerSupply.POWEROUTPUT_ON</c>, according to the
         *   power supply output switch state
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPowerSupply.POWEROUTPUT_INVALID</c>.
         * </para>
         */
        public int get_powerOutput()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PowerSupply connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_powerOutput()+1;
        }

        /**
         * <summary>
         *   Changes the power supply output switch state.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YPowerSupply.POWEROUTPUT_OFF</c> or <c>YPowerSupply.POWEROUTPUT_ON</c>, according to the
         *   power supply output switch state
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
        public int set_powerOutput(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PowerSupply connected");
            }
            if (newval == _PowerOutput_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_powerOutput(newval-1);
        }

        /**
         * <summary>
         *   Returns the measured output voltage, in V.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the measured output voltage, in V
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPowerSupply.MEASUREDVOLTAGE_INVALID</c>.
         * </para>
         */
        public double get_measuredVoltage()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No PowerSupply connected");
            }
            res = _func.get_measuredVoltage();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the measured output current, in mA.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the measured output current, in mA
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPowerSupply.MEASUREDCURRENT_INVALID</c>.
         * </para>
         */
        public double get_measuredCurrent()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No PowerSupply connected");
            }
            res = _func.get_measuredCurrent();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the measured input voltage, in V.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the measured input voltage, in V
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPowerSupply.INPUTVOLTAGE_INVALID</c>.
         * </para>
         */
        public double get_inputVoltage()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No PowerSupply connected");
            }
            res = _func.get_inputVoltage();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the voltage set point at device start up.
         * <para>
         *   Remember to call the matching
         *   module <c>saveToFlash()</c> method, otherwise this call has no effect.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the voltage set point at device start up
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
        public int set_voltageLimitAtStartUp(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PowerSupply connected");
            }
            if (newval == _VoltageLimitAtStartUp_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_voltageLimitAtStartUp(newval);
        }

        /**
         * <summary>
         *   Returns the selected voltage limit at device startup, in V.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the selected voltage limit at device startup, in V
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPowerSupply.VOLTAGELIMITATSTARTUP_INVALID</c>.
         * </para>
         */
        public double get_voltageLimitAtStartUp()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No PowerSupply connected");
            }
            res = _func.get_voltageLimitAtStartUp();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        // property with cached value for instant access (configuration)
        /// <value>Selected voltage limit at device startup, in V.</value>
        public double VoltageLimitAtStartUp
        {
            get
            {
                if (_func == null) {
                    return _VoltageLimitAtStartUp_INVALID;
                }
                if (_online) {
                    return _voltageLimitAtStartUp;
                }
                return _VoltageLimitAtStartUp_INVALID;
            }
            set
            {
                setprop_voltageLimitAtStartUp(value);
            }
        }

        // private helper for magic property
        private void setprop_voltageLimitAtStartUp(double newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _VoltageLimitAtStartUp_INVALID) {
                return;
            }
            if (newval == _voltageLimitAtStartUp) {
                return;
            }
            _func.set_voltageLimitAtStartUp(newval);
            _voltageLimitAtStartUp = newval;
        }

        /**
         * <summary>
         *   Changes the current limit at device start up.
         * <para>
         *   Remember to call the matching
         *   module <c>saveToFlash()</c> method, otherwise this call has no effect.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the current limit at device start up
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
        public int set_currentLimitAtStartUp(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PowerSupply connected");
            }
            if (newval == _CurrentLimitAtStartUp_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_currentLimitAtStartUp(newval);
        }

        /**
         * <summary>
         *   Returns the selected current limit at device startup, in mA.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the selected current limit at device startup, in mA
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPowerSupply.CURRENTLIMITATSTARTUP_INVALID</c>.
         * </para>
         */
        public double get_currentLimitAtStartUp()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No PowerSupply connected");
            }
            res = _func.get_currentLimitAtStartUp();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        // property with cached value for instant access (configuration)
        /// <value>Selected current limit at device startup, in mA.</value>
        public double CurrentLimitAtStartUp
        {
            get
            {
                if (_func == null) {
                    return _CurrentLimitAtStartUp_INVALID;
                }
                if (_online) {
                    return _currentLimitAtStartUp;
                }
                return _CurrentLimitAtStartUp_INVALID;
            }
            set
            {
                setprop_currentLimitAtStartUp(value);
            }
        }

        // private helper for magic property
        private void setprop_currentLimitAtStartUp(double newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _CurrentLimitAtStartUp_INVALID) {
                return;
            }
            if (newval == _currentLimitAtStartUp) {
                return;
            }
            _func.set_currentLimitAtStartUp(newval);
            _currentLimitAtStartUp = newval;
        }

        /**
         * <summary>
         *   Returns the power supply output switch state.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YPowerSupply.POWEROUTPUTATSTARTUP_OFF</c> or <c>YPowerSupply.POWEROUTPUTATSTARTUP_ON</c>,
         *   according to the power supply output switch state
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPowerSupply.POWEROUTPUTATSTARTUP_INVALID</c>.
         * </para>
         */
        public int get_powerOutputAtStartUp()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PowerSupply connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_powerOutputAtStartUp()+1;
        }

        /**
         * <summary>
         *   Changes the power supply output switch state at device start up.
         * <para>
         *   Remember to call the matching
         *   module <c>saveToFlash()</c> method, otherwise this call has no effect.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YPowerSupply.POWEROUTPUTATSTARTUP_OFF</c> or <c>YPowerSupply.POWEROUTPUTATSTARTUP_ON</c>,
         *   according to the power supply output switch state at device start up
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
        public int set_powerOutputAtStartUp(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PowerSupply connected");
            }
            if (newval == _PowerOutputAtStartUp_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_powerOutputAtStartUp(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>Power supply output switch state.</value>
        public int PowerOutputAtStartUp
        {
            get
            {
                if (_func == null) {
                    return _PowerOutputAtStartUp_INVALID;
                }
                if (_online) {
                    return _powerOutputAtStartUp;
                }
                return _PowerOutputAtStartUp_INVALID;
            }
            set
            {
                setprop_powerOutputAtStartUp(value);
            }
        }

        // private helper for magic property
        private void setprop_powerOutputAtStartUp(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _PowerOutputAtStartUp_INVALID) {
                return;
            }
            if (newval == _powerOutputAtStartUp) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_powerOutputAtStartUp(newval-1);
            _powerOutputAtStartUp = newval;
        }

        /**
         * <summary>
         *   Performs a smooth transition of output voltage.
         * <para>
         *   Any explicit voltage
         *   change cancels any ongoing transition process.
         * </para>
         * </summary>
         * <param name="V_target">
         *   new output voltage value at the end of the transition
         *   (floating-point number, representing the end voltage in V)
         * </param>
         * <param name="ms_duration">
         *   total duration of the transition, in milliseconds
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         */
        public virtual int voltageMove(double V_target, int ms_duration)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PowerSupply connected");
            }
            return _func.voltageMove(V_target, ms_duration);
        }
    }
    //--- (end of YPowerSupply implementation)
}

