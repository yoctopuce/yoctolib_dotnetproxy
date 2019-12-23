/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YDataSetProxy, the Proxy API for DataSet
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
    //--- (generated code: YDataSet class start)
    public class YDataSetProxy
    {
        private YDataSet _objref;
        internal YDataSetProxy(YDataSet objref)
        {
             _objref = objref;
             _objref.loadMore();
        }
        //--- (end of generated code: YDataSet class start)

        /**
         * <summary>
         *   Retrieves a <c>YDataSet</c> object holding historical data for a
         *   sensor given by its name or hardware identifier, for a specified time interval.
         * <para>
         *   The measures will be retrieved from the data logger, which must have been turned
         *   on at the desired time. Methods of the <c>YDataSet</c>
         *   class makes it possible to get an overview of the
         *   recorded data, and to load progressively a large set
         *   of measures from the data logger.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="sensorName">
         *   logical name or hardware identifier of the sensor
         *   for which data logger records are requested.
         * </param>
         * <param name="startTime">
         *   the start of the desired measure time interval,
         *   as a Unix timestamp, i.e. the number of seconds since
         *   January 1, 1970 UTC. The special value 0 can be used
         *   to include any measure, without initial limit.
         * </param>
         * <param name="endTime">
         *   the end of the desired measure time interval,
         *   as a Unix timestamp, i.e. the number of seconds since
         *   January 1, 1970 UTC. The special value 0 can be used
         *   to include any measure, without ending limit.
         * </param>
         * <returns>
         *   an instance of <c>YDataSet</c>, providing access to historical
         *   data. Past measures can be loaded progressively
         *   using methods from the <c>YDataSet</c> object.
         * </returns>
         */
        public static YDataSetProxy Init(string sensorName, double startTime, double endTime)
        {
            YSensorProxy sensor = YSensorProxy.FindSensor(sensorName);
            if (!sensor.IsOnline) throw new Exception("sensor " + sensorName + " is offline or does not exist.");
            if (startTime < 0) startTime = 0;
            if (endTime < 0) endTime = 0;
            YDataSetProxy dataset = sensor.get_recordedData(startTime, endTime);

            return dataset;
        }

        public void freeData()
        {
            _objref = null;
        }

        public string HardwareId
        {
            get {
                return get_hardwareId();
            }
        }

        public string FunctionId
        {
            get {
                return get_functionId();
            }
        }

        public string Unit
        {
            get {
                return get_unit();
            }
        }

        public int Progress
        {
            get {
                return get_progress();
            }
        }

        public double SummaryStartTime
        {
            get {
                return get_summaryStartTime();
            }
        }

        public double SummaryEndTime
        {
            get {
                return get_summaryEndTime();
            }
        }

        public double SummaryMin
        {
            get {
                return get_summaryMin();
            }
        }

        public double SummaryAvg
        {
            get {
                return get_summaryAvg();
            }
        }

        public double SummaryMax
        {
            get {
                return get_summaryMax();
            }
        }

        public int PreviewRecordCount
        {
            get {
                return get_previewRecordCount();
            }
        }

        public int MeasuresRecordCount
        {
            get {
                return get_measuresRecordCount();
            }
        }

        /**
         * <summary>
         *   Returns the start time of the first measure in the data set,
         *   relative to the Jan 1, 1970 UTC (Unix timestamp).
         * <para>
         *   When the recording rate is higher then 1 sample
         *   per second, the timestamp may have a fractional part.
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the number of seconds
         *   between the Jan 1, 1970 UTC and the beginning of this measure.
         * </returns>
         */
        public double get_summaryStartTime()
        {
            return _objref.get_summary().get_startTimeUTC();
        }

        /**
         * <summary>
         *   Returns the end time of the last measure in the data set,
         *   relative to the Jan 1, 1970 UTC (Unix timestamp).
         * <para>
         *   When the recording rate is higher then 1 sample
         *   per second, the timestamp may have a fractional part.
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the number of seconds
         *   between the Jan 1, 1970 UTC and the beginning of this measure.
         * </returns>
         */
        public double get_summaryEndTime()
        {
            return _objref.get_summary().get_endTimeUTC();
        }

        /**
         * <summary>
         *   Returns the smallest value observed during the time interval
         *   covered by this data set.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating-point number corresponding to the smallest value observed.
         * </returns>
         */
        public double get_summaryMin()
        {
            return _objref.get_summary().get_minValue();
        }

        /**
         * <summary>
         *   Returns the average value observed during the time interval
         *   covered by this data set.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating-point number corresponding to the average value observed.
         * </returns>
         */
        public double get_summaryAvg()
        {
            return _objref.get_summary().get_averageValue();
        }

        /**
         * <summary>
         *   Returns the largest value observed during the time interval
         *   covered by this data set.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating-point number corresponding to the largest value observed.
         * </returns>
         */
        public double get_summaryMax()
        {
            return _objref.get_summary().get_maxValue();
        }

        /**
         * <summary>
         *   Returns the number of entries in the preview summarizing this data set
         * </summary>
         * <returns>
         *   an integer number corresponding to the number of entries.
         * </returns>
         */
        public int get_previewRecordCount()
        {
            if (_objref == null)
            {
                string msg = "No DataSet connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_preview().Count;
        }

        /**
         * <summary>
         *   Returns the start time of the specified entry in the preview,
         *   relative to the Jan 1, 1970 UTC (Unix timestamp).
         * <para>
         *   When the recording rate is higher then 1 sample
         *   per second, the timestamp may have a fractional part.
         * </para>
         * </summary>
         * <param name="index">
         *   an integer index in the range [<c>0</c>...<c>PreviewRecordCount</c>-1].
         * </param>
         * <returns>
         *   a floating point number corresponding to the number of seconds
         *   between the Jan 1, 1970 UTC and the beginning of this measure.
         * </returns>
         */
        public double get_previewStartTimeAt(int index)
        {
            List<YMeasure> _preview = _objref.get_preview();
            if (index < 0) throw new Exception("Preview index cannot be negative");
            if (index >= _preview.Count) throw new Exception("Preview index (" + index.ToString() + ") is larger than preview record count " + _preview.Count.ToString() + " )");
            return _preview[index].get_startTimeUTC();
        }

        /**
         * <summary>
         *   Returns the end time of the specified entry in the preview,
         *   relative to the Jan 1, 1970 UTC (Unix timestamp).
         * <para>
         *   When the recording rate is higher then 1 sample
         *   per second, the timestamp may have a fractional part.
         * </para>
         * </summary>
         * <param name="index">
         *   an integer index in the range [<c>0</c>...<c>PreviewRecordCount</c>-1].
         * </param>
         * <returns>
         *   a floating point number corresponding to the number of seconds
         *   between the Jan 1, 1970 UTC and the beginning of this measure.
         * </returns>
         */
        public double get_previewEndTimeAt(int index)
        {
            List<YMeasure> _preview = _objref.get_preview();
            if (index < 0) throw new Exception("Preview index cannot be negative");
            if (index >= _preview.Count) throw new Exception("Preview index (" + index.ToString() + ") is larger than preview record count " + _preview.Count.ToString() + " )");
            return _preview[index].get_endTimeUTC();
        }

        /**
         * <summary>
         *   Returns the smallest value observed during the time interval
         *   covered by the specified entry in the preview.
         * <para>
         * </para>
         * </summary>
         * <param name="index">
         *   an integer index in the range [<c>0</c>...<c>PreviewRecordCount</c>-1].
         * </param>
         * <returns>
         *   a floating-point number corresponding to the smallest value observed.
         * </returns>
         */
        public double get_previewMinAt(int index)
        {
            List<YMeasure> _preview = _objref.get_preview();
            if (index < 0) throw new Exception("Preview index cannot be negative");
            if (index >= _preview.Count) throw new Exception("Preview index (" + index.ToString() + ") is larger than preview record count " + _preview.Count.ToString() + " )");
            return _preview[index].get_minValue();
        }

        /**
         * <summary>
         *   Returns the average value observed during the time interval
         *   covered by the specified entry in the preview.
         * <para>
         * </para>
         * </summary>
         * <param name="index">
         *   an integer index in the range [<c>0</c>...<c>PreviewRecordCount</c>-1].
         * </param>
         * <returns>
         *   a floating-point number corresponding to the average value observed.
         * </returns>
         */
        public double get_previewAvgAt(int index)
        {
            List<YMeasure> _preview = _objref.get_preview();
            if (index < 0) throw new Exception("Preview index cannot be negative");
            if (index >= _preview.Count) throw new Exception("Preview index (" + index.ToString() + ") is larger than preview record count " + _preview.Count.ToString() + " )");
            return _preview[index].get_averageValue();
        }

        /**
         * <summary>
         *   Returns the largest value observed during the time interval
         *   covered by the specified entry in the preview.
         * <para>
         * </para>
         * </summary>
         * <param name="index">
         *   an integer index in the range [<c>0</c>...<c>PreviewRecordCount</c>-1].
         * </param>
         * <returns>
         *   a floating-point number corresponding to the largest value observed.
         * </returns>
         */
        public double get_previewMaxAt(int index)
        {
            List<YMeasure> _preview = _objref.get_preview();
            if (index < 0) throw new Exception("Preview index cannot be negative");
            if (index >= _preview.Count) throw new Exception("Preview index (" + index.ToString() + ") is larger than preview record count " + _preview.Count.ToString() + " )");
            return _preview[index].get_maxValue();
        }

        /**
         * <summary>
         *   Returns the number of measurements currently loaded for this data set.
         * <para>
         *   The total number of record is only known when the data set is fully loaded,
         *   i.e. when <c>loadMore()</c> has been invoked until the progresss indicator
         *   returns 100.
         * </para>
         * </summary>
         * <returns>
         *   an integer number corresponding to the number of entries loaded.
         * </returns>
         */
        public int get_measuresRecordCount()
        {
            if (_objref == null)
            {
                string msg = "No DataSet connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_measures().Count;
        }

        /**
         * <summary>
         *   Returns the start time of the specified entry in the preview,
         *   relative to the Jan 1, 1970 UTC (Unix timestamp).
         * <para>
         *   When the recording rate is higher then 1 sample
         *   per second, the timestamp may have a fractional part.
         * </para>
         * </summary>
         * <param name="index">
         *   an integer index in the range [<c>0</c>...<c>MeasuresRecordCount</c>-1].
         * </param>
         * <returns>
         *   a floating point number corresponding to the number of seconds
         *   between the Jan 1, 1970 UTC and the beginning of this measure.
         * </returns>
         */
        public double get_measuresStartTimeAt(int index)
        {
            List<YMeasure> _data = _objref.get_measures();
            if (_data.Count == 0) throw new Exception("measures not loaded yet");
            if (index < 0) throw new Exception("measure index cannot be negative");
            if (index >= _data.Count) throw new Exception("measure index (" + index.ToString() + ") is larger than available measure count " + _data.Count.ToString() + " )");
            return _data[index].get_startTimeUTC();
        }

        /**
         * <summary>
         *   Returns the end time of the specified entry in the preview,
         *   relative to the Jan 1, 1970 UTC (Unix timestamp).
         * <para>
         *   When the recording rate is higher then 1 sample
         *   per second, the timestamp may have a fractional part.
         * </para>
         * </summary>
         * <param name="index">
         *   an integer index in the range [<c>0</c>...<c>MeasuresRecordCount</c>-1].
         * </param>
         * <returns>
         *   a floating point number corresponding to the number of seconds
         *   between the Jan 1, 1970 UTC and the beginning of this measure.
         * </returns>
         */
        public double get_measuresEndTimeAt(int index)
        {
            List<YMeasure> _data = _objref.get_measures();
            if (_data.Count == 0) throw new Exception("measures not loaded yet");
            if (index < 0) throw new Exception("measure index cannot be negative");
            if (index >= _data.Count) throw new Exception("measure index (" + index.ToString() + ") is larger than available measure count " + _data.Count.ToString() + " )");
            return _data[index].get_endTimeUTC();
        }

        /**
         * <summary>
         *   Returns the smallest value observed during the time interval
         *   covered by the specified entry in the preview.
         * <para>
         * </para>
         * </summary>
         * <param name="index">
         *   an integer index in the range [<c>0</c>...<c>MeasuresRecordCount</c>-1].
         * </param>
         * <returns>
         *   a floating-point number corresponding to the smallest value observed.
         * </returns>
         */
        public double get_measuresMinAt(int index)
        {
            List<YMeasure> _data = _objref.get_measures();
            if (_data.Count == 0) throw new Exception("measures not loaded yet");
            if (index < 0) throw new Exception("measure index cannot be negative");
            if (index >= _data.Count) throw new Exception("measure index (" + index.ToString() + ") is larger than available measure count " + _data.Count.ToString() + " )");
            return _data[index].get_minValue();
        }

        /**
         * <summary>
         *   Returns the average value observed during the time interval
         *   covered by the specified entry in the preview.
         * <para>
         * </para>
         * </summary>
         * <param name="index">
         *   an integer index in the range [<c>0</c>...<c>MeasuresRecordCount</c>-1].
         * </param>
         * <returns>
         *   a floating-point number corresponding to the average value observed.
         * </returns>
         */
        public double get_measuresAvgAt(int index)
        {
            List<YMeasure> _data = _objref.get_measures();
            if (_data.Count == 0) throw new Exception("measures not loaded yet");
            if (index < 0) throw new Exception("measure index cannot be negative");
            if (index >= _data.Count) throw new Exception("measure index (" + index.ToString() + ") is larger than available measure count " + _data.Count.ToString() + " )");
            return _data[index].get_averageValue();
        }

        /**
         * <summary>
         *   Returns the largest value observed during the time interval
         *   covered by the specified entry in the preview.
         * <para>
         * </para>
         * </summary>
         * <param name="index">
         *   an integer index in the range [<c>0</c>...<c>MeasuresRecordCount</c>-1].
         * </param>
         * <returns>
         *   a floating-point number corresponding to the largest value observed.
         * </returns>
         */
        public double get_measuresMaxAt(int index)
        {
            List<YMeasure> _data = _objref.get_measures();
            if (_data.Count == 0) throw new Exception("measures not loaded yet");
            if (index < 0) throw new Exception("measure index cannot be negative");
            if (index >= _data.Count) throw new Exception("measure index (" + index.ToString() + ") is larger than available measure count " + _data.Count.ToString() + " )");
            return _data[index].get_maxValue();
        }

        //--- (generated code: YDataSet definitions)
        //--- (end of generated code: YDataSet definitions)
        //--- (generated code: YDataSet implementation)

        /**
         * <summary>
         *   Returns the unique hardware identifier of the function who performed the measures,
         *   in the form <c>SERIAL.FUNCTIONID</c>.
         * <para>
         *   The unique hardware identifier is composed of the
         *   device serial number and of the hardware identifier of the function
         *   (for example <c>THRMCPL1-123456.temperature1</c>)
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string that uniquely identifies the function (ex: <c>THRMCPL1-123456.temperature1</c>)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns  <c>dataset._Hardwareid_INVALID</c>.
         * </para>
         */
        public virtual string get_hardwareId()
        {
            if (_objref == null)
            {
                string msg = "No DataSet connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_hardwareId();
        }

        /**
         * <summary>
         *   Returns the hardware identifier of the function that performed the measure,
         *   without reference to the module.
         * <para>
         *   For example <c>temperature1</c>.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string that identifies the function (ex: <c>temperature1</c>)
         * </returns>
         */
        public virtual string get_functionId()
        {
            if (_objref == null)
            {
                string msg = "No DataSet connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_functionId();
        }

        /**
         * <summary>
         *   Returns the measuring unit for the measured value.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string that represents a physical unit.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns  <c>dataset._Unit_INVALID</c>.
         * </para>
         */
        public virtual string get_unit()
        {
            if (_objref == null)
            {
                string msg = "No DataSet connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_unit();
        }

        /**
         * <summary>
         *   Returns the start time of the dataset, relative to the Jan 1, 1970.
         * <para>
         *   When the <c>YDataSet</c> object is created, the start time is the value passed
         *   in parameter to the <c>get_dataSet()</c> function. After the
         *   very first call to <c>loadMore()</c>, the start time is updated
         *   to reflect the timestamp of the first measure actually found in the
         *   dataLogger within the specified range.
         * </para>
         * <para>
         *   <b>DEPRECATED</b>: This method has been replaced by <c>get_summary()</c>
         *   which contain more precise informations.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an unsigned number corresponding to the number of seconds
         *   between the Jan 1, 1970 and the beginning of this data
         *   set (i.e. Unix time representation of the absolute time).
         * </returns>
         */
        public virtual long get_startTimeUTC()
        {
            if (_objref == null)
            {
                string msg = "No DataSet connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_startTimeUTC();
        }

        /**
         * <summary>
         *   Returns the end time of the dataset, relative to the Jan 1, 1970.
         * <para>
         *   When the <c>YDataSet</c> object is created, the end time is the value passed
         *   in parameter to the <c>get_dataSet()</c> function. After the
         *   very first call to <c>loadMore()</c>, the end time is updated
         *   to reflect the timestamp of the last measure actually found in the
         *   dataLogger within the specified range.
         * </para>
         * <para>
         *   <b>DEPRECATED</b>: This method has been replaced by <c>get_summary()</c>
         *   which contain more precise informations.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an unsigned number corresponding to the number of seconds
         *   between the Jan 1, 1970 and the end of this data
         *   set (i.e. Unix time representation of the absolute time).
         * </returns>
         */
        public virtual long get_endTimeUTC()
        {
            if (_objref == null)
            {
                string msg = "No DataSet connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_endTimeUTC();
        }

        /**
         * <summary>
         *   Returns the progress of the downloads of the measures from the data logger,
         *   on a scale from 0 to 100.
         * <para>
         *   When the object is instantiated by <c>get_dataSet</c>,
         *   the progress is zero. Each time <c>loadMore()</c> is invoked, the progress
         *   is updated, to reach the value 100 only once all measures have been loaded.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer in the range 0 to 100 (percentage of completion).
         * </returns>
         */
        public virtual int get_progress()
        {
            if (_objref == null)
            {
                string msg = "No DataSet connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_progress();
        }

        /**
         * <summary>
         *   Loads the the next block of measures from the dataLogger, and updates
         *   the progress indicator.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer in the range 0 to 100 (percentage of completion),
         *   or a negative error code in case of failure.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int loadMore()
        {
            if (_objref == null)
            {
                string msg = "No DataSet connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.loadMore();
        }

        /**
         * <summary>
         *   Returns an <c>YMeasure</c> object which summarizes the whole
         *   <c>YDataSet</c>.
         * <para>
         *   In includes the following information:
         *   - the start of a time interval
         *   - the end of a time interval
         *   - the minimal value observed during the time interval
         *   - the average value observed during the time interval
         *   - the maximal value observed during the time interval
         * </para>
         * <para>
         *   This summary is available as soon as <c>loadMore()</c> has
         *   been called for the first time.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an <c>YMeasure</c> object
         * </returns>
         */
        public virtual YMeasure get_summary()
        {
            if (_objref == null)
            {
                string msg = "No DataSet connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_summary();
        }

        /**
         * <summary>
         *   Returns a condensed version of the measures that can
         *   retrieved in this <c>YDataSet</c>, as a list of <c>YMeasure</c>
         *   objects.
         * <para>
         *   Each item includes:
         *   - the start of a time interval
         *   - the end of a time interval
         *   - the minimal value observed during the time interval
         *   - the average value observed during the time interval
         *   - the maximal value observed during the time interval
         * </para>
         * <para>
         *   This preview is available as soon as <c>loadMore()</c> has
         *   been called for the first time.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a table of records, where each record depicts the
         *   measured values during a time interval
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty array.
         * </para>
         */
        public virtual YMeasure[] get_preview()
        {
            if (_objref == null)
            {
                string msg = "No DataSet connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_preview().ToArray();
        }

        /**
         * <summary>
         *   Returns the detailed set of measures for the time interval corresponding
         *   to a given condensed measures previously returned by <c>get_preview()</c>.
         * <para>
         *   The result is provided as a list of <c>YMeasure</c> objects.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="measure">
         *   condensed measure from the list previously returned by
         *   <c>get_preview()</c>.
         * </param>
         * <returns>
         *   a table of records, where each record depicts the
         *   measured values during a time interval
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty array.
         * </para>
         */
        public virtual YMeasure[] get_measuresAt(YMeasure measure)
        {
            if (_objref == null)
            {
                string msg = "No DataSet connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_measuresAt(measure).ToArray();
        }

        /**
         * <summary>
         *   Returns all measured values currently available for this DataSet,
         *   as a list of <c>YMeasure</c> objects.
         * <para>
         *   Each item includes:
         *   - the start of the measure time interval
         *   - the end of the measure time interval
         *   - the minimal value observed during the time interval
         *   - the average value observed during the time interval
         *   - the maximal value observed during the time interval
         * </para>
         * <para>
         *   Before calling this method, you should call <c>loadMore()</c>
         *   to load data from the device. You may have to call loadMore()
         *   several time until all rows are loaded, but you can start
         *   looking at available data rows before the load is complete.
         * </para>
         * <para>
         *   The oldest measures are always loaded first, and the most
         *   recent measures will be loaded last. As a result, timestamps
         *   are normally sorted in ascending order within the measure table,
         *   unless there was an unexpected adjustment of the datalogger UTC
         *   clock.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a table of records, where each record depicts the
         *   measured value for a given time interval
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty array.
         * </para>
         */
        public virtual YMeasure[] get_measures()
        {
            if (_objref == null)
            {
                string msg = "No DataSet connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_measures().ToArray();
        }
    }
    //--- (end of generated code: YDataSet implementation)
}

