/*********************************************************************
 *
 *  $Id: yocto_spiport_proxy.cs 43619 2021-01-29 09:14:45Z mvuilleu $
 *
 *  Implements YSpiPortProxy, the Proxy API for SpiPort
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
    //--- (YSpiPort class start)
    static public partial class YoctoProxyManager
    {
        public static YSpiPortProxy FindSpiPort(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YSpiPort func = null;
            YSpiPortProxy res = (YSpiPortProxy)YFunctionProxy.FindSimilarUnknownFunction("YSpiPortProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YSpiPortProxy)YFunctionProxy.FindSimilarKnownFunction("YSpiPortProxy");
                if (res != null) return res;
                func = YSpiPort.FirstSpiPort();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YSpiPortProxy)func.get_userData();
                    }
                }
            } else {
                func = YSpiPort.FindSpiPort(name);
                if (func.get_userData() != null) {
                    return (YSpiPortProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YSpiPortProxy(func, name);
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
 *   The <c>YSpiPort</c> class allows you to fully drive a Yoctopuce SPI port.
 * <para>
 *   It can be used to send and receive data, and to configure communication
 *   parameters (baud rate, bit count, parity, flow control and protocol).
 *   Note that Yoctopuce SPI ports are not exposed as virtual COM ports.
 *   They are meant to be used in the same way as all Yoctopuce devices.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YSpiPortProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a SPI port for a given identifier.
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
         *   This function does not require that the SPI port is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YSpiPort.isOnline()</c> to test if the SPI port is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a SPI port by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the SPI port, for instance
         *   <c>YSPIMK01.spiPort</c>.
         * </param>
         * <returns>
         *   a <c>YSpiPort</c> object allowing you to drive the SPI port.
         * </returns>
         */
        public static YSpiPortProxy FindSpiPort(string func)
        {
            return YoctoProxyManager.FindSpiPort(func);
        }
        //--- (end of YSpiPort class start)
        //--- (YSpiPort definitions)
        public const int _RxCount_INVALID = -1;
        public const int _TxCount_INVALID = -1;
        public const int _ErrCount_INVALID = -1;
        public const int _RxMsgCount_INVALID = -1;
        public const int _TxMsgCount_INVALID = -1;
        public const string _LastMsg_INVALID = YAPI.INVALID_STRING;
        public const string _CurrentJob_INVALID = YAPI.INVALID_STRING;
        public const string _StartupJob_INVALID = YAPI.INVALID_STRING;
        public const int _JobMaxTask_INVALID = -1;
        public const int _JobMaxSize_INVALID = -1;
        public const string _Command_INVALID = YAPI.INVALID_STRING;
        public const string _Protocol_INVALID = YAPI.INVALID_STRING;
        public const int _VoltageLevel_INVALID = 0;
        public const int _VoltageLevel_OFF = 1;
        public const int _VoltageLevel_TTL3V = 2;
        public const int _VoltageLevel_TTL3VR = 3;
        public const int _VoltageLevel_TTL5V = 4;
        public const int _VoltageLevel_TTL5VR = 5;
        public const int _VoltageLevel_RS232 = 6;
        public const int _VoltageLevel_RS485 = 7;
        public const int _VoltageLevel_TTL1V8 = 8;
        public const string _SpiMode_INVALID = YAPI.INVALID_STRING;
        public const int _SsPolarity_INVALID = 0;
        public const int _SsPolarity_ACTIVE_LOW = 1;
        public const int _SsPolarity_ACTIVE_HIGH = 2;
        public const int _ShiftSampling_INVALID = 0;
        public const int _ShiftSampling_OFF = 1;
        public const int _ShiftSampling_ON = 2;

        // reference to real YoctoAPI object
        protected new YSpiPort _func;
        // property cache
        protected string _startupJob = _StartupJob_INVALID;
        protected int _jobMaxTask = _JobMaxTask_INVALID;
        protected int _jobMaxSize = _JobMaxSize_INVALID;
        protected string _protocol = _Protocol_INVALID;
        protected int _voltageLevel = _VoltageLevel_INVALID;
        protected string _spiMode = _SpiMode_INVALID;
        protected int _ssPolarity = _SsPolarity_INVALID;
        protected int _shiftSampling = _ShiftSampling_INVALID;
        //--- (end of YSpiPort definitions)

        //--- (YSpiPort implementation)
        internal YSpiPortProxy(YSpiPort hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("SpiPort " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YSpiPort) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YSpiPort hwd = YSpiPort.FindSpiPort(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YSpiPort hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering SpiPort callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type SpiPort available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YSpiPort.FindSpiPort</c> to obtain an object that can control the
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
            YSpiPort it = YSpiPort.FirstSpiPort();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextSpiPort();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
            _jobMaxTask = _func.get_jobMaxTask();
            _jobMaxSize = _func.get_jobMaxSize();
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            _startupJob = _func.get_startupJob();
            _protocol = _func.get_protocol();
            _voltageLevel = _func.get_voltageLevel()+1;
            _spiMode = _func.get_spiMode();
            _ssPolarity = _func.get_ssPolarity()+1;
            _shiftSampling = _func.get_shiftSampling()+1;
        }

        /**
         * <summary>
         *   Returns the total number of bytes received since last reset.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the total number of bytes received since last reset
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSpiPort.RXCOUNT_INVALID</c>.
         * </para>
         */
        public int get_rxCount()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            res = _func.get_rxCount();
            if (res == YAPI.INVALID_INT) {
                res = _RxCount_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the total number of bytes transmitted since last reset.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the total number of bytes transmitted since last reset
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSpiPort.TXCOUNT_INVALID</c>.
         * </para>
         */
        public int get_txCount()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            res = _func.get_txCount();
            if (res == YAPI.INVALID_INT) {
                res = _TxCount_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the total number of communication errors detected since last reset.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the total number of communication errors detected since last reset
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSpiPort.ERRCOUNT_INVALID</c>.
         * </para>
         */
        public int get_errCount()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            res = _func.get_errCount();
            if (res == YAPI.INVALID_INT) {
                res = _ErrCount_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the total number of messages received since last reset.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the total number of messages received since last reset
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSpiPort.RXMSGCOUNT_INVALID</c>.
         * </para>
         */
        public int get_rxMsgCount()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            res = _func.get_rxMsgCount();
            if (res == YAPI.INVALID_INT) {
                res = _RxMsgCount_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the total number of messages send since last reset.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the total number of messages send since last reset
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSpiPort.TXMSGCOUNT_INVALID</c>.
         * </para>
         */
        public int get_txMsgCount()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            res = _func.get_txMsgCount();
            if (res == YAPI.INVALID_INT) {
                res = _TxMsgCount_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the latest message fully received (for Line and Frame protocols).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the latest message fully received (for Line and Frame protocols)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSpiPort.LASTMSG_INVALID</c>.
         * </para>
         */
        public string get_lastMsg()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.get_lastMsg();
        }

        /**
         * <summary>
         *   Returns the name of the job file currently in use.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the name of the job file currently in use
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSpiPort.CURRENTJOB_INVALID</c>.
         * </para>
         */
        public string get_currentJob()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.get_currentJob();
        }

        /**
         * <summary>
         *   Selects a job file to run immediately.
         * <para>
         *   If an empty string is
         *   given as argument, stops running current job file.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string
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
        public int set_currentJob(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            if (newval == _CurrentJob_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_currentJob(newval);
        }

        /**
         * <summary>
         *   Returns the job file to use when the device is powered on.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the job file to use when the device is powered on
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSpiPort.STARTUPJOB_INVALID</c>.
         * </para>
         */
        public string get_startupJob()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.get_startupJob();
        }

        /**
         * <summary>
         *   Changes the job to use when the device is powered on.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the job to use when the device is powered on
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
        public int set_startupJob(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            if (newval == _StartupJob_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_startupJob(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Job file to use when the device is powered on.</value>
        public string StartupJob
        {
            get
            {
                if (_func == null) {
                    return _StartupJob_INVALID;
                }
                if (_online) {
                    return _startupJob;
                }
                return _StartupJob_INVALID;
            }
            set
            {
                setprop_startupJob(value);
            }
        }

        // private helper for magic property
        private void setprop_startupJob(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _StartupJob_INVALID) {
                return;
            }
            if (newval == _startupJob) {
                return;
            }
            _func.set_startupJob(newval);
            _startupJob = newval;
        }

        /**
         * <summary>
         *   Returns the maximum number of tasks in a job that the device can handle.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the maximum number of tasks in a job that the device can handle
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSpiPort.JOBMAXTASK_INVALID</c>.
         * </para>
         */
        public int get_jobMaxTask()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            res = _func.get_jobMaxTask();
            if (res == YAPI.INVALID_INT) {
                res = _JobMaxTask_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (constant value)
        /// <value>Maximum number of tasks in a job that the device can handle.</value>
        public int JobMaxTask
        {
            get
            {
                if (_func == null) {
                    return _JobMaxTask_INVALID;
                }
                if (_online) {
                    return _jobMaxTask;
                }
                return _JobMaxTask_INVALID;
            }
        }

        /**
         * <summary>
         *   Returns maximum size allowed for job files.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to maximum size allowed for job files
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSpiPort.JOBMAXSIZE_INVALID</c>.
         * </para>
         */
        public int get_jobMaxSize()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            res = _func.get_jobMaxSize();
            if (res == YAPI.INVALID_INT) {
                res = _JobMaxSize_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (constant value)
        /// <value>Maximum size allowed for job files.</value>
        public int JobMaxSize
        {
            get
            {
                if (_func == null) {
                    return _JobMaxSize_INVALID;
                }
                if (_online) {
                    return _jobMaxSize;
                }
                return _JobMaxSize_INVALID;
            }
        }

        /**
         * <summary>
         *   Returns the type of protocol used over the serial line, as a string.
         * <para>
         *   Possible values are "Line" for ASCII messages separated by CR and/or LF,
         *   "Frame:[timeout]ms" for binary messages separated by a delay time,
         *   "Char" for a continuous ASCII stream or
         *   "Byte" for a continuous binary stream.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the type of protocol used over the serial line, as a string
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSpiPort.PROTOCOL_INVALID</c>.
         * </para>
         */
        public string get_protocol()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.get_protocol();
        }

        /**
         * <summary>
         *   Changes the type of protocol used over the serial line.
         * <para>
         *   Possible values are "Line" for ASCII messages separated by CR and/or LF,
         *   "Frame:[timeout]ms" for binary messages separated by a delay time,
         *   "Char" for a continuous ASCII stream or
         *   "Byte" for a continuous binary stream.
         *   The suffix "/[wait]ms" can be added to reduce the transmit rate so that there
         *   is always at lest the specified number of milliseconds between each bytes sent.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the type of protocol used over the serial line
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
        public int set_protocol(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            if (newval == _Protocol_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_protocol(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Type of protocol used over the serial line, as a string.</value>
        public string Protocol
        {
            get
            {
                if (_func == null) {
                    return _Protocol_INVALID;
                }
                if (_online) {
                    return _protocol;
                }
                return _Protocol_INVALID;
            }
            set
            {
                setprop_protocol(value);
            }
        }

        // private helper for magic property
        private void setprop_protocol(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Protocol_INVALID) {
                return;
            }
            if (newval == _protocol) {
                return;
            }
            _func.set_protocol(newval);
            _protocol = newval;
        }

        /**
         * <summary>
         *   Returns the voltage level used on the serial line.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YSpiPort.VOLTAGELEVEL_OFF</c>, <c>YSpiPort.VOLTAGELEVEL_TTL3V</c>,
         *   <c>YSpiPort.VOLTAGELEVEL_TTL3VR</c>, <c>YSpiPort.VOLTAGELEVEL_TTL5V</c>,
         *   <c>YSpiPort.VOLTAGELEVEL_TTL5VR</c>, <c>YSpiPort.VOLTAGELEVEL_RS232</c>,
         *   <c>YSpiPort.VOLTAGELEVEL_RS485</c> and <c>YSpiPort.VOLTAGELEVEL_TTL1V8</c> corresponding to the
         *   voltage level used on the serial line
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSpiPort.VOLTAGELEVEL_INVALID</c>.
         * </para>
         */
        public int get_voltageLevel()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_voltageLevel()+1;
        }

        /**
         * <summary>
         *   Changes the voltage type used on the serial line.
         * <para>
         *   Valid
         *   values  will depend on the Yoctopuce device model featuring
         *   the serial port feature.  Check your device documentation
         *   to find out which values are valid for that specific model.
         *   Trying to set an invalid value will have no effect.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YSpiPort.VOLTAGELEVEL_OFF</c>, <c>YSpiPort.VOLTAGELEVEL_TTL3V</c>,
         *   <c>YSpiPort.VOLTAGELEVEL_TTL3VR</c>, <c>YSpiPort.VOLTAGELEVEL_TTL5V</c>,
         *   <c>YSpiPort.VOLTAGELEVEL_TTL5VR</c>, <c>YSpiPort.VOLTAGELEVEL_RS232</c>,
         *   <c>YSpiPort.VOLTAGELEVEL_RS485</c> and <c>YSpiPort.VOLTAGELEVEL_TTL1V8</c> corresponding to the
         *   voltage type used on the serial line
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
        public int set_voltageLevel(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            if (newval == _VoltageLevel_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_voltageLevel(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>Voltage level used on the serial line.</value>
        public int VoltageLevel
        {
            get
            {
                if (_func == null) {
                    return _VoltageLevel_INVALID;
                }
                if (_online) {
                    return _voltageLevel;
                }
                return _VoltageLevel_INVALID;
            }
            set
            {
                setprop_voltageLevel(value);
            }
        }

        // private helper for magic property
        private void setprop_voltageLevel(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _VoltageLevel_INVALID) {
                return;
            }
            if (newval == _voltageLevel) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_voltageLevel(newval-1);
            _voltageLevel = newval;
        }

        /**
         * <summary>
         *   Returns the SPI port communication parameters, as a string such as
         *   "125000,0,msb".
         * <para>
         *   The string includes the baud rate, the SPI mode (between
         *   0 and 3) and the bit order.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the SPI port communication parameters, as a string such as
         *   "125000,0,msb"
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSpiPort.SPIMODE_INVALID</c>.
         * </para>
         */
        public string get_spiMode()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.get_spiMode();
        }

        /**
         * <summary>
         *   Changes the SPI port communication parameters, with a string such as
         *   "125000,0,msb".
         * <para>
         *   The string includes the baud rate, the SPI mode (between
         *   0 and 3) and the bit order.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the SPI port communication parameters, with a string such as
         *   "125000,0,msb"
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
        public int set_spiMode(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            if (newval == _SpiMode_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_spiMode(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>SPI port communication parameters, as a string such as</value>
        public string SpiMode
        {
            get
            {
                if (_func == null) {
                    return _SpiMode_INVALID;
                }
                if (_online) {
                    return _spiMode;
                }
                return _SpiMode_INVALID;
            }
            set
            {
                setprop_spiMode(value);
            }
        }

        // private helper for magic property
        private void setprop_spiMode(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _SpiMode_INVALID) {
                return;
            }
            if (newval == _spiMode) {
                return;
            }
            _func.set_spiMode(newval);
            _spiMode = newval;
        }

        /**
         * <summary>
         *   Returns the SS line polarity.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YSpiPort.SSPOLARITY_ACTIVE_LOW</c> or <c>YSpiPort.SSPOLARITY_ACTIVE_HIGH</c>, according
         *   to the SS line polarity
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSpiPort.SSPOLARITY_INVALID</c>.
         * </para>
         */
        public int get_ssPolarity()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_ssPolarity()+1;
        }

        /**
         * <summary>
         *   Changes the SS line polarity.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YSpiPort.SSPOLARITY_ACTIVE_LOW</c> or <c>YSpiPort.SSPOLARITY_ACTIVE_HIGH</c>, according
         *   to the SS line polarity
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
        public int set_ssPolarity(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            if (newval == _SsPolarity_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_ssPolarity(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>SS line polarity.</value>
        public int SsPolarity
        {
            get
            {
                if (_func == null) {
                    return _SsPolarity_INVALID;
                }
                if (_online) {
                    return _ssPolarity;
                }
                return _SsPolarity_INVALID;
            }
            set
            {
                setprop_ssPolarity(value);
            }
        }

        // private helper for magic property
        private void setprop_ssPolarity(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _SsPolarity_INVALID) {
                return;
            }
            if (newval == _ssPolarity) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_ssPolarity(newval-1);
            _ssPolarity = newval;
        }

        /**
         * <summary>
         *   Returns true when the SDI line phase is shifted with regards to the SDO line.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YSpiPort.SHIFTSAMPLING_OFF</c> or <c>YSpiPort.SHIFTSAMPLING_ON</c>, according to true
         *   when the SDI line phase is shifted with regards to the SDO line
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSpiPort.SHIFTSAMPLING_INVALID</c>.
         * </para>
         */
        public int get_shiftSampling()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_shiftSampling()+1;
        }

        /**
         * <summary>
         *   Changes the SDI line sampling shift.
         * <para>
         *   When disabled, SDI line is
         *   sampled in the middle of data output time. When enabled, SDI line is
         *   samples at the end of data output time.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YSpiPort.SHIFTSAMPLING_OFF</c> or <c>YSpiPort.SHIFTSAMPLING_ON</c>, according to the SDI
         *   line sampling shift
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
        public int set_shiftSampling(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            if (newval == _ShiftSampling_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_shiftSampling(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>True when the SDI line phase is shifted with regards to the SDO line.</value>
        public int ShiftSampling
        {
            get
            {
                if (_func == null) {
                    return _ShiftSampling_INVALID;
                }
                if (_online) {
                    return _shiftSampling;
                }
                return _ShiftSampling_INVALID;
            }
            set
            {
                setprop_shiftSampling(value);
            }
        }

        // private helper for magic property
        private void setprop_shiftSampling(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _ShiftSampling_INVALID) {
                return;
            }
            if (newval == _shiftSampling) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_shiftSampling(newval-1);
            _shiftSampling = newval;
        }

        /**
         * <summary>
         *   Reads a single line (or message) from the receive buffer, starting at current stream position.
         * <para>
         *   This function is intended to be used when the serial port is configured for a message protocol,
         *   such as 'Line' mode or frame protocols.
         * </para>
         * <para>
         *   If data at current stream position is not available anymore in the receive buffer,
         *   the function returns the oldest available line and moves the stream position just after.
         *   If no new full line is received, the function returns an empty line.
         * </para>
         * </summary>
         * <returns>
         *   a string with a single line of text
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual string readLine()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.readLine();
        }

        /**
         * <summary>
         *   Searches for incoming messages in the serial port receive buffer matching a given pattern,
         *   starting at current position.
         * <para>
         *   This function will only compare and return printable characters
         *   in the message strings. Binary protocols are handled as hexadecimal strings.
         * </para>
         * <para>
         *   The search returns all messages matching the expression provided as argument in the buffer.
         *   If no matching message is found, the search waits for one up to the specified maximum timeout
         *   (in milliseconds).
         * </para>
         * </summary>
         * <param name="pattern">
         *   a limited regular expression describing the expected message format,
         *   or an empty string if all messages should be returned (no filtering).
         *   When using binary protocols, the format applies to the hexadecimal
         *   representation of the message.
         * </param>
         * <param name="maxWait">
         *   the maximum number of milliseconds to wait for a message if none is found
         *   in the receive buffer.
         * </param>
         * <returns>
         *   an array of strings containing the messages found, if any.
         *   Binary messages are converted to hexadecimal representation.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty array.
         * </para>
         */
        public virtual string[] readMessages(string pattern, int maxWait)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.readMessages(pattern, maxWait).ToArray();
        }

        /**
         * <summary>
         *   Changes the current internal stream position to the specified value.
         * <para>
         *   This function
         *   does not affect the device, it only changes the value stored in the API object
         *   for the next read operations.
         * </para>
         * </summary>
         * <param name="absPos">
         *   the absolute position index for next read operations.
         * </param>
         * <returns>
         *   nothing.
         * </returns>
         */
        public virtual int read_seek(int absPos)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.read_seek(absPos);
        }

        /**
         * <summary>
         *   Returns the current absolute stream position pointer of the API object.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the absolute position index for next read operations.
         * </returns>
         */
        public virtual int read_tell()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.read_tell();
        }

        /**
         * <summary>
         *   Returns the number of bytes available to read in the input buffer starting from the
         *   current absolute stream position pointer of the API object.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the number of bytes available to read
         * </returns>
         */
        public virtual int read_avail()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.read_avail();
        }

        /**
         * <summary>
         *   Sends a text line query to the serial port, and reads the reply, if any.
         * <para>
         *   This function is intended to be used when the serial port is configured for 'Line' protocol.
         * </para>
         * </summary>
         * <param name="query">
         *   the line query to send (without CR/LF)
         * </param>
         * <param name="maxWait">
         *   the maximum number of milliseconds to wait for a reply.
         * </param>
         * <returns>
         *   the next text line received after sending the text query, as a string.
         *   Additional lines can be obtained by calling readLine or readMessages.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty string.
         * </para>
         */
        public virtual string queryLine(string query, int maxWait)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.queryLine(query, maxWait);
        }

        /**
         * <summary>
         *   Sends a binary message to the serial port, and reads the reply, if any.
         * <para>
         *   This function is intended to be used when the serial port is configured for
         *   Frame-based protocol.
         * </para>
         * </summary>
         * <param name="hexString">
         *   the message to send, coded in hexadecimal
         * </param>
         * <param name="maxWait">
         *   the maximum number of milliseconds to wait for a reply.
         * </param>
         * <returns>
         *   the next frame received after sending the message, as a hex string.
         *   Additional frames can be obtained by calling readHex or readMessages.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty string.
         * </para>
         */
        public virtual string queryHex(string hexString, int maxWait)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.queryHex(hexString, maxWait);
        }

        /**
         * <summary>
         *   Saves the job definition string (JSON data) into a job file.
         * <para>
         *   The job file can be later enabled using <c>selectJob()</c>.
         * </para>
         * </summary>
         * <param name="jobfile">
         *   name of the job file to save on the device filesystem
         * </param>
         * <param name="jsonDef">
         *   a string containing a JSON definition of the job
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int uploadJob(string jobfile, string jsonDef)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.uploadJob(jobfile, jsonDef);
        }

        /**
         * <summary>
         *   Load and start processing the specified job file.
         * <para>
         *   The file must have
         *   been previously created using the user interface or uploaded on the
         *   device filesystem using the <c>uploadJob()</c> function.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="jobfile">
         *   name of the job file (on the device filesystem)
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int selectJob(string jobfile)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.selectJob(jobfile);
        }

        /**
         * <summary>
         *   Clears the serial port buffer and resets counters to zero.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int reset()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.reset();
        }

        /**
         * <summary>
         *   Sends a single byte to the serial port.
         * <para>
         * </para>
         * </summary>
         * <param name="code">
         *   the byte to send
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int writeByte(int code)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.writeByte(code);
        }

        /**
         * <summary>
         *   Sends an ASCII string to the serial port, as is.
         * <para>
         * </para>
         * </summary>
         * <param name="text">
         *   the text string to send
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int writeStr(string text)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.writeStr(text);
        }

        /**
         * <summary>
         *   Sends a binary buffer to the serial port, as is.
         * <para>
         * </para>
         * </summary>
         * <param name="buff">
         *   the binary buffer to send
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int writeBin(byte[] buff)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.writeBin(buff);
        }

        /**
         * <summary>
         *   Sends a byte sequence (provided as a list of bytes) to the serial port.
         * <para>
         * </para>
         * </summary>
         * <param name="byteList">
         *   a list of byte codes
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int writeArray(int[] byteList)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.writeArray(new List<int>(byteList));
        }

        /**
         * <summary>
         *   Sends a byte sequence (provided as a hexadecimal string) to the serial port.
         * <para>
         * </para>
         * </summary>
         * <param name="hexString">
         *   a string of hexadecimal byte codes
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int writeHex(string hexString)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.writeHex(hexString);
        }

        /**
         * <summary>
         *   Sends an ASCII string to the serial port, followed by a line break (CR LF).
         * <para>
         * </para>
         * </summary>
         * <param name="text">
         *   the text string to send
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int writeLine(string text)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.writeLine(text);
        }

        /**
         * <summary>
         *   Reads one byte from the receive buffer, starting at current stream position.
         * <para>
         *   If data at current stream position is not available anymore in the receive buffer,
         *   or if there is no data available yet, the function returns YAPI.NO_MORE_DATA.
         * </para>
         * </summary>
         * <returns>
         *   the next byte
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int readByte()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.readByte();
        }

        /**
         * <summary>
         *   Reads data from the receive buffer as a string, starting at current stream position.
         * <para>
         *   If data at current stream position is not available anymore in the receive buffer, the
         *   function performs a short read.
         * </para>
         * </summary>
         * <param name="nChars">
         *   the maximum number of characters to read
         * </param>
         * <returns>
         *   a string with receive buffer contents
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual string readStr(int nChars)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.readStr(nChars);
        }

        /**
         * <summary>
         *   Reads data from the receive buffer as a binary buffer, starting at current stream position.
         * <para>
         *   If data at current stream position is not available anymore in the receive buffer, the
         *   function performs a short read.
         * </para>
         * </summary>
         * <param name="nChars">
         *   the maximum number of bytes to read
         * </param>
         * <returns>
         *   a binary object with receive buffer contents
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual byte[] readBin(int nChars)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.readBin(nChars);
        }

        /**
         * <summary>
         *   Reads data from the receive buffer as a list of bytes, starting at current stream position.
         * <para>
         *   If data at current stream position is not available anymore in the receive buffer, the
         *   function performs a short read.
         * </para>
         * </summary>
         * <param name="nChars">
         *   the maximum number of bytes to read
         * </param>
         * <returns>
         *   a sequence of bytes with receive buffer contents
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty array.
         * </para>
         */
        public virtual int[] readArray(int nChars)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.readArray(nChars).ToArray();
        }

        /**
         * <summary>
         *   Reads data from the receive buffer as a hexadecimal string, starting at current stream position.
         * <para>
         *   If data at current stream position is not available anymore in the receive buffer, the
         *   function performs a short read.
         * </para>
         * </summary>
         * <param name="nBytes">
         *   the maximum number of bytes to read
         * </param>
         * <returns>
         *   a string with receive buffer contents, encoded in hexadecimal
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual string readHex(int nBytes)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.readHex(nBytes);
        }

        /**
         * <summary>
         *   Manually sets the state of the SS line.
         * <para>
         *   This function has no effect when
         *   the SS line is handled automatically.
         * </para>
         * </summary>
         * <param name="val">
         *   1 to turn SS active, 0 to release SS.
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_SS(int val)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            return _func.set_SS(val);
        }

        /**
         * <summary>
         *   Retrieves messages (both direction) in the SPI port buffer, starting at current position.
         * <para>
         * </para>
         * <para>
         *   If no message is found, the search waits for one up to the specified maximum timeout
         *   (in milliseconds).
         * </para>
         * </summary>
         * <param name="maxWait">
         *   the maximum number of milliseconds to wait for a message if none is found
         *   in the receive buffer.
         * </param>
         * <returns>
         *   an array of <c>YSpiSnoopingRecord</c> objects containing the messages found, if any.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty array.
         * </para>
         */
        public virtual YSpiSnoopingRecordProxy[] snoopMessages(int maxWait)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpiPort connected");
            }
            int i;
            int arrlen;
            YSpiSnoopingRecord[] std_res;
            YSpiSnoopingRecordProxy[] proxy_res;
            std_res = _func.snoopMessages(maxWait).ToArray();
            arrlen = std_res.Length;
            proxy_res = new YSpiSnoopingRecordProxy[arrlen];
            i = 0;
            while (i < arrlen) {
                proxy_res[i] = new YSpiSnoopingRecordProxy(std_res[i]);
                i = i + 1;
            }
            return proxy_res;
        }
    }
    //--- (end of YSpiPort implementation)
}

