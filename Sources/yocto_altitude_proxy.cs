/*********************************************************************
 *
 *  $Id: yocto_altitude_proxy.cs 38913 2019-12-20 18:59:49Z mvuilleu $
 *
 *  Implements YAltitudeProxy, the Proxy API for Altitude
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
    //--- (YAltitude class start)
    static public partial class YoctoProxyManager
    {
        public static YAltitudeProxy FindAltitude(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YAltitude func = null;
            YAltitudeProxy res = (YAltitudeProxy)YFunctionProxy.FindSimilarUnknownFunction("YAltitudeProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YAltitudeProxy)YFunctionProxy.FindSimilarKnownFunction("YAltitudeProxy");
                if (res != null) return res;
                func = YAltitude.FirstAltitude();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YAltitudeProxy)func.get_userData();
                    }
                }
            } else {
                func = YAltitude.FindAltitude(name);
                if (func.get_userData() != null) {
                    return (YAltitudeProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YAltitudeProxy(func, name);
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
 *   The <c>YAltitude</c> class allows you to read and configure Yoctopuce altimeters.
 * <para>
 *   It inherits from <c>YSensor</c> class the core functions to read measurements,
 *   to register callback functions, and to access the autonomous datalogger.
 *   This class adds the ability to configure the barometric pressure adjusted to
 *   sea level (QNH) for barometric sensors.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YAltitudeProxy : YSensorProxy
    {
        /**
         * <summary>
         *   Retrieves an altimeter for a given identifier.
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
         *   This function does not require that the altimeter is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YAltitude.isOnline()</c> to test if the altimeter is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   an altimeter by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the altimeter, for instance
         *   <c>YALTIMK2.altitude</c>.
         * </param>
         * <returns>
         *   a <c>YAltitude</c> object allowing you to drive the altimeter.
         * </returns>
         */
        public static YAltitudeProxy FindAltitude(string func)
        {
            return YoctoProxyManager.FindAltitude(func);
        }
        //--- (end of YAltitude class start)
        //--- (YAltitude definitions)
        public const double _Qnh_INVALID = Double.NaN;
        public const string _Technology_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YAltitude _func;
        // property cache
        protected double _qnh = _Qnh_INVALID;
        //--- (end of YAltitude definitions)

        //--- (YAltitude implementation)
        internal YAltitudeProxy(YAltitude hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Altitude " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YAltitude) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YAltitude hwd = YAltitude.FindAltitude(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YAltitude hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Altitude callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type Altitude available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YAltitude.FindAltitude</c> to obtain an object that can control the
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
            YAltitude it = YAltitude.FirstAltitude();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextAltitude();
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
            _qnh = _func.get_qnh();
        }

        /**
         * <summary>
         *   Changes the current estimated altitude.
         * <para>
         *   This allows one to compensate for
         *   ambient pressure variations and to work in relative mode.
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the current estimated altitude
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
        public int set_currentValue(double newval)
        {
            if (_func == null)
            {
                string msg = "No Altitude connected";
                throw new YoctoApiProxyException(msg);
            }
            if (Double.IsNaN(newval)) return YAPI.SUCCESS;
            return _func.set_currentValue(newval);
        }


        // property with cached value for instant access (configuration)
        public new double CurrentValue
        {
            get
            {
                if (_func == null) return _CurrentValue_INVALID;
                return (_online ? _currentValue : _CurrentValue_INVALID);
            }
            set
            {
                setprop_currentValue(value);
            }
        }

        // private helper for magic property
        private void setprop_currentValue(double newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (Double.IsNaN(newval)) return;
            if (newval == _currentValue) return;
            _func.set_currentValue(newval);
            _currentValue = newval;
        }

        /**
         * <summary>
         *   Changes the barometric pressure adjusted to sea level used to compute
         *   the altitude (QNH).
         * <para>
         *   This enables you to compensate for atmospheric pressure
         *   changes due to weather conditions. Applicable to barometric altimeters only.
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the barometric pressure adjusted to sea level used to compute
         *   the altitude (QNH)
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
        public int set_qnh(double newval)
        {
            if (_func == null)
            {
                string msg = "No Altitude connected";
                throw new YoctoApiProxyException(msg);
            }
            if (Double.IsNaN(newval)) return YAPI.SUCCESS;
            return _func.set_qnh(newval);
        }


        // property with cached value for instant access (configuration)
        public double Qnh
        {
            get
            {
                if (_func == null) return _Qnh_INVALID;
                return (_online ? _qnh : _Qnh_INVALID);
            }
            set
            {
                setprop_qnh(value);
            }
        }

        // private helper for magic property
        private void setprop_qnh(double newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (Double.IsNaN(newval)) return;
            if (newval == _qnh) return;
            _func.set_qnh(newval);
            _qnh = newval;
        }

        /**
         * <summary>
         *   Returns the barometric pressure adjusted to sea level used to compute
         *   the altitude (QNH).
         * <para>
         *   Applicable to barometric altimeters only.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the barometric pressure adjusted to sea level used to compute
         *   the altitude (QNH)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>altitude._Qnh_INVALID</c>.
         * </para>
         */
        public double get_qnh()
        {
            if (_func == null)
            {
                string msg = "No Altitude connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_qnh();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the technology used by the sesnor to compute
         *   altitude.
         * <para>
         *   Possibles values are  "barometric" and "gps"
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the technology used by the sesnor to compute
         *   altitude
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>altitude._Technology_INVALID</c>.
         * </para>
         */
        public string get_technology()
        {
            if (_func == null)
            {
                string msg = "No Altitude connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_technology();
        }
    }
    //--- (end of YAltitude implementation)
}

