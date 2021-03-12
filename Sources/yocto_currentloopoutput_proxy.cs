/*********************************************************************
 *
 *  $Id: yocto_currentloopoutput_proxy.cs 43619 2021-01-29 09:14:45Z mvuilleu $
 *
 *  Implements YCurrentLoopOutputProxy, the Proxy API for CurrentLoopOutput
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
    //--- (YCurrentLoopOutput class start)
    static public partial class YoctoProxyManager
    {
        public static YCurrentLoopOutputProxy FindCurrentLoopOutput(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YCurrentLoopOutput func = null;
            YCurrentLoopOutputProxy res = (YCurrentLoopOutputProxy)YFunctionProxy.FindSimilarUnknownFunction("YCurrentLoopOutputProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YCurrentLoopOutputProxy)YFunctionProxy.FindSimilarKnownFunction("YCurrentLoopOutputProxy");
                if (res != null) return res;
                func = YCurrentLoopOutput.FirstCurrentLoopOutput();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YCurrentLoopOutputProxy)func.get_userData();
                    }
                }
            } else {
                func = YCurrentLoopOutput.FindCurrentLoopOutput(name);
                if (func.get_userData() != null) {
                    return (YCurrentLoopOutputProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YCurrentLoopOutputProxy(func, name);
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
 *   The <c>YCurrentLoopOutput</c> class allows you to drive a 4-20mA output
 *   by regulating the current flowing through the current loop.
 * <para>
 *   It can also provide information about the power state of the current loop.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YCurrentLoopOutputProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a 4-20mA output for a given identifier.
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
         *   This function does not require that the 4-20mA output is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YCurrentLoopOutput.isOnline()</c> to test if the 4-20mA output is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a 4-20mA output by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the 4-20mA output, for instance
         *   <c>TX420MA1.currentLoopOutput</c>.
         * </param>
         * <returns>
         *   a <c>YCurrentLoopOutput</c> object allowing you to drive the 4-20mA output.
         * </returns>
         */
        public static YCurrentLoopOutputProxy FindCurrentLoopOutput(string func)
        {
            return YoctoProxyManager.FindCurrentLoopOutput(func);
        }
        //--- (end of YCurrentLoopOutput class start)
        //--- (YCurrentLoopOutput definitions)
        public const double _Current_INVALID = Double.NaN;
        public const string _CurrentTransition_INVALID = YAPI.INVALID_STRING;
        public const double _CurrentAtStartUp_INVALID = Double.NaN;
        public const int _LoopPower_INVALID = 0;
        public const int _LoopPower_NOPWR = 1;
        public const int _LoopPower_LOWPWR = 2;
        public const int _LoopPower_POWEROK = 3;

        // reference to real YoctoAPI object
        protected new YCurrentLoopOutput _func;
        // property cache
        protected double _current = _Current_INVALID;
        protected double _currentAtStartUp = _CurrentAtStartUp_INVALID;
        protected int _loopPower = _LoopPower_INVALID;
        //--- (end of YCurrentLoopOutput definitions)

        //--- (YCurrentLoopOutput implementation)
        internal YCurrentLoopOutputProxy(YCurrentLoopOutput hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("CurrentLoopOutput " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YCurrentLoopOutput) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YCurrentLoopOutput hwd = YCurrentLoopOutput.FindCurrentLoopOutput(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YCurrentLoopOutput hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering CurrentLoopOutput callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type CurrentLoopOutput available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YCurrentLoopOutput.FindCurrentLoopOutput</c> to obtain an object that can control the
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
            YCurrentLoopOutput it = YCurrentLoopOutput.FirstCurrentLoopOutput();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextCurrentLoopOutput();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
            _loopPower = _func.get_loopPower()+1;
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            _currentAtStartUp = _func.get_currentAtStartUp();
        }

        /**
         * <summary>
         *   Changes the current loop, the valid range is from 3 to 21mA.
         * <para>
         *   If the loop is
         *   not properly powered, the  target current is not reached and
         *   loopPower is set to LOWPWR.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the current loop, the valid range is from 3 to 21mA
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
        public int set_current(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No CurrentLoopOutput connected");
            }
            if (newval == _Current_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_current(newval);
        }

        /**
         * <summary>
         *   Returns the loop current set point in mA.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the loop current set point in mA
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YCurrentLoopOutput.CURRENT_INVALID</c>.
         * </para>
         */
        public double get_current()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No CurrentLoopOutput connected");
            }
            res = _func.get_current();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        // property with cached value for instant access (advertised value)
        /// <value>Loop current set point in mA.</value>
        public double Current
        {
            get
            {
                if (_func == null) {
                    return _Current_INVALID;
                }
                if (_online) {
                    return _current;
                }
                return _Current_INVALID;
            }
            set
            {
                setprop_current(value);
            }
        }

        // private helper for magic property
        private void setprop_current(double newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Current_INVALID) {
                return;
            }
            if (newval == _current) {
                return;
            }
            _func.set_current(newval);
            _current = newval;
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            if (value == "NOPWR") {
                _loopPower = _LoopPower_NOPWR;
            } else {
                if (value == "LOWPWR") {
                    _loopPower = _LoopPower_LOWPWR;
                } else {
                    _loopPower = _LoopPower_POWEROK;
                    _current = YAPI._atof(value);
                }
            }
        }

        // property with cached value for instant access (derived from advertised value)
        /// <value>POWEROK when the loop is powered, NOPWR when the loop is not powered, LOWPWR when the loop is not powered enough to maintain requested current.</value>
        public int LoopPower
        {
            get
            {
                if (_func == null) {
                    return _LoopPower_INVALID;
                }
                if (_online) {
                    return _loopPower;
                }
                return _LoopPower_INVALID;
            }
        }

        /**
         * <summary>
         *   Changes the loop current at device start up.
         * <para>
         *   Remember to call the matching
         *   module <c>saveToFlash()</c> method, otherwise this call has no effect.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the loop current at device start up
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
        public int set_currentAtStartUp(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No CurrentLoopOutput connected");
            }
            if (newval == _CurrentAtStartUp_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_currentAtStartUp(newval);
        }

        /**
         * <summary>
         *   Returns the current in the loop at device startup, in mA.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the current in the loop at device startup, in mA
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YCurrentLoopOutput.CURRENTATSTARTUP_INVALID</c>.
         * </para>
         */
        public double get_currentAtStartUp()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No CurrentLoopOutput connected");
            }
            res = _func.get_currentAtStartUp();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        // property with cached value for instant access (configuration)
        /// <value>Current in the loop at device startup, in mA.</value>
        public double CurrentAtStartUp
        {
            get
            {
                if (_func == null) {
                    return _CurrentAtStartUp_INVALID;
                }
                if (_online) {
                    return _currentAtStartUp;
                }
                return _CurrentAtStartUp_INVALID;
            }
            set
            {
                setprop_currentAtStartUp(value);
            }
        }

        // private helper for magic property
        private void setprop_currentAtStartUp(double newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _CurrentAtStartUp_INVALID) {
                return;
            }
            if (newval == _currentAtStartUp) {
                return;
            }
            _func.set_currentAtStartUp(newval);
            _currentAtStartUp = newval;
        }

        /**
         * <summary>
         *   Returns the loop powerstate.
         * <para>
         *   POWEROK: the loop
         *   is powered. NOPWR: the loop in not powered. LOWPWR: the loop is not
         *   powered enough to maintain the current required (insufficient voltage).
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YCurrentLoopOutput.LOOPPOWER_NOPWR</c>, <c>YCurrentLoopOutput.LOOPPOWER_LOWPWR</c>
         *   and <c>YCurrentLoopOutput.LOOPPOWER_POWEROK</c> corresponding to the loop powerstate
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YCurrentLoopOutput.LOOPPOWER_INVALID</c>.
         * </para>
         */
        public int get_loopPower()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No CurrentLoopOutput connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_loopPower()+1;
        }

        /**
         * <summary>
         *   Performs a smooth transition of current flowing in the loop.
         * <para>
         *   Any current explicit
         *   change cancels any ongoing transition process.
         * </para>
         * </summary>
         * <param name="mA_target">
         *   new current value at the end of the transition
         *   (floating-point number, representing the end current in mA)
         * </param>
         * <param name="ms_duration">
         *   total duration of the transition, in milliseconds
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         */
        public virtual int currentMove(double mA_target, int ms_duration)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No CurrentLoopOutput connected");
            }
            return _func.currentMove(mA_target, ms_duration);
        }
    }
    //--- (end of YCurrentLoopOutput implementation)
}

