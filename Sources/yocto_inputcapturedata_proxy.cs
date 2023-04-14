/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YInputCaptureDataProxy, the Proxy API for InputCaptureData
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
    //--- (YInputCaptureData class start)
    public class YInputCaptureDataProxy
    {
        private YInputCaptureData _objref;
        internal YInputCaptureDataProxy(YInputCaptureData objref)
        {
             _objref = objref;
        }
        //--- (end of YInputCaptureData class start)
        //--- (YInputCaptureData definitions)
        //--- (end of YInputCaptureData definitions)
        //--- (YInputCaptureData implementation)

        /**
         * <summary>
         *   Returns the number of series available in the capture.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of
         *   simultaneous data series available.
         * </returns>
         */
        public virtual int get_serieCount()
        {
            return _objref.get_serieCount();
        }

        // property with cached value for instant access (storage object)
        public int SerieCount
        {
            get
            {
                return this.get_serieCount();
            }
        }

        /**
         * <summary>
         *   Returns the number of records captured (in a serie).
         * <para>
         *   In the exceptional case where it was not possible
         *   to transfer all data in time, the number of records
         *   actually present in the series might be lower than
         *   the number of records captured
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of
         *   records expected in each serie.
         * </returns>
         */
        public virtual int get_recordCount()
        {
            return _objref.get_recordCount();
        }

        // property with cached value for instant access (storage object)
        public int RecordCount
        {
            get
            {
                return this.get_recordCount();
            }
        }

        /**
         * <summary>
         *   Returns the effective sampling rate of the device.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of
         *   samples taken each second.
         * </returns>
         */
        public virtual int get_samplingRate()
        {
            return _objref.get_samplingRate();
        }

        // property with cached value for instant access (storage object)
        public int SamplingRate
        {
            get
            {
                return this.get_samplingRate();
            }
        }

        /**
         * <summary>
         *   Returns the type of automatic conditional capture
         *   that triggered the capture of this data sequence.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the type of conditional capture.
         * </returns>
         */
        public virtual int get_captureType()
        {
            return _objref.get_captureType()+1;
        }

        // property with cached value for instant access (storage object)
        public int CaptureType
        {
            get
            {
                return this.get_captureType();
            }
        }

        /**
         * <summary>
         *   Returns the threshold value that triggered
         *   this automatic conditional capture, if it was
         *   not an instant captured triggered manually.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the conditional threshold value
         *   at the time of capture.
         * </returns>
         */
        public virtual double get_triggerValue()
        {
            double res;
            res = _objref.get_triggerValue();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        // property with cached value for instant access (storage object)
        public double TriggerValue
        {
            get
            {
                return this.get_triggerValue();
            }
        }

        /**
         * <summary>
         *   Returns the index in the series of the sample
         *   corresponding to the exact time when the capture
         *   was triggered.
         * <para>
         *   In case of trigger based on average
         *   or RMS value, the trigger index corresponds to
         *   the end of the averaging period.
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to a position
         *   in the data serie.
         * </returns>
         */
        public virtual int get_triggerPosition()
        {
            return _objref.get_triggerPosition();
        }

        // property with cached value for instant access (storage object)
        public int TriggerPosition
        {
            get
            {
                return this.get_triggerPosition();
            }
        }

        /**
         * <summary>
         *   Returns the absolute time when the capture was
         *   triggered, as a Unix timestamp.
         * <para>
         *   Milliseconds are
         *   included in this timestamp (floating-point number).
         * </para>
         * </summary>
         * <returns>
         *   a floating-point number corresponding to
         *   the number of seconds between the Jan 1,
         *   1970 and the moment where the capture
         *   was triggered.
         * </returns>
         */
        public virtual double get_triggerRealTimeUTC()
        {
            double res;
            res = _objref.get_triggerRealTimeUTC();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        // property with cached value for instant access (storage object)
        public double TriggerRealTimeUTC
        {
            get
            {
                return this.get_triggerRealTimeUTC();
            }
        }

        /**
         * <summary>
         *   Returns the unit of measurement for data points in
         *   the first serie.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string containing to a physical unit of
         *   measurement.
         * </returns>
         */
        public virtual string get_serie1Unit()
        {
            return _objref.get_serie1Unit();
        }

        // property with cached value for instant access (storage object)
        public string Serie1Unit
        {
            get
            {
                return this.get_serie1Unit();
            }
        }

        /**
         * <summary>
         *   Returns the unit of measurement for data points in
         *   the second serie.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string containing to a physical unit of
         *   measurement.
         * </returns>
         */
        public virtual string get_serie2Unit()
        {
            return _objref.get_serie2Unit();
        }

        // property with cached value for instant access (storage object)
        public string Serie2Unit
        {
            get
            {
                return this.get_serie2Unit();
            }
        }

        /**
         * <summary>
         *   Returns the unit of measurement for data points in
         *   the third serie.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string containing to a physical unit of
         *   measurement.
         * </returns>
         */
        public virtual string get_serie3Unit()
        {
            return _objref.get_serie3Unit();
        }

        // property with cached value for instant access (storage object)
        public string Serie3Unit
        {
            get
            {
                return this.get_serie3Unit();
            }
        }

        /**
         * <summary>
         *   Returns the sampled data corresponding to the first serie.
         * <para>
         *   The corresponding physical unit can be obtained
         *   using the method <c>get_serie1Unit()</c>.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a list of real numbers corresponding to all
         *   samples received for serie 1.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty array.
         * </para>
         */
        public virtual double[] get_serie1Values()
        {
            return _objref.get_serie1Values().ToArray();
        }

        // property with cached value for instant access (storage object)
        public double[] Serie1Values
        {
            get
            {
                return this.get_serie1Values();
            }
        }

        /**
         * <summary>
         *   Returns the sampled data corresponding to the second serie.
         * <para>
         *   The corresponding physical unit can be obtained
         *   using the method <c>get_serie2Unit()</c>.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a list of real numbers corresponding to all
         *   samples received for serie 2.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty array.
         * </para>
         */
        public virtual double[] get_serie2Values()
        {
            return _objref.get_serie2Values().ToArray();
        }

        // property with cached value for instant access (storage object)
        public double[] Serie2Values
        {
            get
            {
                return this.get_serie2Values();
            }
        }

        /**
         * <summary>
         *   Returns the sampled data corresponding to the third serie.
         * <para>
         *   The corresponding physical unit can be obtained
         *   using the method <c>get_serie3Unit()</c>.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a list of real numbers corresponding to all
         *   samples received for serie 3.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty array.
         * </para>
         */
        public virtual double[] get_serie3Values()
        {
            return _objref.get_serie3Values().ToArray();
        }

        // property with cached value for instant access (storage object)
        public double[] Serie3Values
        {
            get
            {
                return this.get_serie3Values();
            }
        }
    }
    //--- (end of YInputCaptureData implementation)
}

