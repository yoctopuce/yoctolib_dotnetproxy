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
    //--- (YDataSet class start)
    public class YDataSetProxy
    {
        private YDataSet _objref;
        internal YDataSetProxy(YDataSet objref)
        {
             _objref = objref;
             _objref.loadMore();
        }
        //--- (end of YDataSet class start)
        //--- (YDataSet definitions)
        //--- (end of YDataSet definitions)
        //--- (YDataSet implementation)

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
            return _objref.get_measures().ToArray();
        }
    }
    //--- (end of YDataSet implementation)
}

