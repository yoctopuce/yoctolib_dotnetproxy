/*********************************************************************
 *
 *  $Id: yocto_gps_proxy.cs 43619 2021-01-29 09:14:45Z mvuilleu $
 *
 *  Implements YGpsProxy, the Proxy API for Gps
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
    //--- (YGps class start)
    static public partial class YoctoProxyManager
    {
        public static YGpsProxy FindGps(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YGps func = null;
            YGpsProxy res = (YGpsProxy)YFunctionProxy.FindSimilarUnknownFunction("YGpsProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YGpsProxy)YFunctionProxy.FindSimilarKnownFunction("YGpsProxy");
                if (res != null) return res;
                func = YGps.FirstGps();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YGpsProxy)func.get_userData();
                    }
                }
            } else {
                func = YGps.FindGps(name);
                if (func.get_userData() != null) {
                    return (YGpsProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YGpsProxy(func, name);
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
 *   The <c>YGps</c> class allows you to retrieve positioning
 *   data from a GPS/GNSS sensor.
 * <para>
 *   This class can provides
 *   complete positioning information. However, if you
 *   wish to define callbacks on position changes or record
 *   the position in the datalogger, you
 *   should use the <c>YLatitude</c> et <c>YLongitude</c> classes.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YGpsProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a geolocalization module for a given identifier.
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
         *   This function does not require that the geolocalization module is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YGps.isOnline()</c> to test if the geolocalization module is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a geolocalization module by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the geolocalization module, for instance
         *   <c>YGNSSMK2.gps</c>.
         * </param>
         * <returns>
         *   a <c>YGps</c> object allowing you to drive the geolocalization module.
         * </returns>
         */
        public static YGpsProxy FindGps(string func)
        {
            return YoctoProxyManager.FindGps(func);
        }
        //--- (end of YGps class start)
        //--- (YGps definitions)
        public const int _IsFixed_INVALID = 0;
        public const int _IsFixed_FALSE = 1;
        public const int _IsFixed_TRUE = 2;
        public const long _SatCount_INVALID = YAPI.INVALID_LONG;
        public const long _SatPerConst_INVALID = YAPI.INVALID_LONG;
        public const double _GpsRefreshRate_INVALID = Double.NaN;
        public const int _CoordSystem_INVALID = 0;
        public const int _CoordSystem_GPS_DMS = 1;
        public const int _CoordSystem_GPS_DM = 2;
        public const int _CoordSystem_GPS_D = 3;
        public const int _Constellation_INVALID = 0;
        public const int _Constellation_GNSS = 1;
        public const int _Constellation_GPS = 2;
        public const int _Constellation_GLONASS = 3;
        public const int _Constellation_GALILEO = 4;
        public const int _Constellation_GPS_GLONASS = 5;
        public const int _Constellation_GPS_GALILEO = 6;
        public const int _Constellation_GLONASS_GALILEO = 7;
        public const string _Latitude_INVALID = YAPI.INVALID_STRING;
        public const string _Longitude_INVALID = YAPI.INVALID_STRING;
        public const double _Dilution_INVALID = Double.NaN;
        public const double _Altitude_INVALID = Double.NaN;
        public const double _GroundSpeed_INVALID = Double.NaN;
        public const double _Direction_INVALID = Double.NaN;
        public const long _UnixTime_INVALID = YAPI.INVALID_LONG;
        public const string _DateTime_INVALID = YAPI.INVALID_STRING;
        public const int _UtcOffset_INVALID = YAPI.INVALID_INT;
        public const string _Command_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YGps _func;
        // property cache
        protected long _satCount = _SatCount_INVALID;
        protected int _coordSystem = _CoordSystem_INVALID;
        protected int _utcOffset = _UtcOffset_INVALID;
        protected int _isFixed = _IsFixed_INVALID;
        //--- (end of YGps definitions)

        //--- (YGps implementation)
        internal YGpsProxy(YGps hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Gps " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YGps) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YGps hwd = YGps.FindGps(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YGps hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Gps callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type Gps available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YGps.FindGps</c> to obtain an object that can control the
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
            YGps it = YGps.FirstGps();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextGps();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
            _isFixed = _func.get_isFixed()+1;
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            _coordSystem = _func.get_coordSystem()+1;
            _utcOffset = _func.get_utcOffset();
        }

        /**
         * <summary>
         *   Returns TRUE if the receiver has found enough satellites to work.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YGps.ISFIXED_FALSE</c> or <c>YGps.ISFIXED_TRUE</c>, according to TRUE if the receiver has
         *   found enough satellites to work
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGps.ISFIXED_INVALID</c>.
         * </para>
         */
        public int get_isFixed()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Gps connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_isFixed()+1;
        }

        /**
         * <summary>
         *   Returns the total count of satellites used to compute GPS position.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the total count of satellites used to compute GPS position
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGps.SATCOUNT_INVALID</c>.
         * </para>
         */
        public long get_satCount()
        {
            long res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Gps connected");
            }
            res = _func.get_satCount();
            if (res == YAPI.INVALID_INT) {
                res = _SatCount_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (advertised value)
        /// <value>Total count of satellites used to compute GPS position.</value>
        public long SatCount
        {
            get
            {
                if (_func == null) {
                    return _SatCount_INVALID;
                }
                if (_online) {
                    return _satCount;
                }
                return _SatCount_INVALID;
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            if (value == "Fixing") {
                _isFixed = _IsFixed_FALSE;
            } else {
                _isFixed = _IsFixed_TRUE;
            }
            value = (YAPI._atoi(value)).ToString();
            _satCount = YAPI._atol(value);
        }

        // property with cached value for instant access (derived from advertised value)
        /// <value>True if the receiver has found enough satellites to work.</value>
        public int IsFixed
        {
            get
            {
                if (_func == null) {
                    return _IsFixed_INVALID;
                }
                if (_online) {
                    return _isFixed;
                }
                return _IsFixed_INVALID;
            }
        }

        /**
         * <summary>
         *   Returns the count of visible satellites per constellation encoded
         *   on a 32 bit integer: bits 0..
         * <para>
         *   5: GPS satellites count,  bits 6..11 : Glonass, bits 12..17 : Galileo.
         *   this value is refreshed every 5 seconds only.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the count of visible satellites per constellation encoded
         *   on a 32 bit integer: bits 0.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGps.SATPERCONST_INVALID</c>.
         * </para>
         */
        public long get_satPerConst()
        {
            long res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Gps connected");
            }
            res = _func.get_satPerConst();
            if (res == YAPI.INVALID_INT) {
                res = _SatPerConst_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns effective GPS data refresh frequency.
         * <para>
         *   this value is refreshed every 5 seconds only.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to effective GPS data refresh frequency
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGps.GPSREFRESHRATE_INVALID</c>.
         * </para>
         */
        public double get_gpsRefreshRate()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Gps connected");
            }
            res = _func.get_gpsRefreshRate();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the representation system used for positioning data.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YGps.COORDSYSTEM_GPS_DMS</c>, <c>YGps.COORDSYSTEM_GPS_DM</c> and
         *   <c>YGps.COORDSYSTEM_GPS_D</c> corresponding to the representation system used for positioning data
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGps.COORDSYSTEM_INVALID</c>.
         * </para>
         */
        public int get_coordSystem()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Gps connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_coordSystem()+1;
        }

        /**
         * <summary>
         *   Changes the representation system used for positioning data.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YGps.COORDSYSTEM_GPS_DMS</c>, <c>YGps.COORDSYSTEM_GPS_DM</c> and
         *   <c>YGps.COORDSYSTEM_GPS_D</c> corresponding to the representation system used for positioning data
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
        public int set_coordSystem(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Gps connected");
            }
            if (newval == _CoordSystem_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_coordSystem(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>Representation system used for positioning data.</value>
        public int CoordSystem
        {
            get
            {
                if (_func == null) {
                    return _CoordSystem_INVALID;
                }
                if (_online) {
                    return _coordSystem;
                }
                return _CoordSystem_INVALID;
            }
            set
            {
                setprop_coordSystem(value);
            }
        }

        // private helper for magic property
        private void setprop_coordSystem(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _CoordSystem_INVALID) {
                return;
            }
            if (newval == _coordSystem) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_coordSystem(newval-1);
            _coordSystem = newval;
        }

        /**
         * <summary>
         *   Returns the the satellites constellation used to compute
         *   positioning data.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YGps.CONSTELLATION_GNSS</c>, <c>YGps.CONSTELLATION_GPS</c>,
         *   <c>YGps.CONSTELLATION_GLONASS</c>, <c>YGps.CONSTELLATION_GALILEO</c>,
         *   <c>YGps.CONSTELLATION_GPS_GLONASS</c>, <c>YGps.CONSTELLATION_GPS_GALILEO</c> and
         *   <c>YGps.CONSTELLATION_GLONASS_GALILEO</c> corresponding to the the satellites constellation used to compute
         *   positioning data
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGps.CONSTELLATION_INVALID</c>.
         * </para>
         */
        public int get_constellation()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Gps connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_constellation()+1;
        }

        /**
         * <summary>
         *   Changes the satellites constellation used to compute
         *   positioning data.
         * <para>
         *   Possible  constellations are GNSS ( = all supported constellations),
         *   GPS, Glonass, Galileo , and the 3 possible pairs. This setting has  no effect on Yocto-GPS (V1).
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YGps.CONSTELLATION_GNSS</c>, <c>YGps.CONSTELLATION_GPS</c>,
         *   <c>YGps.CONSTELLATION_GLONASS</c>, <c>YGps.CONSTELLATION_GALILEO</c>,
         *   <c>YGps.CONSTELLATION_GPS_GLONASS</c>, <c>YGps.CONSTELLATION_GPS_GALILEO</c> and
         *   <c>YGps.CONSTELLATION_GLONASS_GALILEO</c> corresponding to the satellites constellation used to compute
         *   positioning data
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
        public int set_constellation(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Gps connected");
            }
            if (newval == _Constellation_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_constellation(newval-1);
        }

        /**
         * <summary>
         *   Returns the current latitude.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the current latitude
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGps.LATITUDE_INVALID</c>.
         * </para>
         */
        public string get_latitude()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Gps connected");
            }
            return _func.get_latitude();
        }

        /**
         * <summary>
         *   Returns the current longitude.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the current longitude
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGps.LONGITUDE_INVALID</c>.
         * </para>
         */
        public string get_longitude()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Gps connected");
            }
            return _func.get_longitude();
        }

        /**
         * <summary>
         *   Returns the current horizontal dilution of precision,
         *   the smaller that number is, the better .
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the current horizontal dilution of precision,
         *   the smaller that number is, the better
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGps.DILUTION_INVALID</c>.
         * </para>
         */
        public double get_dilution()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Gps connected");
            }
            res = _func.get_dilution();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the current altitude.
         * <para>
         *   Beware:  GPS technology
         *   is very inaccurate regarding altitude.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the current altitude
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGps.ALTITUDE_INVALID</c>.
         * </para>
         */
        public double get_altitude()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Gps connected");
            }
            res = _func.get_altitude();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the current ground speed in Km/h.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the current ground speed in Km/h
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGps.GROUNDSPEED_INVALID</c>.
         * </para>
         */
        public double get_groundSpeed()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Gps connected");
            }
            res = _func.get_groundSpeed();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the current move bearing in degrees, zero
         *   is the true (geographic) north.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the current move bearing in degrees, zero
         *   is the true (geographic) north
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGps.DIRECTION_INVALID</c>.
         * </para>
         */
        public double get_direction()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Gps connected");
            }
            res = _func.get_direction();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the current time in Unix format (number of
         *   seconds elapsed since Jan 1st, 1970).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current time in Unix format (number of
         *   seconds elapsed since Jan 1st, 1970)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGps.UNIXTIME_INVALID</c>.
         * </para>
         */
        public long get_unixTime()
        {
            long res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Gps connected");
            }
            res = _func.get_unixTime();
            if (res == YAPI.INVALID_INT) {
                res = _UnixTime_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the current time in the form "YYYY/MM/DD hh:mm:ss".
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the current time in the form "YYYY/MM/DD hh:mm:ss"
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGps.DATETIME_INVALID</c>.
         * </para>
         */
        public string get_dateTime()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Gps connected");
            }
            return _func.get_dateTime();
        }

        /**
         * <summary>
         *   Returns the number of seconds between current time and UTC time (time zone).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of seconds between current time and UTC time (time zone)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YGps.UTCOFFSET_INVALID</c>.
         * </para>
         */
        public int get_utcOffset()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Gps connected");
            }
            return _func.get_utcOffset();
        }

        /**
         * <summary>
         *   Changes the number of seconds between current time and UTC time (time zone).
         * <para>
         *   The timezone is automatically rounded to the nearest multiple of 15 minutes.
         *   If current UTC time is known, the current time is automatically be updated according to the selected time zone.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the number of seconds between current time and UTC time (time zone)
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
        public int set_utcOffset(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Gps connected");
            }
            if (newval == _UtcOffset_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_utcOffset(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Number of seconds between current time and UTC time (time zone).</value>
        public int UtcOffset
        {
            get
            {
                if (_func == null) {
                    return _UtcOffset_INVALID;
                }
                if (_online) {
                    return _utcOffset;
                }
                return _UtcOffset_INVALID;
            }
            set
            {
                setprop_utcOffset(value);
            }
        }

        // private helper for magic property
        private void setprop_utcOffset(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _UtcOffset_INVALID) {
                return;
            }
            if (newval == _utcOffset) {
                return;
            }
            _func.set_utcOffset(newval);
            _utcOffset = newval;
        }
    }
    //--- (end of YGps implementation)
}

