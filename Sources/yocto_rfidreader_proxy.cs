/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YRfidReaderProxy, the Proxy API for RfidReader
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
    //--- (YRfidReader class start)
    static public partial class YoctoProxyManager
    {
        public static YRfidReaderProxy FindRfidReader(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YRfidReader func = null;
            YRfidReaderProxy res = (YRfidReaderProxy)YFunctionProxy.FindSimilarUnknownFunction("YRfidReaderProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YRfidReaderProxy)YFunctionProxy.FindSimilarKnownFunction("YRfidReaderProxy");
                if (res != null) return res;
                func = YRfidReader.FirstRfidReader();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YRfidReaderProxy)func.get_userData();
                    }
                }
            } else {
                func = YRfidReader.FindRfidReader(name);
                if (func.get_userData() != null) {
                    return (YRfidReaderProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YRfidReaderProxy(func, name);
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
 *   The <c>YRfidReader</c> class allows you to detect RFID tags, as well as
 *   read and write on these tags if the security settings allow it.
 * <para>
 * </para>
 * <para>
 *   Short reminder:
 * </para>
 * <para>
 * </para>
 * <para>
 *   - A tag's memory is generally organized in fixed-size blocks.
 * </para>
 * <para>
 *   - At tag level, each block must be read and written in its entirety.
 * </para>
 * <para>
 *   - Some blocks are special configuration blocks, and may alter the tag's behavior
 *   if they are rewritten with arbitrary data.
 * </para>
 * <para>
 *   - Data blocks can be set to read-only mode, but on many tags, this operation is irreversible.
 * </para>
 * <para>
 * </para>
 * <para>
 *   By default, the RfidReader class automatically manages these blocks so that
 *   arbitrary size data  can be manipulated of  without risk and without knowledge of
 *   tag architecture.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YRfidReaderProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a RFID reader for a given identifier.
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
         *   This function does not require that the RFID reader is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YRfidReader.isOnline()</c> to test if the RFID reader is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a RFID reader by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the RFID reader, for instance
         *   <c>MyDevice.rfidReader</c>.
         * </param>
         * <returns>
         *   a <c>YRfidReader</c> object allowing you to drive the RFID reader.
         * </returns>
         */
        public static YRfidReaderProxy FindRfidReader(string func)
        {
            return YoctoProxyManager.FindRfidReader(func);
        }
        //--- (end of YRfidReader class start)
        //--- (YRfidReader definitions)
        public const int _NTags_INVALID = -1;
        public const int _RefreshRate_INVALID = -1;

        // reference to real YoctoAPI object
        protected new YRfidReader _func;
        // property cache
        protected int _nTags = _NTags_INVALID;
        protected int _refreshRate = _RefreshRate_INVALID;
        //--- (end of YRfidReader definitions)

        //--- (YRfidReader implementation)
        internal YRfidReaderProxy(YRfidReader hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("RfidReader " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YRfidReader) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YRfidReader hwd = YRfidReader.FindRfidReader(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YRfidReader hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering RfidReader callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type RfidReader available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YRfidReader.FindRfidReader</c> to obtain an object that can control the
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
            YRfidReader it = YRfidReader.FirstRfidReader();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextRfidReader();
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
            _refreshRate = _func.get_refreshRate();
        }

        /**
         * <summary>
         *   Returns the number of RFID tags currently detected.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of RFID tags currently detected
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YRfidReader.NTAGS_INVALID</c>.
         * </para>
         */
        public int get_nTags()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            res = _func.get_nTags();
            if (res == YAPI.INVALID_INT) {
                res = _NTags_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (advertised value)
        /// <value>Number of RFID tags currently detected.</value>
        public int NTags
        {
            get
            {
                if (_func == null) {
                    return _NTags_INVALID;
                }
                if (_online) {
                    return _nTags;
                }
                return _NTags_INVALID;
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            _nTags = YAPI._atoi((value).Substring((value).Length-4, 4));
        }

        /**
         * <summary>
         *   Returns the tag list refresh rate, measured in Hz.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the tag list refresh rate, measured in Hz
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YRfidReader.REFRESHRATE_INVALID</c>.
         * </para>
         */
        public int get_refreshRate()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            res = _func.get_refreshRate();
            if (res == YAPI.INVALID_INT) {
                res = _RefreshRate_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the present tag list refresh rate, measured in Hz.
         * <para>
         *   The reader will do
         *   its best to respect it. Note that the reader cannot detect tag arrival or removal
         *   while it is  communicating with a tag.  Maximum frequency is limited to 100Hz,
         *   but in real life it will be difficult to do better than 50Hz.  A zero value
         *   will power off the device radio.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the present tag list refresh rate, measured in Hz
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
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            if (newval == _RefreshRate_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_refreshRate(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Tag list refresh rate, measured in Hz.</value>
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
         *   Returns the list of RFID tags currently detected by the reader.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a list of strings, corresponding to each tag identifier (UID).
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty list.
         * </para>
         */
        public virtual string[] get_tagIdList()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            return _func.get_tagIdList().ToArray();
        }

        /**
         * <summary>
         *   Returns a description of the properties of an existing RFID tag.
         * <para>
         *   This function can cause communications with the tag.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="tagId">
         *   identifier of the tag to check
         * </param>
         * <param name="status">
         *   an <c>RfidStatus</c> object that will contain
         *   the detailled status of the operation
         * </param>
         * <returns>
         *   a <c>YRfidTagInfo</c> object.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty <c>YRfidTagInfo</c> objact.
         *   When it happens, you can get more information from the <c>status</c> object.
         * </para>
         */
        public virtual YRfidTagInfoProxy get_tagInfo(string tagId, ref YRfidStatusProxy status)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            return new YRfidTagInfoProxy(_func.get_tagInfo(tagId, ref status._objref));
        }

        /**
         * <summary>
         *   Changes an RFID tag configuration to prevents any further write to
         *   the selected blocks.
         * <para>
         *   This operation is definitive and irreversible.
         *   Depending on the tag type and block index, adjascent blocks may become
         *   read-only as well, based on the locking granularity.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="tagId">
         *   identifier of the tag to use
         * </param>
         * <param name="firstBlock">
         *   first block to lock
         * </param>
         * <param name="nBlocks">
         *   number of blocks to lock
         * </param>
         * <param name="options">
         *   an <c>YRfidOptions</c> object with the optional
         *   command execution parameters, such as security key
         *   if required
         * </param>
         * <param name="status">
         *   an <c>RfidStatus</c> object that will contain
         *   the detailled status of the operation
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code. When it
         *   happens, you can get more information from the <c>status</c> object.
         * </para>
         */
        public virtual int tagLockBlocks(string tagId, int firstBlock, int nBlocks, YRfidOptionsProxy options, ref YRfidStatusProxy status)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            return _func.tagLockBlocks(tagId, firstBlock, nBlocks, options, ref status._objref);
        }

        /**
         * <summary>
         *   Reads the locked state for RFID tag memory data blocks.
         * <para>
         *   FirstBlock cannot be a special block, and any special
         *   block encountered in the middle of the read operation will be
         *   skipped automatically.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="tagId">
         *   identifier of the tag to use
         * </param>
         * <param name="firstBlock">
         *   number of the first block to check
         * </param>
         * <param name="nBlocks">
         *   number of blocks to check
         * </param>
         * <param name="options">
         *   an <c>YRfidOptions</c> object with the optional
         *   command execution parameters, such as security key
         *   if required
         * </param>
         * <param name="status">
         *   an <c>RfidStatus</c> object that will contain
         *   the detailled status of the operation
         * </param>
         * <returns>
         *   a list of booleans with the lock state of selected blocks
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty list. When it
         *   happens, you can get more information from the <c>status</c> object.
         * </para>
         */
        public virtual bool[] get_tagLockState(string tagId, int firstBlock, int nBlocks, YRfidOptionsProxy options, ref YRfidStatusProxy status)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            return _func.get_tagLockState(tagId, firstBlock, nBlocks, options, ref status._objref).ToArray();
        }

        /**
         * <summary>
         *   Tells which block of a RFID tag memory are special and cannot be used
         *   to store user data.
         * <para>
         *   Mistakely writing a special block can lead to
         *   an irreversible alteration of the tag.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="tagId">
         *   identifier of the tag to use
         * </param>
         * <param name="firstBlock">
         *   number of the first block to check
         * </param>
         * <param name="nBlocks">
         *   number of blocks to check
         * </param>
         * <param name="options">
         *   an <c>YRfidOptions</c> object with the optional
         *   command execution parameters, such as security key
         *   if required
         * </param>
         * <param name="status">
         *   an <c>RfidStatus</c> object that will contain
         *   the detailled status of the operation
         * </param>
         * <returns>
         *   a list of booleans with the lock state of selected blocks
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty list. When it
         *   happens, you can get more information from the <c>status</c> object.
         * </para>
         */
        public virtual bool[] get_tagSpecialBlocks(string tagId, int firstBlock, int nBlocks, YRfidOptionsProxy options, ref YRfidStatusProxy status)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            return _func.get_tagSpecialBlocks(tagId, firstBlock, nBlocks, options, ref status._objref).ToArray();
        }

        /**
         * <summary>
         *   Reads data from an RFID tag memory, as an hexadecimal string.
         * <para>
         *   The read operation may span accross multiple blocks if the requested
         *   number of bytes is larger than the RFID tag block size. By default
         *   firstBlock cannot be a special block, and any special block encountered
         *   in the middle of the read operation will be skipped automatically.
         *   If you rather want to read special blocks, use the <c>EnableRawAccess</c>
         *   field from the <c>options</c> parameter.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="tagId">
         *   identifier of the tag to use
         * </param>
         * <param name="firstBlock">
         *   block number where read should start
         * </param>
         * <param name="nBytes">
         *   total number of bytes to read
         * </param>
         * <param name="options">
         *   an <c>YRfidOptions</c> object with the optional
         *   command execution parameters, such as security key
         *   if required
         * </param>
         * <param name="status">
         *   an <c>RfidStatus</c> object that will contain
         *   the detailled status of the operation
         * </param>
         * <returns>
         *   an hexadecimal string if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty binary buffer. When it
         *   happens, you can get more information from the <c>status</c> object.
         * </para>
         */
        public virtual string tagReadHex(string tagId, int firstBlock, int nBytes, YRfidOptionsProxy options, ref YRfidStatusProxy status)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            return _func.tagReadHex(tagId, firstBlock, nBytes, options, ref status._objref);
        }

        /**
         * <summary>
         *   Reads data from an RFID tag memory, as a binary buffer.
         * <para>
         *   The read operation
         *   may span accross multiple blocks if the requested number of bytes
         *   is larger than the RFID tag block size.  By default
         *   firstBlock cannot be a special block, and any special block encountered
         *   in the middle of the read operation will be skipped automatically.
         *   If you rather want to read special blocks, use the <c>EnableRawAccess</c>
         *   field frrm the <c>options</c> parameter.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="tagId">
         *   identifier of the tag to use
         * </param>
         * <param name="firstBlock">
         *   block number where read should start
         * </param>
         * <param name="nBytes">
         *   total number of bytes to read
         * </param>
         * <param name="options">
         *   an <c>YRfidOptions</c> object with the optional
         *   command execution parameters, such as security key
         *   if required
         * </param>
         * <param name="status">
         *   an <c>RfidStatus</c> object that will contain
         *   the detailled status of the operation
         * </param>
         * <returns>
         *   a binary object with the data read if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty binary buffer. When it
         *   happens, you can get more information from the <c>status</c> object.
         * </para>
         */
        public virtual byte[] tagReadBin(string tagId, int firstBlock, int nBytes, YRfidOptionsProxy options, ref YRfidStatusProxy status)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            return _func.tagReadBin(tagId, firstBlock, nBytes, options, ref status._objref);
        }

        /**
         * <summary>
         *   Reads data from an RFID tag memory, as a byte list.
         * <para>
         *   The read operation
         *   may span accross multiple blocks if the requested number of bytes
         *   is larger than the RFID tag block size.  By default
         *   firstBlock cannot be a special block, and any special block encountered
         *   in the middle of the read operation will be skipped automatically.
         *   If you rather want to read special blocks, use the <c>EnableRawAccess</c>
         *   field from the <c>options</c> parameter.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="tagId">
         *   identifier of the tag to use
         * </param>
         * <param name="firstBlock">
         *   block number where read should start
         * </param>
         * <param name="nBytes">
         *   total number of bytes to read
         * </param>
         * <param name="options">
         *   an <c>YRfidOptions</c> object with the optional
         *   command execution parameters, such as security key
         *   if required
         * </param>
         * <param name="status">
         *   an <c>RfidStatus</c> object that will contain
         *   the detailled status of the operation
         * </param>
         * <returns>
         *   a byte list with the data read if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty list. When it
         *   happens, you can get more information from the <c>status</c> object.
         * </para>
         */
        public virtual int[] tagReadArray(string tagId, int firstBlock, int nBytes, YRfidOptionsProxy options, ref YRfidStatusProxy status)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            return _func.tagReadArray(tagId, firstBlock, nBytes, options, ref status._objref).ToArray();
        }

        /**
         * <summary>
         *   Reads data from an RFID tag memory, as a text string.
         * <para>
         *   The read operation
         *   may span accross multiple blocks if the requested number of bytes
         *   is larger than the RFID tag block size.  By default
         *   firstBlock cannot be a special block, and any special block encountered
         *   in the middle of the read operation will be skipped automatically.
         *   If you rather want to read special blocks, use the <c>EnableRawAccess</c>
         *   field from the <c>options</c> parameter.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="tagId">
         *   identifier of the tag to use
         * </param>
         * <param name="firstBlock">
         *   block number where read should start
         * </param>
         * <param name="nChars">
         *   total number of characters to read
         * </param>
         * <param name="options">
         *   an <c>YRfidOptions</c> object with the optional
         *   command execution parameters, such as security key
         *   if required
         * </param>
         * <param name="status">
         *   an <c>RfidStatus</c> object that will contain
         *   the detailled status of the operation
         * </param>
         * <returns>
         *   a text string with the data read if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty string. When it
         *   happens, you can get more information from the <c>status</c> object.
         * </para>
         */
        public virtual string tagReadStr(string tagId, int firstBlock, int nChars, YRfidOptionsProxy options, ref YRfidStatusProxy status)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            return _func.tagReadStr(tagId, firstBlock, nChars, options, ref status._objref);
        }

        /**
         * <summary>
         *   Writes data provided as a binary buffer to an RFID tag memory.
         * <para>
         *   The write operation may span accross multiple blocks if the
         *   number of bytes to write is larger than the RFID tag block size.
         *   By default firstBlock cannot be a special block, and any special block
         *   encountered in the middle of the write operation will be skipped
         *   automatically. The last data block affected by the operation will
         *   be automatically padded with zeros if neccessary.  If you rather want
         *   to rewrite special blocks as well,
         *   use the <c>EnableRawAccess</c> field from the <c>options</c> parameter.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="tagId">
         *   identifier of the tag to use
         * </param>
         * <param name="firstBlock">
         *   block number where write should start
         * </param>
         * <param name="buff">
         *   the binary buffer to write
         * </param>
         * <param name="options">
         *   an <c>YRfidOptions</c> object with the optional
         *   command execution parameters, such as security key
         *   if required
         * </param>
         * <param name="status">
         *   an <c>RfidStatus</c> object that will contain
         *   the detailled status of the operation
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code. When it
         *   happens, you can get more information from the <c>status</c> object.
         * </para>
         */
        public virtual int tagWriteBin(string tagId, int firstBlock, byte[] buff, YRfidOptionsProxy options, ref YRfidStatusProxy status)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            return _func.tagWriteBin(tagId, firstBlock, buff, options, ref status._objref);
        }

        /**
         * <summary>
         *   Writes data provided as a list of bytes to an RFID tag memory.
         * <para>
         *   The write operation may span accross multiple blocks if the
         *   number of bytes to write is larger than the RFID tag block size.
         *   By default firstBlock cannot be a special block, and any special block
         *   encountered in the middle of the write operation will be skipped
         *   automatically. The last data block affected by the operation will
         *   be automatically padded with zeros if neccessary.
         *   If you rather want to rewrite special blocks as well,
         *   use the <c>EnableRawAccess</c> field from the <c>options</c> parameter.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="tagId">
         *   identifier of the tag to use
         * </param>
         * <param name="firstBlock">
         *   block number where write should start
         * </param>
         * <param name="byteList">
         *   a list of byte to write
         * </param>
         * <param name="options">
         *   an <c>YRfidOptions</c> object with the optional
         *   command execution parameters, such as security key
         *   if required
         * </param>
         * <param name="status">
         *   an <c>RfidStatus</c> object that will contain
         *   the detailled status of the operation
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code. When it
         *   happens, you can get more information from the <c>status</c> object.
         * </para>
         */
        public virtual int tagWriteArray(string tagId, int firstBlock, int[] byteList, YRfidOptionsProxy options, ref YRfidStatusProxy status)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            return _func.tagWriteArray(tagId, firstBlock, new List<int>(byteList), options, ref status._objref);
        }

        /**
         * <summary>
         *   Writes data provided as an hexadecimal string to an RFID tag memory.
         * <para>
         *   The write operation may span accross multiple blocks if the
         *   number of bytes to write is larger than the RFID tag block size.
         *   By default firstBlock cannot be a special block, and any special block
         *   encountered in the middle of the write operation will be skipped
         *   automatically. The last data block affected by the operation will
         *   be automatically padded with zeros if neccessary.
         *   If you rather want to rewrite special blocks as well,
         *   use the <c>EnableRawAccess</c> field from the <c>options</c> parameter.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="tagId">
         *   identifier of the tag to use
         * </param>
         * <param name="firstBlock">
         *   block number where write should start
         * </param>
         * <param name="hexString">
         *   a string of hexadecimal byte codes to write
         * </param>
         * <param name="options">
         *   an <c>YRfidOptions</c> object with the optional
         *   command execution parameters, such as security key
         *   if required
         * </param>
         * <param name="status">
         *   an <c>RfidStatus</c> object that will contain
         *   the detailled status of the operation
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code. When it
         *   happens, you can get more information from the <c>status</c> object.
         * </para>
         */
        public virtual int tagWriteHex(string tagId, int firstBlock, string hexString, YRfidOptionsProxy options, ref YRfidStatusProxy status)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            return _func.tagWriteHex(tagId, firstBlock, hexString, options, ref status._objref);
        }

        /**
         * <summary>
         *   Writes data provided as an ASCII string to an RFID tag memory.
         * <para>
         *   The write operation may span accross multiple blocks if the
         *   number of bytes to write is larger than the RFID tag block size.
         *   Note that only the characters present in the provided string
         *   will be written, there is no notion of string length. If your
         *   string data have variable length, you'll have to encode the
         *   string length yourself, with a terminal zero for instannce.
         * </para>
         * <para>
         *   This function only works with ISO-latin characters, if you wish to
         *   write strings encoded with alternate character sets, you'll have to
         *   use tagWriteBin() function.
         * </para>
         * <para>
         *   By default firstBlock cannot be a special block, and any special block
         *   encountered in the middle of the write operation will be skipped
         *   automatically. The last data block affected by the operation will
         *   be automatically padded with zeros if neccessary.
         *   If you rather want to rewrite special blocks as well,
         *   use the <c>EnableRawAccess</c> field from the <c>options</c> parameter
         *   (definitely not recommanded).
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="tagId">
         *   identifier of the tag to use
         * </param>
         * <param name="firstBlock">
         *   block number where write should start
         * </param>
         * <param name="text">
         *   the text string to write
         * </param>
         * <param name="options">
         *   an <c>YRfidOptions</c> object with the optional
         *   command execution parameters, such as security key
         *   if required
         * </param>
         * <param name="status">
         *   an <c>RfidStatus</c> object that will contain
         *   the detailled status of the operation
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code. When it
         *   happens, you can get more information from the <c>status</c> object.
         * </para>
         */
        public virtual int tagWriteStr(string tagId, int firstBlock, string text, YRfidOptionsProxy options, ref YRfidStatusProxy status)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            return _func.tagWriteStr(tagId, firstBlock, text, options, ref status._objref);
        }

        /**
         * <summary>
         *   Reads an RFID tag AFI byte (ISO 15693 only).
         * <para>
         * </para>
         * </summary>
         * <param name="tagId">
         *   identifier of the tag to use
         * </param>
         * <param name="options">
         *   an <c>YRfidOptions</c> object with the optional
         *   command execution parameters, such as security key
         *   if required
         * </param>
         * <param name="status">
         *   an <c>RfidStatus</c> object that will contain
         *   the detailled status of the operation
         * </param>
         * <returns>
         *   the AFI value (0...255)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code. When it
         *   happens, you can get more information from the <c>status</c> object.
         * </para>
         */
        public virtual int tagGetAFI(string tagId, YRfidOptionsProxy options, ref YRfidStatusProxy status)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            return _func.tagGetAFI(tagId, options, ref status._objref);
        }

        /**
         * <summary>
         *   Changes an RFID tag AFI byte (ISO 15693 only).
         * <para>
         * </para>
         * </summary>
         * <param name="tagId">
         *   identifier of the tag to use
         * </param>
         * <param name="afi">
         *   the AFI value to write (0...255)
         * </param>
         * <param name="options">
         *   an <c>YRfidOptions</c> object with the optional
         *   command execution parameters, such as security key
         *   if required
         * </param>
         * <param name="status">
         *   an <c>RfidStatus</c> object that will contain
         *   the detailled status of the operation
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code. When it
         *   happens, you can get more information from the <c>status</c> object.
         * </para>
         */
        public virtual int tagSetAFI(string tagId, int afi, YRfidOptionsProxy options, ref YRfidStatusProxy status)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            return _func.tagSetAFI(tagId, afi, options, ref status._objref);
        }

        /**
         * <summary>
         *   Locks the RFID tag AFI byte (ISO 15693 only).
         * <para>
         *   This operation is definitive and irreversible.
         * </para>
         * </summary>
         * <param name="tagId">
         *   identifier of the tag to use
         * </param>
         * <param name="options">
         *   an <c>YRfidOptions</c> object with the optional
         *   command execution parameters, such as security key
         *   if required
         * </param>
         * <param name="status">
         *   an <c>RfidStatus</c> object that will contain
         *   the detailled status of the operation
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code. When it
         *   happens, you can get more information from the <c>status</c> object.
         * </para>
         */
        public virtual int tagLockAFI(string tagId, YRfidOptionsProxy options, ref YRfidStatusProxy status)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            return _func.tagLockAFI(tagId, options, ref status._objref);
        }

        /**
         * <summary>
         *   Reads an RFID tag DSFID byte (ISO 15693 only).
         * <para>
         * </para>
         * </summary>
         * <param name="tagId">
         *   identifier of the tag to use
         * </param>
         * <param name="options">
         *   an <c>YRfidOptions</c> object with the optional
         *   command execution parameters, such as security key
         *   if required
         * </param>
         * <param name="status">
         *   an <c>RfidStatus</c> object that will contain
         *   the detailled status of the operation
         * </param>
         * <returns>
         *   the DSFID value (0...255)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code. When it
         *   happens, you can get more information from the <c>status</c> object.
         * </para>
         */
        public virtual int tagGetDSFID(string tagId, YRfidOptionsProxy options, ref YRfidStatusProxy status)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            return _func.tagGetDSFID(tagId, options, ref status._objref);
        }

        /**
         * <summary>
         *   Changes an RFID tag DSFID byte (ISO 15693 only).
         * <para>
         * </para>
         * </summary>
         * <param name="tagId">
         *   identifier of the tag to use
         * </param>
         * <param name="dsfid">
         *   the DSFID value to write (0...255)
         * </param>
         * <param name="options">
         *   an <c>YRfidOptions</c> object with the optional
         *   command execution parameters, such as security key
         *   if required
         * </param>
         * <param name="status">
         *   an <c>RfidStatus</c> object that will contain
         *   the detailled status of the operation
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code. When it
         *   happens, you can get more information from the <c>status</c> object.
         * </para>
         */
        public virtual int tagSetDSFID(string tagId, int dsfid, YRfidOptionsProxy options, ref YRfidStatusProxy status)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            return _func.tagSetDSFID(tagId, dsfid, options, ref status._objref);
        }

        /**
         * <summary>
         *   Locks the RFID tag DSFID byte (ISO 15693 only).
         * <para>
         *   This operation is definitive and irreversible.
         * </para>
         * </summary>
         * <param name="tagId">
         *   identifier of the tag to use
         * </param>
         * <param name="options">
         *   an <c>YRfidOptions</c> object with the optional
         *   command execution parameters, such as security key
         *   if required
         * </param>
         * <param name="status">
         *   an <c>RfidStatus</c> object that will contain
         *   the detailled status of the operation
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code. When it
         *   happens, you can get more information from the <c>status</c> object.
         * </para>
         */
        public virtual int tagLockDSFID(string tagId, YRfidOptionsProxy options, ref YRfidStatusProxy status)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            return _func.tagLockDSFID(tagId, options, ref status._objref);
        }

        /**
         * <summary>
         *   Returns a string with last tag arrival/removal events observed.
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
                throw new YoctoApiProxyException("No RfidReader connected");
            }
            return _func.get_lastEvents();
        }
    }
    //--- (end of YRfidReader implementation)
}

