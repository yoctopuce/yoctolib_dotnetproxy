/*********************************************************************
 *
 *  $Id: yocto_sensor_proxy.cs 59504 2024-02-26 11:42:03Z seb $
 *
 *  Implements YSensorProxy, the Proxy API for Sensor
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
    //--- (generated code: YSensor class start)
    static public partial class YoctoProxyManager
    {
        public static YSensorProxy FindSensor(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YSensor func = null;
            YSensorProxy res = (YSensorProxy)YFunctionProxy.FindSimilarUnknownFunction("YSensorProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YSensorProxy)YFunctionProxy.FindSimilarKnownFunction("YSensorProxy");
                if (res != null) return res;
                func = YSensor.FirstSensor();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YSensorProxy)func.get_userData();
                    }
                }
            } else {
                func = YSensor.FindSensor(name);
                if (func.get_userData() != null) {
                    return (YSensorProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YSensorProxy(func, name);
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
 *   The <c>YSensor</c> class is the parent class for all Yoctopuce sensor types.
 * <para>
 *   It can be
 *   used to read the current value and unit of any sensor, read the min/max
 *   value, configure autonomous recording frequency and access recorded data.
 *   It also provide a function to register a callback invoked each time the
 *   observed value changes, or at a predefined interval. Using this class rather
 *   than a specific subclass makes it possible to create generic applications
 *   that work with any Yoctopuce sensor, even those that do not yet exist.
 *   Note: The <c>YAnButton</c> class is the only analog input which does not inherit
 *   from <c>YSensor</c>.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YSensorProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a sensor for a given identifier.
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
         *   This function does not require that the sensor is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YSensor.isOnline()</c> to test if the sensor is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a sensor by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the sensor, for instance
         *   <c>MyDevice.</c>.
         * </param>
         * <returns>
         *   a <c>YSensor</c> object allowing you to drive the sensor.
         * </returns>
         */
        public static YSensorProxy FindSensor(string func)
        {
            return YoctoProxyManager.FindSensor(func);
        }
        //--- (end of generated code: YSensor class start)
        //--- (generated code: YSensor definitions)
        public const string _Unit_INVALID = YAPI.INVALID_STRING;
        public const double _CurrentValue_INVALID = Double.NaN;
        public const double _LowestValue_INVALID = Double.NaN;
        public const double _HighestValue_INVALID = Double.NaN;
        public const double _CurrentRawValue_INVALID = Double.NaN;
        public const string _LogFrequency_INVALID = YAPI.INVALID_STRING;
        public const string _ReportFrequency_INVALID = YAPI.INVALID_STRING;
        public const int _AdvMode_INVALID = 0;
        public const int _AdvMode_IMMEDIATE = 1;
        public const int _AdvMode_PERIOD_AVG = 2;
        public const int _AdvMode_PERIOD_MIN = 3;
        public const int _AdvMode_PERIOD_MAX = 4;
        public const string _CalibrationParam_INVALID = YAPI.INVALID_STRING;
        public const double _Resolution_INVALID = Double.NaN;
        public const int _SensorState_INVALID = YAPI.INVALID_INT;

        // reference to real YoctoAPI object
        protected new YSensor _func;
        // property cache
        protected double _currentValue = _CurrentValue_INVALID;
        protected string _logFrequency = _LogFrequency_INVALID;
        protected string _reportFrequency = _ReportFrequency_INVALID;
        protected int _advMode = _AdvMode_INVALID;
        protected double _resolution = _Resolution_INVALID;
        //--- (end of generated code: YSensor definitions)
        protected string _unit = "";
        //hidden function for YConsolidatedDataSetProxy
        internal YSensor get_ysensor()
        {
            return _func;
        }

        // property with cached value for instant access
        /// <value>Last measurement reported by the sensor, according to the update policy specified by UpdateFrequency</value>
        public virtual Double CurrentValue
        {
            get
            {
                if (_func == null) return Double.NaN;
                return (_online ? _currentValue : Double.NaN);
            }
        }

        // property with cached value for instant access
        /// <value>Measuring unit for the sensor</value>
        public virtual string Unit
        {
            get
            {
                if (_func == null) return "";
                return (_online ? _unit : "");
            }
        }

        protected string _updateFrequency = "";

        /// <value>Update policy for the sensor: "auto" for an update at every change, "x/s" for an update x time per second with the instant value, "x/m" or "x/h" for an update x times per minute (resp. hour) with the average value over the latest period.</value>
        public virtual string UpdateFrequency
        {
            get
            {
                if (_func == null) return "";
                return (_online ? _updateFrequency : "");
            }

            set
            {
                if (value == "") return;

                value = value.ToLower();
                int p = value.IndexOf("/");
                if (p < 1) return;
                int n;
                if (!int.TryParse(value.Substring(0, p), out n)) return;
                if (n < 1) return;
                string u = value.Substring(p + 1);
                if ((u != "s") && (u != "m") && (u != "h")) return;
                if (!_online) return;

                if (_func == null)
                {
                    string msg = "No Sensor connected";
                    throw new YoctoApiProxyException(msg);
                }

                if (value != _updateFrequency.ToLower())
                {
                    if (value.ToLower() == "auto")
                        _func.set_reportFrequency("OFF");
                    else
                        _func.set_reportFrequency(value);
                    _updateFrequency = value.ToLower();
                }
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            if (_updateFrequency == "auto")
            {
                if (!Double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out _currentValue))
                {
                    _currentValue = _CurrentValue_INVALID;
                }
            }
        }

        protected void timedReportCallback(YFunction source, YMeasure m)
        {
            if (_updateFrequency != "auto")
            {
                _currentValue = m.get_averageValue();
            }
        }

        //--- (generated code: YSensor implementation)
        internal YSensorProxy(YSensor hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Sensor " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YSensor) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YSensor hwd = YSensor.FindSensor(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YSensor hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Sensor callback");
            _func.registerValueCallback(valueChangeCallback);
            _func.registerTimedReportCallback(timedReportCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type Sensor available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YSensor.FindSensor</c> to obtain an object that can control the
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
            YSensor it = YSensor.FirstSensor();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextSensor();
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
            _currentValue = _func.get_currentValue();
            if (_currentValue == YAPI.INVALID_DOUBLE) _currentValue = Double.NaN;
            _unit = _func.get_unit();
            _updateFrequency = _func.get_reportFrequency();
            if (_updateFrequency == "OFF") _updateFrequency = "auto";
            _logFrequency = _func.get_logFrequency();
            _reportFrequency = _func.get_reportFrequency();
            _advMode = _func.get_advMode()+1;
            _resolution = _func.get_resolution();
        }

        /**
         * <summary>
         *   Returns the measuring unit for the measure.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the measuring unit for the measure
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSensor.UNIT_INVALID</c>.
         * </para>
         */
        public string get_unit()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            return _func.get_unit();
        }

        /**
         * <summary>
         *   Returns the current value of the measure, in the specified unit, as a floating point number.
         * <para>
         *   Note that a get_currentValue() call will *not* start a measure in the device, it
         *   will just return the last measure that occurred in the device. Indeed, internally, each Yoctopuce
         *   devices is continuously making measurements at a hardware specific frequency.
         * </para>
         * <para>
         *   If continuously calling  get_currentValue() leads you to performances issues, then
         *   you might consider to switch to callback programming model. Check the "advanced
         *   programming" chapter in in your device user manual for more information.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the current value of the measure, in the specified unit,
         *   as a floating point number
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSensor.CURRENTVALUE_INVALID</c>.
         * </para>
         */
        public double get_currentValue()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            res = _func.get_currentValue();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the recorded minimal value observed.
         * <para>
         *   Can be used to reset the value returned
         *   by get_lowestValue().
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the recorded minimal value observed
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
        public int set_lowestValue(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            if (newval == _LowestValue_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_lowestValue(newval);
        }

        /**
         * <summary>
         *   Returns the minimal value observed for the measure since the device was started.
         * <para>
         *   Can be reset to an arbitrary value thanks to set_lowestValue().
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the minimal value observed for the measure since the device was started
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSensor.LOWESTVALUE_INVALID</c>.
         * </para>
         */
        public double get_lowestValue()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            res = _func.get_lowestValue();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the recorded maximal value observed.
         * <para>
         *   Can be used to reset the value returned
         *   by get_lowestValue().
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the recorded maximal value observed
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
        public int set_highestValue(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            if (newval == _HighestValue_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_highestValue(newval);
        }

        /**
         * <summary>
         *   Returns the maximal value observed for the measure since the device was started.
         * <para>
         *   Can be reset to an arbitrary value thanks to set_highestValue().
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the maximal value observed for the measure since the device was started
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSensor.HIGHESTVALUE_INVALID</c>.
         * </para>
         */
        public double get_highestValue()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            res = _func.get_highestValue();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the uncalibrated, unrounded raw value returned by the
         *   sensor, in the specified unit, as a floating point number.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the uncalibrated, unrounded raw value returned by the
         *   sensor, in the specified unit, as a floating point number
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSensor.CURRENTRAWVALUE_INVALID</c>.
         * </para>
         */
        public double get_currentRawValue()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            res = _func.get_currentRawValue();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the datalogger recording frequency for this function, or "OFF"
         *   when measures are not stored in the data logger flash memory.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the datalogger recording frequency for this function, or "OFF"
         *   when measures are not stored in the data logger flash memory
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSensor.LOGFREQUENCY_INVALID</c>.
         * </para>
         */
        public string get_logFrequency()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            return _func.get_logFrequency();
        }

        /**
         * <summary>
         *   Changes the datalogger recording frequency for this function.
         * <para>
         *   The frequency can be specified as samples per second,
         *   as sample per minute (for instance "15/m") or in samples per
         *   hour (eg. "4/h"). To disable recording for this function, use
         *   the value "OFF". Note that setting the  datalogger recording frequency
         *   to a greater value than the sensor native sampling frequency is useless,
         *   and even counterproductive: those two frequencies are not related.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the datalogger recording frequency for this function
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
        public int set_logFrequency(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            if (newval == _LogFrequency_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_logFrequency(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Datalogger recording frequency for this function, or "OFF"</value>
        public string LogFrequency
        {
            get
            {
                if (_func == null) {
                    return _LogFrequency_INVALID;
                }
                if (_online) {
                    return _logFrequency;
                }
                return _LogFrequency_INVALID;
            }
            set
            {
                setprop_logFrequency(value);
            }
        }

        // private helper for magic property
        private void setprop_logFrequency(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _LogFrequency_INVALID) {
                return;
            }
            if (newval == _logFrequency) {
                return;
            }
            _func.set_logFrequency(newval);
            _logFrequency = newval;
        }

        /**
         * <summary>
         *   Returns the timed value notification frequency, or "OFF" if timed
         *   value notifications are disabled for this function.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the timed value notification frequency, or "OFF" if timed
         *   value notifications are disabled for this function
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSensor.REPORTFREQUENCY_INVALID</c>.
         * </para>
         */
        public string get_reportFrequency()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            return _func.get_reportFrequency();
        }

        /**
         * <summary>
         *   Changes the timed value notification frequency for this function.
         * <para>
         *   The frequency can be specified as samples per second,
         *   as sample per minute (for instance "15/m") or in samples per
         *   hour (e.g. "4/h"). To disable timed value notifications for this
         *   function, use the value "OFF". Note that setting the  timed value
         *   notification frequency to a greater value than the sensor native
         *   sampling frequency is unless, and even counterproductive: those two
         *   frequencies are not related.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the timed value notification frequency for this function
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
        public int set_reportFrequency(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            if (newval == _ReportFrequency_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_reportFrequency(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Timed value notification frequency, or "OFF" if timed</value>
        public string ReportFrequency
        {
            get
            {
                if (_func == null) {
                    return _ReportFrequency_INVALID;
                }
                if (_online) {
                    return _reportFrequency;
                }
                return _ReportFrequency_INVALID;
            }
            set
            {
                setprop_reportFrequency(value);
            }
        }

        // private helper for magic property
        private void setprop_reportFrequency(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _ReportFrequency_INVALID) {
                return;
            }
            if (newval == _reportFrequency) {
                return;
            }
            _func.set_reportFrequency(newval);
            _reportFrequency = newval;
        }

        /**
         * <summary>
         *   Returns the measuring mode used for the advertised value pushed to the parent hub.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YSensor.ADVMODE_IMMEDIATE</c>, <c>YSensor.ADVMODE_PERIOD_AVG</c>,
         *   <c>YSensor.ADVMODE_PERIOD_MIN</c> and <c>YSensor.ADVMODE_PERIOD_MAX</c> corresponding to the
         *   measuring mode used for the advertised value pushed to the parent hub
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSensor.ADVMODE_INVALID</c>.
         * </para>
         */
        public int get_advMode()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_advMode()+1;
        }

        /**
         * <summary>
         *   Changes the measuring mode used for the advertised value pushed to the parent hub.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YSensor.ADVMODE_IMMEDIATE</c>, <c>YSensor.ADVMODE_PERIOD_AVG</c>,
         *   <c>YSensor.ADVMODE_PERIOD_MIN</c> and <c>YSensor.ADVMODE_PERIOD_MAX</c> corresponding to the
         *   measuring mode used for the advertised value pushed to the parent hub
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
        public int set_advMode(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            if (newval == _AdvMode_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_advMode(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>Measuring mode used for the advertised value pushed to the parent hub.</value>
        public int AdvMode
        {
            get
            {
                if (_func == null) {
                    return _AdvMode_INVALID;
                }
                if (_online) {
                    return _advMode;
                }
                return _AdvMode_INVALID;
            }
            set
            {
                setprop_advMode(value);
            }
        }

        // private helper for magic property
        private void setprop_advMode(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _AdvMode_INVALID) {
                return;
            }
            if (newval == _advMode) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_advMode(newval-1);
            _advMode = newval;
        }

        /**
         * <summary>
         *   Changes the resolution of the measured physical values.
         * <para>
         *   The resolution corresponds to the numerical precision
         *   when displaying value. It does not change the precision of the measure itself.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the resolution of the measured physical values
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
        public int set_resolution(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            if (newval == _Resolution_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_resolution(newval);
        }

        /**
         * <summary>
         *   Returns the resolution of the measured values.
         * <para>
         *   The resolution corresponds to the numerical precision
         *   of the measures, which is not always the same as the actual precision of the sensor.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the resolution of the measured values
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSensor.RESOLUTION_INVALID</c>.
         * </para>
         */
        public double get_resolution()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            res = _func.get_resolution();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        // property with cached value for instant access (configuration)
        /// <value>Resolution of the measured values. The resolution corresponds to the numerical precision</value>
        public double Resolution
        {
            get
            {
                if (_func == null) {
                    return _Resolution_INVALID;
                }
                if (_online) {
                    return _resolution;
                }
                return _Resolution_INVALID;
            }
            set
            {
                setprop_resolution(value);
            }
        }

        // private helper for magic property
        private void setprop_resolution(double newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Resolution_INVALID) {
                return;
            }
            if (newval == _resolution) {
                return;
            }
            _func.set_resolution(newval);
            _resolution = newval;
        }

        /**
         * <summary>
         *   Returns the sensor state code, which is zero when there is an up-to-date measure
         *   available or a positive code if the sensor is not able to provide a measure right now.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the sensor state code, which is zero when there is an up-to-date measure
         *   available or a positive code if the sensor is not able to provide a measure right now
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSensor.SENSORSTATE_INVALID</c>.
         * </para>
         */
        public int get_sensorState()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            return _func.get_sensorState();
        }

        /**
         * <summary>
         *   Checks if the sensor is currently able to provide an up-to-date measure.
         * <para>
         *   Returns false if the device is unreachable, or if the sensor does not have
         *   a current measure to transmit. No exception is raised if there is an error
         *   while trying to contact the device hosting $THEFUNCTION$.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>true</c> if the sensor can provide an up-to-date measure, and <c>false</c> otherwise
         * </returns>
         */
        public virtual bool isSensorReady()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            return _func.isSensorReady();
        }

        /**
         * <summary>
         *   Returns the <c>YDatalogger</c> object of the device hosting the sensor.
         * <para>
         *   This method returns an object
         *   that can control global parameters of the data logger. The returned object
         *   should not be freed.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an <c>YDatalogger</c> object, or null on error.
         * </returns>
         */
        public virtual YDataLoggerProxy get_dataLogger()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            return YDataLoggerProxy.FindDataLogger(_func.get_dataLogger().get_serialNumber());
        }

        /**
         * <summary>
         *   Starts the data logger on the device.
         * <para>
         *   Note that the data logger
         *   will only save the measures on this sensor if the logFrequency
         *   is not set to "OFF".
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         */
        public virtual int startDataLogger()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            return _func.startDataLogger();
        }

        /**
         * <summary>
         *   Stops the datalogger on the device.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         */
        public virtual int stopDataLogger()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            return _func.stopDataLogger();
        }

        /**
         * <summary>
         *   Retrieves a <c>YDataSet</c> object holding historical data for this
         *   sensor, for a specified time interval.
         * <para>
         *   The measures will be
         *   retrieved from the data logger, which must have been turned
         *   on at the desired time. See the documentation of the <c>YDataSet</c>
         *   class for information on how to get an overview of the
         *   recorded data, and how to load progressively a large set
         *   of measures from the data logger.
         * </para>
         * <para>
         *   This function only works if the device uses a recent firmware,
         *   as <c>YDataSet</c> objects are not supported by firmwares older than
         *   version 13000.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="startTime">
         *   the start of the desired measure time interval,
         *   as a Unix timestamp, i.e. the number of seconds since
         *   January 1, 1970 UTC. The special value 0 can be used
         *   to include any measure, without initial limit.
         * </param>
         * <param name="endTime">
         *   the end of the desired measure time interval,
         *   as a Unix timestamp, i.e. the number of seconds since
         *   January 1, 1970 UTC. The special value 0 can be used
         *   to include any measure, without ending limit.
         * </param>
         * <returns>
         *   an instance of <c>YDataSet</c>, providing access to historical
         *   data. Past measures can be loaded progressively
         *   using methods from the <c>YDataSet</c> object.
         * </returns>
         */
        public virtual YDataSetProxy get_recordedData(double startTime, double endTime)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            return new YDataSetProxy(_func.get_recordedData(startTime, endTime));
        }

        /**
         * <summary>
         *   Configures error correction data points, in particular to compensate for
         *   a possible perturbation of the measure caused by an enclosure.
         * <para>
         *   It is possible
         *   to configure up to five correction points. Correction points must be provided
         *   in ascending order, and be in the range of the sensor. The device will automatically
         *   perform a linear interpolation of the error correction between specified
         *   points. Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         *   For more information on advanced capabilities to refine the calibration of
         *   sensors, please contact support@yoctopuce.com.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="rawValues">
         *   array of floating point numbers, corresponding to the raw
         *   values returned by the sensor for the correction points.
         * </param>
         * <param name="refValues">
         *   array of floating point numbers, corresponding to the corrected
         *   values for the correction points.
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int calibrateFromPoints(double[] rawValues, double[] refValues)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sensor connected");
            }
            return _func.calibrateFromPoints(new List<double>(rawValues), new List<double>(refValues));
        }
    }
    //--- (end of generated code: YSensor implementation)
}

