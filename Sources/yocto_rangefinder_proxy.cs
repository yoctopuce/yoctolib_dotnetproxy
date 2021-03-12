/*********************************************************************
 *
 *  $Id: yocto_rangefinder_proxy.cs 43619 2021-01-29 09:14:45Z mvuilleu $
 *
 *  Implements YRangeFinderProxy, the Proxy API for RangeFinder
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
    //--- (YRangeFinder class start)
    static public partial class YoctoProxyManager
    {
        public static YRangeFinderProxy FindRangeFinder(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YRangeFinder func = null;
            YRangeFinderProxy res = (YRangeFinderProxy)YFunctionProxy.FindSimilarUnknownFunction("YRangeFinderProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YRangeFinderProxy)YFunctionProxy.FindSimilarKnownFunction("YRangeFinderProxy");
                if (res != null) return res;
                func = YRangeFinder.FirstRangeFinder();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YRangeFinderProxy)func.get_userData();
                    }
                }
            } else {
                func = YRangeFinder.FindRangeFinder(name);
                if (func.get_userData() != null) {
                    return (YRangeFinderProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YRangeFinderProxy(func, name);
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
 *   The <c>YRangeFinder</c> class allows you to read and configure Yoctopuce range finders.
 * <para>
 *   It inherits from <c>YSensor</c> class the core functions to read measurements,
 *   to register callback functions, and to access the autonomous datalogger.
 *   This class adds the ability to easily perform a one-point linear calibration
 *   to compensate the effect of a glass or filter placed in front of the sensor.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YRangeFinderProxy : YSensorProxy
    {
        /**
         * <summary>
         *   Retrieves a range finder for a given identifier.
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
         *   This function does not require that the range finder is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YRangeFinder.isOnline()</c> to test if the range finder is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a range finder by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the range finder, for instance
         *   <c>YRNGFND1.rangeFinder1</c>.
         * </param>
         * <returns>
         *   a <c>YRangeFinder</c> object allowing you to drive the range finder.
         * </returns>
         */
        public static YRangeFinderProxy FindRangeFinder(string func)
        {
            return YoctoProxyManager.FindRangeFinder(func);
        }
        //--- (end of YRangeFinder class start)
        //--- (YRangeFinder definitions)
        public const int _IsPresent_INVALID = 0;
        public const int _IsPresent_FALSE = 1;
        public const int _IsPresent_TRUE = 2;
        public const int _RangeFinderMode_INVALID = 0;
        public const int _RangeFinderMode_DEFAULT = 1;
        public const int _RangeFinderMode_LONG_RANGE = 2;
        public const int _RangeFinderMode_HIGH_ACCURACY = 3;
        public const int _RangeFinderMode_HIGH_SPEED = 4;
        public const long _TimeFrame_INVALID = YAPI.INVALID_LONG;
        public const int _Quality_INVALID = -1;
        public const string _HardwareCalibration_INVALID = YAPI.INVALID_STRING;
        public const double _CurrentTemperature_INVALID = Double.NaN;
        public const string _Command_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YRangeFinder _func;
        // property cache
        protected int _isPresent = _IsPresent_INVALID;
        protected int _rangeFinderMode = _RangeFinderMode_INVALID;
        protected long _timeFrame = _TimeFrame_INVALID;
        //--- (end of YRangeFinder definitions)

        //--- (YRangeFinder implementation)
        internal YRangeFinderProxy(YRangeFinder hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("RangeFinder " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YRangeFinder) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YRangeFinder hwd = YRangeFinder.FindRangeFinder(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YRangeFinder hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering RangeFinder callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type RangeFinder available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YRangeFinder.FindRangeFinder</c> to obtain an object that can control the
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
            YRangeFinder it = YRangeFinder.FirstRangeFinder();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextRangeFinder();
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
            _rangeFinderMode = _func.get_rangeFinderMode()+1;
            _timeFrame = _func.get_timeFrame();
        }

        // property with cached value for instant access (derived from advertisedValue)
        /// <value>True if the sensor detected an object</value>
        public int IsPresent
        {
            get
            {
                if (_func == null) {
                    return _IsPresent_INVALID;
                }
                if (_online) {
                    return _isPresent;
                }
                return _IsPresent_INVALID;
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            if (_currentValue < 999999) _isPresent = _IsPresent_TRUE;
            if (_currentValue >= 999999) _isPresent = _IsPresent_FALSE;
        }

        /**
         * <summary>
         *   Changes the measuring unit for the measured range.
         * <para>
         *   That unit is a string.
         *   String value can be <c>"</c> or <c>mm</c>. Any other value is ignored.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         *   WARNING: if a specific calibration is defined for the rangeFinder function, a
         *   unit system change will probably break it.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the measuring unit for the measured range
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
        public int set_unit(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RangeFinder connected");
            }
            if (newval == _Unit_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_unit(newval);
        }

        /**
         * <summary>
         *   Returns the range finder running mode.
         * <para>
         *   The rangefinder running mode
         *   allows you to put priority on precision, speed or maximum range.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YRangeFinder.RANGEFINDERMODE_DEFAULT</c>, <c>YRangeFinder.RANGEFINDERMODE_LONG_RANGE</c>,
         *   <c>YRangeFinder.RANGEFINDERMODE_HIGH_ACCURACY</c> and <c>YRangeFinder.RANGEFINDERMODE_HIGH_SPEED</c>
         *   corresponding to the range finder running mode
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YRangeFinder.RANGEFINDERMODE_INVALID</c>.
         * </para>
         */
        public int get_rangeFinderMode()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RangeFinder connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_rangeFinderMode()+1;
        }

        /**
         * <summary>
         *   Changes the rangefinder running mode, allowing you to put priority on
         *   precision, speed or maximum range.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YRangeFinder.RANGEFINDERMODE_DEFAULT</c>, <c>YRangeFinder.RANGEFINDERMODE_LONG_RANGE</c>,
         *   <c>YRangeFinder.RANGEFINDERMODE_HIGH_ACCURACY</c> and <c>YRangeFinder.RANGEFINDERMODE_HIGH_SPEED</c>
         *   corresponding to the rangefinder running mode, allowing you to put priority on
         *   precision, speed or maximum range
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
        public int set_rangeFinderMode(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RangeFinder connected");
            }
            if (newval == _RangeFinderMode_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_rangeFinderMode(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>Range finder running mode. The rangefinder running mode</value>
        public int RangeFinderMode
        {
            get
            {
                if (_func == null) {
                    return _RangeFinderMode_INVALID;
                }
                if (_online) {
                    return _rangeFinderMode;
                }
                return _RangeFinderMode_INVALID;
            }
            set
            {
                setprop_rangeFinderMode(value);
            }
        }

        // private helper for magic property
        private void setprop_rangeFinderMode(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _RangeFinderMode_INVALID) {
                return;
            }
            if (newval == _rangeFinderMode) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_rangeFinderMode(newval-1);
            _rangeFinderMode = newval;
        }

        /**
         * <summary>
         *   Returns the time frame used to measure the distance and estimate the measure
         *   reliability.
         * <para>
         *   The time frame is expressed in milliseconds.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the time frame used to measure the distance and estimate the measure
         *   reliability
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YRangeFinder.TIMEFRAME_INVALID</c>.
         * </para>
         */
        public long get_timeFrame()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RangeFinder connected");
            }
            return _func.get_timeFrame();
        }

        /**
         * <summary>
         *   Changes the time frame used to measure the distance and estimate the measure
         *   reliability.
         * <para>
         *   The time frame is expressed in milliseconds. A larger timeframe
         *   improves stability and reliability, at the cost of higher latency, but prevents
         *   the detection of events shorter than the time frame.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the time frame used to measure the distance and estimate the measure
         *   reliability
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
        public int set_timeFrame(long newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RangeFinder connected");
            }
            if (newval == _TimeFrame_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_timeFrame(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Time frame used to measure the distance and estimate the measure</value>
        public long TimeFrame
        {
            get
            {
                if (_func == null) {
                    return _TimeFrame_INVALID;
                }
                if (_online) {
                    return _timeFrame;
                }
                return _TimeFrame_INVALID;
            }
            set
            {
                setprop_timeFrame(value);
            }
        }

        // private helper for magic property
        private void setprop_timeFrame(long newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _TimeFrame_INVALID) {
                return;
            }
            if (newval == _timeFrame) {
                return;
            }
            _func.set_timeFrame(newval);
            _timeFrame = newval;
        }

        /**
         * <summary>
         *   Returns a measure quality estimate, based on measured dispersion.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to a measure quality estimate, based on measured dispersion
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YRangeFinder.QUALITY_INVALID</c>.
         * </para>
         */
        public int get_quality()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No RangeFinder connected");
            }
            res = _func.get_quality();
            if (res == YAPI.INVALID_INT) {
                res = _Quality_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the current sensor temperature, as a floating point number.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the current sensor temperature, as a floating point number
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YRangeFinder.CURRENTTEMPERATURE_INVALID</c>.
         * </para>
         */
        public double get_currentTemperature()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No RangeFinder connected");
            }
            res = _func.get_currentTemperature();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the temperature at the time when the latest calibration was performed.
         * <para>
         *   This function can be used to determine if a new calibration for ambient temperature
         *   is required.
         * </para>
         * </summary>
         * <returns>
         *   a temperature, as a floating point number.
         *   On failure, throws an exception or return YAPI.INVALID_DOUBLE.
         * </returns>
         */
        public virtual double get_hardwareCalibrationTemperature()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RangeFinder connected");
            }
            double res;
            res = _func.get_hardwareCalibrationTemperature();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Triggers a sensor calibration according to the current ambient temperature.
         * <para>
         *   That
         *   calibration process needs no physical interaction with the sensor. It is performed
         *   automatically at device startup, but it is recommended to start it again when the
         *   temperature delta since the latest calibration exceeds 8°C.
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int triggerTemperatureCalibration()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RangeFinder connected");
            }
            return _func.triggerTemperatureCalibration();
        }

        /**
         * <summary>
         *   Triggers the photon detector hardware calibration.
         * <para>
         *   This function is part of the calibration procedure to compensate for the the effect
         *   of a cover glass. Make sure to read the chapter about hardware calibration for details
         *   on the calibration procedure for proper results.
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int triggerSpadCalibration()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RangeFinder connected");
            }
            return _func.triggerSpadCalibration();
        }

        /**
         * <summary>
         *   Triggers the hardware offset calibration of the distance sensor.
         * <para>
         *   This function is part of the calibration procedure to compensate for the the effect
         *   of a cover glass. Make sure to read the chapter about hardware calibration for details
         *   on the calibration procedure for proper results.
         * </para>
         * </summary>
         * <param name="targetDist">
         *   true distance of the calibration target, in mm or inches, depending
         *   on the unit selected in the device
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int triggerOffsetCalibration(double targetDist)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RangeFinder connected");
            }
            return _func.triggerOffsetCalibration(targetDist);
        }

        /**
         * <summary>
         *   Triggers the hardware cross-talk calibration of the distance sensor.
         * <para>
         *   This function is part of the calibration procedure to compensate for the the effect
         *   of a cover glass. Make sure to read the chapter about hardware calibration for details
         *   on the calibration procedure for proper results.
         * </para>
         * </summary>
         * <param name="targetDist">
         *   true distance of the calibration target, in mm or inches, depending
         *   on the unit selected in the device
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int triggerXTalkCalibration(double targetDist)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RangeFinder connected");
            }
            return _func.triggerXTalkCalibration(targetDist);
        }

        /**
         * <summary>
         *   Cancels the effect of previous hardware calibration procedures to compensate
         *   for cover glass, and restores factory settings.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int cancelCoverGlassCalibrations()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RangeFinder connected");
            }
            return _func.cancelCoverGlassCalibrations();
        }
    }
    //--- (end of YRangeFinder implementation)
}

