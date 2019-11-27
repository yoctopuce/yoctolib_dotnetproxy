/*********************************************************************
 *
 *  $Id: yocto_pwminput_proxy.cs 38514 2019-11-26 16:54:39Z seb $
 *
 *  Implements YPwmInputProxy, the Proxy API for PwmInput
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
    //--- (YPwmInput class start)
    static public partial class YoctoProxyManager
    {
        public static YPwmInputProxy FindPwmInput(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YPwmInput func = null;
            YPwmInputProxy res = (YPwmInputProxy)YFunctionProxy.FindSimilarUnknownFunction("YPwmInputProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YPwmInputProxy)YFunctionProxy.FindSimilarKnownFunction("YPwmInputProxy");
                if (res != null) return res;
                func = YPwmInput.FirstPwmInput();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YPwmInputProxy)func.get_userData();
                    }
                }
            } else {
                func = YPwmInput.FindPwmInput(name);
                if (func.get_userData() != null) {
                    return (YPwmInputProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YPwmInputProxy(func, name);
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
 *   The YPwmInput class allows you to read and configure Yoctopuce PWM
 *   sensors, for instance using a Yocto-PWM-Rx.
 * <para>
 *   It inherits from YSensor class the core functions to read measurements,
 *   to register callback functions, to access the autonomous datalogger.
 *   This class adds the ability to configure the signal parameter used to transmit
 *   information: the duty cycle, the frequency or the pulse width.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YPwmInputProxy : YSensorProxy
    {
        //--- (end of YPwmInput class start)
        //--- (YPwmInput definitions)
        public const double _DutyCycle_INVALID = Double.NaN;
        public const double _PulseDuration_INVALID = Double.NaN;
        public const double _Frequency_INVALID = Double.NaN;
        public const double _Period_INVALID = Double.NaN;
        public const long _PulseCounter_INVALID = YAPI.INVALID_LONG;
        public const long _PulseTimer_INVALID = YAPI.INVALID_LONG;
        public const int _PwmReportMode_INVALID = 0;
        public const int _PwmReportMode_PWM_DUTYCYCLE = 1;
        public const int _PwmReportMode_PWM_FREQUENCY = 2;
        public const int _PwmReportMode_PWM_PULSEDURATION = 3;
        public const int _PwmReportMode_PWM_EDGECOUNT = 4;
        public const int _PwmReportMode_PWM_PULSECOUNT = 5;
        public const int _PwmReportMode_PWM_CPS = 6;
        public const int _PwmReportMode_PWM_CPM = 7;
        public const int _PwmReportMode_PWM_STATE = 8;
        public const int _PwmReportMode_PWM_FREQ_CPS = 9;
        public const int _PwmReportMode_PWM_FREQ_CPM = 10;
        public const int _DebouncePeriod_INVALID = -1;

        // reference to real YoctoAPI object
        protected new YPwmInput _func;
        // property cache
        protected int _pwmReportMode = _PwmReportMode_INVALID;
        protected int _debouncePeriod = _DebouncePeriod_INVALID;
        //--- (end of YPwmInput definitions)

        //--- (YPwmInput implementation)
        internal YPwmInputProxy(YPwmInput hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("PwmInput " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YPwmInput) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YPwmInput hwd = YPwmInput.FindPwmInput(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YPwmInput hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering PwmInput callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YPwmInput it = YPwmInput.FirstPwmInput();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextPwmInput();
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
            _pwmReportMode = _func.get_pwmReportMode()+1;
            _debouncePeriod = _func.get_debouncePeriod();
        }

        /**
         * <summary>
         *   Changes the measuring unit for the measured quantity.
         * <para>
         *   That unit
         *   is just a string which is automatically initialized each time
         *   the measurement mode is changed. But is can be set to an
         *   arbitrary value.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the measuring unit for the measured quantity
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
        public int set_unit(string newval)
        {
            if (_func == null)
            {
                string msg = "No PwmInput connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Unit_INVALID) return YAPI.SUCCESS;
            return _func.set_unit(newval);
        }


        /**
         * <summary>
         *   Returns the PWM duty cycle, in per cents.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the PWM duty cycle, in per cents
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPwmInput.DUTYCYCLE_INVALID</c>.
         * </para>
         */
        public double get_dutyCycle()
        {
            if (_func == null)
            {
                string msg = "No PwmInput connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_dutyCycle();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the PWM pulse length in milliseconds, as a floating point number.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the PWM pulse length in milliseconds, as a floating point number
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPwmInput.PULSEDURATION_INVALID</c>.
         * </para>
         */
        public double get_pulseDuration()
        {
            if (_func == null)
            {
                string msg = "No PwmInput connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_pulseDuration();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the PWM frequency in Hz.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the PWM frequency in Hz
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPwmInput.FREQUENCY_INVALID</c>.
         * </para>
         */
        public double get_frequency()
        {
            if (_func == null)
            {
                string msg = "No PwmInput connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_frequency();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the PWM period in milliseconds.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the PWM period in milliseconds
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPwmInput.PERIOD_INVALID</c>.
         * </para>
         */
        public double get_period()
        {
            if (_func == null)
            {
                string msg = "No PwmInput connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_period();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the pulse counter value.
         * <para>
         *   Actually that
         *   counter is incremented twice per period. That counter is
         *   limited  to 1 billion.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the pulse counter value
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPwmInput.PULSECOUNTER_INVALID</c>.
         * </para>
         */
        public long get_pulseCounter()
        {
            if (_func == null)
            {
                string msg = "No PwmInput connected";
                throw new YoctoApiProxyException(msg);
            }
            long res = _func.get_pulseCounter();
            if (res == YAPI.INVALID_INT) res = _PulseCounter_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Returns the timer of the pulses counter (ms).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the timer of the pulses counter (ms)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPwmInput.PULSETIMER_INVALID</c>.
         * </para>
         */
        public long get_pulseTimer()
        {
            if (_func == null)
            {
                string msg = "No PwmInput connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_pulseTimer();
        }

        /**
         * <summary>
         *   Returns the parameter (frequency/duty cycle, pulse width, edges count) returned by the get_currentValue function and callbacks.
         * <para>
         *   Attention
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YPwmInput.PWMREPORTMODE_PWM_DUTYCYCLE</c>, <c>YPwmInput.PWMREPORTMODE_PWM_FREQUENCY</c>,
         *   <c>YPwmInput.PWMREPORTMODE_PWM_PULSEDURATION</c>, <c>YPwmInput.PWMREPORTMODE_PWM_EDGECOUNT</c>,
         *   <c>YPwmInput.PWMREPORTMODE_PWM_PULSECOUNT</c>, <c>YPwmInput.PWMREPORTMODE_PWM_CPS</c>,
         *   <c>YPwmInput.PWMREPORTMODE_PWM_CPM</c>, <c>YPwmInput.PWMREPORTMODE_PWM_STATE</c>,
         *   <c>YPwmInput.PWMREPORTMODE_PWM_FREQ_CPS</c> and <c>YPwmInput.PWMREPORTMODE_PWM_FREQ_CPM</c>
         *   corresponding to the parameter (frequency/duty cycle, pulse width, edges count) returned by the
         *   get_currentValue function and callbacks
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPwmInput.PWMREPORTMODE_INVALID</c>.
         * </para>
         */
        public int get_pwmReportMode()
        {
            if (_func == null)
            {
                string msg = "No PwmInput connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_pwmReportMode()+1;
        }

        /**
         * <summary>
         *   Changes the  parameter  type (frequency/duty cycle, pulse width, or edge count) returned by the get_currentValue function and callbacks.
         * <para>
         *   The edge count value is limited to the 6 lowest digits. For values greater than one million, use
         *   get_pulseCounter().
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YPwmInput.PWMREPORTMODE_PWM_DUTYCYCLE</c>, <c>YPwmInput.PWMREPORTMODE_PWM_FREQUENCY</c>,
         *   <c>YPwmInput.PWMREPORTMODE_PWM_PULSEDURATION</c>, <c>YPwmInput.PWMREPORTMODE_PWM_EDGECOUNT</c>,
         *   <c>YPwmInput.PWMREPORTMODE_PWM_PULSECOUNT</c>, <c>YPwmInput.PWMREPORTMODE_PWM_CPS</c>,
         *   <c>YPwmInput.PWMREPORTMODE_PWM_CPM</c>, <c>YPwmInput.PWMREPORTMODE_PWM_STATE</c>,
         *   <c>YPwmInput.PWMREPORTMODE_PWM_FREQ_CPS</c> and <c>YPwmInput.PWMREPORTMODE_PWM_FREQ_CPM</c>
         *   corresponding to the  parameter  type (frequency/duty cycle, pulse width, or edge count) returned
         *   by the get_currentValue function and callbacks
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
        public int set_pwmReportMode(int newval)
        {
            if (_func == null)
            {
                string msg = "No PwmInput connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _PwmReportMode_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_pwmReportMode(newval-1);
        }


        // property with cached value for instant access (configuration)
        public int PwmReportMode
        {
            get
            {
                if (_func == null) return _PwmReportMode_INVALID;
                return (_online ? _pwmReportMode : _PwmReportMode_INVALID);
            }
            set
            {
                setprop_pwmReportMode(value);
            }
        }

        // private helper for magic property
        private void setprop_pwmReportMode(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _PwmReportMode_INVALID) return;
            if (newval == _pwmReportMode) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_pwmReportMode(newval-1);
            _pwmReportMode = newval;
        }

        /**
         * <summary>
         *   Returns the shortest expected pulse duration, in ms.
         * <para>
         *   Any shorter pulse will be automatically ignored (debounce).
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the shortest expected pulse duration, in ms
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPwmInput.DEBOUNCEPERIOD_INVALID</c>.
         * </para>
         */
        public int get_debouncePeriod()
        {
            if (_func == null)
            {
                string msg = "No PwmInput connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_debouncePeriod();
            if (res == YAPI.INVALID_INT) res = _DebouncePeriod_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the shortest expected pulse duration, in ms.
         * <para>
         *   Any shorter pulse will be automatically ignored (debounce).
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the shortest expected pulse duration, in ms
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
        public int set_debouncePeriod(int newval)
        {
            if (_func == null)
            {
                string msg = "No PwmInput connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _DebouncePeriod_INVALID) return YAPI.SUCCESS;
            return _func.set_debouncePeriod(newval);
        }


        // property with cached value for instant access (configuration)
        public int DebouncePeriod
        {
            get
            {
                if (_func == null) return _DebouncePeriod_INVALID;
                return (_online ? _debouncePeriod : _DebouncePeriod_INVALID);
            }
            set
            {
                setprop_debouncePeriod(value);
            }
        }

        // private helper for magic property
        private void setprop_debouncePeriod(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _DebouncePeriod_INVALID) return;
            if (newval == _debouncePeriod) return;
            _func.set_debouncePeriod(newval);
            _debouncePeriod = newval;
        }

        /**
         * <summary>
         *   Returns the pulse counter value as well as its timer.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int resetCounter()
        {
            if (_func == null)
            {
                string msg = "No PwmInput connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.resetCounter();
        }
    }
    //--- (end of YPwmInput implementation)
}

