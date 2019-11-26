/*********************************************************************
 *
 *  $Id: yocto_genericsensor_proxy.cs 38282 2019-11-21 14:50:25Z seb $
 *
 *  Implements YGenericSensorProxy, the Proxy API for GenericSensor
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
    //--- (YGenericSensor class start)
    static public partial class YoctoProxyManager
    {
        public static YGenericSensorProxy FindGenericSensor(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YGenericSensor func = null;
            YGenericSensorProxy res = (YGenericSensorProxy)YFunctionProxy.FindSimilarUnknownFunction("YGenericSensorProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YGenericSensorProxy)YFunctionProxy.FindSimilarKnownFunction("YGenericSensorProxy");
                if (res != null) return res;
                func = YGenericSensor.FirstGenericSensor();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YGenericSensorProxy)func.get_userData();
                    }
                }
            } else {
                func = YGenericSensor.FindGenericSensor(name);
                if (func.get_userData() != null) {
                    return (YGenericSensorProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YGenericSensorProxy(func, name);
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
 *   The YGenericSensor class allows you to read and configure Yoctopuce signal
 *   transducers, for instance using a Yocto-4-20mA-Rx, a Yocto-0-10V-Rx, a Yocto-milliVolt-Rx or a Yocto-RS232.
 * <para>
 *   It inherits from YSensor class the core functions to read measurements,
 *   to register callback functions, to access the autonomous datalogger.
 *   This class adds the ability to configure the automatic conversion between the
 *   measured signal and the corresponding engineering unit.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YGenericSensorProxy : YSensorProxy
    {
        //--- (end of YGenericSensor class start)
        //--- (YGenericSensor definitions)
        public const double _SignalValue_INVALID = Double.NaN;
        public const string _SignalUnit_INVALID = YAPI.INVALID_STRING;
        public const string _SignalRange_INVALID = YAPI.INVALID_STRING;
        public const string _ValueRange_INVALID = YAPI.INVALID_STRING;
        public const double _SignalBias_INVALID = Double.NaN;
        public const int _SignalSampling_INVALID = 0;
        public const int _SignalSampling_HIGH_RATE = 1;
        public const int _SignalSampling_HIGH_RATE_FILTERED = 2;
        public const int _SignalSampling_LOW_NOISE = 3;
        public const int _SignalSampling_LOW_NOISE_FILTERED = 4;
        public const int _SignalSampling_HIGHEST_RATE = 5;
        public const int _Enabled_INVALID = 0;
        public const int _Enabled_FALSE = 1;
        public const int _Enabled_TRUE = 2;

        // reference to real YoctoAPI object
        protected new YGenericSensor _func;
        // property cache
        protected string _signalUnit = _SignalUnit_INVALID;
        protected string _signalRange = _SignalRange_INVALID;
        protected string _valueRange = _ValueRange_INVALID;
        protected double _signalBias = _SignalBias_INVALID;
        protected int _signalSampling = _SignalSampling_INVALID;
        protected int _enabled = _Enabled_INVALID;
        //--- (end of YGenericSensor definitions)

        //--- (YGenericSensor implementation)
        internal YGenericSensorProxy(YGenericSensor hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("GenericSensor " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YGenericSensor) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YGenericSensor hwd = YGenericSensor.FindGenericSensor(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YGenericSensor hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering GenericSensor callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YGenericSensor it = YGenericSensor.FirstGenericSensor();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextGenericSensor();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
            _signalUnit = _func.get_signalUnit();
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            _signalRange = _func.get_signalRange();
            _valueRange = _func.get_valueRange();
            _signalBias = _func.get_signalBias();
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _signalSampling = _func.get_signalSampling()+1;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _enabled = _func.get_enabled()+1;
        }

        /**
         * <summary>
         *   Changes the measuring unit for the measured value.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the measuring unit for the measured value
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
                string msg = "No GenericSensor connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Unit_INVALID) return YAPI.SUCCESS;
            return _func.set_unit(newval);
        }


        /**
         * <summary>
         *   Returns the current value of the electrical signal measured by the sensor.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the current value of the electrical signal measured by the sensor
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGenericSensor.SIGNALVALUE_INVALID</c>.
         * </para>
         */
        public double get_signalValue()
        {
            if (_func == null)
            {
                string msg = "No GenericSensor connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_signalValue();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        // property with cached value for instant access (constant value)
        public string SignalUnit
        {
            get
            {
                if (_func == null) return _SignalUnit_INVALID;
                return (_online ? _signalUnit : _SignalUnit_INVALID);
            }
        }

        /**
         * <summary>
         *   Returns the measuring unit of the electrical signal used by the sensor.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the measuring unit of the electrical signal used by the sensor
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGenericSensor.SIGNALUNIT_INVALID</c>.
         * </para>
         */
        public string get_signalUnit()
        {
            if (_func == null)
            {
                string msg = "No GenericSensor connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_signalUnit();
        }

        /**
         * <summary>
         *   Returns the input signal range used by the sensor.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the input signal range used by the sensor
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGenericSensor.SIGNALRANGE_INVALID</c>.
         * </para>
         */
        public string get_signalRange()
        {
            if (_func == null)
            {
                string msg = "No GenericSensor connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_signalRange();
        }

        /**
         * <summary>
         *   Changes the input signal range used by the sensor.
         * <para>
         *   When the input signal gets out of the planned range, the output value
         *   will be set to an arbitrary large value, whose sign indicates the direction
         *   of the range overrun.
         * </para>
         * <para>
         *   For a 4-20mA sensor, the default input signal range is "4...20".
         *   For a 0-10V sensor, the default input signal range is "0.1...10".
         *   For numeric communication interfaces, the default input signal range is
         *   "-999999.999...999999.999".
         * </para>
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the input signal range used by the sensor
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
        public int set_signalRange(string newval)
        {
            if (_func == null)
            {
                string msg = "No GenericSensor connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _SignalRange_INVALID) return YAPI.SUCCESS;
            return _func.set_signalRange(newval);
        }


        // property with cached value for instant access (configuration)
        public string SignalRange
        {
            get
            {
                if (_func == null) return _SignalRange_INVALID;
                return (_online ? _signalRange : _SignalRange_INVALID);
            }
            set
            {
                setprop_signalRange(value);
            }
        }

        // private helper for magic property
        private void setprop_signalRange(string newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _SignalRange_INVALID) return;
            if (newval == _signalRange) return;
            _func.set_signalRange(newval);
            _signalRange = newval;
        }

        /**
         * <summary>
         *   Returns the physical value range measured by the sensor.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the physical value range measured by the sensor
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGenericSensor.VALUERANGE_INVALID</c>.
         * </para>
         */
        public string get_valueRange()
        {
            if (_func == null)
            {
                string msg = "No GenericSensor connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_valueRange();
        }

        /**
         * <summary>
         *   Changes the output value range, corresponding to the physical value measured
         *   by the sensor.
         * <para>
         *   The default output value range is the same as the input signal
         *   range (1:1 mapping), but you can change it so that the function automatically
         *   computes the physical value encoded by the input signal. Be aware that, as a
         *   side effect, the range modification may automatically modify the display resolution.
         * </para>
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the output value range, corresponding to the physical value measured
         *   by the sensor
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
        public int set_valueRange(string newval)
        {
            if (_func == null)
            {
                string msg = "No GenericSensor connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _ValueRange_INVALID) return YAPI.SUCCESS;
            return _func.set_valueRange(newval);
        }


        // property with cached value for instant access (configuration)
        public string ValueRange
        {
            get
            {
                if (_func == null) return _ValueRange_INVALID;
                return (_online ? _valueRange : _ValueRange_INVALID);
            }
            set
            {
                setprop_valueRange(value);
            }
        }

        // private helper for magic property
        private void setprop_valueRange(string newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _ValueRange_INVALID) return;
            if (newval == _valueRange) return;
            _func.set_valueRange(newval);
            _valueRange = newval;
        }

        /**
         * <summary>
         *   Changes the electric signal bias for zero shift adjustment.
         * <para>
         *   If your electric signal reads positive when it should be zero, setup
         *   a positive signalBias of the same value to fix the zero shift.
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the electric signal bias for zero shift adjustment
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
        public int set_signalBias(double newval)
        {
            if (_func == null)
            {
                string msg = "No GenericSensor connected";
                throw new YoctoApiProxyException(msg);
            }
            if (Double.IsNaN(newval)) return YAPI.SUCCESS;
            return _func.set_signalBias(newval);
        }


        // property with cached value for instant access (configuration)
        public double SignalBias
        {
            get
            {
                if (_func == null) return _SignalBias_INVALID;
                return (_online ? _signalBias : _SignalBias_INVALID);
            }
            set
            {
                setprop_signalBias(value);
            }
        }

        // private helper for magic property
        private void setprop_signalBias(double newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (Double.IsNaN(newval)) return;
            if (newval == _signalBias) return;
            _func.set_signalBias(newval);
            _signalBias = newval;
        }

        /**
         * <summary>
         *   Returns the electric signal bias for zero shift adjustment.
         * <para>
         *   A positive bias means that the signal is over-reporting the measure,
         *   while a negative bias means that the signal is under-reporting the measure.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the electric signal bias for zero shift adjustment
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGenericSensor.SIGNALBIAS_INVALID</c>.
         * </para>
         */
        public double get_signalBias()
        {
            if (_func == null)
            {
                string msg = "No GenericSensor connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_signalBias();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the electric signal sampling method to use.
         * <para>
         *   The <c>HIGH_RATE</c> method uses the highest sampling frequency, without any filtering.
         *   The <c>HIGH_RATE_FILTERED</c> method adds a windowed 7-sample median filter.
         *   The <c>LOW_NOISE</c> method uses a reduced acquisition frequency to reduce noise.
         *   The <c>LOW_NOISE_FILTERED</c> method combines a reduced frequency with the median filter
         *   to get measures as stable as possible when working on a noisy signal.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YGenericSensor.SIGNALSAMPLING_HIGH_RATE</c>,
         *   <c>YGenericSensor.SIGNALSAMPLING_HIGH_RATE_FILTERED</c>, <c>YGenericSensor.SIGNALSAMPLING_LOW_NOISE</c>,
         *   <c>YGenericSensor.SIGNALSAMPLING_LOW_NOISE_FILTERED</c> and <c>YGenericSensor.SIGNALSAMPLING_HIGHEST_RATE</c>
         *   corresponding to the electric signal sampling method to use
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGenericSensor.SIGNALSAMPLING_INVALID</c>.
         * </para>
         */
        public int get_signalSampling()
        {
            if (_func == null)
            {
                string msg = "No GenericSensor connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_signalSampling()+1;
        }

        /**
         * <summary>
         *   Changes the electric signal sampling method to use.
         * <para>
         *   The <c>HIGH_RATE</c> method uses the highest sampling frequency, without any filtering.
         *   The <c>HIGH_RATE_FILTERED</c> method adds a windowed 7-sample median filter.
         *   The <c>LOW_NOISE</c> method uses a reduced acquisition frequency to reduce noise.
         *   The <c>LOW_NOISE_FILTERED</c> method combines a reduced frequency with the median filter
         *   to get measures as stable as possible when working on a noisy signal.
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YGenericSensor.SIGNALSAMPLING_HIGH_RATE</c>,
         *   <c>YGenericSensor.SIGNALSAMPLING_HIGH_RATE_FILTERED</c>, <c>YGenericSensor.SIGNALSAMPLING_LOW_NOISE</c>,
         *   <c>YGenericSensor.SIGNALSAMPLING_LOW_NOISE_FILTERED</c> and <c>YGenericSensor.SIGNALSAMPLING_HIGHEST_RATE</c>
         *   corresponding to the electric signal sampling method to use
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
        public int set_signalSampling(int newval)
        {
            if (_func == null)
            {
                string msg = "No GenericSensor connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _SignalSampling_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_signalSampling(newval-1);
        }


        // property with cached value for instant access (configuration)
        public int SignalSampling
        {
            get
            {
                if (_func == null) return _SignalSampling_INVALID;
                return (_online ? _signalSampling : _SignalSampling_INVALID);
            }
            set
            {
                setprop_signalSampling(value);
            }
        }

        // private helper for magic property
        private void setprop_signalSampling(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _SignalSampling_INVALID) return;
            if (newval == _signalSampling) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_signalSampling(newval-1);
            _signalSampling = newval;
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
         *   either <c>YGenericSensor.ENABLED_FALSE</c> or <c>YGenericSensor.ENABLED_TRUE</c>, according to the
         *   activation state of this input
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGenericSensor.ENABLED_INVALID</c>.
         * </para>
         */
        public int get_enabled()
        {
            if (_func == null)
            {
                string msg = "No GenericSensor connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_enabled()+1;
        }

        /**
         * <summary>
         *   Changes the activation state of this input.
         * <para>
         *   When an input is disabled,
         *   its value is no more updated. On some devices, disabling an input can
         *   improve the refresh rate of the other active inputs.
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YGenericSensor.ENABLED_FALSE</c> or <c>YGenericSensor.ENABLED_TRUE</c>, according to the
         *   activation state of this input
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
                string msg = "No GenericSensor connected";
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

        /**
         * <summary>
         *   Adjusts the signal bias so that the current signal value is need
         *   precisely as zero.
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int zeroAdjust()
        {
            if (_func == null)
            {
                string msg = "No GenericSensor connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.zeroAdjust();
        }
    }
    //--- (end of YGenericSensor implementation)
}

