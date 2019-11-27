/*********************************************************************
 *
 *  $Id: yocto_displaylayer_proxy.cs 38514 2019-11-26 16:54:39Z seb $
 *
 *  Implements YDisplayLayerProxy, the Proxy API for DisplayLayer
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
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Timers;
using YoctoLib;

namespace YoctoProxyAPI
{
    //--- (generated code: YDisplayLayer class start)
    public class YDisplayLayerProxy
    {
        private YDisplayLayer _objref;
        internal YDisplayLayerProxy(YDisplayLayer objref)
        {
             _objref = objref;
        }
        //--- (end of generated code: YDisplayLayer class start)
        //--- (generated code: YDisplayLayer definitions)
        public const int _CurrentRunIndex_INVALID = -1;
        public const long _TimeUTC_INVALID = YAPI.INVALID_LONG;
        public const int _Recording_INVALID = 0;
        public const int _Recording_OFF = 1;
        public const int _Recording_ON = 2;
        public const int _Recording_PENDING = 3;
        public const int _AutoStart_INVALID = 0;
        public const int _AutoStart_OFF = 1;
        public const int _AutoStart_ON = 2;
        public const int _BeaconDriven_INVALID = 0;
        public const int _BeaconDriven_OFF = 1;
        public const int _BeaconDriven_ON = 2;
        public const int _Usage_INVALID = -1;
        public const int _ClearHistory_INVALID = 0;
        public const int _ClearHistory_FALSE = 1;
        public const int _ClearHistory_TRUE = 2;
        public const string _Unit_INVALID = YAPI.INVALID_STRING;
        public const double _CurrentValue_INVALID = Double.NaN;
        public const double _LowestValue_INVALID = Double.NaN;
        public const double _HighestValue_INVALID = Double.NaN;
        public const double _CurrentRawValue_INVALID = Double.NaN;
        public const string _LogFrequency_INVALID = YAPI.INVALID_STRING;
        public const string _ReportFrequency_INVALID = YAPI.INVALID_STRING;
        public const int _AdvMode_INVALID = 0;
        public const int _AdvMode_IMMEDIATE = 1;
        public const int _AdvMode_PERIOD_AVG = 2;
        public const int _AdvMode_PERIOD_MIN = 3;
        public const int _AdvMode_PERIOD_MAX = 4;
        public const string _CalibrationParam_INVALID = YAPI.INVALID_STRING;
        public const double _Resolution_INVALID = Double.NaN;
        public const int _SensorState_INVALID = YAPI.INVALID_INT;
        public const int _ALIGN_INVALID = 0;
        public const int _ALIGN_TOP_LEFT = 1;
        public const int _ALIGN_CENTER_LEFT = 2;
        public const int _ALIGN_BASELINE_LEFT = 3;
        public const int _ALIGN_BOTTOM_LEFT = 4;
        public const int _ALIGN_TOP_CENTER = 5;
        public const int _ALIGN_CENTER = 6;
        public const int _ALIGN_BASELINE_CENTER = 7;
        public const int _ALIGN_BOTTOM_CENTER = 8;
        public const int _ALIGN_TOP_DECIMAL = 9;
        public const int _ALIGN_CENTER_DECIMAL = 10;
        public const int _ALIGN_BASELINE_DECIMAL = 11;
        public const int _ALIGN_BOTTOM_DECIMAL = 12;
        public const int _ALIGN_TOP_RIGHT = 13;
        public const int _ALIGN_CENTER_RIGHT = 14;
        public const int _ALIGN_BASELINE_RIGHT = 15;
        public const int _ALIGN_BOTTOM_RIGHT = 16;

        private int _ALIGN2Int(YDisplayLayer.ALIGN realenum)
        {
            switch (realenum) {
                    default:
                case YDisplayLayer.ALIGN.TOP_LEFT:
                    return _ALIGN_TOP_LEFT;
                case YDisplayLayer.ALIGN.CENTER_LEFT:
                    return _ALIGN_CENTER_LEFT;
                case YDisplayLayer.ALIGN.BASELINE_LEFT:
                    return _ALIGN_BASELINE_LEFT;
                case YDisplayLayer.ALIGN.BOTTOM_LEFT:
                    return _ALIGN_BOTTOM_LEFT;
                case YDisplayLayer.ALIGN.TOP_CENTER:
                    return _ALIGN_TOP_CENTER;
                case YDisplayLayer.ALIGN.CENTER:
                    return _ALIGN_CENTER;
                case YDisplayLayer.ALIGN.BASELINE_CENTER:
                    return _ALIGN_BASELINE_CENTER;
                case YDisplayLayer.ALIGN.BOTTOM_CENTER:
                    return _ALIGN_BOTTOM_CENTER;
                case YDisplayLayer.ALIGN.TOP_DECIMAL:
                    return _ALIGN_TOP_DECIMAL;
                case YDisplayLayer.ALIGN.CENTER_DECIMAL:
                    return _ALIGN_CENTER_DECIMAL;
                case YDisplayLayer.ALIGN.BASELINE_DECIMAL:
                    return _ALIGN_BASELINE_DECIMAL;
                case YDisplayLayer.ALIGN.BOTTOM_DECIMAL:
                    return _ALIGN_BOTTOM_DECIMAL;
                case YDisplayLayer.ALIGN.TOP_RIGHT:
                    return _ALIGN_TOP_RIGHT;
                case YDisplayLayer.ALIGN.CENTER_RIGHT:
                    return _ALIGN_CENTER_RIGHT;
                case YDisplayLayer.ALIGN.BASELINE_RIGHT:
                    return _ALIGN_BASELINE_RIGHT;
                case YDisplayLayer.ALIGN.BOTTOM_RIGHT:
                    return _ALIGN_BOTTOM_RIGHT;
            }
        }

        private YDisplayLayer.ALIGN _Int2ALIGN(int value)
        {
            switch (value) {
                    default:
                case _ALIGN_TOP_LEFT:
                    return YDisplayLayer.ALIGN.TOP_LEFT;
                case _ALIGN_CENTER_LEFT:
                    return YDisplayLayer.ALIGN.CENTER_LEFT;
                case _ALIGN_BASELINE_LEFT:
                    return YDisplayLayer.ALIGN.BASELINE_LEFT;
                case _ALIGN_BOTTOM_LEFT:
                    return YDisplayLayer.ALIGN.BOTTOM_LEFT;
                case _ALIGN_TOP_CENTER:
                    return YDisplayLayer.ALIGN.TOP_CENTER;
                case _ALIGN_CENTER:
                    return YDisplayLayer.ALIGN.CENTER;
                case _ALIGN_BASELINE_CENTER:
                    return YDisplayLayer.ALIGN.BASELINE_CENTER;
                case _ALIGN_BOTTOM_CENTER:
                    return YDisplayLayer.ALIGN.BOTTOM_CENTER;
                case _ALIGN_TOP_DECIMAL:
                    return YDisplayLayer.ALIGN.TOP_DECIMAL;
                case _ALIGN_CENTER_DECIMAL:
                    return YDisplayLayer.ALIGN.CENTER_DECIMAL;
                case _ALIGN_BASELINE_DECIMAL:
                    return YDisplayLayer.ALIGN.BASELINE_DECIMAL;
                case _ALIGN_BOTTOM_DECIMAL:
                    return YDisplayLayer.ALIGN.BOTTOM_DECIMAL;
                case _ALIGN_TOP_RIGHT:
                    return YDisplayLayer.ALIGN.TOP_RIGHT;
                case _ALIGN_CENTER_RIGHT:
                    return YDisplayLayer.ALIGN.CENTER_RIGHT;
                case _ALIGN_BASELINE_RIGHT:
                    return YDisplayLayer.ALIGN.BASELINE_RIGHT;
                case _ALIGN_BOTTOM_RIGHT:
                    return YDisplayLayer.ALIGN.BOTTOM_RIGHT;
            }
        }
        protected int _recording = _Recording_INVALID;
        protected int _autoStart = _AutoStart_INVALID;
        protected int _beaconDriven = _BeaconDriven_INVALID;
        protected double _currentValue = _CurrentValue_INVALID;
        protected string _logFrequency = _LogFrequency_INVALID;
        protected string _reportFrequency = _ReportFrequency_INVALID;
        protected int _advMode = _AdvMode_INVALID;
        protected double _resolution = _Resolution_INVALID;
        //--- (end of generated code: YDisplayLayer definitions)

        public int DisplayHeight {
            get { return this.get_displayHeight(); }
        }

        public int DisplayWidth {
            get { return this.get_displayWidth(); }
        }


        //--- (generated code: YDisplayLayer implementation)

        /**
         * <summary>
         *   Reverts the layer to its initial state (fully transparent, default settings).
         * <para>
         *   Reinitializes the drawing pointer to the upper left position,
         *   and selects the most visible pen color. If you only want to erase the layer
         *   content, use the method <c>clear()</c> instead.
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int reset()
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.reset();
        }

        /**
         * <summary>
         *   Erases the whole content of the layer (makes it fully transparent).
         * <para>
         *   This method does not change any other attribute of the layer.
         *   To reinitialize the layer attributes to defaults settings, use the method
         *   <c>reset()</c> instead.
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int clear()
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.clear();
        }

        /**
         * <summary>
         *   Selects the pen color for all subsequent drawing functions,
         *   including text drawing.
         * <para>
         *   The pen color is provided as an RGB value.
         *   For grayscale or monochrome displays, the value is
         *   automatically converted to the proper range.
         * </para>
         * </summary>
         * <param name="color">
         *   the desired pen color, as a 24-bit RGB value
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int selectColorPen(int color)
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.selectColorPen(color);
        }

        /**
         * <summary>
         *   Selects the pen gray level for all subsequent drawing functions,
         *   including text drawing.
         * <para>
         *   The gray level is provided as a number between
         *   0 (black) and 255 (white, or whichever the lightest color is).
         *   For monochrome displays (without gray levels), any value
         *   lower than 128 is rendered as black, and any value equal
         *   or above to 128 is non-black.
         * </para>
         * </summary>
         * <param name="graylevel">
         *   the desired gray level, from 0 to 255
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int selectGrayPen(int graylevel)
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.selectGrayPen(graylevel);
        }

        /**
         * <summary>
         *   Selects an eraser instead of a pen for all subsequent drawing functions,
         *   except for bitmap copy functions.
         * <para>
         *   Any point drawn using the eraser
         *   becomes transparent (as when the layer is empty), showing the other
         *   layers beneath it.
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int selectEraser()
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.selectEraser();
        }

        /**
         * <summary>
         *   Enables or disables anti-aliasing for drawing oblique lines and circles.
         * <para>
         *   Anti-aliasing provides a smoother aspect when looked from far enough,
         *   but it can add fuzziness when the display is looked from very close.
         *   At the end of the day, it is your personal choice.
         *   Anti-aliasing is enabled by default on grayscale and color displays,
         *   but you can disable it if you prefer. This setting has no effect
         *   on monochrome displays.
         * </para>
         * </summary>
         * <param name="mode">
         *   <c>true</c> to enable anti-aliasing, <c>false</c> to
         *   disable it.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int setAntialiasingMode(bool mode)
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.setAntialiasingMode(mode);
        }

        /**
         * <summary>
         *   Draws a single pixel at the specified position.
         * <para>
         * </para>
         * </summary>
         * <param name="x">
         *   the distance from left of layer, in pixels
         * </param>
         * <param name="y">
         *   the distance from top of layer, in pixels
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int drawPixel(int x, int y)
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.drawPixel(x, y);
        }

        /**
         * <summary>
         *   Draws an empty rectangle at a specified position.
         * <para>
         * </para>
         * </summary>
         * <param name="x1">
         *   the distance from left of layer to the left border of the rectangle, in pixels
         * </param>
         * <param name="y1">
         *   the distance from top of layer to the top border of the rectangle, in pixels
         * </param>
         * <param name="x2">
         *   the distance from left of layer to the right border of the rectangle, in pixels
         * </param>
         * <param name="y2">
         *   the distance from top of layer to the bottom border of the rectangle, in pixels
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int drawRect(int x1, int y1, int x2, int y2)
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.drawRect(x1, y1, x2, y2);
        }

        /**
         * <summary>
         *   Draws a filled rectangular bar at a specified position.
         * <para>
         * </para>
         * </summary>
         * <param name="x1">
         *   the distance from left of layer to the left border of the rectangle, in pixels
         * </param>
         * <param name="y1">
         *   the distance from top of layer to the top border of the rectangle, in pixels
         * </param>
         * <param name="x2">
         *   the distance from left of layer to the right border of the rectangle, in pixels
         * </param>
         * <param name="y2">
         *   the distance from top of layer to the bottom border of the rectangle, in pixels
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int drawBar(int x1, int y1, int x2, int y2)
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.drawBar(x1, y1, x2, y2);
        }

        /**
         * <summary>
         *   Draws an empty circle at a specified position.
         * <para>
         * </para>
         * </summary>
         * <param name="x">
         *   the distance from left of layer to the center of the circle, in pixels
         * </param>
         * <param name="y">
         *   the distance from top of layer to the center of the circle, in pixels
         * </param>
         * <param name="r">
         *   the radius of the circle, in pixels
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int drawCircle(int x, int y, int r)
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.drawCircle(x, y, r);
        }

        /**
         * <summary>
         *   Draws a filled disc at a given position.
         * <para>
         * </para>
         * </summary>
         * <param name="x">
         *   the distance from left of layer to the center of the disc, in pixels
         * </param>
         * <param name="y">
         *   the distance from top of layer to the center of the disc, in pixels
         * </param>
         * <param name="r">
         *   the radius of the disc, in pixels
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int drawDisc(int x, int y, int r)
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.drawDisc(x, y, r);
        }

        /**
         * <summary>
         *   Selects a font to use for the next text drawing functions, by providing the name of the
         *   font file.
         * <para>
         *   You can use a built-in font as well as a font file that you have previously
         *   uploaded to the device built-in memory. If you experience problems selecting a font
         *   file, check the device logs for any error message such as missing font file or bad font
         *   file format.
         * </para>
         * </summary>
         * <param name="fontname">
         *   the font file name, embedded fonts are 8x8.yfm, Small.yfm, Medium.yfm, Large.yfm (not available on
         *   Yocto-MiniDisplay).
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int selectFont(string fontname)
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.selectFont(fontname);
        }

        /**
         * <summary>
         *   Draws a text string at the specified position.
         * <para>
         *   The point of the text that is aligned
         *   to the specified pixel position is called the anchor point, and can be chosen among
         *   several options. Text is rendered from left to right, without implicit wrapping.
         * </para>
         * </summary>
         * <param name="x">
         *   the distance from left of layer to the text anchor point, in pixels
         * </param>
         * <param name="y">
         *   the distance from top of layer to the text anchor point, in pixels
         * </param>
         * <param name="anchor">
         *   the text anchor point, chosen among the <c>YDisplayLayer.ALIGN</c> enumeration:
         *   <c>YDisplayLayer.ALIGN_TOP_LEFT</c>,    <c>YDisplayLayer.ALIGN_CENTER_LEFT</c>,   
         *   <c>YDisplayLayer.ALIGN_BASELINE_LEFT</c>,    <c>YDisplayLayer.ALIGN_BOTTOM_LEFT</c>,
         *   <c>YDisplayLayer.ALIGN_TOP_CENTER</c>,  <c>YDisplayLayer.ALIGN_CENTER</c>,        
         *   <c>YDisplayLayer.ALIGN_BASELINE_CENTER</c>,  <c>YDisplayLayer.ALIGN_BOTTOM_CENTER</c>,
         *   <c>YDisplayLayer.ALIGN_TOP_DECIMAL</c>, <c>YDisplayLayer.ALIGN_CENTER_DECIMAL</c>,
         *   <c>YDisplayLayer.ALIGN_BASELINE_DECIMAL</c>, <c>YDisplayLayer.ALIGN_BOTTOM_DECIMAL</c>,
         *   <c>YDisplayLayer.ALIGN_TOP_RIGHT</c>,   <c>YDisplayLayer.ALIGN_CENTER_RIGHT</c>,  
         *   <c>YDisplayLayer.ALIGN_BASELINE_RIGHT</c>,   <c>YDisplayLayer.ALIGN_BOTTOM_RIGHT</c>.
         * </param>
         * <param name="text">
         *   the text string to draw
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int drawText(int x, int y, int anchor, string text)
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.drawText(x, y, _Int2ALIGN(anchor), text);
        }

        /**
         * <summary>
         *   Draws a GIF image at the specified position.
         * <para>
         *   The GIF image must have been previously
         *   uploaded to the device built-in memory. If you experience problems using an image
         *   file, check the device logs for any error message such as missing image file or bad
         *   image file format.
         * </para>
         * </summary>
         * <param name="x">
         *   the distance from left of layer to the left of the image, in pixels
         * </param>
         * <param name="y">
         *   the distance from top of layer to the top of the image, in pixels
         * </param>
         * <param name="imagename">
         *   the GIF file name
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int drawImage(int x, int y, string imagename)
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.drawImage(x, y, imagename);
        }

        /**
         * <summary>
         *   Draws a bitmap at the specified position.
         * <para>
         *   The bitmap is provided as a binary object,
         *   where each pixel maps to a bit, from left to right and from top to bottom.
         *   The most significant bit of each byte maps to the leftmost pixel, and the least
         *   significant bit maps to the rightmost pixel. Bits set to 1 are drawn using the
         *   layer selected pen color. Bits set to 0 are drawn using the specified background
         *   gray level, unless -1 is specified, in which case they are not drawn at all
         *   (as if transparent).
         * </para>
         * </summary>
         * <param name="x">
         *   the distance from left of layer to the left of the bitmap, in pixels
         * </param>
         * <param name="y">
         *   the distance from top of layer to the top of the bitmap, in pixels
         * </param>
         * <param name="w">
         *   the width of the bitmap, in pixels
         * </param>
         * <param name="bitmap">
         *   a binary object
         * </param>
         * <param name="bgcol">
         *   the background gray level to use for zero bits (0 = black,
         *   255 = white), or -1 to leave the pixels unchanged
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int drawBitmap(int x, int y, int w, byte[] bitmap, int bgcol)
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.drawBitmap(x, y, w, bitmap, bgcol);
        }

        /**
         * <summary>
         *   Moves the drawing pointer of this layer to the specified position.
         * <para>
         * </para>
         * </summary>
         * <param name="x">
         *   the distance from left of layer, in pixels
         * </param>
         * <param name="y">
         *   the distance from top of layer, in pixels
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int moveTo(int x, int y)
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.moveTo(x, y);
        }

        /**
         * <summary>
         *   Draws a line from current drawing pointer position to the specified position.
         * <para>
         *   The specified destination pixel is included in the line. The pointer position
         *   is then moved to the end point of the line.
         * </para>
         * </summary>
         * <param name="x">
         *   the distance from left of layer to the end point of the line, in pixels
         * </param>
         * <param name="y">
         *   the distance from top of layer to the end point of the line, in pixels
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int lineTo(int x, int y)
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.lineTo(x, y);
        }

        /**
         * <summary>
         *   Outputs a message in the console area, and advances the console pointer accordingly.
         * <para>
         *   The console pointer position is automatically moved to the beginning
         *   of the next line when a newline character is met, or when the right margin
         *   is hit. When the new text to display extends below the lower margin, the
         *   console area is automatically scrolled up.
         * </para>
         * </summary>
         * <param name="text">
         *   the message to display
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int consoleOut(string text)
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.consoleOut(text);
        }

        /**
         * <summary>
         *   Sets up display margins for the <c>consoleOut</c> function.
         * <para>
         * </para>
         * </summary>
         * <param name="x1">
         *   the distance from left of layer to the left margin, in pixels
         * </param>
         * <param name="y1">
         *   the distance from top of layer to the top margin, in pixels
         * </param>
         * <param name="x2">
         *   the distance from left of layer to the right margin, in pixels
         * </param>
         * <param name="y2">
         *   the distance from top of layer to the bottom margin, in pixels
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int setConsoleMargins(int x1, int y1, int x2, int y2)
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.setConsoleMargins(x1, y1, x2, y2);
        }

        /**
         * <summary>
         *   Sets up the background color used by the <c>clearConsole</c> function and by
         *   the console scrolling feature.
         * <para>
         * </para>
         * </summary>
         * <param name="bgcol">
         *   the background gray level to use when scrolling (0 = black,
         *   255 = white), or -1 for transparent
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int setConsoleBackground(int bgcol)
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.setConsoleBackground(bgcol);
        }

        /**
         * <summary>
         *   Sets up the wrapping behavior used by the <c>consoleOut</c> function.
         * <para>
         * </para>
         * </summary>
         * <param name="wordwrap">
         *   <c>true</c> to wrap only between words,
         *   <c>false</c> to wrap on the last column anyway.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int setConsoleWordWrap(bool wordwrap)
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.setConsoleWordWrap(wordwrap);
        }

        /**
         * <summary>
         *   Blanks the console area within console margins, and resets the console pointer
         *   to the upper left corner of the console.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int clearConsole()
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.clearConsole();
        }

        /**
         * <summary>
         *   Sets the position of the layer relative to the display upper left corner.
         * <para>
         *   When smooth scrolling is used, the display offset of the layer is
         *   automatically updated during the next milliseconds to animate the move of the layer.
         * </para>
         * </summary>
         * <param name="x">
         *   the distance from left of display to the upper left corner of the layer
         * </param>
         * <param name="y">
         *   the distance from top of display to the upper left corner of the layer
         * </param>
         * <param name="scrollTime">
         *   number of milliseconds to use for smooth scrolling, or
         *   0 if the scrolling should be immediate.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int setLayerPosition(int x, int y, int scrollTime)
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.setLayerPosition(x, y, scrollTime);
        }

        /**
         * <summary>
         *   Hides the layer.
         * <para>
         *   The state of the layer is preserved but the layer is not displayed
         *   on the screen until the next call to <c>unhide()</c>. Hiding the layer can positively
         *   affect the drawing speed, since it postpones the rendering until all operations are
         *   completed (double-buffering).
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int hide()
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.hide();
        }

        /**
         * <summary>
         *   Shows the layer.
         * <para>
         *   Shows the layer again after a hide command.
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int unhide()
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.unhide();
        }

        /**
         * <summary>
         *   Gets parent YDisplay.
         * <para>
         *   Returns the parent YDisplay object of the current YDisplayLayer.
         * </para>
         * </summary>
         * <returns>
         *   an <c>YDisplay</c> object
         * </returns>
         */
        public virtual YDisplay get_display()
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_display();
        }

        /**
         * <summary>
         *   Returns the display width, in pixels.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the display width, in pixels
         * </returns>
         * <para>
         *   On failure, throws an exception or returns YDisplayLayer.DISPLAYWIDTH_INVALID.
         * </para>
         */
        public virtual int get_displayWidth()
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_displayWidth();
        }

        /**
         * <summary>
         *   Returns the display height, in pixels.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the display height, in pixels
         * </returns>
         * <para>
         *   On failure, throws an exception or returns YDisplayLayer.DISPLAYHEIGHT_INVALID.
         * </para>
         */
        public virtual int get_displayHeight()
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_displayHeight();
        }

        /**
         * <summary>
         *   Returns the width of the layers to draw on, in pixels.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the width of the layers to draw on, in pixels
         * </returns>
         * <para>
         *   On failure, throws an exception or returns YDisplayLayer.LAYERWIDTH_INVALID.
         * </para>
         */
        public virtual int get_layerWidth()
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_layerWidth();
        }

        /**
         * <summary>
         *   Returns the height of the layers to draw on, in pixels.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the height of the layers to draw on, in pixels
         * </returns>
         * <para>
         *   On failure, throws an exception or returns YDisplayLayer.LAYERHEIGHT_INVALID.
         * </para>
         */
        public virtual int get_layerHeight()
        {
            if (_objref == null)
            {
                string msg = "No DisplayLayer connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_layerHeight();
        }
    }
    //--- (end of generated code: YDisplayLayer implementation)
}