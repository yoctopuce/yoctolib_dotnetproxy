/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YSdi12PortProxy, the Proxy API for Sdi12Port
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
    //--- (YSdi12Port class start)
    static public partial class YoctoProxyManager
    {
        public static YSdi12PortProxy FindSdi12Port(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YSdi12Port func = null;
            YSdi12PortProxy res = (YSdi12PortProxy)YFunctionProxy.FindSimilarUnknownFunction("YSdi12PortProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YSdi12PortProxy)YFunctionProxy.FindSimilarKnownFunction("YSdi12PortProxy");
                if (res != null) return res;
                func = YSdi12Port.FirstSdi12Port();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YSdi12PortProxy)func.get_userData();
                    }
                }
            } else {
                func = YSdi12Port.FindSdi12Port(name);
                if (func.get_userData() != null) {
                    return (YSdi12PortProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YSdi12PortProxy(func, name);
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
 *   The <c>YSdi12Port</c> class allows you to fully drive a Yoctopuce SDI12 port.
 * <para>
 *   It can be used to send and receive data, and to configure communication
 *   parameters (baud rate, bit count, parity, flow control and protocol).
 *   Note that Yoctopuce SDI12 ports are not exposed as virtual COM ports.
 *   They are meant to be used in the same way as all Yoctopuce devices.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YSdi12PortProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves an SDI12 port for a given identifier.
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
         *   This function does not require that the SDI12 port is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YSdi12Port.isOnline()</c> to test if the SDI12 port is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   an SDI12 port by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the SDI12 port, for instance
         *   <c>MyDevice.sdi12Port</c>.
         * </param>
         * <returns>
         *   a <c>YSdi12Port</c> object allowing you to drive the SDI12 port.
         * </returns>
         */
        public static YSdi12PortProxy FindSdi12Port(string func)
        {
            return YoctoProxyManager.FindSdi12Port(func);
        }
        //--- (end of YSdi12Port class start)
        //--- (YSdi12Port definitions)
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
        public const int _VoltageLevel_SDI12 = 9;
        public const string _SerialMode_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YSdi12Port _func;
        // property cache
        protected string _startupJob = _StartupJob_INVALID;
        protected int _jobMaxTask = _JobMaxTask_INVALID;
        protected int _jobMaxSize = _JobMaxSize_INVALID;
        protected string _protocol = _Protocol_INVALID;
        protected int _voltageLevel = _VoltageLevel_INVALID;
        protected string _serialMode = _SerialMode_INVALID;
        //--- (end of YSdi12Port definitions)

        //--- (YSdi12Port implementation)
        internal YSdi12PortProxy(YSdi12Port hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Sdi12Port " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YSdi12Port) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YSdi12Port hwd = YSdi12Port.FindSdi12Port(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YSdi12Port hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Sdi12Port callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type Sdi12Port available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YSdi12Port.FindSdi12Port</c> to obtain an object that can control the
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
            YSdi12Port it = YSdi12Port.FirstSdi12Port();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextSdi12Port();
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
            _serialMode = _func.get_serialMode();
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
         *   On failure, throws an exception or returns <c>YSdi12Port.RXCOUNT_INVALID</c>.
         * </para>
         */
        public int get_rxCount()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
         *   On failure, throws an exception or returns <c>YSdi12Port.TXCOUNT_INVALID</c>.
         * </para>
         */
        public int get_txCount()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
         *   On failure, throws an exception or returns <c>YSdi12Port.ERRCOUNT_INVALID</c>.
         * </para>
         */
        public int get_errCount()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
         *   On failure, throws an exception or returns <c>YSdi12Port.RXMSGCOUNT_INVALID</c>.
         * </para>
         */
        public int get_rxMsgCount()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
         *   On failure, throws an exception or returns <c>YSdi12Port.TXMSGCOUNT_INVALID</c>.
         * </para>
         */
        public int get_txMsgCount()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
            }
            res = _func.get_txMsgCount();
            if (res == YAPI.INVALID_INT) {
                res = _TxMsgCount_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the latest message fully received.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the latest message fully received
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSdi12Port.LASTMSG_INVALID</c>.
         * </para>
         */
        public string get_lastMsg()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
         *   On failure, throws an exception or returns <c>YSdi12Port.CURRENTJOB_INVALID</c>.
         * </para>
         */
        public string get_currentJob()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
         *   On failure, throws an exception or returns <c>YSdi12Port.STARTUPJOB_INVALID</c>.
         * </para>
         */
        public string get_startupJob()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
         *   On failure, throws an exception or returns <c>YSdi12Port.JOBMAXTASK_INVALID</c>.
         * </para>
         */
        public int get_jobMaxTask()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
         *   On failure, throws an exception or returns <c>YSdi12Port.JOBMAXSIZE_INVALID</c>.
         * </para>
         */
        public int get_jobMaxSize()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
         *   On failure, throws an exception or returns <c>YSdi12Port.PROTOCOL_INVALID</c>.
         * </para>
         */
        public string get_protocol()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
         *   a value among <c>YSdi12Port.VOLTAGELEVEL_OFF</c>, <c>YSdi12Port.VOLTAGELEVEL_TTL3V</c>,
         *   <c>YSdi12Port.VOLTAGELEVEL_TTL3VR</c>, <c>YSdi12Port.VOLTAGELEVEL_TTL5V</c>,
         *   <c>YSdi12Port.VOLTAGELEVEL_TTL5VR</c>, <c>YSdi12Port.VOLTAGELEVEL_RS232</c>,
         *   <c>YSdi12Port.VOLTAGELEVEL_RS485</c>, <c>YSdi12Port.VOLTAGELEVEL_TTL1V8</c> and
         *   <c>YSdi12Port.VOLTAGELEVEL_SDI12</c> corresponding to the voltage level used on the serial line
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSdi12Port.VOLTAGELEVEL_INVALID</c>.
         * </para>
         */
        public int get_voltageLevel()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
         *   a value among <c>YSdi12Port.VOLTAGELEVEL_OFF</c>, <c>YSdi12Port.VOLTAGELEVEL_TTL3V</c>,
         *   <c>YSdi12Port.VOLTAGELEVEL_TTL3VR</c>, <c>YSdi12Port.VOLTAGELEVEL_TTL5V</c>,
         *   <c>YSdi12Port.VOLTAGELEVEL_TTL5VR</c>, <c>YSdi12Port.VOLTAGELEVEL_RS232</c>,
         *   <c>YSdi12Port.VOLTAGELEVEL_RS485</c>, <c>YSdi12Port.VOLTAGELEVEL_TTL1V8</c> and
         *   <c>YSdi12Port.VOLTAGELEVEL_SDI12</c> corresponding to the voltage type used on the serial line
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
         *   Returns the serial port communication parameters, as a string such as
         *   "1200,7E1,Simplex".
         * <para>
         *   The string includes the baud rate, the number of data bits,
         *   the parity, and the number of stop bits. The suffix "Simplex" denotes
         *   the fact that transmission in both directions is multiplexed on the
         *   same transmission line.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the serial port communication parameters, as a string such as
         *   "1200,7E1,Simplex"
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSdi12Port.SERIALMODE_INVALID</c>.
         * </para>
         */
        public string get_serialMode()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
            }
            return _func.get_serialMode();
        }

        /**
         * <summary>
         *   Changes the serial port communication parameters, with a string such as
         *   "1200,7E1,Simplex".
         * <para>
         *   The string includes the baud rate, the number of data bits,
         *   the parity, and the number of stop bits. The suffix "Simplex" denotes
         *   the fact that transmission in both directions is multiplexed on the
         *   same transmission line.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the serial port communication parameters, with a string such as
         *   "1200,7E1,Simplex"
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
        public int set_serialMode(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
            }
            if (newval == _SerialMode_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_serialMode(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Serial port communication parameters, as a string such as</value>
        public string SerialMode
        {
            get
            {
                if (_func == null) {
                    return _SerialMode_INVALID;
                }
                if (_online) {
                    return _serialMode;
                }
                return _SerialMode_INVALID;
            }
            set
            {
                setprop_serialMode(value);
            }
        }

        // private helper for magic property
        private void setprop_serialMode(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _SerialMode_INVALID) {
                return;
            }
            if (newval == _serialMode) {
                return;
            }
            _func.set_serialMode(newval);
            _serialMode = newval;
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
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
                throw new YoctoApiProxyException("No Sdi12Port connected");
            }
            return _func.readHex(nBytes);
        }

        /**
         * <summary>
         *   Sends a SDI-12 query to the bus, and reads the sensor immediate reply.
         * <para>
         *   This function is intended to be used when the serial port is configured for 'SDI-12' protocol.
         * </para>
         * </summary>
         * <param name="sensorAddr">
         *   the sensor address, as a string
         * </param>
         * <param name="cmd">
         *   the SDI12 query to send (without address and exclamation point)
         * </param>
         * <param name="maxWait">
         *   the maximum timeout to wait for a reply from sensor, in millisecond
         * </param>
         * <returns>
         *   the reply returned by the sensor, without newline, as a string.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty string.
         * </para>
         */
        public virtual string querySdi12(string sensorAddr, string cmd, int maxWait)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
            }
            return _func.querySdi12(sensorAddr, cmd, maxWait);
        }

        /**
         * <summary>
         *   Sends a discovery command to the bus, and reads the sensor information reply.
         * <para>
         *   This function is intended to be used when the serial port is configured for 'SDI-12' protocol.
         *   This function work when only one sensor is connected.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the reply returned by the sensor, as a YSdi12SensorInfo object.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty string.
         * </para>
         */
        public virtual YSdi12SensorInfoProxy discoverSingleSensor()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
            }
            return new YSdi12SensorInfoProxy(_func.discoverSingleSensor());
        }

        /**
         * <summary>
         *   Sends a discovery command to the bus, and reads all sensors information reply.
         * <para>
         *   This function is intended to be used when the serial port is configured for 'SDI-12' protocol.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   all the information from every connected sensor, as an array of YSdi12SensorInfo object.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty string.
         * </para>
         */
        public virtual YSdi12SensorInfoProxy[] discoverAllSensors()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
            }
            int i;
            int arrlen;
            YSdi12SensorInfo[] std_res;
            YSdi12SensorInfoProxy[] proxy_res;
            std_res = _func.discoverAllSensors().ToArray();
            arrlen = std_res.Length;
            proxy_res = new YSdi12SensorInfoProxy[arrlen];
            i = 0;
            while (i < arrlen) {
                proxy_res[i] = new YSdi12SensorInfoProxy(std_res[i]);
                i = i + 1;
            }
            return proxy_res;
        }

        /**
         * <summary>
         *   Sends a mesurement command to the SDI-12 bus, and reads the sensor immediate reply.
         * <para>
         *   The supported commands are:
         *   M: Measurement start control
         *   M1...M9: Additional measurement start command
         *   D: Measurement reading control
         *   This function is intended to be used when the serial port is configured for 'SDI-12' protocol.
         * </para>
         * </summary>
         * <param name="sensorAddr">
         *   the sensor address, as a string
         * </param>
         * <param name="measCmd">
         *   the SDI12 query to send (without address and exclamation point)
         * </param>
         * <param name="maxWait">
         *   the maximum timeout to wait for a reply from sensor, in millisecond
         * </param>
         * <returns>
         *   the reply returned by the sensor, without newline, as a list of float.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty string.
         * </para>
         */
        public virtual double[] readSensor(string sensorAddr, string measCmd, int maxWait)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
            }
            return _func.readSensor(sensorAddr, measCmd, maxWait).ToArray();
        }

        /**
         * <summary>
         *   Changes the address of the selected sensor, and returns the sensor information with the new address.
         * <para>
         *   This function is intended to be used when the serial port is configured for 'SDI-12' protocol.
         * </para>
         * </summary>
         * <param name="oldAddress">
         *   Actual sensor address, as a string
         * </param>
         * <param name="newAddress">
         *   New sensor address, as a string
         * </param>
         * <returns>
         *   the sensor address and information , as a YSdi12SensorInfo object.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty string.
         * </para>
         */
        public virtual YSdi12SensorInfoProxy changeAddress(string oldAddress, string newAddress)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
            }
            return new YSdi12SensorInfoProxy(_func.changeAddress(oldAddress, newAddress));
        }

        /**
         * <summary>
         *   Sends a information command to the bus, and reads sensors information selected.
         * <para>
         *   This function is intended to be used when the serial port is configured for 'SDI-12' protocol.
         * </para>
         * </summary>
         * <param name="sensorAddr">
         *   Sensor address, as a string
         * </param>
         * <returns>
         *   the reply returned by the sensor, as a YSdi12Port object.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty string.
         * </para>
         */
        public virtual YSdi12SensorInfoProxy getSensorInformation(string sensorAddr)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
            }
            return new YSdi12SensorInfoProxy(_func.getSensorInformation(sensorAddr));
        }

        /**
         * <summary>
         *   Sends a information command to the bus, and reads sensors information selected.
         * <para>
         *   This function is intended to be used when the serial port is configured for 'SDI-12' protocol.
         * </para>
         * </summary>
         * <param name="sensorAddr">
         *   Sensor address, as a string
         * </param>
         * <returns>
         *   the reply returned by the sensor, as a YSdi12Port object.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty string.
         * </para>
         */
        public virtual double[] readConcurrentMeasurements(string sensorAddr)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
            }
            return _func.readConcurrentMeasurements(sensorAddr).ToArray();
        }

        /**
         * <summary>
         *   Sends a information command to the bus, and reads sensors information selected.
         * <para>
         *   This function is intended to be used when the serial port is configured for 'SDI-12' protocol.
         * </para>
         * </summary>
         * <param name="sensorAddr">
         *   Sensor address, as a string
         * </param>
         * <returns>
         *   the reply returned by the sensor, as a YSdi12Port object.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty string.
         * </para>
         */
        public virtual int requestConcurrentMeasurements(string sensorAddr)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
            }
            return _func.requestConcurrentMeasurements(sensorAddr);
        }

        /**
         * <summary>
         *   Retrieves messages (both direction) in the SDI12 port buffer, starting at current position.
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
         * <param name="maxMsg">
         *   the maximum number of messages to be returned by the function; up to 254.
         * </param>
         * <returns>
         *   an array of <c>YSdi12SnoopingRecord</c> objects containing the messages found, if any.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty array.
         * </para>
         */
        public virtual YSdi12SnoopingRecordProxy[] snoopMessagesEx(int maxWait, int maxMsg)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
            }
            int i;
            int arrlen;
            YSdi12SnoopingRecord[] std_res;
            YSdi12SnoopingRecordProxy[] proxy_res;
            std_res = _func.snoopMessagesEx(maxWait, maxMsg).ToArray();
            arrlen = std_res.Length;
            proxy_res = new YSdi12SnoopingRecordProxy[arrlen];
            i = 0;
            while (i < arrlen) {
                proxy_res[i] = new YSdi12SnoopingRecordProxy(std_res[i]);
                i = i + 1;
            }
            return proxy_res;
        }

        /**
         * <summary>
         *   Retrieves messages (both direction) in the SDI12 port buffer, starting at current position.
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
         *   an array of <c>YSdi12SnoopingRecord</c> objects containing the messages found, if any.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty array.
         * </para>
         */
        public virtual YSdi12SnoopingRecordProxy[] snoopMessages(int maxWait)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Sdi12Port connected");
            }
            int i;
            int arrlen;
            YSdi12SnoopingRecord[] std_res;
            YSdi12SnoopingRecordProxy[] proxy_res;
            std_res = _func.snoopMessages(maxWait).ToArray();
            arrlen = std_res.Length;
            proxy_res = new YSdi12SnoopingRecordProxy[arrlen];
            i = 0;
            while (i < arrlen) {
                proxy_res[i] = new YSdi12SnoopingRecordProxy(std_res[i]);
                i = i + 1;
            }
            return proxy_res;
        }
    }
    //--- (end of YSdi12Port implementation)
}

