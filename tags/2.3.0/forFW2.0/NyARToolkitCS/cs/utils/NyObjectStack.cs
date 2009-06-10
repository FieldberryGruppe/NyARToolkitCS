/* 
 * PROJECT: NyARToolkitCS
 * --------------------------------------------------------------------------------
 * The NyARToolkit is Java version ARToolkit class library.
 * Copyright (C)2008 R.Iizuka
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this framework; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 * For further information please contact.
 *	http://nyatla.jp/nyatoolkit/
 *	<airmail(at)ebony.plala.or.jp>
 * 
 */
namespace jp.nyatla.nyartoolkit.cs.utils
{
    /**
     * オンデマンド割り当てをするオブジェクト配列。
     * 配列には実体を格納します。
     */
    public abstract class NyObjectStack
    {
        private const int ARRAY_APPEND_STEP = 64;

        protected object[] _items;

        private int _allocated_size;

        protected int _length;

        /**
         * 最大ARRAY_MAX個の動的割り当てバッファを準備する。
         * 
         * @param i_array
         */
        public NyObjectStack(object[] i_array)
        {
            // ポインタだけははじめに確保しておく
            this._items = i_array;
            // アロケート済サイズと、使用中個数をリセット
            this._allocated_size = 0;
            this._length = 0;
        }

        /**
         * ポインタを1進めて、その要素を予約し、その要素へのポインタを返します。
         * 特定型に依存させるときには、継承したクラスでこの関数をオーバーライドしてください。
         */
        virtual public object prePush()
        {
            // 必要に応じてアロケート
            if (this._length >= this._allocated_size)
            {
                // 要求されたインデクスは範囲外
                if (this._length >= this._items.Length)
                {
                    throw new NyARException();
                }
                // 追加アロケート範囲を計算
                int range = this._length + ARRAY_APPEND_STEP;
                if (range >= this._items.Length)
                {
                    range = this._items.Length;
                }
                // アロケート
                this.onReservRequest(this._allocated_size, range, this._items);
                this._allocated_size = range;
            }
            // 使用領域を+1して、予約した領域を返す。
            object ret = this._items[this._length];
            this._length++;
            return ret;
        }
        /**
         * 見かけ上の要素数を1減らして、最後尾のアイテムを返します。
         * @return
         */
        virtual public object pop()
        {
            if (this._length < 1)
            {
                throw new NyARException();
            }
            this._length--;
            return this.getItem(this._length);
        }
        /**
         * 0～i_number_of_item-1までの領域を予約します。
         * 予約すると、見かけ上の要素数は0にリセットされます。
         * @param i_number_of_reserv
         */
        virtual public void reserv(int i_number_of_item)
        {
            // 必要に応じてアロケート
            if (i_number_of_item >= this._allocated_size)
            {
                // 要求されたインデクスは範囲外
                if (i_number_of_item >= this._items.Length)
                {
                    throw new NyARException();
                }
                // 追加アロケート範囲を計算
                int range = i_number_of_item + ARRAY_APPEND_STEP;
                if (range >= this._items.Length)
                {
                    range = this._items.Length;
                }
                // アロケート
                this.onReservRequest(this._allocated_size, range, this._items);
                this._allocated_size = range;
            }
            //見かけ上の配列サイズを指定
            this._length = i_number_of_item;
            return;
        }

        /**
         * 配列を返します。
         * 
         * @return
         */
        virtual protected object[] getArray()
        {
            return this._items;
        }
        virtual protected object getItem(int i_index)
        {
            return this._items[i_index];
        }

        /**
         * この関数を継承先クラスで実装して下さい。
         * i_bufferの配列の、i_start番目からi_end-1番目までの要素に、オブジェクトを割り当てて下さい。
         * 
         * @param i_start
         * @param i_end
         * @param i_buffer
         */
        protected abstract void onReservRequest(int i_start, int i_end, object[] i_buffer);

        /**
         * 配列の見かけ上の要素数を返却します。
         * @return
         */
        public int getLength()
        {
            return this._length;
        }

        /**
         * 見かけ上の要素数をリセットします。
         */
        public void clear()
        {
            this._length = 0;
        }
    }
}