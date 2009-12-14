/* 
 * PROJECT: NyARToolkitCS
 * --------------------------------------------------------------------------------
 * This work is based on the original ARToolKit developed by
 *   Hirokazu Kato
 *   Mark Billinghurst
 *   HITLab, University of Washington, Seattle
 * http://www.hitl.washington.edu/artoolkit/
 *
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
using jp.nyatla.nyartoolkit.cs.utils;

namespace jp.nyatla.nyartoolkit.cs.core
{
    public class NyARGrayscaleRaster : NyARRaster_BasicClass
    {
        protected int[] _ref_buf;
        private INyARBufferReader _buffer_reader;

        public NyARGrayscaleRaster(int i_width, int i_height)
            : base(new NyARIntSize(i_width, i_height))
        {
            this._ref_buf = new int[i_height * i_width];
            this._buffer_reader = new NyARBufferReader(this._ref_buf, INyARBufferReader.BUFFERFORMAT_INT1D_GRAY_8);
        }
        public override INyARBufferReader getBufferReader()
        {
            return this._buffer_reader;
        }
        /**
         * 4�ߖT�̉�f�x�N�g�����擾���܂��B
         * 0,1,0
         * 1,x,1
         * 0,1,0
         * @param i_raster
         * @param x
         * @param y
         * @param o_v
         */
        public void getPixelVector4(int x, int y, NyARIntPoint2d o_v)
        {
            int[] buf = this._ref_buf;
            int w = this._size.w;
            int idx = w * y + x;
            o_v.x = buf[idx + 1] - buf[idx - 1];
            o_v.y = buf[idx + w] - buf[idx - w];
        }
        /**
         * 8�ߖT��f�x�N�g��
         * 1,2,1
         * 2,x,2
         * 1,2,1
         * @param i_raster
         * @param x
         * @param y
         * @param o_v
         */
        public void getPixelVector8(int x, int y, NyARIntPoint2d o_v)
        {
            int[] buf = this._ref_buf;
            NyARIntSize s = this._size;
            int idx_0 = s.w * y + x;
            int idx_p1 = idx_0 + s.w;
            int idx_m1 = idx_0 - s.w;
            int b = buf[idx_m1 - 1];
            int d = buf[idx_m1 + 1];
            int h = buf[idx_p1 - 1];
            int f = buf[idx_p1 + 1];
            o_v.x = buf[idx_0 + 1] - buf[idx_0 - 1] + (d - b + f - h) / 2;
            o_v.y = buf[idx_p1] - buf[idx_m1] + (f - d + h - b) / 2;
        }

        public void copyFrom(NyARGrayscaleRaster i_input)
        {
            int[] out_buf = (int[])this._ref_buf;
            int[] in_buf = (int[])i_input._ref_buf;
            System.Array.Copy(in_buf, 0, out_buf, 0, this._size.h * this._size.w);
            return;
        }


    }
}