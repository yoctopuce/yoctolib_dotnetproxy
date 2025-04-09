/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YPwmOutputProxy, the Proxy API for PwmOutput
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
    //--- (YPwmOutput class start)
    static public partial class YoctoProxyManager
    {
        public static YPwmOutputProxy FindPwmOutput(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YPwmOutput func = null;
            YPwmOutputProxy res = (YPwmOutputProxy)YFunctionProxy.FindSimilarUnknownFunction("YPwmOutputProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YPwmOutputProxy)YFunctionProxy.FindSimilarKnownFunction("YPwmOutputProxy");
                if (res != null) return res;
                func = YPwmOutput.FirstPwmOutput();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YPwmOutputProxy)func.get_userData();
                    }
                }
            } else {
                func = YPwmOutput.FindPwmOutput(name);
                if (func.get_userData() != null) {
                    return (YPwmOutputProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YPwmOutputProxy(func, name);
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
 *   The <c>YPwmOutput</c> class allows you to drive a pulse-width modulated output (PWM).
 * <para>
 *   You can configure the frequency as well as the duty cycle, and set up progressive
 *   transitions.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YPwmOutputProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a PWM generator for a given identifier.
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
         *   This function does not require that the PWM generator is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YPwmOutput.isOnline()</c> to test if the PWM generator is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a PWM generator by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the PWM generator, for instance
         *   <c>YPWMTX01.pwmOutput1</c>.
         * </param>
         * <returns>
         *   a <c>YPwmOutput</c> object allowing you to drive the PWM generator.
         * </returns>
         */
        public static YPwmOutputProxy FindPwmOutput(string func)
        {
            return YoctoProxyManager.FindPwmOutput(func);
        }
        //--- (end of YPwmOutput class start)
        //--- (YPwmOutput definitions)
        public const int _Enabled_INVALID = 0;
        public const int _Enabled_FALSE = 1;
        public const int _Enabled_TRUE = 2;
        public const double _Frequency_INVALID = Double.NaN;
        public const double _Period_INVALID = Double.NaN;
        public const double _DutyCycle_INVALID = Double.NaN;
        public const double _PulseDuration_INVALID = Double.NaN;
        public const string _PwmTransition_INVALID = YAPI.INVALID_STRING;
        public const int _InvertedOutput_INVALID = 0;
        public const int _InvertedOutput_FALSE = 1;
        public const int _InvertedOutput_TRUE = 2;
        public const int _EnabledAtPowerOn_INVALID = 0;
        public const int _EnabledAtPowerOn_FALSE = 1;
        public const int _EnabledAtPowerOn_TRUE = 2;
        public const double _DutyCycleAtPowerOn_INVALID = Double.NaN;

        // reference to real YoctoAPI object
        protected new YPwmOutput _func;
        // property cache
        protected double _frequency = _Frequency_INVALID;
        protected double _period = _Period_INVALID;
        protected double _dutyCycle = _DutyCycle_INVALID;
        protected int _invertedOutput = _InvertedOutput_INVALID;
        protected int _enabledAtPowerOn = _EnabledAtPowerOn_INVALID;
        protected double _dutyCycleAtPowerOn = _DutyCycleAtPowerOn_INVALID;
        protected int _enabled = _Enabled_INVALID;
        //--- (end of YPwmOutput definitions)

        //--- (YPwmOutput implementation)
        internal YPwmOutputProxy(YPwmOutput hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("PwmOutput " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YPwmOutput) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YPwmOutput hwd = YPwmOutput.FindPwmOutput(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YPwmOutput hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering PwmOutput callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type PwmOutput available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YPwmOutput.FindPwmOutput</c> to obtain an object that can control the
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
            YPwmOutput it = YPwmOutput.FirstPwmOutput();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextPwmOutput();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
            _enabled = _func.get_enabled()+1;
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            _frequency = _func.get_frequency();
            _period = _func.get_period();
            _invertedOutput = _func.get_invertedOutput()+1;
            _enabledAtPowerOn = _func.get_enabledAtPowerOn()+1;
            _dutyCycleAtPowerOn = _func.get_dutyCycleAtPowerOn();
        }

        /**
         * <summary>
         *   Returns the state of the PWM generators.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YPwmOutput.ENABLED_FALSE</c> or <c>YPwmOutput.ENABLED_TRUE</c>, according to the state of
         *   the PWM generators
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPwmOutput.ENABLED_INVALID</c>.
         * </para>
         */
        public int get_enabled()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_enabled()+1;
        }

        /**
         * <summary>
         *   Stops or starts the PWM.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YPwmOutput.ENABLED_FALSE</c> or <c>YPwmOutput.ENABLED_TRUE</c>
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
        public int set_enabled(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            if (newval == _Enabled_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_enabled(newval-1);
        }

        /**
         * <summary>
         *   Changes the PWM frequency.
         * <para>
         *   The duty cycle is kept unchanged thanks to an
         *   automatic pulse width change, in other words, the change will not be applied
         *   before the end of the current period. This can significantly affect reaction
         *   time at low frequencies. If you call the matching module <c>saveToFlash()</c>
         *   method, the frequency will be kept after a device power cycle.
         *   To stop the PWM signal, do not set the frequency to zero, use the set_enabled()
         *   method instead.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the PWM frequency
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
        public int set_frequency(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            if (newval == _Frequency_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_frequency(newval);
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
         *   On failure, throws an exception or returns <c>YPwmOutput.FREQUENCY_INVALID</c>.
         * </para>
         */
        public double get_frequency()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            res = _func.get_frequency();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        // property with cached value for instant access (configuration)
        /// <value>PWM frequency in Hz.</value>
        public double Frequency
        {
            get
            {
                if (_func == null) {
                    return _Frequency_INVALID;
                }
                if (_online) {
                    return _frequency;
                }
                return _Frequency_INVALID;
            }
            set
            {
                setprop_frequency(value);
            }
        }

        // private helper for magic property
        private void setprop_frequency(double newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Frequency_INVALID) {
                return;
            }
            if (newval == _frequency) {
                return;
            }
            _func.set_frequency(newval);
            _frequency = newval;
        }

        /**
         * <summary>
         *   Changes the PWM period in milliseconds.
         * <para>
         *   Caution: in order to avoid  random truncation of
         *   the current pulse, the change will not be applied
         *   before the end of the current period. This can significantly affect reaction
         *   time at low frequencies. If you call the matching module <c>saveToFlash()</c>
         *   method, the frequency will be kept after a device power cycle.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the PWM period in milliseconds
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
        public int set_period(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            if (newval == _Period_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_period(newval);
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
         *   On failure, throws an exception or returns <c>YPwmOutput.PERIOD_INVALID</c>.
         * </para>
         */
        public double get_period()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            res = _func.get_period();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        // property with cached value for instant access (configuration)
        /// <value>PWM period in milliseconds.</value>
        public double Period
        {
            get
            {
                if (_func == null) {
                    return _Period_INVALID;
                }
                if (_online) {
                    return _period;
                }
                return _Period_INVALID;
            }
            set
            {
                setprop_period(value);
            }
        }

        // private helper for magic property
        private void setprop_period(double newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Period_INVALID) {
                return;
            }
            if (newval == _period) {
                return;
            }
            _func.set_period(newval);
            _period = newval;
        }

        /**
         * <summary>
         *   Changes the PWM duty cycle, in per cents.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the PWM duty cycle, in per cents
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
        public int set_dutyCycle(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            if (newval == _DutyCycle_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_dutyCycle(newval);
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
         *   On failure, throws an exception or returns <c>YPwmOutput.DUTYCYCLE_INVALID</c>.
         * </para>
         */
        public double get_dutyCycle()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            res = _func.get_dutyCycle();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        // property with cached value for instant access (advertised value)
        /// <value>PWM duty cycle, in per cents.</value>
        public double DutyCycle
        {
            get
            {
                if (_func == null) {
                    return _DutyCycle_INVALID;
                }
                if (_online) {
                    return _dutyCycle;
                }
                return _DutyCycle_INVALID;
            }
            set
            {
                setprop_dutyCycle(value);
            }
        }

        // private helper for magic property
        private void setprop_dutyCycle(double newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _DutyCycle_INVALID) {
                return;
            }
            if (newval == _dutyCycle) {
                return;
            }
            _func.set_dutyCycle(newval);
            _dutyCycle = newval;
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            if (value == "OFF") {
                _enabled = _Enabled_FALSE;
            } else {
                _enabled = _Enabled_TRUE;
                _dutyCycle = YAPI._atoi(value);
            }
        }

        // property with cached value for instant access (derived from advertised value)
        /// <value>True if the port output is enabled.</value>
        public int Enabled
        {
            get
            {
                if (_func == null) {
                    return _Enabled_INVALID;
                }
                if (_online) {
                    return _enabled;
                }
                return _Enabled_INVALID;
            }
            set
            {
                setprop_enabled(value);
            }
        }

        // private helper for magic property
        private void setprop_enabled(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Enabled_INVALID) {
                return;
            }
            if (newval == _enabled) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_enabled(newval-1);
            _enabled = newval;
        }

        /**
         * <summary>
         *   Changes the PWM pulse length, in milliseconds.
         * <para>
         *   A pulse length cannot be longer than period, otherwise it is truncated.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the PWM pulse length, in milliseconds
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
        public int set_pulseDuration(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            if (newval == _PulseDuration_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_pulseDuration(newval);
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
         *   On failure, throws an exception or returns <c>YPwmOutput.PULSEDURATION_INVALID</c>.
         * </para>
         */
        public double get_pulseDuration()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            res = _func.get_pulseDuration();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns true if the output signal is configured as inverted, and false otherwise.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YPwmOutput.INVERTEDOUTPUT_FALSE</c> or <c>YPwmOutput.INVERTEDOUTPUT_TRUE</c>, according
         *   to true if the output signal is configured as inverted, and false otherwise
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPwmOutput.INVERTEDOUTPUT_INVALID</c>.
         * </para>
         */
        public int get_invertedOutput()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_invertedOutput()+1;
        }

        /**
         * <summary>
         *   Changes the inversion mode of the output signal.
         * <para>
         *   Remember to call the matching module <c>saveToFlash()</c> method if you want
         *   the change to be kept after power cycle.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YPwmOutput.INVERTEDOUTPUT_FALSE</c> or <c>YPwmOutput.INVERTEDOUTPUT_TRUE</c>, according
         *   to the inversion mode of the output signal
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
        public int set_invertedOutput(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            if (newval == _InvertedOutput_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_invertedOutput(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>True if the output signal is configured as inverted, and false otherwise.</value>
        public int InvertedOutput
        {
            get
            {
                if (_func == null) {
                    return _InvertedOutput_INVALID;
                }
                if (_online) {
                    return _invertedOutput;
                }
                return _InvertedOutput_INVALID;
            }
            set
            {
                setprop_invertedOutput(value);
            }
        }

        // private helper for magic property
        private void setprop_invertedOutput(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _InvertedOutput_INVALID) {
                return;
            }
            if (newval == _invertedOutput) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_invertedOutput(newval-1);
            _invertedOutput = newval;
        }

        /**
         * <summary>
         *   Returns the state of the PWM at device power on.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YPwmOutput.ENABLEDATPOWERON_FALSE</c> or <c>YPwmOutput.ENABLEDATPOWERON_TRUE</c>,
         *   according to the state of the PWM at device power on
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPwmOutput.ENABLEDATPOWERON_INVALID</c>.
         * </para>
         */
        public int get_enabledAtPowerOn()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_enabledAtPowerOn()+1;
        }

        /**
         * <summary>
         *   Changes the state of the PWM at device power on.
         * <para>
         *   Remember to call the matching module <c>saveToFlash()</c>
         *   method, otherwise this call will have no effect.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YPwmOutput.ENABLEDATPOWERON_FALSE</c> or <c>YPwmOutput.ENABLEDATPOWERON_TRUE</c>,
         *   according to the state of the PWM at device power on
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
        public int set_enabledAtPowerOn(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            if (newval == _EnabledAtPowerOn_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_enabledAtPowerOn(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>State of the PWM at device power on.</value>
        public int EnabledAtPowerOn
        {
            get
            {
                if (_func == null) {
                    return _EnabledAtPowerOn_INVALID;
                }
                if (_online) {
                    return _enabledAtPowerOn;
                }
                return _EnabledAtPowerOn_INVALID;
            }
            set
            {
                setprop_enabledAtPowerOn(value);
            }
        }

        // private helper for magic property
        private void setprop_enabledAtPowerOn(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _EnabledAtPowerOn_INVALID) {
                return;
            }
            if (newval == _enabledAtPowerOn) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_enabledAtPowerOn(newval-1);
            _enabledAtPowerOn = newval;
        }

        /**
         * <summary>
         *   Changes the PWM duty cycle at device power on.
         * <para>
         *   Remember to call the matching
         *   module <c>saveToFlash()</c> method, otherwise this call will have no effect.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the PWM duty cycle at device power on
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
        public int set_dutyCycleAtPowerOn(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            if (newval == _DutyCycleAtPowerOn_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_dutyCycleAtPowerOn(newval);
        }

        /**
         * <summary>
         *   Returns the PWM generators duty cycle at device power on as a floating point number between 0 and 100.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the PWM generators duty cycle at device power on as a
         *   floating point number between 0 and 100
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPwmOutput.DUTYCYCLEATPOWERON_INVALID</c>.
         * </para>
         */
        public double get_dutyCycleAtPowerOn()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            res = _func.get_dutyCycleAtPowerOn();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        // property with cached value for instant access (configuration)
        /// <value>PWM generators duty cycle at device power on as a floating point number between 0 and 100.</value>
        public double DutyCycleAtPowerOn
        {
            get
            {
                if (_func == null) {
                    return _DutyCycleAtPowerOn_INVALID;
                }
                if (_online) {
                    return _dutyCycleAtPowerOn;
                }
                return _DutyCycleAtPowerOn_INVALID;
            }
            set
            {
                setprop_dutyCycleAtPowerOn(value);
            }
        }

        // private helper for magic property
        private void setprop_dutyCycleAtPowerOn(double newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _DutyCycleAtPowerOn_INVALID) {
                return;
            }
            if (newval == _dutyCycleAtPowerOn) {
                return;
            }
            _func.set_dutyCycleAtPowerOn(newval);
            _dutyCycleAtPowerOn = newval;
        }

        /**
         * <summary>
         *   Performs a smooth transition of the pulse duration toward a given value.
         * <para>
         *   Any period, frequency, duty cycle or pulse width change will cancel any ongoing transition process.
         * </para>
         * </summary>
         * <param name="ms_target">
         *   new pulse duration at the end of the transition
         *   (floating-point number, representing the pulse duration in milliseconds)
         * </param>
         * <param name="ms_duration">
         *   total duration of the transition, in milliseconds
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int pulseDurationMove(double ms_target, int ms_duration)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            return _func.pulseDurationMove(ms_target, ms_duration);
        }

        /**
         * <summary>
         *   Performs a smooth change of the duty cycle toward a given value.
         * <para>
         *   Any period, frequency, duty cycle or pulse width change will cancel any ongoing transition process.
         * </para>
         * </summary>
         * <param name="target">
         *   new duty cycle at the end of the transition
         *   (percentage, floating-point number between 0 and 100)
         * </param>
         * <param name="ms_duration">
         *   total duration of the transition, in milliseconds
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int dutyCycleMove(double target, int ms_duration)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            return _func.dutyCycleMove(target, ms_duration);
        }

        /**
         * <summary>
         *   Performs a smooth frequency change toward a given value.
         * <para>
         *   Any period, frequency, duty cycle or pulse width change will cancel any ongoing transition process.
         * </para>
         * </summary>
         * <param name="target">
         *   new frequency at the end of the transition (floating-point number)
         * </param>
         * <param name="ms_duration">
         *   total duration of the transition, in milliseconds
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int frequencyMove(double target, int ms_duration)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            return _func.frequencyMove(target, ms_duration);
        }

        /**
         * <summary>
         *   Performs a smooth transition toward a specified value of the phase shift between this channel
         *   and the other channel.
         * <para>
         *   The phase shift is executed by slightly changing the frequency
         *   temporarily during the specified duration. This function only makes sense when both channels
         *   are running, either at the same frequency, or at a multiple of the channel frequency.
         *   Any period, frequency, duty cycle or pulse width change will cancel any ongoing transition process.
         * </para>
         * </summary>
         * <param name="target">
         *   phase shift at the end of the transition, in milliseconds (floating-point number)
         * </param>
         * <param name="ms_duration">
         *   total duration of the transition, in milliseconds
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int phaseMove(double target, int ms_duration)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            return _func.phaseMove(target, ms_duration);
        }

        /**
         * <summary>
         *   Trigger a given number of pulses of specified duration, at current frequency.
         * <para>
         *   At the end of the pulse train, revert to the original state of the PWM generator.
         * </para>
         * </summary>
         * <param name="ms_target">
         *   desired pulse duration
         *   (floating-point number, representing the pulse duration in milliseconds)
         * </param>
         * <param name="n_pulses">
         *   desired pulse count
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int triggerPulsesByDuration(double ms_target, int n_pulses)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            return _func.triggerPulsesByDuration(ms_target, n_pulses);
        }

        /**
         * <summary>
         *   Trigger a given number of pulses of specified duration, at current frequency.
         * <para>
         *   At the end of the pulse train, revert to the original state of the PWM generator.
         * </para>
         * </summary>
         * <param name="target">
         *   desired duty cycle for the generated pulses
         *   (percentage, floating-point number between 0 and 100)
         * </param>
         * <param name="n_pulses">
         *   desired pulse count
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int triggerPulsesByDutyCycle(double target, int n_pulses)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            return _func.triggerPulsesByDutyCycle(target, n_pulses);
        }

        /**
         * <summary>
         *   Trigger a given number of pulses at the specified frequency, using current duty cycle.
         * <para>
         *   At the end of the pulse train, revert to the original state of the PWM generator.
         * </para>
         * </summary>
         * <param name="target">
         *   desired frequency for the generated pulses (floating-point number)
         * </param>
         * <param name="n_pulses">
         *   desired pulse count
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int triggerPulsesByFrequency(double target, int n_pulses)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmOutput connected");
            }
            return _func.triggerPulsesByFrequency(target, n_pulses);
        }
    }
    //--- (end of YPwmOutput implementation)
}

