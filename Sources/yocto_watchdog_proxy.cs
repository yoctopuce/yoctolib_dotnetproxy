/*********************************************************************
 *
 *  $Id: yocto_watchdog_proxy.cs 38514 2019-11-26 16:54:39Z seb $
 *
 *  Implements YWatchdogProxy, the Proxy API for Watchdog
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
    //--- (YWatchdog class start)
    static public partial class YoctoProxyManager
    {
        public static YWatchdogProxy FindWatchdog(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YWatchdog func = null;
            YWatchdogProxy res = (YWatchdogProxy)YFunctionProxy.FindSimilarUnknownFunction("YWatchdogProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YWatchdogProxy)YFunctionProxy.FindSimilarKnownFunction("YWatchdogProxy");
                if (res != null) return res;
                func = YWatchdog.FirstWatchdog();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YWatchdogProxy)func.get_userData();
                    }
                }
            } else {
                func = YWatchdog.FindWatchdog(name);
                if (func.get_userData() != null) {
                    return (YWatchdogProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YWatchdogProxy(func, name);
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
 *   The YWatchdog class allows you to drive a Yoctopuce watchdog, for instance using a Yocto-WatchdogDC.
 * <para>
 *   A watchdog works like a relay, with an extra timer that can automatically
 *   trigger a brief power cycle to an appliance after a preset delay, to force this
 *   appliance to reset if a problem occurs. During normal use, the watchdog timer
 *   is reset periodically by the application to prevent the automated power cycle.
 *   Whenever the application dies, the watchdog will automatically trigger the power cycle.
 *   The watchdog can also be driven directly with <i>pulse</i> and <i>delayedPulse</i>
 *   methods to switch off an appliance for a given duration.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YWatchdogProxy : YFunctionProxy
    {
        //--- (end of YWatchdog class start)
        //--- (YWatchdog definitions)
        public const int _State_INVALID = 0;
        public const int _State_A = 1;
        public const int _State_B = 2;
        public const int _StateAtPowerOn_INVALID = 0;
        public const int _StateAtPowerOn_UNCHANGED = 1;
        public const int _StateAtPowerOn_A = 2;
        public const int _StateAtPowerOn_B = 3;
        public const long _MaxTimeOnStateA_INVALID = YAPI.INVALID_LONG;
        public const long _MaxTimeOnStateB_INVALID = YAPI.INVALID_LONG;
        public const int _Output_INVALID = 0;
        public const int _Output_OFF = 1;
        public const int _Output_ON = 2;
        public const long _PulseTimer_INVALID = YAPI.INVALID_LONG;
        public const long _Countdown_INVALID = YAPI.INVALID_LONG;
        public const int _AutoStart_INVALID = 0;
        public const int _AutoStart_OFF = 1;
        public const int _AutoStart_ON = 2;
        public const int _Running_INVALID = 0;
        public const int _Running_OFF = 1;
        public const int _Running_ON = 2;
        public const long _TriggerDelay_INVALID = YAPI.INVALID_LONG;
        public const long _TriggerDuration_INVALID = YAPI.INVALID_LONG;

        // reference to real YoctoAPI object
        protected new YWatchdog _func;
        // property cache
        protected int _state = _State_INVALID;
        protected int _stateAtPowerOn = _StateAtPowerOn_INVALID;
        protected long _maxTimeOnStateA = _MaxTimeOnStateA_INVALID;
        protected long _maxTimeOnStateB = _MaxTimeOnStateB_INVALID;
        protected int _autoStart = _AutoStart_INVALID;
        protected int _running = _Running_INVALID;
        protected long _triggerDelay = _TriggerDelay_INVALID;
        protected long _triggerDuration = _TriggerDuration_INVALID;
        //--- (end of YWatchdog definitions)

        //--- (YWatchdog implementation)
        internal YWatchdogProxy(YWatchdog hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Watchdog " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YWatchdog) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YWatchdog hwd = YWatchdog.FindWatchdog(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YWatchdog hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Watchdog callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YWatchdog it = YWatchdog.FirstWatchdog();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextWatchdog();
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
            _stateAtPowerOn = _func.get_stateAtPowerOn()+1;
            _maxTimeOnStateA = _func.get_maxTimeOnStateA();
            _maxTimeOnStateB = _func.get_maxTimeOnStateB();
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _autoStart = _func.get_autoStart()+1;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _running = _func.get_running()+1;
            _triggerDelay = _func.get_triggerDelay();
            _triggerDuration = _func.get_triggerDuration();
        }

        // property with cached value for instant access (advertised value)
        public int State
        {
            get
            {
                if (_func == null) return _State_INVALID;
                return (_online ? _state : _State_INVALID);
            }
            set
            {
                setprop_state(value);
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            if (value == "A") _state = 1;
            if (value == "B") _state = 2;
        }

        /**
         * <summary>
         *   Returns the state of the watchdog (A for the idle position, B for the active position).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YWatchdog.STATE_A</c> or <c>YWatchdog.STATE_B</c>, according to the state of the watchdog
         *   (A for the idle position, B for the active position)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWatchdog.STATE_INVALID</c>.
         * </para>
         */
        public int get_state()
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_state()+1;
        }

        /**
         * <summary>
         *   Changes the state of the watchdog (A for the idle position, B for the active position).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YWatchdog.STATE_A</c> or <c>YWatchdog.STATE_B</c>, according to the state of the watchdog
         *   (A for the idle position, B for the active position)
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
        public int set_state(int newval)
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _State_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_state(newval-1);
        }


        // private helper for magic property
        private void setprop_state(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _State_INVALID) return;
            if (newval == _state) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_state(newval-1);
            _state = newval;
        }

        /**
         * <summary>
         *   Returns the state of the watchdog at device startup (A for the idle position, B for the active position, UNCHANGED for no change).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YWatchdog.STATEATPOWERON_UNCHANGED</c>, <c>YWatchdog.STATEATPOWERON_A</c> and
         *   <c>YWatchdog.STATEATPOWERON_B</c> corresponding to the state of the watchdog at device startup (A
         *   for the idle position, B for the active position, UNCHANGED for no change)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWatchdog.STATEATPOWERON_INVALID</c>.
         * </para>
         */
        public int get_stateAtPowerOn()
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_stateAtPowerOn()+1;
        }

        /**
         * <summary>
         *   Changes the state of the watchdog at device startup (A for the idle position,
         *   B for the active position, UNCHANGED for no modification).
         * <para>
         *   Remember to call the matching module <c>saveToFlash()</c>
         *   method, otherwise this call will have no effect.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YWatchdog.STATEATPOWERON_UNCHANGED</c>, <c>YWatchdog.STATEATPOWERON_A</c> and
         *   <c>YWatchdog.STATEATPOWERON_B</c> corresponding to the state of the watchdog at device startup (A
         *   for the idle position,
         *   B for the active position, UNCHANGED for no modification)
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
        public int set_stateAtPowerOn(int newval)
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _StateAtPowerOn_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_stateAtPowerOn(newval-1);
        }


        // property with cached value for instant access (configuration)
        public int StateAtPowerOn
        {
            get
            {
                if (_func == null) return _StateAtPowerOn_INVALID;
                return (_online ? _stateAtPowerOn : _StateAtPowerOn_INVALID);
            }
            set
            {
                setprop_stateAtPowerOn(value);
            }
        }

        // private helper for magic property
        private void setprop_stateAtPowerOn(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _StateAtPowerOn_INVALID) return;
            if (newval == _stateAtPowerOn) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_stateAtPowerOn(newval-1);
            _stateAtPowerOn = newval;
        }

        /**
         * <summary>
         *   Returns the maximum time (ms) allowed for $THEFUNCTIONS$ to stay in state
         *   A before automatically switching back in to B state.
         * <para>
         *   Zero means no time limit.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the maximum time (ms) allowed for $THEFUNCTIONS$ to stay in state
         *   A before automatically switching back in to B state
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWatchdog.MAXTIMEONSTATEA_INVALID</c>.
         * </para>
         */
        public long get_maxTimeOnStateA()
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_maxTimeOnStateA();
        }

        /**
         * <summary>
         *   Changes the maximum time (ms) allowed for $THEFUNCTIONS$ to stay in state A
         *   before automatically switching back in to B state.
         * <para>
         *   Use zero for no time limit.
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the maximum time (ms) allowed for $THEFUNCTIONS$ to stay in state A
         *   before automatically switching back in to B state
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
        public int set_maxTimeOnStateA(long newval)
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _MaxTimeOnStateA_INVALID) return YAPI.SUCCESS;
            return _func.set_maxTimeOnStateA(newval);
        }


        // property with cached value for instant access (configuration)
        public long MaxTimeOnStateA
        {
            get
            {
                if (_func == null) return _MaxTimeOnStateA_INVALID;
                return (_online ? _maxTimeOnStateA : _MaxTimeOnStateA_INVALID);
            }
            set
            {
                setprop_maxTimeOnStateA(value);
            }
        }

        // private helper for magic property
        private void setprop_maxTimeOnStateA(long newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _MaxTimeOnStateA_INVALID) return;
            if (newval == _maxTimeOnStateA) return;
            _func.set_maxTimeOnStateA(newval);
            _maxTimeOnStateA = newval;
        }

        /**
         * <summary>
         *   Retourne the maximum time (ms) allowed for $THEFUNCTIONS$ to stay in state B
         *   before automatically switching back in to A state.
         * <para>
         *   Zero means no time limit.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWatchdog.MAXTIMEONSTATEB_INVALID</c>.
         * </para>
         */
        public long get_maxTimeOnStateB()
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_maxTimeOnStateB();
        }

        /**
         * <summary>
         *   Changes the maximum time (ms) allowed for $THEFUNCTIONS$ to stay in state B before
         *   automatically switching back in to A state.
         * <para>
         *   Use zero for no time limit.
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the maximum time (ms) allowed for $THEFUNCTIONS$ to stay in state B before
         *   automatically switching back in to A state
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
        public int set_maxTimeOnStateB(long newval)
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _MaxTimeOnStateB_INVALID) return YAPI.SUCCESS;
            return _func.set_maxTimeOnStateB(newval);
        }


        // property with cached value for instant access (configuration)
        public long MaxTimeOnStateB
        {
            get
            {
                if (_func == null) return _MaxTimeOnStateB_INVALID;
                return (_online ? _maxTimeOnStateB : _MaxTimeOnStateB_INVALID);
            }
            set
            {
                setprop_maxTimeOnStateB(value);
            }
        }

        // private helper for magic property
        private void setprop_maxTimeOnStateB(long newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _MaxTimeOnStateB_INVALID) return;
            if (newval == _maxTimeOnStateB) return;
            _func.set_maxTimeOnStateB(newval);
            _maxTimeOnStateB = newval;
        }

        /**
         * <summary>
         *   Returns the output state of the watchdog, when used as a simple switch (single throw).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YWatchdog.OUTPUT_OFF</c> or <c>YWatchdog.OUTPUT_ON</c>, according to the output state of
         *   the watchdog, when used as a simple switch (single throw)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWatchdog.OUTPUT_INVALID</c>.
         * </para>
         */
        public int get_output()
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_output()+1;
        }

        /**
         * <summary>
         *   Changes the output state of the watchdog, when used as a simple switch (single throw).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YWatchdog.OUTPUT_OFF</c> or <c>YWatchdog.OUTPUT_ON</c>, according to the output state of
         *   the watchdog, when used as a simple switch (single throw)
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
        public int set_output(int newval)
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Output_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_output(newval-1);
        }


        /**
         * <summary>
         *   Returns the number of milliseconds remaining before the watchdog is returned to idle position
         *   (state A), during a measured pulse generation.
         * <para>
         *   When there is no ongoing pulse, returns zero.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of milliseconds remaining before the watchdog is returned to
         *   idle position
         *   (state A), during a measured pulse generation
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWatchdog.PULSETIMER_INVALID</c>.
         * </para>
         */
        public long get_pulseTimer()
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_pulseTimer();
        }

        /**
         * <summary>
         *   Sets the relay to output B (active) for a specified duration, then brings it
         *   automatically back to output A (idle state).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="ms_duration">
         *   pulse duration, in milliseconds
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
        public int pulse(int ms_duration)
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.pulse(ms_duration);
        }

        /**
         * <summary>
         *   Schedules a pulse.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="ms_delay">
         *   waiting time before the pulse, in milliseconds
         * </param>
         * <param name="ms_duration">
         *   pulse duration, in milliseconds
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
        public int delayedPulse(int ms_delay,int ms_duration)
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.delayedPulse(ms_delay, ms_duration);
        }

        /**
         * <summary>
         *   Returns the number of milliseconds remaining before a pulse (delayedPulse() call)
         *   When there is no scheduled pulse, returns zero.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of milliseconds remaining before a pulse (delayedPulse() call)
         *   When there is no scheduled pulse, returns zero
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWatchdog.COUNTDOWN_INVALID</c>.
         * </para>
         */
        public long get_countdown()
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_countdown();
        }

        /**
         * <summary>
         *   Returns the watchdog running state at module power on.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YWatchdog.AUTOSTART_OFF</c> or <c>YWatchdog.AUTOSTART_ON</c>, according to the watchdog
         *   running state at module power on
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWatchdog.AUTOSTART_INVALID</c>.
         * </para>
         */
        public int get_autoStart()
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_autoStart()+1;
        }

        /**
         * <summary>
         *   Changes the watchdog running state at module power on.
         * <para>
         *   Remember to call the
         *   <c>saveToFlash()</c> method and then to reboot the module to apply this setting.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YWatchdog.AUTOSTART_OFF</c> or <c>YWatchdog.AUTOSTART_ON</c>, according to the watchdog
         *   running state at module power on
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
        public int set_autoStart(int newval)
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _AutoStart_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_autoStart(newval-1);
        }


        // property with cached value for instant access (configuration)
        public int AutoStart
        {
            get
            {
                if (_func == null) return _AutoStart_INVALID;
                return (_online ? _autoStart : _AutoStart_INVALID);
            }
            set
            {
                setprop_autoStart(value);
            }
        }

        // private helper for magic property
        private void setprop_autoStart(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _AutoStart_INVALID) return;
            if (newval == _autoStart) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_autoStart(newval-1);
            _autoStart = newval;
        }

        /**
         * <summary>
         *   Returns the watchdog running state.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YWatchdog.RUNNING_OFF</c> or <c>YWatchdog.RUNNING_ON</c>, according to the watchdog running state
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWatchdog.RUNNING_INVALID</c>.
         * </para>
         */
        public int get_running()
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_running()+1;
        }

        /**
         * <summary>
         *   Changes the running state of the watchdog.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YWatchdog.RUNNING_OFF</c> or <c>YWatchdog.RUNNING_ON</c>, according to the running state
         *   of the watchdog
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
        public int set_running(int newval)
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Running_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_running(newval-1);
        }


        // property with cached value for instant access (configuration)
        public int Running
        {
            get
            {
                if (_func == null) return _Running_INVALID;
                return (_online ? _running : _Running_INVALID);
            }
            set
            {
                setprop_running(value);
            }
        }

        // private helper for magic property
        private void setprop_running(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Running_INVALID) return;
            // Always call set_running(), in order to reset watchdog
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_running(newval-1);
            _running = newval;
        }

        /**
         * <summary>
         *   Resets the watchdog.
         * <para>
         *   When the watchdog is running, this function
         *   must be called on a regular basis to prevent the watchdog to
         *   trigger
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int resetWatchdog()
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.resetWatchdog();
        }

        /**
         * <summary>
         *   Returns  the waiting duration before a reset is automatically triggered by the watchdog, in milliseconds.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to  the waiting duration before a reset is automatically triggered by the
         *   watchdog, in milliseconds
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWatchdog.TRIGGERDELAY_INVALID</c>.
         * </para>
         */
        public long get_triggerDelay()
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_triggerDelay();
        }

        /**
         * <summary>
         *   Changes the waiting delay before a reset is triggered by the watchdog,
         *   in milliseconds.
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the waiting delay before a reset is triggered by the watchdog,
         *   in milliseconds
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
        public int set_triggerDelay(long newval)
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _TriggerDelay_INVALID) return YAPI.SUCCESS;
            return _func.set_triggerDelay(newval);
        }


        // property with cached value for instant access (configuration)
        public long TriggerDelay
        {
            get
            {
                if (_func == null) return _TriggerDelay_INVALID;
                return (_online ? _triggerDelay : _TriggerDelay_INVALID);
            }
            set
            {
                setprop_triggerDelay(value);
            }
        }

        // private helper for magic property
        private void setprop_triggerDelay(long newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _TriggerDelay_INVALID) return;
            if (newval == _triggerDelay) return;
            _func.set_triggerDelay(newval);
            _triggerDelay = newval;
        }

        /**
         * <summary>
         *   Returns the duration of resets caused by the watchdog, in milliseconds.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the duration of resets caused by the watchdog, in milliseconds
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWatchdog.TRIGGERDURATION_INVALID</c>.
         * </para>
         */
        public long get_triggerDuration()
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_triggerDuration();
        }

        /**
         * <summary>
         *   Changes the duration of resets caused by the watchdog, in milliseconds.
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the duration of resets caused by the watchdog, in milliseconds
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
        public int set_triggerDuration(long newval)
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _TriggerDuration_INVALID) return YAPI.SUCCESS;
            return _func.set_triggerDuration(newval);
        }


        // property with cached value for instant access (configuration)
        public long TriggerDuration
        {
            get
            {
                if (_func == null) return _TriggerDuration_INVALID;
                return (_online ? _triggerDuration : _TriggerDuration_INVALID);
            }
            set
            {
                setprop_triggerDuration(value);
            }
        }

        // private helper for magic property
        private void setprop_triggerDuration(long newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _TriggerDuration_INVALID) return;
            if (newval == _triggerDuration) return;
            _func.set_triggerDuration(newval);
            _triggerDuration = newval;
        }

        /**
         * <summary>
         *   Switch the relay to the opposite state.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int toggle()
        {
            if (_func == null)
            {
                string msg = "No Watchdog connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.toggle();
        }
    }
    //--- (end of YWatchdog implementation)
}

