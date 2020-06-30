/*********************************************************************
 *
 *  $Id: yocto_relay_proxy.cs 41109 2020-06-29 12:40:42Z seb $
 *
 *  Implements YRelayProxy, the Proxy API for Relay
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
    //--- (YRelay class start)
    static public partial class YoctoProxyManager
    {
        public static YRelayProxy FindRelay(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YRelay func = null;
            YRelayProxy res = (YRelayProxy)YFunctionProxy.FindSimilarUnknownFunction("YRelayProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YRelayProxy)YFunctionProxy.FindSimilarKnownFunction("YRelayProxy");
                if (res != null) return res;
                func = YRelay.FirstRelay();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YRelayProxy)func.get_userData();
                    }
                }
            } else {
                func = YRelay.FindRelay(name);
                if (func.get_userData() != null) {
                    return (YRelayProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YRelayProxy(func, name);
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
 *   The <c>YRelay</c> class allows you to drive a Yoctopuce relay or optocoupled output.
 * <para>
 *   It can be used to simply switch the output on or off, but also to automatically generate short
 *   pulses of determined duration.
 *   On devices with two output for each relay (double throw), the two outputs are named A and B,
 *   with output A corresponding to the idle position (normally closed) and the output B corresponding to the
 *   active state (normally open).
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YRelayProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a relay for a given identifier.
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
         *   This function does not require that the relay is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YRelay.isOnline()</c> to test if the relay is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a relay by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the relay, for instance
         *   <c>YLTCHRL1.relay1</c>.
         * </param>
         * <returns>
         *   a <c>YRelay</c> object allowing you to drive the relay.
         * </returns>
         */
        public static YRelayProxy FindRelay(string func)
        {
            return YoctoProxyManager.FindRelay(func);
        }
        //--- (end of YRelay class start)
        //--- (YRelay definitions)
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

        // reference to real YoctoAPI object
        protected new YRelay _func;
        // property cache
        protected int _state = _State_INVALID;
        protected int _stateAtPowerOn = _StateAtPowerOn_INVALID;
        protected long _maxTimeOnStateA = _MaxTimeOnStateA_INVALID;
        protected long _maxTimeOnStateB = _MaxTimeOnStateB_INVALID;
        //--- (end of YRelay definitions)

        //--- (YRelay implementation)
        internal YRelayProxy(YRelay hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Relay " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YRelay) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YRelay hwd = YRelay.FindRelay(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YRelay hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Relay callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type Relay available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YRelay.FindRelay</c> to obtain an object that can control the
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
            YRelay it = YRelay.FirstRelay();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextRelay();
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
            _stateAtPowerOn = _func.get_stateAtPowerOn()+1;
            _maxTimeOnStateA = _func.get_maxTimeOnStateA();
            _maxTimeOnStateB = _func.get_maxTimeOnStateB();
        }

        /**
         * <summary>
         *   Returns the state of the relays (A for the idle position, B for the active position).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>relay._State_A</c> or <c>relay._State_B</c>, according to the state of the relays (A for
         *   the idle position, B for the active position)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>relay._State_INVALID</c>.
         * </para>
         */
        public int get_state()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Relay connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_state()+1;
        }

        /**
         * <summary>
         *   Changes the state of the relays (A for the idle position, B for the active position).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>relay._State_A</c> or <c>relay._State_B</c>, according to the state of the relays (A for
         *   the idle position, B for the active position)
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
            if (_func == null) {
                throw new YoctoApiProxyException("No Relay connected");
            }
            if (newval == _State_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_state(newval-1);
        }

        // property with cached value for instant access (advertised value)
        /// <value>State of the relays (A for the idle position, B for the active position).</value>
        public int State
        {
            get
            {
                if (_func == null) {
                    return _State_INVALID;
                }
                if (_online) {
                    return _state;
                }
                return _State_INVALID;
            }
            set
            {
                setprop_state(value);
            }
        }

        // private helper for magic property
        private void setprop_state(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _State_INVALID) {
                return;
            }
            if (newval == _state) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_state(newval-1);
            _state = newval;
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            if (value == "A") {
                _state = 1;
            }
            if (value == "B") {
                _state = 2;
            }
        }

        /**
         * <summary>
         *   Returns the state of the relays at device startup (A for the idle position,
         *   B for the active position, UNCHANGED to leave the relay state as is).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>relay._Stateatpoweron_UNCHANGED</c>, <c>relay._Stateatpoweron_A</c> and
         *   <c>relay._Stateatpoweron_B</c> corresponding to the state of the relays at device startup (A for
         *   the idle position,
         *   B for the active position, UNCHANGED to leave the relay state as is)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>relay._Stateatpoweron_INVALID</c>.
         * </para>
         */
        public int get_stateAtPowerOn()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Relay connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_stateAtPowerOn()+1;
        }

        /**
         * <summary>
         *   Changes the state of the relays at device startup (A for the idle position,
         *   B for the active position, UNCHANGED to leave the relay state as is).
         * <para>
         *   Remember to call the matching module <c>saveToFlash()</c>
         *   method, otherwise this call will have no effect.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>relay._Stateatpoweron_UNCHANGED</c>, <c>relay._Stateatpoweron_A</c> and
         *   <c>relay._Stateatpoweron_B</c> corresponding to the state of the relays at device startup (A for
         *   the idle position,
         *   B for the active position, UNCHANGED to leave the relay state as is)
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
            if (_func == null) {
                throw new YoctoApiProxyException("No Relay connected");
            }
            if (newval == _StateAtPowerOn_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_stateAtPowerOn(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>State of the relays at device startup (A for the idle position,</value>
        public int StateAtPowerOn
        {
            get
            {
                if (_func == null) {
                    return _StateAtPowerOn_INVALID;
                }
                if (_online) {
                    return _stateAtPowerOn;
                }
                return _StateAtPowerOn_INVALID;
            }
            set
            {
                setprop_stateAtPowerOn(value);
            }
        }

        // private helper for magic property
        private void setprop_stateAtPowerOn(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _StateAtPowerOn_INVALID) {
                return;
            }
            if (newval == _stateAtPowerOn) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_stateAtPowerOn(newval-1);
            _stateAtPowerOn = newval;
        }

        /**
         * <summary>
         *   Returns the maximum time (ms) allowed for the relay to stay in state
         *   A before automatically switching back in to B state.
         * <para>
         *   Zero means no time limit.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the maximum time (ms) allowed for the relay to stay in state
         *   A before automatically switching back in to B state
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>relay._Maxtimeonstatea_INVALID</c>.
         * </para>
         */
        public long get_maxTimeOnStateA()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Relay connected");
            }
            return _func.get_maxTimeOnStateA();
        }

        /**
         * <summary>
         *   Changes the maximum time (ms) allowed for the relay to stay in state A
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
         *   an integer corresponding to the maximum time (ms) allowed for the relay to stay in state A
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
            if (_func == null) {
                throw new YoctoApiProxyException("No Relay connected");
            }
            if (newval == _MaxTimeOnStateA_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_maxTimeOnStateA(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Maximum time (ms) allowed for the relay to stay in state</value>
        public long MaxTimeOnStateA
        {
            get
            {
                if (_func == null) {
                    return _MaxTimeOnStateA_INVALID;
                }
                if (_online) {
                    return _maxTimeOnStateA;
                }
                return _MaxTimeOnStateA_INVALID;
            }
            set
            {
                setprop_maxTimeOnStateA(value);
            }
        }

        // private helper for magic property
        private void setprop_maxTimeOnStateA(long newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _MaxTimeOnStateA_INVALID) {
                return;
            }
            if (newval == _maxTimeOnStateA) {
                return;
            }
            _func.set_maxTimeOnStateA(newval);
            _maxTimeOnStateA = newval;
        }

        /**
         * <summary>
         *   Retourne the maximum time (ms) allowed for the relay to stay in state B
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
         *   On failure, throws an exception or returns <c>relay._Maxtimeonstateb_INVALID</c>.
         * </para>
         */
        public long get_maxTimeOnStateB()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Relay connected");
            }
            return _func.get_maxTimeOnStateB();
        }

        /**
         * <summary>
         *   Changes the maximum time (ms) allowed for the relay to stay in state B before
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
         *   an integer corresponding to the maximum time (ms) allowed for the relay to stay in state B before
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
            if (_func == null) {
                throw new YoctoApiProxyException("No Relay connected");
            }
            if (newval == _MaxTimeOnStateB_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_maxTimeOnStateB(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>The maximum time (ms) allowed for the relay to stay in state B</value>
        public long MaxTimeOnStateB
        {
            get
            {
                if (_func == null) {
                    return _MaxTimeOnStateB_INVALID;
                }
                if (_online) {
                    return _maxTimeOnStateB;
                }
                return _MaxTimeOnStateB_INVALID;
            }
            set
            {
                setprop_maxTimeOnStateB(value);
            }
        }

        // private helper for magic property
        private void setprop_maxTimeOnStateB(long newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _MaxTimeOnStateB_INVALID) {
                return;
            }
            if (newval == _maxTimeOnStateB) {
                return;
            }
            _func.set_maxTimeOnStateB(newval);
            _maxTimeOnStateB = newval;
        }

        /**
         * <summary>
         *   Returns the output state of the relays, when used as a simple switch (single throw).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>relay._Output_OFF</c> or <c>relay._Output_ON</c>, according to the output state of the
         *   relays, when used as a simple switch (single throw)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>relay._Output_INVALID</c>.
         * </para>
         */
        public int get_output()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Relay connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_output()+1;
        }

        /**
         * <summary>
         *   Changes the output state of the relays, when used as a simple switch (single throw).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>relay._Output_OFF</c> or <c>relay._Output_ON</c>, according to the output state of the
         *   relays, when used as a simple switch (single throw)
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
            if (_func == null) {
                throw new YoctoApiProxyException("No Relay connected");
            }
            if (newval == _Output_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_output(newval-1);
        }

        /**
         * <summary>
         *   Returns the number of milliseconds remaining before the relays is returned to idle position
         *   (state A), during a measured pulse generation.
         * <para>
         *   When there is no ongoing pulse, returns zero.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of milliseconds remaining before the relays is returned to idle position
         *   (state A), during a measured pulse generation
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>relay._Pulsetimer_INVALID</c>.
         * </para>
         */
        public long get_pulseTimer()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Relay connected");
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
            if (_func == null) {
                throw new YoctoApiProxyException("No Relay connected");
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
            if (_func == null) {
                throw new YoctoApiProxyException("No Relay connected");
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
         *   On failure, throws an exception or returns <c>relay._Countdown_INVALID</c>.
         * </para>
         */
        public long get_countdown()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Relay connected");
            }
            return _func.get_countdown();
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
            if (_func == null) {
                throw new YoctoApiProxyException("No Relay connected");
            }
            return _func.toggle();
        }
    }
    //--- (end of YRelay implementation)
}

