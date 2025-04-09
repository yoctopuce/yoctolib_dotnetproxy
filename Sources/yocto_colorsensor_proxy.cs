/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YColorSensorProxy, the Proxy API for ColorSensor
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
    //--- (YColorSensor class start)
    static public partial class YoctoProxyManager
    {
        public static YColorSensorProxy FindColorSensor(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YColorSensor func = null;
            YColorSensorProxy res = (YColorSensorProxy)YFunctionProxy.FindSimilarUnknownFunction("YColorSensorProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YColorSensorProxy)YFunctionProxy.FindSimilarKnownFunction("YColorSensorProxy");
                if (res != null) return res;
                func = YColorSensor.FirstColorSensor();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YColorSensorProxy)func.get_userData();
                    }
                }
            } else {
                func = YColorSensor.FindColorSensor(name);
                if (func.get_userData() != null) {
                    return (YColorSensorProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YColorSensorProxy(func, name);
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
 *   The <c>YColorSensor</c> class allows you to read and configure Yoctopuce color sensors.
 * <para>
 *   It inherits from <c>YSensor</c> class the core functions to read measurements,
 *   to register callback functions, and to access the autonomous datalogger.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YColorSensorProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a color sensor for a given identifier.
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
         *   This function does not require that the color sensor is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YColorSensor.isOnline()</c> to test if the color sensor is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a color sensor by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the color sensor, for instance
         *   <c>MyDevice.colorSensor</c>.
         * </param>
         * <returns>
         *   a <c>YColorSensor</c> object allowing you to drive the color sensor.
         * </returns>
         */
        public static YColorSensorProxy FindColorSensor(string func)
        {
            return YoctoProxyManager.FindColorSensor(func);
        }
        //--- (end of YColorSensor class start)
        //--- (YColorSensor definitions)
        public const int _EstimationModel_INVALID = 0;
        public const int _EstimationModel_REFLECTION = 1;
        public const int _EstimationModel_EMISSION = 2;
        public const int _WorkingMode_INVALID = 0;
        public const int _WorkingMode_AUTO = 1;
        public const int _WorkingMode_EXPERT = 2;
        public const int _Saturation_INVALID = -1;
        public const int _LedCurrent_INVALID = -1;
        public const int _LedCalibration_INVALID = -1;
        public const int _IntegrationTime_INVALID = -1;
        public const int _Gain_INVALID = -1;
        public const int _EstimatedRGB_INVALID = -1;
        public const int _EstimatedHSL_INVALID = -1;
        public const string _EstimatedXYZ_INVALID = YAPI.INVALID_STRING;
        public const string _EstimatedOkLab_INVALID = YAPI.INVALID_STRING;
        public const string _NearRAL1_INVALID = YAPI.INVALID_STRING;
        public const string _NearRAL2_INVALID = YAPI.INVALID_STRING;
        public const string _NearRAL3_INVALID = YAPI.INVALID_STRING;
        public const string _NearHTMLColor_INVALID = YAPI.INVALID_STRING;
        public const string _NearSimpleColor_INVALID = YAPI.INVALID_STRING;
        public const int _NearSimpleColorIndex_INVALID = 0;
        public const int _NearSimpleColorIndex_BROWN = 1;
        public const int _NearSimpleColorIndex_RED = 2;
        public const int _NearSimpleColorIndex_ORANGE = 3;
        public const int _NearSimpleColorIndex_YELLOW = 4;
        public const int _NearSimpleColorIndex_WHITE = 5;
        public const int _NearSimpleColorIndex_GRAY = 6;
        public const int _NearSimpleColorIndex_BLACK = 7;
        public const int _NearSimpleColorIndex_GREEN = 8;
        public const int _NearSimpleColorIndex_BLUE = 9;
        public const int _NearSimpleColorIndex_PURPLE = 10;
        public const int _NearSimpleColorIndex_PINK = 11;

        // reference to real YoctoAPI object
        protected new YColorSensor _func;
        // property cache
        protected int _estimatedRGB = _EstimatedRGB_INVALID;
        protected int _estimatedHSL = _EstimatedHSL_INVALID;
        protected int _nearSimpleColorIndex = _NearSimpleColorIndex_INVALID;
        protected int _saturation = _Saturation_INVALID;
        protected int _ledCurrent = _LedCurrent_INVALID;
        protected int _estimationModel = _EstimationModel_INVALID;
        protected int _workingMode = _WorkingMode_INVALID;
        protected int _ledCalibration = _LedCalibration_INVALID;
        protected int _integrationTime = _IntegrationTime_INVALID;
        protected int _gain = _Gain_INVALID;
        //--- (end of YColorSensor definitions)

        //--- (YColorSensor implementation)
        internal YColorSensorProxy(YColorSensor hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("ColorSensor " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YColorSensor) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YColorSensor hwd = YColorSensor.FindColorSensor(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YColorSensor hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering ColorSensor callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type ColorSensor available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YColorSensor.FindColorSensor</c> to obtain an object that can control the
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
            YColorSensor it = YColorSensor.FirstColorSensor();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextColorSensor();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
            _estimatedRGB = _func.get_estimatedRGB();
            _estimatedHSL = _func.get_estimatedHSL();
            _nearSimpleColorIndex = _func.get_nearSimpleColorIndex();
            _saturation = _func.get_saturation();
            _ledCurrent = _func.get_ledCurrent();
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            _estimationModel = _func.get_estimationModel()+1;
            _workingMode = _func.get_workingMode()+1;
            _ledCalibration = _func.get_ledCalibration();
            _integrationTime = _func.get_integrationTime();
            _gain = _func.get_gain();
        }

        // property with cached value for instant access (derived from advertised value)
        /// <value>Estimated color in the RGB (0xRRGGBB) format.</value>
        public int EstimatedRGB
        {
            get
            {
                if (_func == null) {
                    return _EstimatedRGB_INVALID;
                }
                if (_online) {
                    return _estimatedRGB;
                }
                return _EstimatedRGB_INVALID;
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            _ledCurrent = YAPI._hexStrToInt((value).Substring(10, 2));
        }

        // property with cached value for instant access (derived from advertised value)
        /// <value>Estimated color in the HSL (0xHHSSLL) format.</value>
        public int EstimatedHSL
        {
            get
            {
                if (_func == null) {
                    return _EstimatedHSL_INVALID;
                }
                if (_online) {
                    return _estimatedHSL;
                }
                return _EstimatedHSL_INVALID;
            }
        }

        // property with cached value for instant access (derived from advertised value)
        /// <value>Nearest simple color index.</value>
        public int NearSimpleColorIndex
        {
            get
            {
                if (_func == null) {
                    return _NearSimpleColorIndex_INVALID;
                }
                if (_online) {
                    return _nearSimpleColorIndex;
                }
                return _NearSimpleColorIndex_INVALID;
            }
        }

        // property with cached value for instant access (derived from advertised value)
        /// <value>Sensor saturation diagnostic bits.</value>
        public int Saturation
        {
            get
            {
                if (_func == null) {
                    return _Saturation_INVALID;
                }
                if (_online) {
                    return _saturation;
                }
                return _Saturation_INVALID;
            }
        }

        // property with cached value for instant access (derived from advertised value)
        /// <value>Current poweing the reflection LEDs.</value>
        public int LedCurrent
        {
            get
            {
                if (_func == null) {
                    return _LedCurrent_INVALID;
                }
                if (_online) {
                    return _ledCurrent;
                }
                return _LedCurrent_INVALID;
            }
            set
            {
                setprop_ledCurrent(value);
            }
        }

        // private helper for magic property
        private void setprop_ledCurrent(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _LedCurrent_INVALID) {
                return;
            }
            if (newval == _ledCurrent) {
                return;
            }
            _func.set_ledCurrent(newval);
            _ledCurrent = newval;
        }

        /**
         * <summary>
         *   Returns the model for color estimation.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YColorSensor.ESTIMATIONMODEL_REFLECTION</c> or <c>YColorSensor.ESTIMATIONMODEL_EMISSION</c>,
         *   according to the model for color estimation
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorSensor.ESTIMATIONMODEL_INVALID</c>.
         * </para>
         */
        public int get_estimationModel()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_estimationModel()+1;
        }

        /**
         * <summary>
         *   Changes the model for color estimation.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YColorSensor.ESTIMATIONMODEL_REFLECTION</c> or <c>YColorSensor.ESTIMATIONMODEL_EMISSION</c>,
         *   according to the model for color estimation
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
        public int set_estimationModel(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            if (newval == _EstimationModel_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_estimationModel(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>Model for color estimation.</value>
        public int EstimationModel
        {
            get
            {
                if (_func == null) {
                    return _EstimationModel_INVALID;
                }
                if (_online) {
                    return _estimationModel;
                }
                return _EstimationModel_INVALID;
            }
            set
            {
                setprop_estimationModel(value);
            }
        }

        // private helper for magic property
        private void setprop_estimationModel(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _EstimationModel_INVALID) {
                return;
            }
            if (newval == _estimationModel) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_estimationModel(newval-1);
            _estimationModel = newval;
        }

        /**
         * <summary>
         *   Returns the active working mode.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YColorSensor.WORKINGMODE_AUTO</c> or <c>YColorSensor.WORKINGMODE_EXPERT</c>, according to
         *   the active working mode
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorSensor.WORKINGMODE_INVALID</c>.
         * </para>
         */
        public int get_workingMode()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_workingMode()+1;
        }

        /**
         * <summary>
         *   Changes the operating mode.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YColorSensor.WORKINGMODE_AUTO</c> or <c>YColorSensor.WORKINGMODE_EXPERT</c>, according to
         *   the operating mode
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
        public int set_workingMode(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            if (newval == _WorkingMode_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_workingMode(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>Active working mode.</value>
        public int WorkingMode
        {
            get
            {
                if (_func == null) {
                    return _WorkingMode_INVALID;
                }
                if (_online) {
                    return _workingMode;
                }
                return _WorkingMode_INVALID;
            }
            set
            {
                setprop_workingMode(value);
            }
        }

        // private helper for magic property
        private void setprop_workingMode(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _WorkingMode_INVALID) {
                return;
            }
            if (newval == _workingMode) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_workingMode(newval-1);
            _workingMode = newval;
        }

        /**
         * <summary>
         *   Returns the current saturation of the sensor.
         * <para>
         *   This function updates the sensor's saturation value.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current saturation of the sensor
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorSensor.SATURATION_INVALID</c>.
         * </para>
         */
        public int get_saturation()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            res = _func.get_saturation();
            if (res == YAPI.INVALID_INT) {
                res = _Saturation_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the current value of the LED.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current value of the LED
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorSensor.LEDCURRENT_INVALID</c>.
         * </para>
         */
        public int get_ledCurrent()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            res = _func.get_ledCurrent();
            if (res == YAPI.INVALID_INT) {
                res = _LedCurrent_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the luminosity of the module leds.
         * <para>
         *   The parameter is a
         *   value between 0 and 254.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the luminosity of the module leds
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
        public int set_ledCurrent(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            if (newval == _LedCurrent_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_ledCurrent(newval);
        }

        /**
         * <summary>
         *   Returns the LED current at calibration.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the LED current at calibration
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorSensor.LEDCALIBRATION_INVALID</c>.
         * </para>
         */
        public int get_ledCalibration()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            res = _func.get_ledCalibration();
            if (res == YAPI.INVALID_INT) {
                res = _LedCalibration_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Sets the LED current for calibration.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer
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
        public int set_ledCalibration(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            if (newval == _LedCalibration_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_ledCalibration(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>LED current at calibration.</value>
        public int LedCalibration
        {
            get
            {
                if (_func == null) {
                    return _LedCalibration_INVALID;
                }
                if (_online) {
                    return _ledCalibration;
                }
                return _LedCalibration_INVALID;
            }
            set
            {
                setprop_ledCalibration(value);
            }
        }

        // private helper for magic property
        private void setprop_ledCalibration(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _LedCalibration_INVALID) {
                return;
            }
            if (newval == _ledCalibration) {
                return;
            }
            _func.set_ledCalibration(newval);
            _ledCalibration = newval;
        }

        /**
         * <summary>
         *   Returns the current integration time.
         * <para>
         *   This method retrieves the integration time value
         *   used for data processing and returns it as an integer or an object.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current integration time
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorSensor.INTEGRATIONTIME_INVALID</c>.
         * </para>
         */
        public int get_integrationTime()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            res = _func.get_integrationTime();
            if (res == YAPI.INVALID_INT) {
                res = _IntegrationTime_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the integration time for data processing.
         * <para>
         *   This method takes a parameter and assigns it
         *   as the new integration time. This affects the duration
         *   for which data is integrated before being processed.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the integration time for data processing
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
        public int set_integrationTime(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            if (newval == _IntegrationTime_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_integrationTime(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Current integration time.</value>
        public int IntegrationTime
        {
            get
            {
                if (_func == null) {
                    return _IntegrationTime_INVALID;
                }
                if (_online) {
                    return _integrationTime;
                }
                return _IntegrationTime_INVALID;
            }
            set
            {
                setprop_integrationTime(value);
            }
        }

        // private helper for magic property
        private void setprop_integrationTime(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _IntegrationTime_INVALID) {
                return;
            }
            if (newval == _integrationTime) {
                return;
            }
            _func.set_integrationTime(newval);
            _integrationTime = newval;
        }

        /**
         * <summary>
         *   Returns the current gain.
         * <para>
         *   This method updates the gain value.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current gain
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorSensor.GAIN_INVALID</c>.
         * </para>
         */
        public int get_gain()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            res = _func.get_gain();
            if (res == YAPI.INVALID_INT) {
                res = _Gain_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the gain for signal processing.
         * <para>
         *   This method takes a parameter and assigns it
         *   as the new gain. This affects the sensitivity and
         *   intensity of the processed signal.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the gain for signal processing
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
        public int set_gain(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            if (newval == _Gain_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_gain(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Current gain.</value>
        public int Gain
        {
            get
            {
                if (_func == null) {
                    return _Gain_INVALID;
                }
                if (_online) {
                    return _gain;
                }
                return _Gain_INVALID;
            }
            set
            {
                setprop_gain(value);
            }
        }

        // private helper for magic property
        private void setprop_gain(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Gain_INVALID) {
                return;
            }
            if (newval == _gain) {
                return;
            }
            _func.set_gain(newval);
            _gain = newval;
        }

        /**
         * <summary>
         *   Returns the estimated color in RGB format (0xRRGGBB).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the estimated color in RGB format (0xRRGGBB)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorSensor.ESTIMATEDRGB_INVALID</c>.
         * </para>
         */
        public int get_estimatedRGB()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            res = _func.get_estimatedRGB();
            if (res == YAPI.INVALID_INT) {
                res = _EstimatedRGB_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the estimated color in HSL (Hue, Saturation, Lightness) format.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the estimated color in HSL (Hue, Saturation, Lightness) format
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorSensor.ESTIMATEDHSL_INVALID</c>.
         * </para>
         */
        public int get_estimatedHSL()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            res = _func.get_estimatedHSL();
            if (res == YAPI.INVALID_INT) {
                res = _EstimatedHSL_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the estimated color in XYZ format.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the estimated color in XYZ format
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorSensor.ESTIMATEDXYZ_INVALID</c>.
         * </para>
         */
        public string get_estimatedXYZ()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            return _func.get_estimatedXYZ();
        }

        /**
         * <summary>
         *   Returns the estimated color in OkLab format.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the estimated color in OkLab format
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorSensor.ESTIMATEDOKLAB_INVALID</c>.
         * </para>
         */
        public string get_estimatedOkLab()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            return _func.get_estimatedOkLab();
        }

        /**
         * <summary>
         *   Returns the estimated color in RAL format.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the estimated color in RAL format
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorSensor.NEARRAL1_INVALID</c>.
         * </para>
         */
        public string get_nearRAL1()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            return _func.get_nearRAL1();
        }

        /**
         * <summary>
         *   Returns the estimated color in RAL format.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the estimated color in RAL format
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorSensor.NEARRAL2_INVALID</c>.
         * </para>
         */
        public string get_nearRAL2()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            return _func.get_nearRAL2();
        }

        /**
         * <summary>
         *   Returns the estimated color in RAL format.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the estimated color in RAL format
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorSensor.NEARRAL3_INVALID</c>.
         * </para>
         */
        public string get_nearRAL3()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            return _func.get_nearRAL3();
        }

        /**
         * <summary>
         *   Returns the estimated HTML color .
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the estimated HTML color
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorSensor.NEARHTMLCOLOR_INVALID</c>.
         * </para>
         */
        public string get_nearHTMLColor()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            return _func.get_nearHTMLColor();
        }

        /**
         * <summary>
         *   Returns the estimated color .
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the estimated color
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorSensor.NEARSIMPLECOLOR_INVALID</c>.
         * </para>
         */
        public string get_nearSimpleColor()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            return _func.get_nearSimpleColor();
        }

        /**
         * <summary>
         *   Returns the estimated color as an index.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YColorSensor.NEARSIMPLECOLORINDEX_BROWN</c>,
         *   <c>YColorSensor.NEARSIMPLECOLORINDEX_RED</c>, <c>YColorSensor.NEARSIMPLECOLORINDEX_ORANGE</c>,
         *   <c>YColorSensor.NEARSIMPLECOLORINDEX_YELLOW</c>, <c>YColorSensor.NEARSIMPLECOLORINDEX_WHITE</c>,
         *   <c>YColorSensor.NEARSIMPLECOLORINDEX_GRAY</c>, <c>YColorSensor.NEARSIMPLECOLORINDEX_BLACK</c>,
         *   <c>YColorSensor.NEARSIMPLECOLORINDEX_GREEN</c>, <c>YColorSensor.NEARSIMPLECOLORINDEX_BLUE</c>,
         *   <c>YColorSensor.NEARSIMPLECOLORINDEX_PURPLE</c> and <c>YColorSensor.NEARSIMPLECOLORINDEX_PINK</c>
         *   corresponding to the estimated color as an index
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorSensor.NEARSIMPLECOLORINDEX_INVALID</c>.
         * </para>
         */
        public int get_nearSimpleColorIndex()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_nearSimpleColorIndex()+1;
        }

        /**
         * <summary>
         *   Turns on the LEDs at the current used during calibration.
         * <para>
         *   On failure, throws an exception or returns colorsensor._Data_INVALID.
         * </para>
         * </summary>
         */
        public virtual int turnLedOn()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            return _func.turnLedOn();
        }

        /**
         * <summary>
         *   Turns off the LEDs.
         * <para>
         *   On failure, throws an exception or returns colorsensor._Data_INVALID.
         * </para>
         * </summary>
         */
        public virtual int turnLedOff()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorSensor connected");
            }
            return _func.turnLedOff();
        }
    }
    //--- (end of YColorSensor implementation)
}

