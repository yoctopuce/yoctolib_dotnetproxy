/*********************************************************************
 *
 *  $Id: yocto_realtimeclock_proxy.cs 50595 2022-07-28 07:54:15Z mvuilleu $
 *
 *  Implements YRealTimeClockProxy, the Proxy API for RealTimeClock
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
    //--- (YRealTimeClock class start)
    static public partial class YoctoProxyManager
    {
        public static YRealTimeClockProxy FindRealTimeClock(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YRealTimeClock func = null;
            YRealTimeClockProxy res = (YRealTimeClockProxy)YFunctionProxy.FindSimilarUnknownFunction("YRealTimeClockProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YRealTimeClockProxy)YFunctionProxy.FindSimilarKnownFunction("YRealTimeClockProxy");
                if (res != null) return res;
                func = YRealTimeClock.FirstRealTimeClock();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YRealTimeClockProxy)func.get_userData();
                    }
                }
            } else {
                func = YRealTimeClock.FindRealTimeClock(name);
                if (func.get_userData() != null) {
                    return (YRealTimeClockProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YRealTimeClockProxy(func, name);
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
 *   The <c>YRealTimeClock</c> class provide access to the embedded real-time clock available on some Yoctopuce
 *   devices.
 * <para>
 *   It can provide current date and time, even after a power outage
 *   lasting several days. It is the base for automated wake-up functions provided by the WakeUpScheduler.
 *   The current time may represent a local time as well as an UTC time, but no automatic time change
 *   will occur to account for daylight saving time.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YRealTimeClockProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a real-time clock for a given identifier.
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
         *   This function does not require that the real-time clock is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YRealTimeClock.isOnline()</c> to test if the real-time clock is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a real-time clock by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the real-time clock, for instance
         *   <c>YHUBGSM5.realTimeClock</c>.
         * </param>
         * <returns>
         *   a <c>YRealTimeClock</c> object allowing you to drive the real-time clock.
         * </returns>
         */
        public static YRealTimeClockProxy FindRealTimeClock(string func)
        {
            return YoctoProxyManager.FindRealTimeClock(func);
        }
        //--- (end of YRealTimeClock class start)
        //--- (YRealTimeClock definitions)
        public const string _Clock_INVALID = YAPI.INVALID_STRING;
        public const long _UnixTime_INVALID = YAPI.INVALID_LONG;
        public const string _DateTime_INVALID = YAPI.INVALID_STRING;
        public const int _UtcOffset_INVALID = YAPI.INVALID_INT;
        public const int _TimeSet_INVALID = 0;
        public const int _TimeSet_FALSE = 1;
        public const int _TimeSet_TRUE = 2;
        public const int _DisableHostSync_INVALID = 0;
        public const int _DisableHostSync_FALSE = 1;
        public const int _DisableHostSync_TRUE = 2;

        // reference to real YoctoAPI object
        protected new YRealTimeClock _func;
        // property cache
        protected string _clock = _Clock_INVALID;
        protected int _utcOffset = _UtcOffset_INVALID;
        //--- (end of YRealTimeClock definitions)

        //--- (YRealTimeClock implementation)
        internal YRealTimeClockProxy(YRealTimeClock hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("RealTimeClock " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YRealTimeClock) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YRealTimeClock hwd = YRealTimeClock.FindRealTimeClock(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YRealTimeClock hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering RealTimeClock callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type RealTimeClock available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YRealTimeClock.FindRealTimeClock</c> to obtain an object that can control the
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
            YRealTimeClock it = YRealTimeClock.FirstRealTimeClock();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextRealTimeClock();
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
            _utcOffset = _func.get_utcOffset();
        }

        // property with cached value for instant access (derived from advertisedValue)
        public string Clock
        {
            get
            {
                if (_func == null) return _Clock_INVALID;
                return (_online ? _advertisedValue : _Clock_INVALID);
            }
        }

        /**
         * <summary>
         *   Returns the current time in Unix format (number of elapsed seconds since Jan 1st, 1970).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current time in Unix format (number of elapsed seconds since Jan 1st, 1970)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YRealTimeClock.UNIXTIME_INVALID</c>.
         * </para>
         */
        public long get_unixTime()
        {
            long res;
            if (_func == null) {
                throw new YoctoApiProxyException("No RealTimeClock connected");
            }
            res = _func.get_unixTime();
            if (res == YAPI.INVALID_INT) {
                res = _UnixTime_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the current time.
         * <para>
         *   Time is specifid in Unix format (number of elapsed seconds since Jan 1st, 1970).
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the current time
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
        public int set_unixTime(long newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RealTimeClock connected");
            }
            if (newval == _UnixTime_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_unixTime(newval);
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
         *   On failure, throws an exception or returns <c>YRealTimeClock.DATETIME_INVALID</c>.
         * </para>
         */
        public string get_dateTime()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RealTimeClock connected");
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
         *   On failure, throws an exception or returns <c>YRealTimeClock.UTCOFFSET_INVALID</c>.
         * </para>
         */
        public int get_utcOffset()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RealTimeClock connected");
            }
            return _func.get_utcOffset();
        }

        /**
         * <summary>
         *   Changes the number of seconds between current time and UTC time (time zone).
         * <para>
         *   The timezone is automatically rounded to the nearest multiple of 15 minutes.
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
                throw new YoctoApiProxyException("No RealTimeClock connected");
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

        /**
         * <summary>
         *   Returns true if the clock has been set, and false otherwise.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YRealTimeClock.TIMESET_FALSE</c> or <c>YRealTimeClock.TIMESET_TRUE</c>, according to true
         *   if the clock has been set, and false otherwise
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YRealTimeClock.TIMESET_INVALID</c>.
         * </para>
         */
        public int get_timeSet()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RealTimeClock connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_timeSet()+1;
        }

        /**
         * <summary>
         *   Returns true if the automatic clock synchronization with host has been disabled,
         *   and false otherwise.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YRealTimeClock.DISABLEHOSTSYNC_FALSE</c> or <c>YRealTimeClock.DISABLEHOSTSYNC_TRUE</c>,
         *   according to true if the automatic clock synchronization with host has been disabled,
         *   and false otherwise
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YRealTimeClock.DISABLEHOSTSYNC_INVALID</c>.
         * </para>
         */
        public int get_disableHostSync()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RealTimeClock connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_disableHostSync()+1;
        }

        /**
         * <summary>
         *   Changes the automatic clock synchronization with host working state.
         * <para>
         *   To disable automatic synchronization, set the value to true.
         *   To enable automatic synchronization (default), set the value to false.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YRealTimeClock.DISABLEHOSTSYNC_FALSE</c> or <c>YRealTimeClock.DISABLEHOSTSYNC_TRUE</c>,
         *   according to the automatic clock synchronization with host working state
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
        public int set_disableHostSync(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RealTimeClock connected");
            }
            if (newval == _DisableHostSync_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_disableHostSync(newval-1);
        }
    }
    //--- (end of YRealTimeClock implementation)
}

