/*********************************************************************
 *
 *  $Id: yocto_digitalio_proxy.cs 38913 2019-12-20 18:59:49Z mvuilleu $
 *
 *  Implements YDigitalIOProxy, the Proxy API for DigitalIO
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
    //--- (YDigitalIO class start)
    static public partial class YoctoProxyManager
    {
        public static YDigitalIOProxy FindDigitalIO(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YDigitalIO func = null;
            YDigitalIOProxy res = (YDigitalIOProxy)YFunctionProxy.FindSimilarUnknownFunction("YDigitalIOProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YDigitalIOProxy)YFunctionProxy.FindSimilarKnownFunction("YDigitalIOProxy");
                if (res != null) return res;
                func = YDigitalIO.FirstDigitalIO();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YDigitalIOProxy)func.get_userData();
                    }
                }
            } else {
                func = YDigitalIO.FindDigitalIO(name);
                if (func.get_userData() != null) {
                    return (YDigitalIOProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YDigitalIOProxy(func, name);
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
 *   The <c>YDigitalIO</c> class allows you drive a Yoctopuce digital input/output port.
 * <para>
 *   It can be used to setup the direction of each channel, to read the state of each channel
 *   and to switch the state of each channel configures as an output.
 *   You can work on all channels at once, or one by one. Most functions
 *   use a binary representation for channels where bit 0 matches channel #0 , bit 1 matches channel
 *   #1 and so on. If you are not familiar with numbers binary representation, you will find more
 *   information here: <c>https://en.wikipedia.org/wiki/Binary_number#Representation</c>. It is also possible
 *   to automatically generate short pulses of a determined duration. Electrical behavior
 *   of each I/O can be modified (open drain and reverse polarity).
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YDigitalIOProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a digital IO port for a given identifier.
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
         *   This function does not require that the digital IO port is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YDigitalIO.isOnline()</c> to test if the digital IO port is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a digital IO port by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the digital IO port, for instance
         *   <c>YMINIIO0.digitalIO</c>.
         * </param>
         * <returns>
         *   a <c>YDigitalIO</c> object allowing you to drive the digital IO port.
         * </returns>
         */
        public static YDigitalIOProxy FindDigitalIO(string func)
        {
            return YoctoProxyManager.FindDigitalIO(func);
        }
        //--- (end of YDigitalIO class start)
        //--- (YDigitalIO definitions)
        public const int _PortState_INVALID = -1;
        public const int _PortDirection_INVALID = -1;
        public const int _PortOpenDrain_INVALID = -1;
        public const int _PortPolarity_INVALID = -1;
        public const int _PortDiags_INVALID = -1;
        public const int _PortSize_INVALID = -1;
        public const int _OutputVoltage_INVALID = 0;
        public const int _OutputVoltage_USB_5V = 1;
        public const int _OutputVoltage_USB_3V = 2;
        public const int _OutputVoltage_EXT_V = 3;
        public const string _Command_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YDigitalIO _func;
        // property cache
        protected int _portState = _PortState_INVALID;
        protected int _portDirection = _PortDirection_INVALID;
        protected int _portOpenDrain = _PortOpenDrain_INVALID;
        protected int _portPolarity = _PortPolarity_INVALID;
        protected int _portSize = _PortSize_INVALID;
        protected int _outputVoltage = _OutputVoltage_INVALID;
        //--- (end of YDigitalIO definitions)

        //--- (YDigitalIO implementation)
        internal YDigitalIOProxy(YDigitalIO hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("DigitalIO " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YDigitalIO) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YDigitalIO hwd = YDigitalIO.FindDigitalIO(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YDigitalIO hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering DigitalIO callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type DigitalIO available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YDigitalIO.FindDigitalIO</c> to obtain an object that can control the
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
            YDigitalIO it = YDigitalIO.FirstDigitalIO();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextDigitalIO();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
            _portSize = _func.get_portSize();
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            _portDirection = _func.get_portDirection();
            _portOpenDrain = _func.get_portOpenDrain();
            _portPolarity = _func.get_portPolarity();
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _outputVoltage = _func.get_outputVoltage()+1;
        }

        // property with cached value for instant access (advertised value)
        public int PortState
        {
            get
            {
                if (_func == null) return _PortState_INVALID;
                return (_online ? _portState : _PortState_INVALID);
            }
            set
            {
                setprop_portState(value);
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            Int32.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture,out _portState);
        }

        /**
         * <summary>
         *   Returns the digital IO port state as an integer with each bit
         *   representing a channel.
         * <para>
         *   value 0 = <c>0b00000000</c> -> all channels are OFF
         *   value 1 = <c>0b00000001</c> -> channel #0 is ON
         *   value 2 = <c>0b00000010</c> -> channel #1 is ON
         *   value 3 = <c>0b00000011</c> -> channels #0 and #1 are ON
         *   value 4 = <c>0b00000100</c> -> channel #2 is ON
         *   and so on...
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the digital IO port state as an integer with each bit
         *   representing a channel
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>digitalio._Portstate_INVALID</c>.
         * </para>
         */
        public int get_portState()
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_portState();
            if (res == YAPI.INVALID_INT) res = _PortState_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the state of all digital IO port's channels at once: the parameter
         *   is an integer where each bit represents a channel, with bit 0 matching channel #0.
         * <para>
         *   To set all channels to  0 -> <c>0b00000000</c> -> parameter = 0
         *   To set channel #0 to 1 -> <c>0b00000001</c> -> parameter =  1
         *   To set channel #1 to  1 -> <c>0b00000010</c> -> parameter = 2
         *   To set channel #0 and #1 -> <c>0b00000011</c> -> parameter =  3
         *   To set channel #2 to 1 -> <c>0b00000100</c> -> parameter =  4
         *   an so on....
         *   Only channels configured as outputs will be affecter, according to the value
         *   configured using <c>set_portDirection</c>.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the state of all digital IO port's channels at once: the parameter
         *   is an integer where each bit represents a channel, with bit 0 matching channel #0
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
        public int set_portState(int newval)
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _PortState_INVALID) return YAPI.SUCCESS;
            return _func.set_portState(newval);
        }


        // private helper for magic property
        private void setprop_portState(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _PortState_INVALID) return;
            // Touch output bits only
            newval &= _portDirection;
            newval |= (_portState & ~_portDirection);
            if (newval == _portState) return;
            _func.set_portState(newval);
            _portState = newval;
        }

        /**
         * <summary>
         *   Returns the I/O direction of all channels of the port (bitmap): 0 makes a bit an input, 1 makes it an output.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the I/O direction of all channels of the port (bitmap): 0 makes a bit
         *   an input, 1 makes it an output
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>digitalio._Portdirection_INVALID</c>.
         * </para>
         */
        public int get_portDirection()
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_portDirection();
            if (res == YAPI.INVALID_INT) res = _PortDirection_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the I/O direction of all channels of the port (bitmap): 0 makes a bit an input, 1 makes it an output.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method  to make sure the setting is kept after a reboot.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the I/O direction of all channels of the port (bitmap): 0 makes a bit
         *   an input, 1 makes it an output
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
        public int set_portDirection(int newval)
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _PortDirection_INVALID) return YAPI.SUCCESS;
            return _func.set_portDirection(newval);
        }


        // property with cached value for instant access (configuration)
        public int PortDirection
        {
            get
            {
                if (_func == null) return _PortDirection_INVALID;
                return (_online ? _portDirection : _PortDirection_INVALID);
            }
            set
            {
                setprop_portDirection(value);
            }
        }

        // private helper for magic property
        private void setprop_portDirection(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _PortDirection_INVALID) return;
            if (newval == _portDirection) return;
            _func.set_portDirection(newval);
            _portDirection = newval;
        }

        /**
         * <summary>
         *   Returns the electrical interface for each bit of the port.
         * <para>
         *   For each bit set to 0  the matching I/O works in the regular,
         *   intuitive way, for each bit set to 1, the I/O works in reverse mode.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the electrical interface for each bit of the port
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>digitalio._Portopendrain_INVALID</c>.
         * </para>
         */
        public int get_portOpenDrain()
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_portOpenDrain();
            if (res == YAPI.INVALID_INT) res = _PortOpenDrain_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the electrical interface for each bit of the port.
         * <para>
         *   0 makes a bit a regular input/output, 1 makes
         *   it an open-drain (open-collector) input/output. Remember to call the
         *   <c>saveToFlash()</c> method  to make sure the setting is kept after a reboot.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the electrical interface for each bit of the port
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
        public int set_portOpenDrain(int newval)
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _PortOpenDrain_INVALID) return YAPI.SUCCESS;
            return _func.set_portOpenDrain(newval);
        }


        // property with cached value for instant access (configuration)
        public int PortOpenDrain
        {
            get
            {
                if (_func == null) return _PortOpenDrain_INVALID;
                return (_online ? _portOpenDrain : _PortOpenDrain_INVALID);
            }
            set
            {
                setprop_portOpenDrain(value);
            }
        }

        // private helper for magic property
        private void setprop_portOpenDrain(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _PortOpenDrain_INVALID) return;
            if (newval == _portOpenDrain) return;
            _func.set_portOpenDrain(newval);
            _portOpenDrain = newval;
        }

        /**
         * <summary>
         *   Returns the polarity of all the bits of the port.
         * <para>
         *   For each bit set to 0, the matching I/O works the regular,
         *   intuitive way; for each bit set to 1, the I/O works in reverse mode.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the polarity of all the bits of the port
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>digitalio._Portpolarity_INVALID</c>.
         * </para>
         */
        public int get_portPolarity()
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_portPolarity();
            if (res == YAPI.INVALID_INT) res = _PortPolarity_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the polarity of all the bits of the port: For each bit set to 0, the matching I/O works the regular,
         *   intuitive way; for each bit set to 1, the I/O works in reverse mode.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method  to make sure the setting will be kept after a reboot.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the polarity of all the bits of the port: For each bit set to 0, the
         *   matching I/O works the regular,
         *   intuitive way; for each bit set to 1, the I/O works in reverse mode
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
        public int set_portPolarity(int newval)
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _PortPolarity_INVALID) return YAPI.SUCCESS;
            return _func.set_portPolarity(newval);
        }


        // property with cached value for instant access (configuration)
        public int PortPolarity
        {
            get
            {
                if (_func == null) return _PortPolarity_INVALID;
                return (_online ? _portPolarity : _PortPolarity_INVALID);
            }
            set
            {
                setprop_portPolarity(value);
            }
        }

        // private helper for magic property
        private void setprop_portPolarity(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _PortPolarity_INVALID) return;
            if (newval == _portPolarity) return;
            _func.set_portPolarity(newval);
            _portPolarity = newval;
        }

        /**
         * <summary>
         *   Returns the port state diagnostics (Yocto-IO and Yocto-MaxiIO-V2 only).
         * <para>
         *   Bit 0 indicates a shortcut on
         *   output 0, etc. Bit 8 indicates a power failure, and bit 9 signals overheating (overcurrent).
         *   During normal use, all diagnostic bits should stay clear.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the port state diagnostics (Yocto-IO and Yocto-MaxiIO-V2 only)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>digitalio._Portdiags_INVALID</c>.
         * </para>
         */
        public int get_portDiags()
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_portDiags();
            if (res == YAPI.INVALID_INT) res = _PortDiags_INVALID;
            return res;
        }

        // property with cached value for instant access (constant value)
        public int PortSize
        {
            get
            {
                if (_func == null) return _PortSize_INVALID;
                return (_online ? _portSize : _PortSize_INVALID);
            }
        }

        /**
         * <summary>
         *   Returns the number of bits (i.e.
         * <para>
         *   channels)implemented in the I/O port.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of bits (i.e
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>digitalio._Portsize_INVALID</c>.
         * </para>
         */
        public int get_portSize()
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_portSize();
            if (res == YAPI.INVALID_INT) res = _PortSize_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Returns the voltage source used to drive output bits.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>digitalio._Outputvoltage_USB_5V</c>, <c>digitalio._Outputvoltage_USB_3V</c> and
         *   <c>digitalio._Outputvoltage_EXT_V</c> corresponding to the voltage source used to drive output bits
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>digitalio._Outputvoltage_INVALID</c>.
         * </para>
         */
        public int get_outputVoltage()
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_outputVoltage()+1;
        }

        /**
         * <summary>
         *   Changes the voltage source used to drive output bits.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method  to make sure the setting is kept after a reboot.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>digitalio._Outputvoltage_USB_5V</c>, <c>digitalio._Outputvoltage_USB_3V</c> and
         *   <c>digitalio._Outputvoltage_EXT_V</c> corresponding to the voltage source used to drive output bits
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
        public int set_outputVoltage(int newval)
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _OutputVoltage_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_outputVoltage(newval-1);
        }


        // property with cached value for instant access (configuration)
        public int OutputVoltage
        {
            get
            {
                if (_func == null) return _OutputVoltage_INVALID;
                return (_online ? _outputVoltage : _OutputVoltage_INVALID);
            }
            set
            {
                setprop_outputVoltage(value);
            }
        }

        // private helper for magic property
        private void setprop_outputVoltage(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _OutputVoltage_INVALID) return;
            if (newval == _outputVoltage) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_outputVoltage(newval-1);
            _outputVoltage = newval;
        }

        /**
         * <summary>
         *   Sets a single bit (i.e.
         * <para>
         *   channel) of the I/O port.
         * </para>
         * </summary>
         * <param name="bitno">
         *   the bit number; lowest bit has index 0
         * </param>
         * <param name="bitstate">
         *   the state of the bit (1 or 0)
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_bitState(int bitno, int bitstate)
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.set_bitState(bitno, bitstate);
        }

        /**
         * <summary>
         *   Returns the state of a single bit (i.e.
         * <para>
         *   channel)  of the I/O port.
         * </para>
         * </summary>
         * <param name="bitno">
         *   the bit number; lowest bit has index 0
         * </param>
         * <returns>
         *   the bit state (0 or 1)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int get_bitState(int bitno)
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_bitState(bitno);
        }

        /**
         * <summary>
         *   Reverts a single bit (i.e.
         * <para>
         *   channel) of the I/O port.
         * </para>
         * </summary>
         * <param name="bitno">
         *   the bit number; lowest bit has index 0
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int toggle_bitState(int bitno)
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.toggle_bitState(bitno);
        }

        /**
         * <summary>
         *   Changes  the direction of a single bit (i.e.
         * <para>
         *   channel) from the I/O port.
         * </para>
         * </summary>
         * <param name="bitno">
         *   the bit number; lowest bit has index 0
         * </param>
         * <param name="bitdirection">
         *   direction to set, 0 makes the bit an input, 1 makes it an output.
         *   Remember to call the   <c>saveToFlash()</c> method to make sure the setting is kept after a reboot.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_bitDirection(int bitno, int bitdirection)
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.set_bitDirection(bitno, bitdirection);
        }

        /**
         * <summary>
         *   Returns the direction of a single bit (i.e.
         * <para>
         *   channel) from the I/O port (0 means the bit is an input, 1  an output).
         * </para>
         * </summary>
         * <param name="bitno">
         *   the bit number; lowest bit has index 0
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int get_bitDirection(int bitno)
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_bitDirection(bitno);
        }

        /**
         * <summary>
         *   Changes the polarity of a single bit from the I/O port.
         * <para>
         * </para>
         * </summary>
         * <param name="bitno">
         *   the bit number; lowest bit has index 0.
         * </param>
         * <param name="bitpolarity">
         *   polarity to set, 0 makes the I/O work in regular mode, 1 makes the I/O  works in reverse mode.
         *   Remember to call the   <c>saveToFlash()</c> method to make sure the setting is kept after a reboot.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_bitPolarity(int bitno, int bitpolarity)
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.set_bitPolarity(bitno, bitpolarity);
        }

        /**
         * <summary>
         *   Returns the polarity of a single bit from the I/O port (0 means the I/O works in regular mode, 1 means the I/O  works in reverse mode).
         * <para>
         * </para>
         * </summary>
         * <param name="bitno">
         *   the bit number; lowest bit has index 0
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int get_bitPolarity(int bitno)
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_bitPolarity(bitno);
        }

        /**
         * <summary>
         *   Changes  the electrical interface of a single bit from the I/O port.
         * <para>
         * </para>
         * </summary>
         * <param name="bitno">
         *   the bit number; lowest bit has index 0
         * </param>
         * <param name="opendrain">
         *   0 makes a bit a regular input/output, 1 makes
         *   it an open-drain (open-collector) input/output. Remember to call the
         *   <c>saveToFlash()</c> method to make sure the setting is kept after a reboot.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_bitOpenDrain(int bitno, int opendrain)
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.set_bitOpenDrain(bitno, opendrain);
        }

        /**
         * <summary>
         *   Returns the type of electrical interface of a single bit from the I/O port.
         * <para>
         *   (0 means the bit is an input, 1  an output).
         * </para>
         * </summary>
         * <param name="bitno">
         *   the bit number; lowest bit has index 0
         * </param>
         * <returns>
         *   0 means the a bit is a regular input/output, 1 means the bit is an open-drain
         *   (open-collector) input/output.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int get_bitOpenDrain(int bitno)
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_bitOpenDrain(bitno);
        }

        /**
         * <summary>
         *   Triggers a pulse on a single bit for a specified duration.
         * <para>
         *   The specified bit
         *   will be turned to 1, and then back to 0 after the given duration.
         * </para>
         * </summary>
         * <param name="bitno">
         *   the bit number; lowest bit has index 0
         * </param>
         * <param name="ms_duration">
         *   desired pulse duration in milliseconds. Be aware that the device time
         *   resolution is not guaranteed up to the millisecond.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int pulse(int bitno, int ms_duration)
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.pulse(bitno, ms_duration);
        }

        /**
         * <summary>
         *   Schedules a pulse on a single bit for a specified duration.
         * <para>
         *   The specified bit
         *   will be turned to 1, and then back to 0 after the given duration.
         * </para>
         * </summary>
         * <param name="bitno">
         *   the bit number; lowest bit has index 0
         * </param>
         * <param name="ms_delay">
         *   waiting time before the pulse, in milliseconds
         * </param>
         * <param name="ms_duration">
         *   desired pulse duration in milliseconds. Be aware that the device time
         *   resolution is not guaranteed up to the millisecond.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int delayedPulse(int bitno, int ms_delay, int ms_duration)
        {
            if (_func == null)
            {
                string msg = "No DigitalIO connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.delayedPulse(bitno, ms_delay, ms_duration);
        }
    }
    //--- (end of YDigitalIO implementation)
}

