﻿/* 
 * PROJECT: NyARToolkit
 * --------------------------------------------------------------------------------
 * This work is based on the original ARToolKit developed by
 *   Hirokazu Kato
 *   Mark Billinghurst
 *   HITLab, University of Washington, Seattle
 * http://www.hitl.washington.edu/artoolkit/
 *
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
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using jp.nyatla.nyartoolkit.cs.core;
/**
 * 単純Sinテーブル
 *
 */
namespace jp.nyatla.nyartoolkit.cs.sandbox.x2
{
    public class NyARSinTable
    {
        private double[] _table = null;
        private int _resolution;

        private void initTable(int i_resolution)
        {
            //解像度は4の倍数で無いとダメ
            Debug.Assert(i_resolution % 4 == 0);
            if (this._table == null)
            {
                this._table = new double[i_resolution];
                int d4 = i_resolution / 4;
                // テーブル初期化(0-2PIを0-1024に作成)
                for (int i = 1; i < i_resolution; i++)
                {
                    this._table[i] = (Math.Sin(2 * Math.PI * (double)i / (double)i_resolution));
                }
                this._table[0] = 0;
                this._table[d4 - 1] = 1;
                this._table[d4 * 2 - 1] = 0;
                this._table[d4 * 3 - 1] = -1;
            }
            return;
        }

        public NyARSinTable(int i_resolution)
        {
            initTable(i_resolution);
            this._resolution = i_resolution;
            return;
        }

        public double sin(double i_rad)
        {
            int resolution = this._resolution;
            // 0～2PIを0～1024に変換
            int rad_index = (int)(i_rad * resolution / (2 * Math.PI));
            rad_index = rad_index % resolution;
            if (rad_index < 0)
            {
                rad_index += resolution;
            }
            // ここで0-1024にいる
            return this._table[rad_index];
        }

        public double cos(double i_rad)
        {
            int resolution = this._resolution;
            // 0～Math.PI/2を 0～256の値空間に変換
            int rad_index = (int)(i_rad * resolution / (2 * Math.PI));
            // 90度ずらす
            rad_index = (rad_index + resolution / 4) % resolution;
            // 負の領域に居たら、+1024しておく
            if (rad_index < 0)
            {
                rad_index += resolution;
            }
            // ここで0-1024にいる
            return this._table[rad_index];
        }
        /**
         * ラジアン角度をテーブルの角度インデックス番号に変換する。
         * 角度インデックスは、0<=n<_resolutionの範囲の整数
         * @param i_rad
         * @return
         */
        public int rad2tableIndex(double i_rad)
        {
            int resolution = this._resolution;
            int rad_index = (int)(i_rad * resolution / (2 * Math.PI));
            rad_index = rad_index % resolution;
            if (rad_index < 0)
            {
                rad_index += resolution;
            }
            return rad_index;
        }
        public double sinByIdx(int i_rad_idx)
        {
            return 0;
        }
        public double cosByIdx(int i_rad_idx)
        {
            return 0;
        }

    }
}