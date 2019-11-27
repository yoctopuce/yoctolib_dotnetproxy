/*********************************************************************
 *
 *  $Id: yocto_wakeupmonitor_proxy.cs 38514 2019-11-26 16:54:39Z seb $
 *
 *  Implements YWakeUpMonitorProxy, the Proxy API for WakeUpMonitor
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
    //--- (YWakeUpMonitor class start)
    static public partial class YoctoProxyManager
    {
        public static YWakeUpMonitorProxy FindWakeUpMonitor(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YWakeUpMonitor func = null;
            YWakeUpMonitorProxy res = (YWakeUpMonitorProxy)YFunctionProxy.FindSimilarUnknownFunction("YWakeUpMonitorProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YWakeUpMonitorProxy)YFunctionProxy.FindSimilarKnownFunction("YWakeUpMonitorProxy");
                if (res != null) return res;
                func = YWakeUpMonitor.FirstWakeUpMonitor();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YWakeUpMonitorProxy)func.get_userData();
                    }
                }
            } else {
                func = YWakeUpMonitor.FindWakeUpMonitor(name);
                if (func.get_userData() != null) {
                    return (YWakeUpMonitorProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YWakeUpMonitorProxy(func, name);
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
 *   The YWakeUpMonitor class handles globally all wake-up sources, as well
 *   as automated sleep mode, for instance using a YoctoHub-GSM-3G-EU, a YoctoHub-GSM-3G-NA, a YoctoHub-Wireless-SR or a YoctoHub-Wireless-g.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YWakeUpMonitorProxy : YFunctionProxy
    {
        //--- (end of YWakeUpMonitor class start)
        //--- (YWakeUpMonitor definitions)
        public const int _PowerDuration_INVALID = -1;
        public const int _SleepCountdown_INVALID = -1;
        public const long _NextWakeUp_INVALID = YAPI.INVALID_LONG;
        public const int _WakeUpReason_INVALID = 0;
        public const int _WakeUpReason_USBPOWER = 1;
        public const int _WakeUpReason_EXTPOWER = 2;
        public const int _WakeUpReason_ENDOFSLEEP = 3;
        public const int _WakeUpReason_EXTSIG1 = 4;
        public const int _WakeUpReason_SCHEDULE1 = 5;
        public const int _WakeUpReason_SCHEDULE2 = 6;
        public const int _WakeUpState_INVALID = 0;
        public const int _WakeUpState_SLEEPING = 1;
        public const int _WakeUpState_AWAKE = 2;
        public const long _RtcTime_INVALID = YAPI.INVALID_LONG;

        // reference to real YoctoAPI object
        protected new YWakeUpMonitor _func;
        // property cache
        protected int _powerDuration = _PowerDuration_INVALID;
        protected long _nextWakeUp = _NextWakeUp_INVALID;
        //--- (end of YWakeUpMonitor definitions)

        //--- (YWakeUpMonitor implementation)
        internal YWakeUpMonitorProxy(YWakeUpMonitor hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("WakeUpMonitor " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YWakeUpMonitor) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YWakeUpMonitor hwd = YWakeUpMonitor.FindWakeUpMonitor(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YWakeUpMonitor hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering WakeUpMonitor callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YWakeUpMonitor it = YWakeUpMonitor.FirstWakeUpMonitor();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextWakeUpMonitor();
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
            _powerDuration = _func.get_powerDuration();
        }

        /**
         * <summary>
         *   Returns the maximal wake up time (in seconds) before automatically going to sleep.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the maximal wake up time (in seconds) before automatically going to sleep
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWakeUpMonitor.POWERDURATION_INVALID</c>.
         * </para>
         */
        public int get_powerDuration()
        {
            if (_func == null)
            {
                string msg = "No WakeUpMonitor connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_powerDuration();
            if (res == YAPI.INVALID_INT) res = _PowerDuration_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the maximal wake up time (seconds) before automatically going to sleep.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the maximal wake up time (seconds) before automatically going to sleep
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
        public int set_powerDuration(int newval)
        {
            if (_func == null)
            {
                string msg = "No WakeUpMonitor connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _PowerDuration_INVALID) return YAPI.SUCCESS;
            return _func.set_powerDuration(newval);
        }


        // property with cached value for instant access (configuration)
        public int PowerDuration
        {
            get
            {
                if (_func == null) return _PowerDuration_INVALID;
                return (_online ? _powerDuration : _PowerDuration_INVALID);
            }
            set
            {
                setprop_powerDuration(value);
            }
        }

        // private helper for magic property
        private void setprop_powerDuration(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _PowerDuration_INVALID) return;
            if (newval == _powerDuration) return;
            _func.set_powerDuration(newval);
            _powerDuration = newval;
        }

        /**
         * <summary>
         *   Returns the delay before the  next sleep period.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the delay before the  next sleep period
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWakeUpMonitor.SLEEPCOUNTDOWN_INVALID</c>.
         * </para>
         */
        public int get_sleepCountdown()
        {
            if (_func == null)
            {
                string msg = "No WakeUpMonitor connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_sleepCountdown();
            if (res == YAPI.INVALID_INT) res = _SleepCountdown_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the delay before the next sleep period.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the delay before the next sleep period
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
        public int set_sleepCountdown(int newval)
        {
            if (_func == null)
            {
                string msg = "No WakeUpMonitor connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _SleepCountdown_INVALID) return YAPI.SUCCESS;
            return _func.set_sleepCountdown(newval);
        }


        // property with cached value for instant access (advertised value)
        public long NextWakeUp
        {
            get
            {
                if (_func == null) return _NextWakeUp_INVALID;
                return (_online ? _nextWakeUp : _NextWakeUp_INVALID);
            }
            set
            {
                setprop_nextWakeUp(value);
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            Int64.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture,out _nextWakeUp);
        }

        /**
         * <summary>
         *   Returns the next scheduled wake up date/time (UNIX format).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the next scheduled wake up date/time (UNIX format)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWakeUpMonitor.NEXTWAKEUP_INVALID</c>.
         * </para>
         */
        public long get_nextWakeUp()
        {
            if (_func == null)
            {
                string msg = "No WakeUpMonitor connected";
                throw new YoctoApiProxyException(msg);
            }
            long res = _func.get_nextWakeUp();
            if (res == YAPI.INVALID_INT) res = _NextWakeUp_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the days of the week when a wake up must take place.
         * <para>
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
        public int set_nextWakeUp(long newval)
        {
            if (_func == null)
            {
                string msg = "No WakeUpMonitor connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _NextWakeUp_INVALID) return YAPI.SUCCESS;
            return _func.set_nextWakeUp(newval);
        }


        // private helper for magic property
        private void setprop_nextWakeUp(long newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _NextWakeUp_INVALID) return;
            if (newval == _nextWakeUp) return;
            _func.set_nextWakeUp(newval);
            _nextWakeUp = newval;
        }

        /**
         * <summary>
         *   Returns the latest wake up reason.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YWakeUpMonitor.WAKEUPREASON_USBPOWER</c>, <c>YWakeUpMonitor.WAKEUPREASON_EXTPOWER</c>,
         *   <c>YWakeUpMonitor.WAKEUPREASON_ENDOFSLEEP</c>, <c>YWakeUpMonitor.WAKEUPREASON_EXTSIG1</c>,
         *   <c>YWakeUpMonitor.WAKEUPREASON_SCHEDULE1</c> and <c>YWakeUpMonitor.WAKEUPREASON_SCHEDULE2</c>
         *   corresponding to the latest wake up reason
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWakeUpMonitor.WAKEUPREASON_INVALID</c>.
         * </para>
         */
        public int get_wakeUpReason()
        {
            if (_func == null)
            {
                string msg = "No WakeUpMonitor connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_wakeUpReason()+1;
        }

        /**
         * <summary>
         *   Returns  the current state of the monitor.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YWakeUpMonitor.WAKEUPSTATE_SLEEPING</c> or <c>YWakeUpMonitor.WAKEUPSTATE_AWAKE</c>,
         *   according to  the current state of the monitor
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWakeUpMonitor.WAKEUPSTATE_INVALID</c>.
         * </para>
         */
        public int get_wakeUpState()
        {
            if (_func == null)
            {
                string msg = "No WakeUpMonitor connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_wakeUpState()+1;
        }

        /**
         * <summary>
         *   Forces a wake up.
         * <para>
         * </para>
         * </summary>
         */
        public virtual int wakeUp()
        {
            if (_func == null)
            {
                string msg = "No WakeUpMonitor connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.wakeUp();
        }

        /**
         * <summary>
         *   Goes to sleep until the next wake up condition is met,  the
         *   RTC time must have been set before calling this function.
         * <para>
         * </para>
         * </summary>
         * <param name="secBeforeSleep">
         *   number of seconds before going into sleep mode,
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int sleep(int secBeforeSleep)
        {
            if (_func == null)
            {
                string msg = "No WakeUpMonitor connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.sleep(secBeforeSleep);
        }

        /**
         * <summary>
         *   Goes to sleep for a specific duration or until the next wake up condition is met, the
         *   RTC time must have been set before calling this function.
         * <para>
         *   The count down before sleep
         *   can be canceled with resetSleepCountDown.
         * </para>
         * </summary>
         * <param name="secUntilWakeUp">
         *   number of seconds before next wake up
         * </param>
         * <param name="secBeforeSleep">
         *   number of seconds before going into sleep mode
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int sleepFor(int secUntilWakeUp, int secBeforeSleep)
        {
            if (_func == null)
            {
                string msg = "No WakeUpMonitor connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.sleepFor(secUntilWakeUp, secBeforeSleep);
        }

        /**
         * <summary>
         *   Go to sleep until a specific date is reached or until the next wake up condition is met, the
         *   RTC time must have been set before calling this function.
         * <para>
         *   The count down before sleep
         *   can be canceled with resetSleepCountDown.
         * </para>
         * </summary>
         * <param name="wakeUpTime">
         *   wake-up datetime (UNIX format)
         * </param>
         * <param name="secBeforeSleep">
         *   number of seconds before going into sleep mode
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int sleepUntil(int wakeUpTime, int secBeforeSleep)
        {
            if (_func == null)
            {
                string msg = "No WakeUpMonitor connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.sleepUntil(wakeUpTime, secBeforeSleep);
        }

        /**
         * <summary>
         *   Resets the sleep countdown.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int resetSleepCountDown()
        {
            if (_func == null)
            {
                string msg = "No WakeUpMonitor connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.resetSleepCountDown();
        }
    }
    //--- (end of YWakeUpMonitor implementation)
}

