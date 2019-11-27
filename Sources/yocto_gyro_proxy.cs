/*********************************************************************
 *
 *  $Id: yocto_gyro_proxy.cs 38514 2019-11-26 16:54:39Z seb $
 *
 *  Implements YGyroProxy, the Proxy API for Gyro
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
using YoctoLib;

namespace YoctoProxyAPI
{
    //--- (generated code: YGyro class start)
    static public partial class YoctoProxyManager
    {
        public static YGyroProxy FindGyro(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YGyro func = null;
            YGyroProxy res = (YGyroProxy)YFunctionProxy.FindSimilarUnknownFunction("YGyroProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YGyroProxy)YFunctionProxy.FindSimilarKnownFunction("YGyroProxy");
                if (res != null) return res;
                func = YGyro.FirstGyro();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YGyroProxy)func.get_userData();
                    }
                }
            } else {
                func = YGyro.FindGyro(name);
                if (func.get_userData() != null) {
                    return (YGyroProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YGyroProxy(func, name);
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
 *   The YGyro class allows you to read and configure Yoctopuce angular velocity
 *   sensors, for instance using a Yocto-3D-V2.
 * <para>
 *   It inherits from YSensor class the core functions to read measurements,
 *   to register callback functions, to access the autonomous datalogger.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YGyroProxy : YSensorProxy
    {
        //--- (end of generated code: YGyro class start)
        //--- (generated code: YGyro definitions)
        public const int _Bandwidth_INVALID = -1;
        public const double _XValue_INVALID = Double.NaN;
        public const double _YValue_INVALID = Double.NaN;
        public const double _ZValue_INVALID = Double.NaN;

        // reference to real YoctoAPI object
        protected new YGyro _func;
        // property cache
        protected int _bandwidth = _Bandwidth_INVALID;
        //--- (end of generated code: YGyro definitions)

        //--- (generated code: YGyro implementation)
        internal YGyroProxy(YGyro hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Gyro " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YGyro) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YGyro hwd = YGyro.FindGyro(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YGyro hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Gyro callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YGyro it = YGyro.FirstGyro();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextGyro();
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
         *   On failure, throws an exception or returns <c>YGyro.BANDWIDTH_INVALID</c>.
         * </para>
         */
        public int get_bandwidth()
        {
            if (_func == null)
            {
                string msg = "No Gyro connected";
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
                string msg = "No Gyro connected";
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
         *   Returns the angular velocity around the X axis of the device, as a floating point number.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the angular velocity around the X axis of the device, as a
         *   floating point number
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGyro.XVALUE_INVALID</c>.
         * </para>
         */
        public double get_xValue()
        {
            if (_func == null)
            {
                string msg = "No Gyro connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_xValue();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the angular velocity around the Y axis of the device, as a floating point number.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the angular velocity around the Y axis of the device, as a
         *   floating point number
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGyro.YVALUE_INVALID</c>.
         * </para>
         */
        public double get_yValue()
        {
            if (_func == null)
            {
                string msg = "No Gyro connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_yValue();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the angular velocity around the Z axis of the device, as a floating point number.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the angular velocity around the Z axis of the device, as a
         *   floating point number
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGyro.ZVALUE_INVALID</c>.
         * </para>
         */
        public double get_zValue()
        {
            if (_func == null)
            {
                string msg = "No Gyro connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_zValue();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the estimated roll angle, based on the integration of
         *   gyroscopic measures combined with acceleration and
         *   magnetic field measurements.
         * <para>
         *   The axis corresponding to the roll angle can be mapped to any
         *   of the device X, Y or Z physical directions using methods of
         *   the class <c>YRefFrame</c>.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating-point number corresponding to roll angle
         *   in degrees, between -180 and +180.
         * </returns>
         */
        public virtual double get_roll()
        {
            if (_func == null)
            {
                string msg = "No Gyro connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_roll();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the estimated pitch angle, based on the integration of
         *   gyroscopic measures combined with acceleration and
         *   magnetic field measurements.
         * <para>
         *   The axis corresponding to the pitch angle can be mapped to any
         *   of the device X, Y or Z physical directions using methods of
         *   the class <c>YRefFrame</c>.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating-point number corresponding to pitch angle
         *   in degrees, between -90 and +90.
         * </returns>
         */
        public virtual double get_pitch()
        {
            if (_func == null)
            {
                string msg = "No Gyro connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_pitch();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the estimated heading angle, based on the integration of
         *   gyroscopic measures combined with acceleration and
         *   magnetic field measurements.
         * <para>
         *   The axis corresponding to the heading can be mapped to any
         *   of the device X, Y or Z physical directions using methods of
         *   the class <c>YRefFrame</c>.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating-point number corresponding to heading
         *   in degrees, between 0 and 360.
         * </returns>
         */
        public virtual double get_heading()
        {
            if (_func == null)
            {
                string msg = "No Gyro connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_heading();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the <c>w</c> component (real part) of the quaternion
         *   describing the device estimated orientation, based on the
         *   integration of gyroscopic measures combined with acceleration and
         *   magnetic field measurements.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating-point number corresponding to the <c>w</c>
         *   component of the quaternion.
         * </returns>
         */
        public virtual double get_quaternionW()
        {
            if (_func == null)
            {
                string msg = "No Gyro connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_quaternionW();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the <c>x</c> component of the quaternion
         *   describing the device estimated orientation, based on the
         *   integration of gyroscopic measures combined with acceleration and
         *   magnetic field measurements.
         * <para>
         *   The <c>x</c> component is
         *   mostly correlated with rotations on the roll axis.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating-point number corresponding to the <c>x</c>
         *   component of the quaternion.
         * </returns>
         */
        public virtual double get_quaternionX()
        {
            if (_func == null)
            {
                string msg = "No Gyro connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_quaternionX();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the <c>y</c> component of the quaternion
         *   describing the device estimated orientation, based on the
         *   integration of gyroscopic measures combined with acceleration and
         *   magnetic field measurements.
         * <para>
         *   The <c>y</c> component is
         *   mostly correlated with rotations on the pitch axis.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating-point number corresponding to the <c>y</c>
         *   component of the quaternion.
         * </returns>
         */
        public virtual double get_quaternionY()
        {
            if (_func == null)
            {
                string msg = "No Gyro connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_quaternionY();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the <c>x</c> component of the quaternion
         *   describing the device estimated orientation, based on the
         *   integration of gyroscopic measures combined with acceleration and
         *   magnetic field measurements.
         * <para>
         *   The <c>x</c> component is
         *   mostly correlated with changes of heading.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating-point number corresponding to the <c>z</c>
         *   component of the quaternion.
         * </returns>
         */
        public virtual double get_quaternionZ()
        {
            if (_func == null)
            {
                string msg = "No Gyro connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_quaternionZ();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }
    }
    //--- (end of generated code: YGyro implementation)
}

