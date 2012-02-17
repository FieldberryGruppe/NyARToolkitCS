/* 
 * PROJECT: NyARToolkitCS(Extension)
 * --------------------------------------------------------------------------------
 * The NyARToolkitCS is Java edition ARToolKit class library.
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
    /**
     * このインタフェイスは、行列クラスに共通な関数を定義します。
     */
    public interface INyARDoubleMatrix
    {
        /**
         * この関数は、配列の内容を行列にセットします。
         * 実装クラスでは、配列の内容をインスタンスにセットする処理を実装してください。
         * @param i_value
         * セットする配列。
         */
        void setValue(double[] i_value);
        /**
         * この関数は、配列の内容を行列に返します。
         * 実装クラスでは、インスタンスの内容を配列に返す処理を実装してください。
         * @param o_value
         * 値を受け取る配列
         */
        void getValue(double[] o_value);

    }
}
