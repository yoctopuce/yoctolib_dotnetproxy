/*********************************************************************
 *
 *  $Id: yocto_anbutton_proxy.cs 38282 2019-11-21 14:50:25Z seb $
 *
 *  Implements YAnButtonProxy, the Proxy API for AnButton
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
    //--- (YAnButton class start)
    static public partial class YoctoProxyManager
    {
        public static YAnButtonProxy FindAnButton(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YAnButton func = null;
            YAnButtonProxy res = (YAnButtonProxy)YFunctionProxy.FindSimilarUnknownFunction("YAnButtonProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YAnButtonProxy)YFunctionProxy.FindSimilarKnownFunction("YAnButtonProxy");
                if (res != null) return res;
                func = YAnButton.FirstAnButton();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YAnButtonProxy)func.get_userData();
                    }
                }
            } else {
                func = YAnButton.FindAnButton(name);
                if (func.get_userData() != null) {
                    return (YAnButtonProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YAnButtonProxy(func, name);
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
 *   The YAnButton class allows you to access simple resistive inputs on Yoctopuce
 *   devices, for instance using a Yocto-Knob, a Yocto-MaxiDisplay, a Yocto-Buzzer or a Yocto-MaxiDisplay-G.
 * <para>
 *   Such inputs can be used to measure the state
 *   of a simple button as well as to read an analog potentiometer (variable resistance).
 *   This can be use for instance with a continuous rotating knob, a throttle grip
 *   or a joystick. The module is capable to calibrate itself on min and max values,
 *   in order to compute a calibrated value that varies proportionally with the
 *   potentiometer position, regardless of its total resistance.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YAnButtonProxy : YFunctionProxy
    {
        //--- (end of YAnButton class start)
        //--- (YAnButton definitions)
        public const int _CalibratedValue_INVALID = -1;
        public const int _RawValue_INVALID = -1;
        public const int _AnalogCalibration_INVALID = 0;
        public const int _AnalogCalibration_OFF = 1;
        public const int _AnalogCalibration_ON = 2;
        public const int _CalibrationMax_INVALID = -1;
        public const int _CalibrationMin_INVALID = -1;
        public const int _Sensitivity_INVALID = -1;
        public const int _IsPressed_INVALID = 0;
        public const int _IsPressed_FALSE = 1;
        public const int _IsPressed_TRUE = 2;
        public const long _LastTimePressed_INVALID = YAPI.INVALID_LONG;
        public const long _LastTimeReleased_INVALID = YAPI.INVALID_LONG;
        public const long _PulseCounter_INVALID = YAPI.INVALID_LONG;
        public const long _PulseTimer_INVALID = YAPI.INVALID_LONG;

        // reference to real YoctoAPI object
        protected new YAnButton _func;
        // property cache
        protected int _calibratedValue = _CalibratedValue_INVALID;
        protected int _analogCalibration = _AnalogCalibration_INVALID;
        protected int _calibrationMax = _CalibrationMax_INVALID;
        protected int _calibrationMin = _CalibrationMin_INVALID;
        protected int _sensitivity = _Sensitivity_INVALID;
        protected int _isPressed = _IsPressed_INVALID;
        //--- (end of YAnButton definitions)

        //--- (YAnButton implementation)
        internal YAnButtonProxy(YAnButton hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("AnButton " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YAnButton) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YAnButton hwd = YAnButton.FindAnButton(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YAnButton hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering AnButton callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YAnButton it = YAnButton.FirstAnButton();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextAnButton();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _isPressed = _func.get_isPressed()+1;
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _analogCalibration = _func.get_analogCalibration()+1;
            _calibrationMax = _func.get_calibrationMax();
            _calibrationMin = _func.get_calibrationMin();
            _sensitivity = _func.get_sensitivity();
        }

        // property with cached value for instant access (advertised value)
        public int CalibratedValue
        {
            get
            {
                if (_func == null) return _CalibratedValue_INVALID;
                return (_online ? _calibratedValue : _CalibratedValue_INVALID);
            }
        }

        // property with cached value for instant access (derived from advertised value)
        public int IsPressed
        {
            get
            {
                if (_func == null) return _IsPressed_INVALID;
                return (_online ? _isPressed : _IsPressed_INVALID);
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            Int32.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture,out _calibratedValue);
            if (_calibratedValue < 350) _isPressed = _IsPressed_TRUE;
            if (_calibratedValue > 650) _isPressed = _IsPressed_FALSE;
        }

        /**
         * <summary>
         *   Returns the current calibrated input value (between 0 and 1000, included).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current calibrated input value (between 0 and 1000, included)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YAnButton.CALIBRATEDVALUE_INVALID</c>.
         * </para>
         */
        public int get_calibratedValue()
        {
            if (_func == null)
            {
                string msg = "No AnButton connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_calibratedValue();
            if (res == YAPI.INVALID_INT) res = _CalibratedValue_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Returns the current measured input value as-is (between 0 and 4095, included).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current measured input value as-is (between 0 and 4095, included)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YAnButton.RAWVALUE_INVALID</c>.
         * </para>
         */
        public int get_rawValue()
        {
            if (_func == null)
            {
                string msg = "No AnButton connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_rawValue();
            if (res == YAPI.INVALID_INT) res = _RawValue_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Tells if a calibration process is currently ongoing.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YAnButton.ANALOGCALIBRATION_OFF</c> or <c>YAnButton.ANALOGCALIBRATION_ON</c>
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YAnButton.ANALOGCALIBRATION_INVALID</c>.
         * </para>
         */
        public int get_analogCalibration()
        {
            if (_func == null)
            {
                string msg = "No AnButton connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_analogCalibration()+1;
        }

        /**
         * <summary>
         *   Starts or stops the calibration process.
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module at the end of the calibration if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YAnButton.ANALOGCALIBRATION_OFF</c> or <c>YAnButton.ANALOGCALIBRATION_ON</c>
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
        public int set_analogCalibration(int newval)
        {
            if (_func == null)
            {
                string msg = "No AnButton connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _AnalogCalibration_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_analogCalibration(newval-1);
        }


        // property with cached value for instant access (configuration)
        public int AnalogCalibration
        {
            get
            {
                if (_func == null) return _AnalogCalibration_INVALID;
                return (_online ? _analogCalibration : _AnalogCalibration_INVALID);
            }
            set
            {
                setprop_analogCalibration(value);
            }
        }

        // private helper for magic property
        private void setprop_analogCalibration(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _AnalogCalibration_INVALID) return;
            if (newval == _analogCalibration) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_analogCalibration(newval-1);
            _analogCalibration = newval;
        }

        /**
         * <summary>
         *   Returns the maximal value measured during the calibration (between 0 and 4095, included).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the maximal value measured during the calibration (between 0 and 4095, included)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YAnButton.CALIBRATIONMAX_INVALID</c>.
         * </para>
         */
        public int get_calibrationMax()
        {
            if (_func == null)
            {
                string msg = "No AnButton connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_calibrationMax();
            if (res == YAPI.INVALID_INT) res = _CalibrationMax_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the maximal calibration value for the input (between 0 and 4095, included), without actually
         *   starting the automated calibration.
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the maximal calibration value for the input (between 0 and 4095,
         *   included), without actually
         *   starting the automated calibration
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
        public int set_calibrationMax(int newval)
        {
            if (_func == null)
            {
                string msg = "No AnButton connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _CalibrationMax_INVALID) return YAPI.SUCCESS;
            return _func.set_calibrationMax(newval);
        }


        // property with cached value for instant access (configuration)
        public int CalibrationMax
        {
            get
            {
                if (_func == null) return _CalibrationMax_INVALID;
                return (_online ? _calibrationMax : _CalibrationMax_INVALID);
            }
            set
            {
                setprop_calibrationMax(value);
            }
        }

        // private helper for magic property
        private void setprop_calibrationMax(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _CalibrationMax_INVALID) return;
            if (newval == _calibrationMax) return;
            _func.set_calibrationMax(newval);
            _calibrationMax = newval;
        }

        /**
         * <summary>
         *   Returns the minimal value measured during the calibration (between 0 and 4095, included).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the minimal value measured during the calibration (between 0 and 4095, included)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YAnButton.CALIBRATIONMIN_INVALID</c>.
         * </para>
         */
        public int get_calibrationMin()
        {
            if (_func == null)
            {
                string msg = "No AnButton connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_calibrationMin();
            if (res == YAPI.INVALID_INT) res = _CalibrationMin_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the minimal calibration value for the input (between 0 and 4095, included), without actually
         *   starting the automated calibration.
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the minimal calibration value for the input (between 0 and 4095,
         *   included), without actually
         *   starting the automated calibration
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
        public int set_calibrationMin(int newval)
        {
            if (_func == null)
            {
                string msg = "No AnButton connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _CalibrationMin_INVALID) return YAPI.SUCCESS;
            return _func.set_calibrationMin(newval);
        }


        // property with cached value for instant access (configuration)
        public int CalibrationMin
        {
            get
            {
                if (_func == null) return _CalibrationMin_INVALID;
                return (_online ? _calibrationMin : _CalibrationMin_INVALID);
            }
            set
            {
                setprop_calibrationMin(value);
            }
        }

        // private helper for magic property
        private void setprop_calibrationMin(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _CalibrationMin_INVALID) return;
            if (newval == _calibrationMin) return;
            _func.set_calibrationMin(newval);
            _calibrationMin = newval;
        }

        /**
         * <summary>
         *   Returns the sensibility for the input (between 1 and 1000) for triggering user callbacks.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the sensibility for the input (between 1 and 1000) for triggering user callbacks
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YAnButton.SENSITIVITY_INVALID</c>.
         * </para>
         */
        public int get_sensitivity()
        {
            if (_func == null)
            {
                string msg = "No AnButton connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_sensitivity();
            if (res == YAPI.INVALID_INT) res = _Sensitivity_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the sensibility for the input (between 1 and 1000) for triggering user callbacks.
         * <para>
         *   The sensibility is used to filter variations around a fixed value, but does not preclude the
         *   transmission of events when the input value evolves constantly in the same direction.
         *   Special case: when the value 1000 is used, the callback will only be thrown when the logical state
         *   of the input switches from pressed to released and back.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the sensibility for the input (between 1 and 1000) for triggering user callbacks
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
        public int set_sensitivity(int newval)
        {
            if (_func == null)
            {
                string msg = "No AnButton connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Sensitivity_INVALID) return YAPI.SUCCESS;
            return _func.set_sensitivity(newval);
        }


        // property with cached value for instant access (configuration)
        public int Sensitivity
        {
            get
            {
                if (_func == null) return _Sensitivity_INVALID;
                return (_online ? _sensitivity : _Sensitivity_INVALID);
            }
            set
            {
                setprop_sensitivity(value);
            }
        }

        // private helper for magic property
        private void setprop_sensitivity(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Sensitivity_INVALID) return;
            if (newval == _sensitivity) return;
            _func.set_sensitivity(newval);
            _sensitivity = newval;
        }

        /**
         * <summary>
         *   Returns true if the input (considered as binary) is active (closed contact), and false otherwise.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YAnButton.ISPRESSED_FALSE</c> or <c>YAnButton.ISPRESSED_TRUE</c>, according to true if
         *   the input (considered as binary) is active (closed contact), and false otherwise
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YAnButton.ISPRESSED_INVALID</c>.
         * </para>
         */
        public int get_isPressed()
        {
            if (_func == null)
            {
                string msg = "No AnButton connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_isPressed()+1;
        }

        /**
         * <summary>
         *   Returns the number of elapsed milliseconds between the module power on and the last time
         *   the input button was pressed (the input contact transitioned from open to closed).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of elapsed milliseconds between the module power on and the last time
         *   the input button was pressed (the input contact transitioned from open to closed)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YAnButton.LASTTIMEPRESSED_INVALID</c>.
         * </para>
         */
        public long get_lastTimePressed()
        {
            if (_func == null)
            {
                string msg = "No AnButton connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_lastTimePressed();
        }

        /**
         * <summary>
         *   Returns the number of elapsed milliseconds between the module power on and the last time
         *   the input button was released (the input contact transitioned from closed to open).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of elapsed milliseconds between the module power on and the last time
         *   the input button was released (the input contact transitioned from closed to open)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YAnButton.LASTTIMERELEASED_INVALID</c>.
         * </para>
         */
        public long get_lastTimeReleased()
        {
            if (_func == null)
            {
                string msg = "No AnButton connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_lastTimeReleased();
        }

        /**
         * <summary>
         *   Returns the pulse counter value.
         * <para>
         *   The value is a 32 bit integer. In case
         *   of overflow (>=2^32), the counter will wrap. To reset the counter, just
         *   call the resetCounter() method.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the pulse counter value
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YAnButton.PULSECOUNTER_INVALID</c>.
         * </para>
         */
        public long get_pulseCounter()
        {
            if (_func == null)
            {
                string msg = "No AnButton connected";
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
         *   On failure, throws an exception or returns <c>YAnButton.PULSETIMER_INVALID</c>.
         * </para>
         */
        public long get_pulseTimer()
        {
            if (_func == null)
            {
                string msg = "No AnButton connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_pulseTimer();
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
                string msg = "No AnButton connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.resetCounter();
        }
    }
    //--- (end of YAnButton implementation)
}

