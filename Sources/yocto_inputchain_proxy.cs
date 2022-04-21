/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YInputChainProxy, the Proxy API for InputChain
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
    //--- (YInputChain class start)
    static public partial class YoctoProxyManager
    {
        public static YInputChainProxy FindInputChain(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YInputChain func = null;
            YInputChainProxy res = (YInputChainProxy)YFunctionProxy.FindSimilarUnknownFunction("YInputChainProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YInputChainProxy)YFunctionProxy.FindSimilarKnownFunction("YInputChainProxy");
                if (res != null) return res;
                func = YInputChain.FirstInputChain();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YInputChainProxy)func.get_userData();
                    }
                }
            } else {
                func = YInputChain.FindInputChain(name);
                if (func.get_userData() != null) {
                    return (YInputChainProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YInputChainProxy(func, name);
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
 *   The <c>YInputChain</c> class provides access to separate
 *   digital inputs connected in a chain.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YInputChainProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a digital input chain for a given identifier.
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
         *   This function does not require that the digital input chain is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YInputChain.isOnline()</c> to test if the digital input chain is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a digital input chain by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the digital input chain, for instance
         *   <c>MyDevice.inputChain</c>.
         * </param>
         * <returns>
         *   a <c>YInputChain</c> object allowing you to drive the digital input chain.
         * </returns>
         */
        public static YInputChainProxy FindInputChain(string func)
        {
            return YoctoProxyManager.FindInputChain(func);
        }
        //--- (end of YInputChain class start)
        //--- (YInputChain definitions)
        public const int _ExpectedNodes_INVALID = -1;
        public const int _DetectedNodes_INVALID = -1;
        public const int _LoopbackTest_INVALID = 0;
        public const int _LoopbackTest_OFF = 1;
        public const int _LoopbackTest_ON = 2;
        public const int _RefreshRate_INVALID = -1;
        public const string _BitChain1_INVALID = YAPI.INVALID_STRING;
        public const string _BitChain2_INVALID = YAPI.INVALID_STRING;
        public const string _BitChain3_INVALID = YAPI.INVALID_STRING;
        public const string _BitChain4_INVALID = YAPI.INVALID_STRING;
        public const string _BitChain5_INVALID = YAPI.INVALID_STRING;
        public const string _BitChain6_INVALID = YAPI.INVALID_STRING;
        public const string _BitChain7_INVALID = YAPI.INVALID_STRING;
        public const int _WatchdogPeriod_INVALID = -1;
        public const int _ChainDiags_INVALID = -1;

        // reference to real YoctoAPI object
        protected new YInputChain _func;
        // property cache
        protected int _expectedNodes = _ExpectedNodes_INVALID;
        protected int _refreshRate = _RefreshRate_INVALID;
        protected int _watchdogPeriod = _WatchdogPeriod_INVALID;
        //--- (end of YInputChain definitions)

        //--- (YInputChain implementation)
        internal YInputChainProxy(YInputChain hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("InputChain " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YInputChain) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YInputChain hwd = YInputChain.FindInputChain(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YInputChain hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering InputChain callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type InputChain available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YInputChain.FindInputChain</c> to obtain an object that can control the
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
            YInputChain it = YInputChain.FirstInputChain();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextInputChain();
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
            _expectedNodes = _func.get_expectedNodes();
            _refreshRate = _func.get_refreshRate();
            _watchdogPeriod = _func.get_watchdogPeriod();
        }

        /**
         * <summary>
         *   Returns the number of nodes expected in the chain.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of nodes expected in the chain
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputChain.EXPECTEDNODES_INVALID</c>.
         * </para>
         */
        public int get_expectedNodes()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No InputChain connected");
            }
            res = _func.get_expectedNodes();
            if (res == YAPI.INVALID_INT) {
                res = _ExpectedNodes_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the number of nodes expected in the chain.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the number of nodes expected in the chain
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
        public int set_expectedNodes(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputChain connected");
            }
            if (newval == _ExpectedNodes_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_expectedNodes(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Number of nodes expected in the chain.</value>
        public int ExpectedNodes
        {
            get
            {
                if (_func == null) {
                    return _ExpectedNodes_INVALID;
                }
                if (_online) {
                    return _expectedNodes;
                }
                return _ExpectedNodes_INVALID;
            }
            set
            {
                setprop_expectedNodes(value);
            }
        }

        // private helper for magic property
        private void setprop_expectedNodes(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _ExpectedNodes_INVALID) {
                return;
            }
            if (newval == _expectedNodes) {
                return;
            }
            _func.set_expectedNodes(newval);
            _expectedNodes = newval;
        }

        /**
         * <summary>
         *   Returns the number of nodes detected in the chain.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of nodes detected in the chain
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputChain.DETECTEDNODES_INVALID</c>.
         * </para>
         */
        public int get_detectedNodes()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No InputChain connected");
            }
            res = _func.get_detectedNodes();
            if (res == YAPI.INVALID_INT) {
                res = _DetectedNodes_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the activation state of the exhaustive chain connectivity test.
         * <para>
         *   The connectivity test requires a cable connecting the end of the chain
         *   to the loopback test connector.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YInputChain.LOOPBACKTEST_OFF</c> or <c>YInputChain.LOOPBACKTEST_ON</c>, according to the
         *   activation state of the exhaustive chain connectivity test
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputChain.LOOPBACKTEST_INVALID</c>.
         * </para>
         */
        public int get_loopbackTest()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputChain connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_loopbackTest()+1;
        }

        /**
         * <summary>
         *   Changes the activation state of the exhaustive chain connectivity test.
         * <para>
         *   The connectivity test requires a cable connecting the end of the chain
         *   to the loopback test connector.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YInputChain.LOOPBACKTEST_OFF</c> or <c>YInputChain.LOOPBACKTEST_ON</c>, according to the
         *   activation state of the exhaustive chain connectivity test
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
        public int set_loopbackTest(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputChain connected");
            }
            if (newval == _LoopbackTest_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_loopbackTest(newval-1);
        }

        /**
         * <summary>
         *   Returns the desired refresh rate, measured in Hz.
         * <para>
         *   The higher the refresh rate is set, the higher the
         *   communication speed on the chain will be.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the desired refresh rate, measured in Hz
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputChain.REFRESHRATE_INVALID</c>.
         * </para>
         */
        public int get_refreshRate()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No InputChain connected");
            }
            res = _func.get_refreshRate();
            if (res == YAPI.INVALID_INT) {
                res = _RefreshRate_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the desired refresh rate, measured in Hz.
         * <para>
         *   The higher the refresh rate is set, the higher the
         *   communication speed on the chain will be.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the desired refresh rate, measured in Hz
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
        public int set_refreshRate(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputChain connected");
            }
            if (newval == _RefreshRate_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_refreshRate(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Desired refresh rate, measured in Hz.</value>
        public int RefreshRate
        {
            get
            {
                if (_func == null) {
                    return _RefreshRate_INVALID;
                }
                if (_online) {
                    return _refreshRate;
                }
                return _RefreshRate_INVALID;
            }
            set
            {
                setprop_refreshRate(value);
            }
        }

        // private helper for magic property
        private void setprop_refreshRate(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _RefreshRate_INVALID) {
                return;
            }
            if (newval == _refreshRate) {
                return;
            }
            _func.set_refreshRate(newval);
            _refreshRate = newval;
        }

        /**
         * <summary>
         *   Returns the state of input 1 for all nodes of the input chain,
         *   as a hexadecimal string.
         * <para>
         *   The node nearest to the controller
         *   is the lowest bit of the result.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the state of input 1 for all nodes of the input chain,
         *   as a hexadecimal string
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputChain.BITCHAIN1_INVALID</c>.
         * </para>
         */
        public string get_bitChain1()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputChain connected");
            }
            return _func.get_bitChain1();
        }

        /**
         * <summary>
         *   Returns the state of input 2 for all nodes of the input chain,
         *   as a hexadecimal string.
         * <para>
         *   The node nearest to the controller
         *   is the lowest bit of the result.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the state of input 2 for all nodes of the input chain,
         *   as a hexadecimal string
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputChain.BITCHAIN2_INVALID</c>.
         * </para>
         */
        public string get_bitChain2()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputChain connected");
            }
            return _func.get_bitChain2();
        }

        /**
         * <summary>
         *   Returns the state of input 3 for all nodes of the input chain,
         *   as a hexadecimal string.
         * <para>
         *   The node nearest to the controller
         *   is the lowest bit of the result.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the state of input 3 for all nodes of the input chain,
         *   as a hexadecimal string
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputChain.BITCHAIN3_INVALID</c>.
         * </para>
         */
        public string get_bitChain3()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputChain connected");
            }
            return _func.get_bitChain3();
        }

        /**
         * <summary>
         *   Returns the state of input 4 for all nodes of the input chain,
         *   as a hexadecimal string.
         * <para>
         *   The node nearest to the controller
         *   is the lowest bit of the result.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the state of input 4 for all nodes of the input chain,
         *   as a hexadecimal string
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputChain.BITCHAIN4_INVALID</c>.
         * </para>
         */
        public string get_bitChain4()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputChain connected");
            }
            return _func.get_bitChain4();
        }

        /**
         * <summary>
         *   Returns the state of input 5 for all nodes of the input chain,
         *   as a hexadecimal string.
         * <para>
         *   The node nearest to the controller
         *   is the lowest bit of the result.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the state of input 5 for all nodes of the input chain,
         *   as a hexadecimal string
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputChain.BITCHAIN5_INVALID</c>.
         * </para>
         */
        public string get_bitChain5()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputChain connected");
            }
            return _func.get_bitChain5();
        }

        /**
         * <summary>
         *   Returns the state of input 6 for all nodes of the input chain,
         *   as a hexadecimal string.
         * <para>
         *   The node nearest to the controller
         *   is the lowest bit of the result.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the state of input 6 for all nodes of the input chain,
         *   as a hexadecimal string
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputChain.BITCHAIN6_INVALID</c>.
         * </para>
         */
        public string get_bitChain6()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputChain connected");
            }
            return _func.get_bitChain6();
        }

        /**
         * <summary>
         *   Returns the state of input 7 for all nodes of the input chain,
         *   as a hexadecimal string.
         * <para>
         *   The node nearest to the controller
         *   is the lowest bit of the result.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the state of input 7 for all nodes of the input chain,
         *   as a hexadecimal string
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputChain.BITCHAIN7_INVALID</c>.
         * </para>
         */
        public string get_bitChain7()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputChain connected");
            }
            return _func.get_bitChain7();
        }

        /**
         * <summary>
         *   Returns the wait time in seconds before triggering an inactivity
         *   timeout error.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the wait time in seconds before triggering an inactivity
         *   timeout error
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputChain.WATCHDOGPERIOD_INVALID</c>.
         * </para>
         */
        public int get_watchdogPeriod()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No InputChain connected");
            }
            res = _func.get_watchdogPeriod();
            if (res == YAPI.INVALID_INT) {
                res = _WatchdogPeriod_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the wait time in seconds before triggering an inactivity
         *   timeout error.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method
         *   of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the wait time in seconds before triggering an inactivity
         *   timeout error
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
        public int set_watchdogPeriod(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputChain connected");
            }
            if (newval == _WatchdogPeriod_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_watchdogPeriod(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Wait time in seconds before triggering an inactivity</value>
        public int WatchdogPeriod
        {
            get
            {
                if (_func == null) {
                    return _WatchdogPeriod_INVALID;
                }
                if (_online) {
                    return _watchdogPeriod;
                }
                return _WatchdogPeriod_INVALID;
            }
            set
            {
                setprop_watchdogPeriod(value);
            }
        }

        // private helper for magic property
        private void setprop_watchdogPeriod(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _WatchdogPeriod_INVALID) {
                return;
            }
            if (newval == _watchdogPeriod) {
                return;
            }
            _func.set_watchdogPeriod(newval);
            _watchdogPeriod = newval;
        }

        /**
         * <summary>
         *   Returns the controller state diagnostics.
         * <para>
         *   Bit 0 indicates a chain length
         *   error, bit 1 indicates an inactivity timeout and bit 2 indicates
         *   a loopback test failure.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the controller state diagnostics
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputChain.CHAINDIAGS_INVALID</c>.
         * </para>
         */
        public int get_chainDiags()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No InputChain connected");
            }
            res = _func.get_chainDiags();
            if (res == YAPI.INVALID_INT) {
                res = _ChainDiags_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Resets the application watchdog countdown.
         * <para>
         *   If you have setup a non-zero <c>watchdogPeriod</c>, you should
         *   call this function on a regular basis to prevent the application
         *   inactivity error to be triggered.
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
        public virtual int resetWatchdog()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputChain connected");
            }
            return _func.resetWatchdog();
        }

        /**
         * <summary>
         *   Returns a string with last events observed on the digital input chain.
         * <para>
         *   This method return only events that are still buffered in the device memory.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string with last events observed (one per line).
         * </returns>
         * <para>
         *   On failure, throws an exception or returns  <c>YAPI.INVALID_STRING</c>.
         * </para>
         */
        public virtual string get_lastEvents()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputChain connected");
            }
            return _func.get_lastEvents();
        }
    }
    //--- (end of YInputChain implementation)
}

