/*********************************************************************
 *
 *  $Id: yocto_accelerometer_proxy.cs 38514 2019-11-26 16:54:39Z seb $
 *
 *  Implements YAccelerometerProxy, the Proxy API for Accelerometer
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
    //--- (YAccelerometer class start)
    static public partial class YoctoProxyManager
    {
        public static YAccelerometerProxy FindAccelerometer(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YAccelerometer func = null;
            YAccelerometerProxy res = (YAccelerometerProxy)YFunctionProxy.FindSimilarUnknownFunction("YAccelerometerProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YAccelerometerProxy)YFunctionProxy.FindSimilarKnownFunction("YAccelerometerProxy");
                if (res != null) return res;
                func = YAccelerometer.FirstAccelerometer();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YAccelerometerProxy)func.get_userData();
                    }
                }
            } else {
                func = YAccelerometer.FindAccelerometer(name);
                if (func.get_userData() != null) {
                    return (YAccelerometerProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YAccelerometerProxy(func, name);
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
 *   The YAccelerometer class allows you to read and configure Yoctopuce acceleration
 *   sensors, for instance using a Yocto-3D-V2.
 * <para>
 *   It inherits from YSensor class the core functions to read measurements,
 *   to register callback functions, to access the autonomous datalogger.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YAccelerometerProxy : YSensorProxy
    {
        //--- (end of YAccelerometer class start)
        //--- (YAccelerometer definitions)
        public const int _Bandwidth_INVALID = -1;
        public const double _XValue_INVALID = Double.NaN;
        public const double _YValue_INVALID = Double.NaN;
        public const double _ZValue_INVALID = Double.NaN;
        public const int _GravityCancellation_INVALID = 0;
        public const int _GravityCancellation_OFF = 1;
        public const int _GravityCancellation_ON = 2;

        // reference to real YoctoAPI object
        protected new YAccelerometer _func;
        // property cache
        protected int _bandwidth = _Bandwidth_INVALID;
        //--- (end of YAccelerometer definitions)

        //--- (YAccelerometer implementation)
        internal YAccelerometerProxy(YAccelerometer hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Accelerometer " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YAccelerometer) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YAccelerometer hwd = YAccelerometer.FindAccelerometer(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YAccelerometer hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Accelerometer callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YAccelerometer it = YAccelerometer.FirstAccelerometer();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextAccelerometer();
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
            _bandwidth = _func.get_bandwidth();
        }

        /**
         * <summary>
         *   Returns the measure update frequency, measured in Hz (Yocto-3D-V2 only).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the measure update frequency, measured in Hz (Yocto-3D-V2 only)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YAccelerometer.BANDWIDTH_INVALID</c>.
         * </para>
         */
        public int get_bandwidth()
        {
            if (_func == null)
            {
                string msg = "No Accelerometer connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_bandwidth();
            if (res == YAPI.INVALID_INT) res = _Bandwidth_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the measure update frequency, measured in Hz (Yocto-3D-V2 only).
         * <para>
         *   When the
         *   frequency is lower, the device performs averaging.
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the measure update frequency, measured in Hz (Yocto-3D-V2 only)
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
        public int set_bandwidth(int newval)
        {
            if (_func == null)
            {
                string msg = "No Accelerometer connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Bandwidth_INVALID) return YAPI.SUCCESS;
            return _func.set_bandwidth(newval);
        }


        // property with cached value for instant access (configuration)
        public int Bandwidth
        {
            get
            {
                if (_func == null) return _Bandwidth_INVALID;
                return (_online ? _bandwidth : _Bandwidth_INVALID);
            }
            set
            {
                setprop_bandwidth(value);
            }
        }

        // private helper for magic property
        private void setprop_bandwidth(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Bandwidth_INVALID) return;
            if (newval == _bandwidth) return;
            _func.set_bandwidth(newval);
            _bandwidth = newval;
        }

        /**
         * <summary>
         *   Returns the X component of the acceleration, as a floating point number.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the X component of the acceleration, as a floating point number
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YAccelerometer.XVALUE_INVALID</c>.
         * </para>
         */
        public double get_xValue()
        {
            if (_func == null)
            {
                string msg = "No Accelerometer connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_xValue();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the Y component of the acceleration, as a floating point number.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the Y component of the acceleration, as a floating point number
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YAccelerometer.YVALUE_INVALID</c>.
         * </para>
         */
        public double get_yValue()
        {
            if (_func == null)
            {
                string msg = "No Accelerometer connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_yValue();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the Z component of the acceleration, as a floating point number.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the Z component of the acceleration, as a floating point number
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YAccelerometer.ZVALUE_INVALID</c>.
         * </para>
         */
        public double get_zValue()
        {
            if (_func == null)
            {
                string msg = "No Accelerometer connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_zValue();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }
    }
    //--- (end of YAccelerometer implementation)
}

