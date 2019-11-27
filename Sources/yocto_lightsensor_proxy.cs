/*********************************************************************
 *
 *  $Id: yocto_lightsensor_proxy.cs 38514 2019-11-26 16:54:39Z seb $
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
 *   The YLightSensor class allows you to read and configure Yoctopuce light
 *   sensors, for instance using a Yocto-Light-V3, a Yocto-Proximity or a Yocto-RangeFinder.
 * <para>
 *   It inherits from YSensor class the core functions to read measurements,
 *   to register callback functions, to access the autonomous datalogger.
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

        public override string[] GetSimilarFunctions()
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
            // our enums start at 0 instead of the 'usual' -1 for invalid
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
            if (_func == null)
            {
                string msg = "No LightSensor connected";
                throw new YoctoApiProxyException(msg);
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
         *   a value among <c>YLightSensor.MEASURETYPE_HUMAN_EYE</c>, <c>YLightSensor.MEASURETYPE_WIDE_SPECTRUM</c>,
         *   <c>YLightSensor.MEASURETYPE_INFRARED</c>, <c>YLightSensor.MEASURETYPE_HIGH_RATE</c> and
         *   <c>YLightSensor.MEASURETYPE_HIGH_ENERGY</c> corresponding to the type of light measure
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YLightSensor.MEASURETYPE_INVALID</c>.
         * </para>
         */
        public int get_measureType()
        {
            if (_func == null)
            {
                string msg = "No LightSensor connected";
                throw new YoctoApiProxyException(msg);
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
         *   a value among <c>YLightSensor.MEASURETYPE_HUMAN_EYE</c>, <c>YLightSensor.MEASURETYPE_WIDE_SPECTRUM</c>,
         *   <c>YLightSensor.MEASURETYPE_INFRARED</c>, <c>YLightSensor.MEASURETYPE_HIGH_RATE</c> and
         *   <c>YLightSensor.MEASURETYPE_HIGH_ENERGY</c> corresponding to the light sensor type used in the device
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
            if (_func == null)
            {
                string msg = "No LightSensor connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _MeasureType_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_measureType(newval-1);
        }


        // property with cached value for instant access (configuration)
        public int MeasureType
        {
            get
            {
                if (_func == null) return _MeasureType_INVALID;
                return (_online ? _measureType : _MeasureType_INVALID);
            }
            set
            {
                setprop_measureType(value);
            }
        }

        // private helper for magic property
        private void setprop_measureType(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _MeasureType_INVALID) return;
            if (newval == _measureType) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_measureType(newval-1);
            _measureType = newval;
        }
    }
    //--- (end of YLightSensor implementation)
}

