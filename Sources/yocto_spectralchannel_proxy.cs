/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YSpectralChannelProxy, the Proxy API for SpectralChannel
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
    //--- (YSpectralChannel class start)
    static public partial class YoctoProxyManager
    {
        public static YSpectralChannelProxy FindSpectralChannel(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YSpectralChannel func = null;
            YSpectralChannelProxy res = (YSpectralChannelProxy)YFunctionProxy.FindSimilarUnknownFunction("YSpectralChannelProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YSpectralChannelProxy)YFunctionProxy.FindSimilarKnownFunction("YSpectralChannelProxy");
                if (res != null) return res;
                func = YSpectralChannel.FirstSpectralChannel();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YSpectralChannelProxy)func.get_userData();
                    }
                }
            } else {
                func = YSpectralChannel.FindSpectralChannel(name);
                if (func.get_userData() != null) {
                    return (YSpectralChannelProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YSpectralChannelProxy(func, name);
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
 *   The <c>YSpectralChannel</c> class allows you to read and configure Yoctopuce spectral analysis channels.
 * <para>
 *   It inherits from <c>YSensor</c> class the core functions to read measures,
 *   to register callback functions, and to access the autonomous datalogger.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YSpectralChannelProxy : YSensorProxy
    {
        /**
         * <summary>
         *   Retrieves a spectral analysis channel for a given identifier.
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
         *   This function does not require that the spectral analysis channel is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YSpectralChannel.isOnline()</c> to test if the spectral analysis channel is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a spectral analysis channel by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the spectral analysis channel, for instance
         *   <c>MyDevice.spectralChannel1</c>.
         * </param>
         * <returns>
         *   a <c>YSpectralChannel</c> object allowing you to drive the spectral analysis channel.
         * </returns>
         */
        public static YSpectralChannelProxy FindSpectralChannel(string func)
        {
            return YoctoProxyManager.FindSpectralChannel(func);
        }
        //--- (end of YSpectralChannel class start)
        //--- (YSpectralChannel definitions)
        public const int _RawCount_INVALID = YAPI.INVALID_INT;
        public const string _ChannelName_INVALID = YAPI.INVALID_STRING;
        public const int _PeakWavelength_INVALID = YAPI.INVALID_INT;

        // reference to real YoctoAPI object
        protected new YSpectralChannel _func;
        // property cache
        //--- (end of YSpectralChannel definitions)

        //--- (YSpectralChannel implementation)
        internal YSpectralChannelProxy(YSpectralChannel hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("SpectralChannel " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YSpectralChannel) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YSpectralChannel hwd = YSpectralChannel.FindSpectralChannel(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YSpectralChannel hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering SpectralChannel callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type SpectralChannel available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YSpectralChannel.FindSpectralChannel</c> to obtain an object that can control the
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
            YSpectralChannel it = YSpectralChannel.FirstSpectralChannel();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextSpectralChannel();
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
        }

        /**
         * <summary>
         *   Retrieves the raw spectral intensity value as measured by the sensor, without any scaling or calibration.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSpectralChannel.RAWCOUNT_INVALID</c>.
         * </para>
         */
        public int get_rawCount()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpectralChannel connected");
            }
            return _func.get_rawCount();
        }

        /**
         * <summary>
         *   Returns the target spectral band name.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the target spectral band name
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSpectralChannel.CHANNELNAME_INVALID</c>.
         * </para>
         */
        public string get_channelName()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpectralChannel connected");
            }
            return _func.get_channelName();
        }

        /**
         * <summary>
         *   Returns the target spectral band peak wavelenght, in nm.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the target spectral band peak wavelenght, in nm
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSpectralChannel.PEAKWAVELENGTH_INVALID</c>.
         * </para>
         */
        public int get_peakWavelength()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SpectralChannel connected");
            }
            return _func.get_peakWavelength();
        }
    }
    //--- (end of YSpectralChannel implementation)
}

