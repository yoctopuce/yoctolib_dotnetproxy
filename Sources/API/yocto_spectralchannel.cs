namespace YoctoLib 
{/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements yFindSpectralChannel(), the high-level API for SpectralChannel functions
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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using YDEV_DESCR = System.Int32;
using YFUN_DESCR = System.Int32;

#pragma warning disable 1591
//--- (YSpectralChannel return codes)
//--- (end of YSpectralChannel return codes)
//--- (YSpectralChannel dlldef_core)
//--- (end of YSpectralChannel dlldef_core)
//--- (YSpectralChannel dll_core_map)
//--- (end of YSpectralChannel dll_core_map)
//--- (YSpectralChannel dlldef)
//--- (end of YSpectralChannel dlldef)
//--- (YSpectralChannel yapiwrapper)
//--- (end of YSpectralChannel yapiwrapper)
//--- (YSpectralChannel class start)
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
public class YSpectralChannel : YSensor
{
//--- (end of YSpectralChannel class start)
    //--- (YSpectralChannel definitions)
    public new delegate void ValueCallback(YSpectralChannel func, string value);
    public new delegate void TimedReportCallback(YSpectralChannel func, YMeasure measure);

    public const int RAWCOUNT_INVALID = YAPI.INVALID_INT;
    public const string CHANNELNAME_INVALID = YAPI.INVALID_STRING;
    public const int PEAKWAVELENGTH_INVALID = YAPI.INVALID_INT;
    protected int _rawCount = RAWCOUNT_INVALID;
    protected string _channelName = CHANNELNAME_INVALID;
    protected int _peakWavelength = PEAKWAVELENGTH_INVALID;
    protected ValueCallback _valueCallbackSpectralChannel = null;
    protected TimedReportCallback _timedReportCallbackSpectralChannel = null;
    //--- (end of YSpectralChannel definitions)

    public YSpectralChannel(string func)
        : base(func)
    {
        _className = "SpectralChannel";
        //--- (YSpectralChannel attributes initialization)
        //--- (end of YSpectralChannel attributes initialization)
    }

    //--- (YSpectralChannel implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("rawCount"))
        {
            _rawCount = json_val.getInt("rawCount");
        }
        if (json_val.has("channelName"))
        {
            _channelName = json_val.getString("channelName");
        }
        if (json_val.has("peakWavelength"))
        {
            _peakWavelength = json_val.getInt("peakWavelength");
        }
        base._parseAttr(json_val);
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
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return RAWCOUNT_INVALID;
                }
            }
            res = this._rawCount;
        }
        return res;
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
        string res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return CHANNELNAME_INVALID;
                }
            }
            res = this._channelName;
        }
        return res;
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
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return PEAKWAVELENGTH_INVALID;
                }
            }
            res = this._peakWavelength;
        }
        return res;
    }


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
    public static YSpectralChannel FindSpectralChannel(string func)
    {
        YSpectralChannel obj;
        lock (YAPI.globalLock) {
            obj = (YSpectralChannel) YFunction._FindFromCache("SpectralChannel", func);
            if (obj == null) {
                obj = new YSpectralChannel(func);
                YFunction._AddToCache("SpectralChannel", func, obj);
            }
        }
        return obj;
    }


    /**
     * <summary>
     *   Registers the callback function that is invoked on every change of advertised value.
     * <para>
     *   The callback is invoked only during the execution of <c>ySleep</c> or <c>yHandleEvents</c>.
     *   This provides control over the time when the callback is triggered. For good responsiveness, remember to call
     *   one of these two functions periodically. To unregister a callback, pass a null pointer as argument.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="callback">
     *   the callback function to call, or a null pointer. The callback function should take two
     *   arguments: the function object of which the value has changed, and the character string describing
     *   the new advertised value.
     * @noreturn
     * </param>
     */
    public int registerValueCallback(ValueCallback callback)
    {
        string val;
        if (callback != null) {
            YFunction._UpdateValueCallbackList(this, true);
        } else {
            YFunction._UpdateValueCallbackList(this, false);
        }
        this._valueCallbackSpectralChannel = callback;
        // Immediately invoke value callback with current value
        if (callback != null && this.isOnline()) {
            val = this._advertisedValue;
            if (!(val == "")) {
                this._invokeValueCallback(val);
            }
        }
        return 0;
    }


    public override int _invokeValueCallback(string value)
    {
        if (this._valueCallbackSpectralChannel != null) {
            this._valueCallbackSpectralChannel(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }


    /**
     * <summary>
     *   Registers the callback function that is invoked on every periodic timed notification.
     * <para>
     *   The callback is invoked only during the execution of <c>ySleep</c> or <c>yHandleEvents</c>.
     *   This provides control over the time when the callback is triggered. For good responsiveness, remember to call
     *   one of these two functions periodically. To unregister a callback, pass a null pointer as argument.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="callback">
     *   the callback function to call, or a null pointer. The callback function should take two
     *   arguments: the function object of which the value has changed, and an <c>YMeasure</c> object describing
     *   the new advertised value.
     * @noreturn
     * </param>
     */
    public int registerTimedReportCallback(TimedReportCallback callback)
    {
        YSensor sensor;
        sensor = this;
        if (callback != null) {
            YFunction._UpdateTimedReportCallbackList(sensor, true);
        } else {
            YFunction._UpdateTimedReportCallbackList(sensor, false);
        }
        this._timedReportCallbackSpectralChannel = callback;
        return 0;
    }


    public override int _invokeTimedReportCallback(YMeasure value)
    {
        if (this._timedReportCallbackSpectralChannel != null) {
            this._timedReportCallbackSpectralChannel(this, value);
        } else {
            base._invokeTimedReportCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of spectral analysis channels started using <c>yFirstSpectralChannel()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned spectral analysis channels order.
     *   If you want to find a specific a spectral analysis channel, use <c>SpectralChannel.findSpectralChannel()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YSpectralChannel</c> object, corresponding to
     *   a spectral analysis channel currently online, or a <c>null</c> pointer
     *   if there are no more spectral analysis channels to enumerate.
     * </returns>
     */
    public YSpectralChannel nextSpectralChannel()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindSpectralChannel(hwid);
    }

    //--- (end of YSpectralChannel implementation)

    //--- (YSpectralChannel functions)

    /**
     * <summary>
     *   Starts the enumeration of spectral analysis channels currently accessible.
     * <para>
     *   Use the method <c>YSpectralChannel.nextSpectralChannel()</c> to iterate on
     *   next spectral analysis channels.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YSpectralChannel</c> object, corresponding to
     *   the first spectral analysis channel currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YSpectralChannel FirstSpectralChannel()
    {
        YFUN_DESCR[] v_fundescr = new YFUN_DESCR[1];
        YDEV_DESCR dev = default(YDEV_DESCR);
        int neededsize = 0;
        int err = 0;
        string serial = null;
        string funcId = null;
        string funcName = null;
        string funcVal = null;
        string errmsg = "";
        int size = Marshal.SizeOf(v_fundescr[0]);
        IntPtr p = Marshal.AllocHGlobal(Marshal.SizeOf(v_fundescr[0]));
        err = YAPI.apiGetFunctionsByClass("SpectralChannel", 0, p, size, ref neededsize, ref errmsg);
        Marshal.Copy(p, v_fundescr, 0, 1);
        Marshal.FreeHGlobal(p);
        if ((YAPI.YISERR(err) | (neededsize == 0)))
            return null;
        serial = "";
        funcId = "";
        funcName = "";
        funcVal = "";
        errmsg = "";
        if ((YAPI.YISERR(YAPI.yapiGetFunctionInfo(v_fundescr[0], ref dev, ref serial, ref funcId, ref funcName, ref funcVal, ref errmsg))))
            return null;
        return FindSpectralChannel(serial + "." + funcId);
    }

    //--- (end of YSpectralChannel functions)
}
#pragma warning restore 1591


}
