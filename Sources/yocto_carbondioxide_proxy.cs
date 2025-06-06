/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YCarbonDioxideProxy, the Proxy API for CarbonDioxide
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
    //--- (YCarbonDioxide class start)
    static public partial class YoctoProxyManager
    {
        public static YCarbonDioxideProxy FindCarbonDioxide(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YCarbonDioxide func = null;
            YCarbonDioxideProxy res = (YCarbonDioxideProxy)YFunctionProxy.FindSimilarUnknownFunction("YCarbonDioxideProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YCarbonDioxideProxy)YFunctionProxy.FindSimilarKnownFunction("YCarbonDioxideProxy");
                if (res != null) return res;
                func = YCarbonDioxide.FirstCarbonDioxide();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YCarbonDioxideProxy)func.get_userData();
                    }
                }
            } else {
                func = YCarbonDioxide.FindCarbonDioxide(name);
                if (func.get_userData() != null) {
                    return (YCarbonDioxideProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YCarbonDioxideProxy(func, name);
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
 *   The <c>YCarbonDioxide</c> class allows you to read and configure Yoctopuce CO2 sensors.
 * <para>
 *   It inherits from <c>YSensor</c> class the core functions to read measures,
 *   to register callback functions, and to access the autonomous datalogger.
 *   This class adds the ability to perform manual calibration if required.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YCarbonDioxideProxy : YSensorProxy
    {
        /**
         * <summary>
         *   Retrieves a CO2 sensor for a given identifier.
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
         *   This function does not require that the CO2 sensor is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YCarbonDioxide.isOnline()</c> to test if the CO2 sensor is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a CO2 sensor by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the CO2 sensor, for instance
         *   <c>YCO2MK02.carbonDioxide</c>.
         * </param>
         * <returns>
         *   a <c>YCarbonDioxide</c> object allowing you to drive the CO2 sensor.
         * </returns>
         */
        public static YCarbonDioxideProxy FindCarbonDioxide(string func)
        {
            return YoctoProxyManager.FindCarbonDioxide(func);
        }
        //--- (end of YCarbonDioxide class start)
        //--- (YCarbonDioxide definitions)
        public const int _AbcPeriod_INVALID = -1;
        public const string _Command_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YCarbonDioxide _func;
        // property cache
        protected int _abcPeriod = _AbcPeriod_INVALID;
        //--- (end of YCarbonDioxide definitions)

        //--- (YCarbonDioxide implementation)
        internal YCarbonDioxideProxy(YCarbonDioxide hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("CarbonDioxide " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YCarbonDioxide) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YCarbonDioxide hwd = YCarbonDioxide.FindCarbonDioxide(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YCarbonDioxide hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering CarbonDioxide callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type CarbonDioxide available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YCarbonDioxide.FindCarbonDioxide</c> to obtain an object that can control the
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
            YCarbonDioxide it = YCarbonDioxide.FirstCarbonDioxide();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextCarbonDioxide();
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
            _abcPeriod = _func.get_abcPeriod();
        }

        /**
         * <summary>
         *   Returns the Automatic Baseline Calibration period, in hours.
         * <para>
         *   A negative value
         *   means that automatic baseline calibration is disabled.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the Automatic Baseline Calibration period, in hours
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YCarbonDioxide.ABCPERIOD_INVALID</c>.
         * </para>
         */
        public int get_abcPeriod()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No CarbonDioxide connected");
            }
            res = _func.get_abcPeriod();
            if (res == YAPI.INVALID_INT) {
                res = _AbcPeriod_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes Automatic Baseline Calibration period, in hours.
         * <para>
         *   If you need
         *   to disable automatic baseline calibration (for instance when using the
         *   sensor in an environment that is constantly above 400 ppm CO2), set the
         *   period to -1. For the Yocto-CO2-V2, the only possible values are 24 and -1.
         *   Remember to call the <c>saveToFlash()</c> method of the
         *   module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to Automatic Baseline Calibration period, in hours
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
        public int set_abcPeriod(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No CarbonDioxide connected");
            }
            if (newval == _AbcPeriod_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_abcPeriod(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Automatic Baseline Calibration period, in hours. A negative value</value>
        public int AbcPeriod
        {
            get
            {
                if (_func == null) {
                    return _AbcPeriod_INVALID;
                }
                if (_online) {
                    return _abcPeriod;
                }
                return _AbcPeriod_INVALID;
            }
            set
            {
                setprop_abcPeriod(value);
            }
        }

        // private helper for magic property
        private void setprop_abcPeriod(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _AbcPeriod_INVALID) {
                return;
            }
            if (newval == _abcPeriod) {
                return;
            }
            _func.set_abcPeriod(newval);
            _abcPeriod = newval;
        }

        /**
         * <summary>
         *   Triggers a forced calibration of the sensor at a given CO2 level, specified
         *   between 400ppm and 2000ppm.
         * <para>
         *   Before invoking this command, the sensor must
         *   have been maintained within the specified CO2 density during at least two
         *   minutes.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="refVal">
         *   reference CO2 density for the calibration
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int triggerForcedCalibration(double refVal)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No CarbonDioxide connected");
            }
            return _func.triggerForcedCalibration(refVal);
        }

        /**
         * <summary>
         *   Triggers a baseline calibration at standard CO2 ambiant level (400ppm).
         * <para>
         *   It is normally not necessary to manually calibrate the sensor, because
         *   the built-in automatic baseline calibration procedure will automatically
         *   fix any long-term drift based on the lowest level of CO2 observed over the
         *   automatic calibration period. However, if automatic baseline calibration
         *   is disabled, you may want to manually trigger a calibration from time to
         *   time. Before starting a baseline calibration, make sure to put the sensor
         *   in a standard environment (e.g. outside in fresh air) at around 400 ppm
         *   for at least two minutes.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int triggerBaselineCalibration()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No CarbonDioxide connected");
            }
            return _func.triggerBaselineCalibration();
        }

        /**
         * <summary>
         *   Triggers a zero calibration of the sensor on carbon dioxide-free air -
         *   for use with first generation Yocto-CO2 only.
         * <para>
         *   It is normally not necessary to manually calibrate the sensor, because
         *   the built-in automatic baseline calibration procedure will automatically
         *   fix any long-term drift based on the lowest level of CO2 observed over the
         *   automatic calibration period. However, if you disable automatic baseline
         *   calibration, you may want to manually trigger a calibration from time to
         *   time. Before starting a zero calibration, you should circulate carbon
         *   dioxide-free air within the sensor for a minute or two, using a small pipe
         *   connected to the sensor. Please contact support@yoctopuce.com for more details
         *   on the zero calibration procedure.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int triggerZeroCalibration()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No CarbonDioxide connected");
            }
            return _func.triggerZeroCalibration();
        }
    }
    //--- (end of YCarbonDioxide implementation)
}

