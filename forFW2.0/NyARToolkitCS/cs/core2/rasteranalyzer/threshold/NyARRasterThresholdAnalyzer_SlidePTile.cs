﻿/* 
 * PROJECT: NyARToolkitCS(Extension)
 * --------------------------------------------------------------------------------
 * The NyARToolkitCS is C# edition ARToolKit class library.
 * Copyright (C)2008-2009 Ryo Iizuka
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * For further information please contact.
 *	http://nyatla.jp/nyatoolkit/
 *	<airmail(at)ebony.plala.or.jp> or <nyatla(at)nyatla.jp>
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using jp.nyatla.nyartoolkit.cs.core;

namespace jp.nyatla.nyartoolkit.cs.core2
{
    /**
     * 明点と暗点をPタイル法で検出して、その中央値を閾値とする。
     * 
     * 
     */
    public class NyARRasterThresholdAnalyzer_SlidePTile : INyARRasterThresholdAnalyzer
    {
        interface ICreateHistgramImpl
        {
            int createHistgramImpl(INyARBufferReader i_reader, NyARIntSize i_size, int[] o_histgram);
        }
        /**
         * Glayscale(MAX256)のヒストグラム計算クラス
         */
        class CreateHistgramImpl_INT1D_GLAY_8 : ICreateHistgramImpl
        {
            public int _v_interval;
            public CreateHistgramImpl_INT1D_GLAY_8(int i_v_interval)
            {
                this._v_interval = i_v_interval;
                return;
            }
            public int createHistgramImpl(INyARBufferReader i_reader, NyARIntSize i_size, int[] o_histgram)
            {
                Debug.Assert(i_reader.isEqualBufferType(INyARBufferReader.BUFFERFORMAT_INT1D_GRAY_8));

                int sum = 0;
                int[] input = (int[])i_reader.getBuffer();
                for (int y = i_size.h - 1; y >= 0; y -= this._v_interval)
                {
                    sum += i_size.w;
                    int pt = y * i_size.w;
                    for (int x = i_size.w - 1; x >= 0; x--)
                    {
                        o_histgram[input[pt]]++;
                        pt++;
                    }
                }
                return sum;
            }
        }
        class CreateHistgramImpl_BYTE1D_RGB_24 : ICreateHistgramImpl
        {
            private int _v_interval;
            public CreateHistgramImpl_BYTE1D_RGB_24(int i_v_interval)
            {
                this._v_interval = i_v_interval;
                return;
            }
            public int createHistgramImpl(INyARBufferReader i_reader, NyARIntSize i_size, int[] o_histgram)
            {
                Debug.Assert(
				    i_reader.isEqualBufferType(INyARBufferReader.BUFFERFORMAT_BYTE1D_B8G8R8_24)||
				    i_reader.isEqualBufferType(INyARBufferReader.BUFFERFORMAT_BYTE1D_R8G8B8_24));

                byte[] input = (byte[])i_reader.getBuffer();
                int pix_count = i_size.w;
                int pix_mod_part = pix_count - (pix_count % 8);
                int sum = 0;
                for (int y = i_size.h - 1; y >= 0; y -= this._v_interval)
                {
                    sum += i_size.w;
                    int pt = y * i_size.w * 3;
                    int x, v;
                    for (x = pix_count - 1; x >= pix_mod_part; x--)
                    {
                        v = ((input[pt + 0] & 0xff) + (input[pt + 1] & 0xff) + (input[pt + 2] & 0xff)) / 3;
                        o_histgram[v]++;
                        pt += 3;
                    }
                    //タイリング
                    for (; x >= 0; x -= 8)
                    {
                        v = ((input[pt + 0] & 0xff) + (input[pt + 1] & 0xff) + (input[pt + 2] & 0xff)) / 3;
                        o_histgram[v]++;
                        v = ((input[pt + 3] & 0xff) + (input[pt + 4] & 0xff) + (input[pt + 5] & 0xff)) / 3;
                        o_histgram[v]++;
                        v = ((input[pt + 6] & 0xff) + (input[pt + 7] & 0xff) + (input[pt + 8] & 0xff)) / 3;
                        o_histgram[v]++;
                        v = ((input[pt + 9] & 0xff) + (input[pt + 10] & 0xff) + (input[pt + 11] & 0xff)) / 3;
                        o_histgram[v]++;
                        v = ((input[pt + 12] & 0xff) + (input[pt + 13] & 0xff) + (input[pt + 14] & 0xff)) / 3;
                        o_histgram[v]++;
                        v = ((input[pt + 15] & 0xff) + (input[pt + 16] & 0xff) + (input[pt + 17] & 0xff)) / 3;
                        o_histgram[v]++;
                        v = ((input[pt + 18] & 0xff) + (input[pt + 19] & 0xff) + (input[pt + 20] & 0xff)) / 3;
                        o_histgram[v]++;
                        v = ((input[pt + 21] & 0xff) + (input[pt + 22] & 0xff) + (input[pt + 23] & 0xff)) / 3;
                        o_histgram[v]++;
                        pt += 3 * 8;
                    }
                }
                return sum;
            }
        }
        class CreateHistgramImpl_BYTE1D_B8G8R8X8_32 : ICreateHistgramImpl
        {
            private int _v_interval;
            public CreateHistgramImpl_BYTE1D_B8G8R8X8_32(int i_v_interval)
            {
                this._v_interval = i_v_interval;
                return;
            }
            public int createHistgramImpl(INyARBufferReader i_reader, NyARIntSize i_size, int[] o_histgram)
            {
                Debug.Assert(i_reader.isEqualBufferType(INyARBufferReader.BUFFERFORMAT_BYTE1D_B8G8R8X8_32));
                byte[] input = (byte[])i_reader.getBuffer();
                int pix_count = i_size.w;
                int pix_mod_part = pix_count - (pix_count % 8);
                int sum = 0;
                for (int y = i_size.h - 1; y >= 0; y -= this._v_interval)
                {
                    sum += i_size.w;
                    int pt = y * i_size.w * 3;
                    int x, v;
                    for (x = pix_count - 1; x >= pix_mod_part; x--)
                    {
                        v = ((input[pt + 0] & 0xff) + (input[pt + 1] & 0xff) + (input[pt + 2] & 0xff)) / 3;
                        o_histgram[v]++;
                        pt += 4;
                    }
                    //タイリング
                    for (; x >= 0; x -= 8)
                    {
                        v = ((input[pt + 0] & 0xff) + (input[pt + 1] & 0xff) + (input[pt + 2] & 0xff)) / 3;
                        o_histgram[v]++;
                        v = ((input[pt + 4] & 0xff) + (input[pt + 5] & 0xff) + (input[pt + 6] & 0xff)) / 3;
                        o_histgram[v]++;
                        v = ((input[pt + 8] & 0xff) + (input[pt + 9] & 0xff) + (input[pt + 10] & 0xff)) / 3;
                        o_histgram[v]++;
                        v = ((input[pt + 12] & 0xff) + (input[pt + 13] & 0xff) + (input[pt + 14] & 0xff)) / 3;
                        o_histgram[v]++;
                        v = ((input[pt + 16] & 0xff) + (input[pt + 17] & 0xff) + (input[pt + 18] & 0xff)) / 3;
                        o_histgram[v]++;
                        v = ((input[pt + 20] & 0xff) + (input[pt + 21] & 0xff) + (input[pt + 22] & 0xff)) / 3;
                        o_histgram[v]++;
                        v = ((input[pt + 24] & 0xff) + (input[pt + 25] & 0xff) + (input[pt + 26] & 0xff)) / 3;
                        o_histgram[v]++;
                        v = ((input[pt + 28] & 0xff) + (input[pt + 29] & 0xff) + (input[pt + 30] & 0xff)) / 3;
                        o_histgram[v]++;
                        pt += 4 * 8;
                    }
                }
                return sum;
            }
        }
	    /**
	     * WORD1D_R5G6B5_16LEのヒストグラム計算クラス
	     *
	     */	
        class CreateHistgramImpl_WORD1D_R5G6B5_16LE : ICreateHistgramImpl
        {
            private int _v_interval;
            public CreateHistgramImpl_WORD1D_R5G6B5_16LE(int i_v_interval)
            {
                this._v_interval = i_v_interval;
                return;
            }
            public int createHistgramImpl(INyARBufferReader i_reader, NyARIntSize i_size, int[] o_histgram)
            {
                Debug.Assert(i_reader.isEqualBufferType(INyARBufferReader.BUFFERFORMAT_WORD1D_R5G6B5_16LE));
                short[] input = (short[])i_reader.getBuffer();
                int pix_count = i_size.w;
                int pix_mod_part = pix_count - (pix_count % 8);
                int sum = 0;
                for (int y = i_size.h - 1; y >= 0; y -= this._v_interval)
                {
                    sum += i_size.w;
                    int pt = y * i_size.w;
                    int x, v;
                    for (x = pix_count - 1; x >= pix_mod_part; x--)
                    {
                        v =(int)input[pt];
                        v = (((v & 0xf800) >> 8) + ((v & 0x07e0) >> 3) + ((v & 0x001f) << 3))/3;
                        o_histgram[v]++;
                        pt++;
                    }
                    //タイリング
                    for (; x >= 0; x -= 8)
                    {
                        v =(int)input[pt];pt++;
                        v = (((v & 0xf800) >> 8) + ((v & 0x07e0) >> 3) + ((v & 0x001f) << 3))/3;
                        o_histgram[v]++;
                        v =(int)input[pt];pt++;
                        v = (((v & 0xf800) >> 8) + ((v & 0x07e0) >> 3) + ((v & 0x001f) << 3))/3;
                        o_histgram[v]++;
                        v =(int)input[pt];pt++;
                        v = (((v & 0xf800) >> 8) + ((v & 0x07e0) >> 3) + ((v & 0x001f) << 3))/3;
                        o_histgram[v]++;
                        v =(int)input[pt];pt++;
                        v = (((v & 0xf800) >> 8) + ((v & 0x07e0) >> 3) + ((v & 0x001f) << 3))/3;
                        o_histgram[v]++;
                        v =(int)input[pt];pt++;
                        v = (((v & 0xf800) >> 8) + ((v & 0x07e0) >> 3) + ((v & 0x001f) << 3))/3;
                        o_histgram[v]++;
                        v =(int)input[pt];pt++;
                        v = (((v & 0xf800) >> 8) + ((v & 0x07e0) >> 3) + ((v & 0x001f) << 3))/3;
                        o_histgram[v]++;
                        v =(int)input[pt];pt++;
                        v = (((v & 0xf800) >> 8) + ((v & 0x07e0) >> 3) + ((v & 0x001f) << 3))/3;
                        o_histgram[v]++;
                        v =(int)input[pt];pt++;
                        v = (((v & 0xf800) >> 8) + ((v & 0x07e0) >> 3) + ((v & 0x001f) << 3))/3;
                        o_histgram[v]++;
                    }
                }
                return sum;
            }
        }
        private int _persentage;
        private int _threshold;
        private ICreateHistgramImpl _histgram;

        /**
         * @param i_persentage
         * 0<=50であること。白/黒マーカーの場合は10～20を推奨 正の場合、黒点を基準にします。 負の場合、白点を基準にします。
         * (CMOSカメラの場合、基準点は白点の方が良い)
         */
        public NyARRasterThresholdAnalyzer_SlidePTile(int i_persentage, int i_raster_format, int i_vertical_interval)
        {
            Debug.Assert(0 <= i_persentage && i_persentage <= 50);
            this._persentage = i_persentage;
            switch (i_raster_format)
            {
                case INyARBufferReader.BUFFERFORMAT_BYTE1D_B8G8R8_24:
                case INyARBufferReader.BUFFERFORMAT_BYTE1D_R8G8B8_24:
                    this._histgram = new CreateHistgramImpl_BYTE1D_RGB_24(i_vertical_interval);
                    break;
                case INyARBufferReader.BUFFERFORMAT_INT1D_GRAY_8:
                    this._histgram = new CreateHistgramImpl_INT1D_GLAY_8(i_vertical_interval);
                    break;
                case INyARBufferReader.BUFFERFORMAT_BYTE1D_B8G8R8X8_32:
                    this._histgram = new CreateHistgramImpl_BYTE1D_B8G8R8X8_32(i_vertical_interval);
                    break;
                case INyARBufferReader.BUFFERFORMAT_WORD1D_R5G6B5_16LE:
                    this._histgram = new CreateHistgramImpl_WORD1D_R5G6B5_16LE(i_vertical_interval);
                    break;
                default:
                    throw new NyARException();
            }
        }
        public void setVerticalInterval(int i_step)
        {
            return;//未実装一号
        }

        private int[] _histgram_buf = new int[256];
        public void analyzeRaster(INyARRaster i_input)
        {
            INyARBufferReader buffer_reader = i_input.getBufferReader();

            int[] histgram = this._histgram_buf;
            NyARIntSize size = i_input.getSize();

            //最大画像サイズの制限
            Debug.Assert(size.w * size.h < 0x40000000);

            //ヒストグラム初期化
            for (int i = 0; i < 256; i++)
            {
                histgram[i] = 0;
            }
            int sum_of_pixel = this._histgram.createHistgramImpl(i_input.getBufferReader(), size, histgram);

            // 閾値ピクセル数確定
            int th_pixcels = sum_of_pixel * this._persentage / 100;
            int th_wk;
            int th_w, th_b;

            // 黒点基準
            th_wk = th_pixcels;
            for (th_b = 0; th_b < 254; th_b++)
            {
                th_wk -= histgram[th_b];
                if (th_wk <= 0)
                {
                    break;
                }
            }
            // 白点基準
            th_wk = th_pixcels;
            for (th_w = 255; th_w > 1; th_w--)
            {
                th_wk -= histgram[th_w];
                if (th_wk <= 0)
                {
                    break;
                }
            }
            // 閾値の保存
            this._threshold = (th_w + th_b) / 2;
            return;
        }
        public int getThreshold()
        {
            return this._threshold;
        }

        public int getThreshold(int i_x, int i_y)
        {
            return this._threshold;
        }
    }

}
