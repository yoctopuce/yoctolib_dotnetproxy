/*********************************************************************
 *
 *  $Id: yocto_lightsensor_proxy.cs 40656 2020-05-25 14:13:34Z mvuilleu $
 *
 *  Implements YLightSensorProxy, the Proxy API for LightSensor
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
    //--- (YLightSensor class start)
    static public partial class YoctoProxyManager
    {
        public static YLightSensorProxy FindLightSensor(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YLightSensor func = null;
            YLightSensorProxy res = (YLightSensorProxy)YFunctionProxy.FindSimilarUnknownFunction("YLightSensorProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YLightSensorProxy)YFunctionProxy.FindSimilarKnownFunction("YLightSensorProxy");
                if (res != null) return res;
                func = YLightSensor.FirstLightSensor();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YLightSensorProxy)func.get_userData();
                    }
                }
            } else {
                func = YLightSensor.FindLightSensor(name);
                if (func.get_userData() != null) {
                    return (YLightSensorProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YLightSensorProxy(func, name);
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
 *   The <c>YLightSensor</c> class allows you to read and configure Yoctopuce light sensors.
 * <para>
 *   It inherits from <c>YSensor</c> class the core functions to read measurements,
 *   to register callback functions, and to access the autonomous datalogger.
 *   This class adds the ability to easily perform a one-point linear calibration
 *   to compensate the effect of a glass or filter placed in front of the sensor.
 *   For some light sensors with several working modes, this class can select the
 *   desired working mode.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YLightSensorProxy : YSensorProxy
    {
        /**
         * <summary>
         *   Retrieves a light sensor for a given identifier.
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
         *   This function does not require that the light sensor is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YLightSensor.isOnline()</c> to test if the light sensor is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a light sensor by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the light sensor, for instance
         *   <c>LIGHTMK3.lightSensor</c>.
         * </param>
         * <returns>
         *   a <c>YLightSensor</c> object allowing you to drive the light sensor.
         * </returns>
         */
        public static YLightSensorProxy FindLightSensor(string func)
        {
            return YoctoProxyManager.FindLightSensor(func);
        }
        //--- (end of YLightSensor class start)
        //--- (YLightSensor definitions)
        public const int _MeasureType_INVALID = 0;
        public const int _MeasureType_HUMAN_EYE = 1;
        public const int _MeasureType_WIDE_SPECTRUM = 2;
        public const int _MeasureType_INFRARED = 3;
        public const int _MeasureType_HIGH_RATE = 4;
        public const int _MeasureType_HIGH_ENERGY = 5;

        // reference to real YoctoAPI object
        protected new YLightSensor _func;
        // property cache
        protected int _measureType = _MeasureType_INVALID;
        //--- (end of YLightSensor definitions)

        //--- (YLightSensor implementation)
        internal YLightSensorProxy(YLightSensor hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("LightSensor " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YLightSensor) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YLightSensor hwd = YLightSensor.FindLightSensor(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YLightSensor hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering LightSensor callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type LightSensor available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YLightSensor.FindLightSensor</c> to obtain an object that can control the
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
            YLightSensor it = YLightSensor.FirstLightSensor();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextLightSensor();
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
            _measureType = _func.get_measureType()+1;
        }

        /**
         * <summary>
         *   Changes the sensor-specific calibration parameter so that the current value
         *   matches a desired target (linear scaling).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="calibratedVal">
         *   the desired target value.
         * </param>
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int calibrate(double calibratedVal)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No LightSensor connected");
            }
            return _func.calibrate(calibratedVal);
        }

        /**
         * <summary>
         *   Returns the type of light measure.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>lightsensor._Measuretype_HUMAN_EYE</c>, <c>lightsensor._Measuretype_WIDE_SPECTRUM</c>,
         *   <c>lightsensor._Measuretype_INFRARED</c>, <c>lightsensor._Measuretype_HIGH_RATE</c> and
         *   <c>lightsensor._Measuretype_HIGH_ENERGY</c> corresponding to the type of light measure
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>lightsensor._Measuretype_INVALID</c>.
         * </para>
         */
        public int get_measureType()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No LightSensor connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_measureType()+1;
        }

        /**
         * <summary>
         *   Changes the light sensor type used in the device.
         * <para>
         *   The measure can either
         *   approximate the response of the human eye, focus on a specific light
         *   spectrum, depending on the capabilities of the light-sensitive cell.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>lightsensor._Measuretype_HUMAN_EYE</c>, <c>lightsensor._Measuretype_WIDE_SPECTRUM</c>,
         *   <c>lightsensor._Measuretype_INFRARED</c>, <c>lightsensor._Measuretype_HIGH_RATE</c> and
         *   <c>lightsensor._Measuretype_HIGH_ENERGY</c> corresponding to the light sensor type used in the device
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
        public int set_measureType(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No LightSensor connected");
            }
            if (newval == _MeasureType_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_measureType(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>Type of light measure.</value>
        public int MeasureType
        {
            get
            {
                if (_func == null) {
                    return _MeasureType_INVALID;
                }
                if (_online) {
                    return _measureType;
                }
                return _MeasureType_INVALID;
            }
            set
            {
                setprop_measureType(value);
            }
        }

        // private helper for magic property
        private void setprop_measureType(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _MeasureType_INVALID) {
                return;
            }
            if (newval == _measureType) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_measureType(newval-1);
            _measureType = newval;
        }
    }
    //--- (end of YLightSensor implementation)
}

