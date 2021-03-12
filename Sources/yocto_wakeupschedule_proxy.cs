/*********************************************************************
 *
 *  $Id: yocto_wakeupschedule_proxy.cs 43619 2021-01-29 09:14:45Z mvuilleu $
 *
 *  Implements YWakeUpScheduleProxy, the Proxy API for WakeUpSchedule
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
    //--- (YWakeUpSchedule class start)
    static public partial class YoctoProxyManager
    {
        public static YWakeUpScheduleProxy FindWakeUpSchedule(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YWakeUpSchedule func = null;
            YWakeUpScheduleProxy res = (YWakeUpScheduleProxy)YFunctionProxy.FindSimilarUnknownFunction("YWakeUpScheduleProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YWakeUpScheduleProxy)YFunctionProxy.FindSimilarKnownFunction("YWakeUpScheduleProxy");
                if (res != null) return res;
                func = YWakeUpSchedule.FirstWakeUpSchedule();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YWakeUpScheduleProxy)func.get_userData();
                    }
                }
            } else {
                func = YWakeUpSchedule.FindWakeUpSchedule(name);
                if (func.get_userData() != null) {
                    return (YWakeUpScheduleProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YWakeUpScheduleProxy(func, name);
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
 *   The <c>YWakeUpSchedule</c> class implements a wake up condition.
 * <para>
 *   The wake up time is
 *   specified as a set of months and/or days and/or hours and/or minutes when the
 *   wake up should happen.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YWakeUpScheduleProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a wake up schedule for a given identifier.
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
         *   This function does not require that the wake up schedule is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YWakeUpSchedule.isOnline()</c> to test if the wake up schedule is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a wake up schedule by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the wake up schedule, for instance
         *   <c>YHUBGSM3.wakeUpSchedule1</c>.
         * </param>
         * <returns>
         *   a <c>YWakeUpSchedule</c> object allowing you to drive the wake up schedule.
         * </returns>
         */
        public static YWakeUpScheduleProxy FindWakeUpSchedule(string func)
        {
            return YoctoProxyManager.FindWakeUpSchedule(func);
        }
        //--- (end of YWakeUpSchedule class start)
        //--- (YWakeUpSchedule definitions)
        public const int _MinutesA_INVALID = -1;
        public const int _MinutesB_INVALID = -1;
        public const int _Hours_INVALID = -1;
        public const int _WeekDays_INVALID = -1;
        public const int _MonthDays_INVALID = -1;
        public const int _Months_INVALID = -1;
        public const long _NextOccurence_INVALID = YAPI.INVALID_LONG;

        // reference to real YoctoAPI object
        protected new YWakeUpSchedule _func;
        // property cache
        protected int _minutesA = _MinutesA_INVALID;
        protected int _minutesB = _MinutesB_INVALID;
        protected int _hours = _Hours_INVALID;
        protected int _weekDays = _WeekDays_INVALID;
        protected int _monthDays = _MonthDays_INVALID;
        protected int _months = _Months_INVALID;
        protected long _nextOccurence = _NextOccurence_INVALID;
        //--- (end of YWakeUpSchedule definitions)

        //--- (YWakeUpSchedule implementation)
        internal YWakeUpScheduleProxy(YWakeUpSchedule hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("WakeUpSchedule " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YWakeUpSchedule) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YWakeUpSchedule hwd = YWakeUpSchedule.FindWakeUpSchedule(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YWakeUpSchedule hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering WakeUpSchedule callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type WakeUpSchedule available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YWakeUpSchedule.FindWakeUpSchedule</c> to obtain an object that can control the
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
            YWakeUpSchedule it = YWakeUpSchedule.FirstWakeUpSchedule();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextWakeUpSchedule();
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
            _minutesA = _func.get_minutesA();
            _minutesB = _func.get_minutesB();
            _hours = _func.get_hours();
            _weekDays = _func.get_weekDays();
            _monthDays = _func.get_monthDays();
            _months = _func.get_months();
        }

        /**
         * <summary>
         *   Returns the minutes in the 00-29 interval of each hour scheduled for wake up.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the minutes in the 00-29 interval of each hour scheduled for wake up
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWakeUpSchedule.MINUTESA_INVALID</c>.
         * </para>
         */
        public int get_minutesA()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No WakeUpSchedule connected");
            }
            res = _func.get_minutesA();
            if (res == YAPI.INVALID_INT) {
                res = _MinutesA_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the minutes in the 00-29 interval when a wake up must take place.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the minutes in the 00-29 interval when a wake up must take place
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
        public int set_minutesA(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No WakeUpSchedule connected");
            }
            if (newval == _MinutesA_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_minutesA(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Minutes in the 00-29 interval of each hour scheduled for wake up.</value>
        public int MinutesA
        {
            get
            {
                if (_func == null) {
                    return _MinutesA_INVALID;
                }
                if (_online) {
                    return _minutesA;
                }
                return _MinutesA_INVALID;
            }
            set
            {
                setprop_minutesA(value);
            }
        }

        // private helper for magic property
        private void setprop_minutesA(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _MinutesA_INVALID) {
                return;
            }
            if (newval == _minutesA) {
                return;
            }
            _func.set_minutesA(newval);
            _minutesA = newval;
        }

        /**
         * <summary>
         *   Returns the minutes in the 30-59 interval of each hour scheduled for wake up.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the minutes in the 30-59 interval of each hour scheduled for wake up
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWakeUpSchedule.MINUTESB_INVALID</c>.
         * </para>
         */
        public int get_minutesB()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No WakeUpSchedule connected");
            }
            res = _func.get_minutesB();
            if (res == YAPI.INVALID_INT) {
                res = _MinutesB_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the minutes in the 30-59 interval when a wake up must take place.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the minutes in the 30-59 interval when a wake up must take place
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
        public int set_minutesB(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No WakeUpSchedule connected");
            }
            if (newval == _MinutesB_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_minutesB(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Minutes in the 30-59 interval of each hour scheduled for wake up.</value>
        public int MinutesB
        {
            get
            {
                if (_func == null) {
                    return _MinutesB_INVALID;
                }
                if (_online) {
                    return _minutesB;
                }
                return _MinutesB_INVALID;
            }
            set
            {
                setprop_minutesB(value);
            }
        }

        // private helper for magic property
        private void setprop_minutesB(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _MinutesB_INVALID) {
                return;
            }
            if (newval == _minutesB) {
                return;
            }
            _func.set_minutesB(newval);
            _minutesB = newval;
        }

        /**
         * <summary>
         *   Returns the hours scheduled for wake up.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the hours scheduled for wake up
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWakeUpSchedule.HOURS_INVALID</c>.
         * </para>
         */
        public int get_hours()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No WakeUpSchedule connected");
            }
            res = _func.get_hours();
            if (res == YAPI.INVALID_INT) {
                res = _Hours_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the hours when a wake up must take place.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the hours when a wake up must take place
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
        public int set_hours(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No WakeUpSchedule connected");
            }
            if (newval == _Hours_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_hours(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Hours scheduled for wake up.</value>
        public int Hours
        {
            get
            {
                if (_func == null) {
                    return _Hours_INVALID;
                }
                if (_online) {
                    return _hours;
                }
                return _Hours_INVALID;
            }
            set
            {
                setprop_hours(value);
            }
        }

        // private helper for magic property
        private void setprop_hours(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Hours_INVALID) {
                return;
            }
            if (newval == _hours) {
                return;
            }
            _func.set_hours(newval);
            _hours = newval;
        }

        /**
         * <summary>
         *   Returns the days of the week scheduled for wake up.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the days of the week scheduled for wake up
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWakeUpSchedule.WEEKDAYS_INVALID</c>.
         * </para>
         */
        public int get_weekDays()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No WakeUpSchedule connected");
            }
            res = _func.get_weekDays();
            if (res == YAPI.INVALID_INT) {
                res = _WeekDays_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the days of the week when a wake up must take place.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the days of the week when a wake up must take place
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
        public int set_weekDays(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No WakeUpSchedule connected");
            }
            if (newval == _WeekDays_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_weekDays(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Days of the week scheduled for wake up.</value>
        public int WeekDays
        {
            get
            {
                if (_func == null) {
                    return _WeekDays_INVALID;
                }
                if (_online) {
                    return _weekDays;
                }
                return _WeekDays_INVALID;
            }
            set
            {
                setprop_weekDays(value);
            }
        }

        // private helper for magic property
        private void setprop_weekDays(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _WeekDays_INVALID) {
                return;
            }
            if (newval == _weekDays) {
                return;
            }
            _func.set_weekDays(newval);
            _weekDays = newval;
        }

        /**
         * <summary>
         *   Returns the days of the month scheduled for wake up.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the days of the month scheduled for wake up
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWakeUpSchedule.MONTHDAYS_INVALID</c>.
         * </para>
         */
        public int get_monthDays()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No WakeUpSchedule connected");
            }
            res = _func.get_monthDays();
            if (res == YAPI.INVALID_INT) {
                res = _MonthDays_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the days of the month when a wake up must take place.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the days of the month when a wake up must take place
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
        public int set_monthDays(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No WakeUpSchedule connected");
            }
            if (newval == _MonthDays_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_monthDays(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Days of the month scheduled for wake up.</value>
        public int MonthDays
        {
            get
            {
                if (_func == null) {
                    return _MonthDays_INVALID;
                }
                if (_online) {
                    return _monthDays;
                }
                return _MonthDays_INVALID;
            }
            set
            {
                setprop_monthDays(value);
            }
        }

        // private helper for magic property
        private void setprop_monthDays(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _MonthDays_INVALID) {
                return;
            }
            if (newval == _monthDays) {
                return;
            }
            _func.set_monthDays(newval);
            _monthDays = newval;
        }

        /**
         * <summary>
         *   Returns the months scheduled for wake up.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the months scheduled for wake up
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWakeUpSchedule.MONTHS_INVALID</c>.
         * </para>
         */
        public int get_months()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No WakeUpSchedule connected");
            }
            res = _func.get_months();
            if (res == YAPI.INVALID_INT) {
                res = _Months_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the months when a wake up must take place.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the months when a wake up must take place
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
        public int set_months(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No WakeUpSchedule connected");
            }
            if (newval == _Months_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_months(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Months scheduled for wake up.</value>
        public int Months
        {
            get
            {
                if (_func == null) {
                    return _Months_INVALID;
                }
                if (_online) {
                    return _months;
                }
                return _Months_INVALID;
            }
            set
            {
                setprop_months(value);
            }
        }

        // private helper for magic property
        private void setprop_months(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Months_INVALID) {
                return;
            }
            if (newval == _months) {
                return;
            }
            _func.set_months(newval);
            _months = newval;
        }

        /**
         * <summary>
         *   Returns the date/time (seconds) of the next wake up occurrence.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the date/time (seconds) of the next wake up occurrence
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWakeUpSchedule.NEXTOCCURENCE_INVALID</c>.
         * </para>
         */
        public long get_nextOccurence()
        {
            long res;
            if (_func == null) {
                throw new YoctoApiProxyException("No WakeUpSchedule connected");
            }
            res = _func.get_nextOccurence();
            if (res == YAPI.INVALID_INT) {
                res = _NextOccurence_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (advertised value)
        /// <value>Date/time (seconds) of the next wake up occurrence.</value>
        public long NextOccurence
        {
            get
            {
                if (_func == null) {
                    return _NextOccurence_INVALID;
                }
                if (_online) {
                    return _nextOccurence;
                }
                return _NextOccurence_INVALID;
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            _nextOccurence = YAPI._hexStrToLong(value);
        }

        /**
         * <summary>
         *   Returns all the minutes of each hour that are scheduled for wake up.
         * <para>
         * </para>
         * </summary>
         */
        public virtual long get_minutes()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No WakeUpSchedule connected");
            }
            return _func.get_minutes();
        }

        /**
         * <summary>
         *   Changes all the minutes where a wake up must take place.
         * <para>
         * </para>
         * </summary>
         * <param name="bitmap">
         *   Minutes 00-59 of each hour scheduled for wake up.
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_minutes(long bitmap)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No WakeUpSchedule connected");
            }
            return _func.set_minutes(bitmap);
        }
    }
    //--- (end of YWakeUpSchedule implementation)
}

