/*********************************************************************
 *
 *  $Id: yocto_firmwareupdate_proxy.cs 38514 2019-11-26 16:54:39Z seb $
 *
 *  Implements YFirmwareUpdateProxy, the Proxy API for FirmwareUpdate
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
    //--- (generated code: YFirmwareUpdate class start)
    public class YFirmwareUpdateProxy
    {
        private YFirmwareUpdate _objref;
        internal YFirmwareUpdateProxy(YFirmwareUpdate objref)
        {
             _objref = objref;
        }
        //--- (end of generated code: YFirmwareUpdate class start)
        //--- (generated code: YFirmwareUpdate definitions)
        //--- (end of generated code: YFirmwareUpdate definitions)
        //--- (generated code: YFirmwareUpdate implementation)

        /**
         * <summary>
         *   Returns a list of all the modules in "firmware update" mode.
         * <para>
         *   Only devices
         *   connected over USB are listed. For devices connected to a YoctoHub, you
         *   must connect yourself to the YoctoHub web interface.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an array of strings containing the serial numbers of devices in "firmware update" mode.
         * </returns>
         */
        public static string[] GetAllBootLoaders()
        {
            return YFirmwareUpdate.GetAllBootLoaders().ToArray();
        }

        /**
         * <summary>
         *   Test if the byn file is valid for this module.
         * <para>
         *   It is possible to pass a directory instead of a file.
         *   In that case, this method returns the path of the most recent appropriate byn file. This method will
         *   ignore any firmware older than minrelease.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="serial">
         *   the serial number of the module to update
         * </param>
         * <param name="path">
         *   the path of a byn file or a directory that contains byn files
         * </param>
         * <param name="minrelease">
         *   a positive integer
         * </param>
         * <returns>
         *   : the path of the byn file to use, or an empty string if no byn files matches the requirement
         * </returns>
         * <para>
         *   On failure, returns a string that starts with "error:".
         * </para>
         */
        public static string CheckFirmware(string serial, string path, int minrelease)
        {
            return YFirmwareUpdate.CheckFirmware(serial, path, minrelease);
        }

        /**
         * <summary>
         *   Returns the progress of the firmware update, on a scale from 0 to 100.
         * <para>
         *   When the object is
         *   instantiated, the progress is zero. The value is updated during the firmware update process until
         *   the value of 100 is reached. The 100 value means that the firmware update was completed
         *   successfully. If an error occurs during the firmware update, a negative value is returned, and the
         *   error message can be retrieved with <c>get_progressMessage</c>.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer in the range 0 to 100 (percentage of completion)
         *   or a negative error code in case of failure.
         * </returns>
         */
        public virtual int get_progress()
        {
            if (_objref == null)
            {
                string msg = "No FirmwareUpdate connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_progress();
        }

        /**
         * <summary>
         *   Returns the last progress message of the firmware update process.
         * <para>
         *   If an error occurs during the
         *   firmware update process, the error message is returned
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string  with the latest progress message, or the error message.
         * </returns>
         */
        public virtual string get_progressMessage()
        {
            if (_objref == null)
            {
                string msg = "No FirmwareUpdate connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_progressMessage();
        }

        /**
         * <summary>
         *   Starts the firmware update process.
         * <para>
         *   This method starts the firmware update process in background. This method
         *   returns immediately. You can monitor the progress of the firmware update with the <c>get_progress()</c>
         *   and <c>get_progressMessage()</c> methods.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer in the range 0 to 100 (percentage of completion),
         *   or a negative error code in case of failure.
         * </returns>
         * <para>
         *   On failure returns a negative error code.
         * </para>
         */
        public virtual int startUpdate()
        {
            if (_objref == null)
            {
                string msg = "No FirmwareUpdate connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.startUpdate();
        }
    }
    //--- (end of generated code: YFirmwareUpdate implementation)
}

