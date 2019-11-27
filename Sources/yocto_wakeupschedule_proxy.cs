/*********************************************************************
 *
 *  $Id: yocto_wakeupschedule_proxy.cs 38514 2019-11-26 16:54:39Z seb $
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
 *   The YWakeUpSchedule class implements a wake up condition, for instance using a YoctoHub-GSM-3G-EU, a YoctoHub-GSM-3G-NA, a YoctoHub-Wireless-SR or a YoctoHub-Wireless-g.
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

        public override string[] GetSimilarFunctions()
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
            if (_func == null)
            {
                string msg = "No WakeUpSchedule connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_minutesA();
            if (res == YAPI.INVALID_INT) res = _MinutesA_INVALID;
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
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_minutesA(int newval)
        {
            if (_func == null)
            {
                string msg = "No WakeUpSchedule connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _MinutesA_INVALID) return YAPI.SUCCESS;
            return _func.set_minutesA(newval);
        }


        // property with cached value for instant access (configuration)
        public int MinutesA
        {
            get
            {
                if (_func == null) return _MinutesA_INVALID;
                return (_online ? _minutesA : _MinutesA_INVALID);
            }
            set
            {
                setprop_minutesA(value);
            }
        }

        // private helper for magic property
        private void setprop_minutesA(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _MinutesA_INVALID) return;
            if (newval == _minutesA) return;
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
            if (_func == null)
            {
                string msg = "No WakeUpSchedule connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_minutesB();
            if (res == YAPI.INVALID_INT) res = _MinutesB_INVALID;
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
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_minutesB(int newval)
        {
            if (_func == null)
            {
                string msg = "No WakeUpSchedule connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _MinutesB_INVALID) return YAPI.SUCCESS;
            return _func.set_minutesB(newval);
        }


        // property with cached value for instant access (configuration)
        public int MinutesB
        {
            get
            {
                if (_func == null) return _MinutesB_INVALID;
                return (_online ? _minutesB : _MinutesB_INVALID);
            }
            set
            {
                setprop_minutesB(value);
            }
        }

        // private helper for magic property
        private void setprop_minutesB(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _MinutesB_INVALID) return;
            if (newval == _minutesB) return;
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
            if (_func == null)
            {
                string msg = "No WakeUpSchedule connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_hours();
            if (res == YAPI.INVALID_INT) res = _Hours_INVALID;
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
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_hours(int newval)
        {
            if (_func == null)
            {
                string msg = "No WakeUpSchedule connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Hours_INVALID) return YAPI.SUCCESS;
            return _func.set_hours(newval);
        }


        // property with cached value for instant access (configuration)
        public int Hours
        {
            get
            {
                if (_func == null) return _Hours_INVALID;
                return (_online ? _hours : _Hours_INVALID);
            }
            set
            {
                setprop_hours(value);
            }
        }

        // private helper for magic property
        private void setprop_hours(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Hours_INVALID) return;
            if (newval == _hours) return;
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
            if (_func == null)
            {
                string msg = "No WakeUpSchedule connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_weekDays();
            if (res == YAPI.INVALID_INT) res = _WeekDays_INVALID;
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
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_weekDays(int newval)
        {
            if (_func == null)
            {
                string msg = "No WakeUpSchedule connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _WeekDays_INVALID) return YAPI.SUCCESS;
            return _func.set_weekDays(newval);
        }


        // property with cached value for instant access (configuration)
        public int WeekDays
        {
            get
            {
                if (_func == null) return _WeekDays_INVALID;
                return (_online ? _weekDays : _WeekDays_INVALID);
            }
            set
            {
                setprop_weekDays(value);
            }
        }

        // private helper for magic property
        private void setprop_weekDays(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _WeekDays_INVALID) return;
            if (newval == _weekDays) return;
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
            if (_func == null)
            {
                string msg = "No WakeUpSchedule connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_monthDays();
            if (res == YAPI.INVALID_INT) res = _MonthDays_INVALID;
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
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_monthDays(int newval)
        {
            if (_func == null)
            {
                string msg = "No WakeUpSchedule connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _MonthDays_INVALID) return YAPI.SUCCESS;
            return _func.set_monthDays(newval);
        }


        // property with cached value for instant access (configuration)
        public int MonthDays
        {
            get
            {
                if (_func == null) return _MonthDays_INVALID;
                return (_online ? _monthDays : _MonthDays_INVALID);
            }
            set
            {
                setprop_monthDays(value);
            }
        }

        // private helper for magic property
        private void setprop_monthDays(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _MonthDays_INVALID) return;
            if (newval == _monthDays) return;
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
            if (_func == null)
            {
                string msg = "No WakeUpSchedule connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_months();
            if (res == YAPI.INVALID_INT) res = _Months_INVALID;
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
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_months(int newval)
        {
            if (_func == null)
            {
                string msg = "No WakeUpSchedule connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Months_INVALID) return YAPI.SUCCESS;
            return _func.set_months(newval);
        }


        // property with cached value for instant access (configuration)
        public int Months
        {
            get
            {
                if (_func == null) return _Months_INVALID;
                return (_online ? _months : _Months_INVALID);
            }
            set
            {
                setprop_months(value);
            }
        }

        // private helper for magic property
        private void setprop_months(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Months_INVALID) return;
            if (newval == _months) return;
            _func.set_months(newval);
            _months = newval;
        }

        // property with cached value for instant access (advertised value)
        public long NextOccurence
        {
            get
            {
                if (_func == null) return _NextOccurence_INVALID;
                return (_online ? _nextOccurence : _NextOccurence_INVALID);
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            Int64.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture,out _nextOccurence);
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
            if (_func == null)
            {
                string msg = "No WakeUpSchedule connected";
                throw new YoctoApiProxyException(msg);
            }
            long res = _func.get_nextOccurence();
            if (res == YAPI.INVALID_INT) res = _NextOccurence_INVALID;
            return res;
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
            if (_func == null)
            {
                string msg = "No WakeUpSchedule connected";
                throw new YoctoApiProxyException(msg);
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
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_minutes(long bitmap)
        {
            if (_func == null)
            {
                string msg = "No WakeUpSchedule connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.set_minutes(bitmap);
        }
    }
    //--- (end of YWakeUpSchedule implementation)
}

