﻿/* 
 * PROJECT: NyARToolkitCS
 * --------------------------------------------------------------------------------
 * This work is based on the original ARToolKit developed by
 *   Hirokazu Kato
 *   Mark Billinghurst
 *   HITLab, University of Washington, Seattle
 * http://www.hitl.washington.edu/artoolkit/
 *
 * The NyARToolkitCS is C# version NyARToolkit class library.
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
using System;
using System.Collections.Generic;
using jp.nyatla.nyartoolkit.cs;
using jp.nyatla.nyartoolkit.cs.raster;

namespace jp.nyatla.nyartoolkit.cs.core
{
public interface NyARColorPatt
{
    //	消すかも。
    //    /**
    //     * カラーパターンのサイズを変更します。
    //     * 変更を行うと、既にgetPatArrayで参照中の配列内容は不定になり、インスタンスのパラメータは初期状態に戻ります。
    //     * @param i_new_width
    //     * 新しいパターン幅
    //     * @param i_new_height
    //     * 新しいパターン高
    //     */
    //    public void setSize(int i_new_width,int i_new_height);
        /**
         * カラーパターンの幅をピクセル値で返します。
         * @return
         */
        int getWidth();
        /**
         * カラーパターンの高さをピクセル値で返します。
         * @return
         */
        int getHeight();
        /**
         * カメラパターンを格納した配列への参照値を返します。
         * 配列は最低でも[height][wight][3]のサイズを持ちますが、
         * 配列のlengthとwidth,heightの数は一致しないことがあります。
         * setSize関数を実行すると、以前に呼び出されたgetPatArrayが返した値は不定になります。
         * @return
         */
        int[,,] getPatArray();
        /**
         * ラスタイメージからi_marker部分のカラーパターンを抽出して、保持します。
         * @param image
         * @param i_marker
         * @return
         * ラスターの取得に成功するとTRUE/失敗するとFALSE
         * @throws NyARException
         */
        bool pickFromRaster(NyARRaster image, NyARMarker i_marker);
    }
}
