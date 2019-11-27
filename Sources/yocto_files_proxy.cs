/*********************************************************************
 *
 *  $Id: yocto_files_proxy.cs 38514 2019-11-26 16:54:39Z seb $
 *
 *  Implements YFilesProxy, the Proxy API for Files
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
using System.Globalization;
using System.IO;
using System.Threading;
using System.Timers;
using YoctoLib;

namespace YoctoProxyAPI
{
    //--- (generated code: YFiles class start)
    static public partial class YoctoProxyManager
    {
        public static YFilesProxy FindFiles(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YFiles func = null;
            YFilesProxy res = (YFilesProxy)YFunctionProxy.FindSimilarUnknownFunction("YFilesProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YFilesProxy)YFunctionProxy.FindSimilarKnownFunction("YFilesProxy");
                if (res != null) return res;
                func = YFiles.FirstFiles();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YFilesProxy)func.get_userData();
                    }
                }
            } else {
                func = YFiles.FindFiles(name);
                if (func.get_userData() != null) {
                    return (YFilesProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YFilesProxy(func, name);
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
 *   The YFiles class is used to access the filesystem embedded on
 *   some Yoctopuce devices, for instance using a Yocto-Color-V2, a Yocto-RS232, a YoctoHub-Ethernet or a YoctoHub-Wireless-g.
 * <para>
 *   This filesystem makes it
 *   possible for instance to design a custom web UI
 *   (for networked devices) or to add fonts (on display devices).
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YFilesProxy : YFunctionProxy
    {
        //--- (end of generated code: YFiles class start)
        //--- (generated code: YFiles definitions)
        public const int _FilesCount_INVALID = -1;
        public const int _FreeSpace_INVALID = -1;

        // reference to real YoctoAPI object
        protected new YFiles _func;
        // property cache
        protected int _filesCount = _FilesCount_INVALID;
        //--- (end of generated code: YFiles definitions)

        /**
         * <summary>
         *   Returns an array of strings that lists files currently loaded
         *   in the filesystem.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an array of <c>string</c> objects, containing the file names.
         * </returns>
         * <para>
         *   On failure, throws an exception.
         * </para>
         */
        public string[] get_filenames()
        {
            if (_func == null)
            {
                string msg = "No Files connected";
                throw new YoctoApiProxyException(msg);
            }
            List<string> res = new List<string>();
            List<YFileRecord> records = _func.get_list("");
            for (int i = 0; i < records.Count; i++)
            {
                res.Add(records[i].get_name());
            }
            return res.ToArray();
        }

        //--- (generated code: YFiles implementation)
        internal YFilesProxy(YFiles hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Files " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YFiles) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YFiles hwd = YFiles.FindFiles(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YFiles hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Files callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YFiles it = YFiles.FirstFiles();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextFiles();
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

        // property with cached value for instant access (advertised value)
        public int FilesCount
        {
            get
            {
                if (_func == null) return _FilesCount_INVALID;
                return (_online ? _filesCount : _FilesCount_INVALID);
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            Int32.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture,out _filesCount);
        }

        /**
         * <summary>
         *   Returns the number of files currently loaded in the filesystem.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of files currently loaded in the filesystem
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YFiles.FILESCOUNT_INVALID</c>.
         * </para>
         */
        public int get_filesCount()
        {
            if (_func == null)
            {
                string msg = "No Files connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_filesCount();
            if (res == YAPI.INVALID_INT) res = _FilesCount_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Returns the free space for uploading new files to the filesystem, in bytes.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the free space for uploading new files to the filesystem, in bytes
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YFiles.FREESPACE_INVALID</c>.
         * </para>
         */
        public int get_freeSpace()
        {
            if (_func == null)
            {
                string msg = "No Files connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_freeSpace();
            if (res == YAPI.INVALID_INT) res = _FreeSpace_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Reinitialize the filesystem to its clean, unfragmented, empty state.
         * <para>
         *   All files previously uploaded are permanently lost.
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int format_fs()
        {
            if (_func == null)
            {
                string msg = "No Files connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.format_fs();
        }

        /**
         * <summary>
         *   Returns a list of YFileRecord objects that describe files currently loaded
         *   in the filesystem.
         * <para>
         * </para>
         * </summary>
         * <param name="pattern">
         *   an optional filter pattern, using star and question marks
         *   as wild cards. When an empty pattern is provided, all file records
         *   are returned.
         * </param>
         * <returns>
         *   a list of <c>YFileRecord</c> objects, containing the file path
         *   and name, byte size and 32-bit CRC of the file content.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty list.
         * </para>
         */
        public virtual YFileRecordProxy[] get_list(string pattern)
        {
            if (_func == null)
            {
                string msg = "No Files connected";
                throw new YoctoApiProxyException(msg);
            }
            int i = 0;
            var std_res = _func.get_list(pattern);
            YFileRecordProxy[] proxy_res = new YFileRecordProxy[std_res.Count];
            foreach (var record in std_res) {
                proxy_res[i++] = new YFileRecordProxy(record);
            }
            return proxy_res;
        }

        /**
         * <summary>
         *   Test if a file exist on the filesystem of the module.
         * <para>
         * </para>
         * </summary>
         * <param name="filename">
         *   the file name to test.
         * </param>
         * <returns>
         *   a true if the file exist, false otherwise.
         * </returns>
         * <para>
         *   On failure, throws an exception.
         * </para>
         */
        public virtual bool fileExist(string filename)
        {
            if (_func == null)
            {
                string msg = "No Files connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.fileExist(filename);
        }

        /**
         * <summary>
         *   Downloads the requested file and returns a binary buffer with its content.
         * <para>
         * </para>
         * </summary>
         * <param name="pathname">
         *   path and name of the file to download
         * </param>
         * <returns>
         *   a binary buffer with the file content
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty content.
         * </para>
         */
        public virtual byte[] download(string pathname)
        {
            if (_func == null)
            {
                string msg = "No Files connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.download(pathname);
        }

        /**
         * <summary>
         *   Uploads a file to the filesystem, to the specified full path name.
         * <para>
         *   If a file already exists with the same path name, its content is overwritten.
         * </para>
         * </summary>
         * <param name="pathname">
         *   path and name of the new file to create
         * </param>
         * <param name="content">
         *   binary buffer with the content to set
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int upload(string pathname, byte[] content)
        {
            if (_func == null)
            {
                string msg = "No Files connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.upload(pathname, content);
        }

        /**
         * <summary>
         *   Deletes a file, given by its full path name, from the filesystem.
         * <para>
         *   Because of filesystem fragmentation, deleting a file may not always
         *   free up the whole space used by the file. However, rewriting a file
         *   with the same path name will always reuse any space not freed previously.
         *   If you need to ensure that no space is taken by previously deleted files,
         *   you can use <c>format_fs</c> to fully reinitialize the filesystem.
         * </para>
         * </summary>
         * <param name="pathname">
         *   path and name of the file to remove.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int remove(string pathname)
        {
            if (_func == null)
            {
                string msg = "No Files connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.remove(pathname);
        }
    }
    //--- (end of generated code: YFiles implementation)
}

