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
namespace jp.nyatla.nyartoolkit.cs.core
{


    public class NyARTransOffset
    {
        public NyARDoublePoint3d[] vertex = NyARDoublePoint3d.createArray(4);
        public NyARDoublePoint3d point = new NyARDoublePoint3d();
        /**
         * 中心位置と辺長から、オフセット情報を作成して設定する。
         * @param i_width
         * @param i_center
         */
        public void setSquare(double i_width, NyARDoublePoint2d i_center)
        {
            double w_2 = i_width / 2.0;

            NyARDoublePoint3d vertex3d_ptr;
            vertex3d_ptr = this.vertex[0];
            vertex3d_ptr.x = -w_2;
            vertex3d_ptr.y = w_2;
            vertex3d_ptr.z = 0.0;
            vertex3d_ptr = this.vertex[1];
            vertex3d_ptr.x = w_2;
            vertex3d_ptr.y = w_2;
            vertex3d_ptr.z = 0.0;
            vertex3d_ptr = this.vertex[2];
            vertex3d_ptr.x = w_2;
            vertex3d_ptr.y = -w_2;
            vertex3d_ptr.z = 0.0;
            vertex3d_ptr = this.vertex[3];
            vertex3d_ptr.x = -w_2;
            vertex3d_ptr.y = -w_2;
            vertex3d_ptr.z = 0.0;

            this.point.x = -i_center.x;
            this.point.y = -i_center.y;
            this.point.z = 0;
            return;

        }
    }
}