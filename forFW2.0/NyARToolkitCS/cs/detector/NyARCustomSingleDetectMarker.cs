﻿/* 
 * PROJECT: NyARToolkitCS
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
using System.Collections.Generic;
using System.Text;
using jp.nyatla.nyartoolkit.cs.core;

namespace jp.nyatla.nyartoolkit.cs.detector
{
/**
 * 画像からARCodeに最も一致するマーカーを1個検出し、その変換行列を計算するクラスです。
 * 変換行列を求めるには、detectMarkerLite関数にラスタイメージを入力して、計算対象の矩形を特定します。
 * detectMarkerLiteが成功すると、getTransmationMatrix等の関数が使用可能な状態になり、変換行列を求めることができます。
 * 
 * 
 */
    public class NyARCustomSingleDetectMarker
    {
        private const int AR_SQUARE_MAX = 100;

        private bool _is_continue = false;
        private NyARMatchPatt_Color_WITHOUT_PCA _match_patt;
        private INyARSquareDetector _square_detect;

        private NyARSquareStack _square_list = new NyARSquareStack(AR_SQUARE_MAX);

        protected INyARTransMat _transmat;

        private double _marker_width;
        // 検出結果の保存用
        private int _detected_direction;
        private double _detected_confidence;
        private NyARSquare _detected_square;
        private INyARColorPatt _patt;
        //画処理用
        private NyARBinRaster _bin_raster;
        protected INyARRasterFilter_RgbToBin _tobin_filter;

        private NyARMatchPattDeviationColorData _deviation_data;
        protected NyARCustomSingleDetectMarker()
        {
	        return;
        }
        protected void initInstance(
	        INyARColorPatt i_patt_inst,
	        INyARSquareDetector i_sqdetect_inst,
	        INyARRasterFilter_RgbToBin i_filter,
	        NyARParam	i_ref_param,
	        NyARCode	i_ref_code,
	        double		i_marker_width)
        {
            NyARIntSize scr_size=i_ref_param.getScreenSize();		
            // 解析オブジェクトを作る
            this._square_detect = i_sqdetect_inst;
            this._transmat = new NyARTransMat(i_ref_param);
            this._tobin_filter=i_filter;
            // 比較コードを保存
            this._marker_width = i_marker_width;
            //パターンピックアップを作成
            this._patt = i_patt_inst;
            //取得パターンの差分データ器を作成
            this._deviation_data=new NyARMatchPattDeviationColorData(i_ref_code.getWidth(),i_ref_code.getHeight());
            //i_code用の評価器を作成
            this._match_patt = new NyARMatchPatt_Color_WITHOUT_PCA(i_ref_code);
            //２値画像バッファを作る
            this._bin_raster=new NyARBinRaster(scr_size.w,scr_size.h);
            return;
        }

        private NyARMatchPattResult __detectMarkerLite_mr = new NyARMatchPattResult();

        /**
         * i_imageにマーカー検出処理を実行し、結果を記録します。
         * 
         * @param i_raster
         * マーカーを検出するイメージを指定します。イメージサイズは、カメラパラメータ
         * と一致していなければなりません。
         * @return マーカーが検出できたかを真偽値で返します。
         * @throws NyARException
         */
        public bool detectMarkerLite(INyARRgbRaster i_raster)
        {
            //サイズチェック
            if (!this._bin_raster.getSize().isEqualSize(i_raster.getSize()))
            {
                throw new NyARException();
            }

            //ラスタを２値イメージに変換する.
            this._tobin_filter.doFilter(i_raster, this._bin_raster);


            this._detected_square = null;
            NyARSquareStack l_square_list = this._square_list;
            // スクエアコードを探す
            this._square_detect.detectMarker(this._bin_raster, l_square_list);


            int number_of_square = l_square_list.getLength();
            // コードは見つかった？
            if (number_of_square < 1)
            {
                return false;
            }

            bool result = false;
            NyARMatchPattResult mr = this.__detectMarkerLite_mr;
            int square_index = 0;
            int direction = NyARSquare.DIRECTION_UNKNOWN;
            double confidence = 0;
            for (int i = 0; i < number_of_square; i++)
            {
                // 評価基準になるパターンをイメージから切り出す
                if (!this._patt.pickFromRaster(i_raster, (NyARSquare)l_square_list.getItem(i)))
                {
                    continue;
                }
                //取得パターンをカラー差分データに変換して評価する。
                this._deviation_data.setRaster(this._patt);
                if (!this._match_patt.evaluate(this._deviation_data, mr))
                {
                    continue;
                }
                double c2 = mr.confidence;
                if (confidence > c2)
                {
                    continue;
                }
                // もっと一致するマーカーがあったぽい
                square_index = i;
                direction = mr.direction;
                confidence = c2;
                result = true;
            }

            // マーカー情報を保存
            this._detected_square = (NyARSquare)l_square_list.getItem(square_index);
            this._detected_direction = direction;
            this._detected_confidence = confidence;
            return result;
        }

        /**
         * 検出したマーカーの変換行列を計算して、o_resultへ値を返します。
         * 直前に実行したdetectMarkerLiteが成功していないと使えません。
         * 
         * @param o_result
         * 変換行列を受け取るオブジェクトを指定します。
         * @throws NyARException
         */
        public void getTransmationMatrix(NyARTransMatResult o_result)
        {
            // 一番一致したマーカーの位置とかその辺を計算
            if (this._is_continue)
            {
                this._transmat.transMatContinue(this._detected_square, this._detected_direction, this._marker_width, o_result);
            }
            else
            {
                this._transmat.transMat(this._detected_square, this._detected_direction, this._marker_width, o_result);
            }
            return;
        }
        /**
         * 画面上のマーカ頂点情報を配列へ取得します。
         * @param o_point
         * 4要素以上の配列を指定して下さい。先頭の4要素に値がコピーされます。
         */
        public void getSquarePosition(NyARIntPoint2d[] o_point)
        {
            NyARIntPoint2d.copyArray(this._detected_square.imvertex, o_point);
            return;
        }
        /**
         * 画面上のマーカ頂点情報を配列へのリファレンスを返します。
         * 返されたオブジェクトはクラスに所有し続けられています。クラスのメンバ関数を実行すると内容が書き変わります。
         * 外部でデータをストックする場合は、getSquarePositionで複製して下さい。
         * @return
         */
        public NyARIntPoint2d[] refSquarePosition()
        {
            return this._detected_square.imvertex;
        }


        /**
         * 検出したマーカーの一致度を返します。
         * 
         * @return マーカーの一致度を返します。0～1までの値をとります。 一致度が低い場合には、誤認識の可能性が高くなります。
         * @throws NyARException
         */
        public double getConfidence()
        {
            return this._detected_confidence;
        }

        /**
         * 検出したマーカーの方位を返します。
         * 
         * @return 0,1,2,3の何れかを返します。
         */
        public int getDirection()
        {
            return this._detected_direction;
        }

        /**
         * getTransmationMatrixの計算モードを設定します。 初期値はTRUEです。
         * 
         * @param i_is_continue
         * TRUEなら、transMatCont互換の計算をします。 FALSEなら、transMat互換の計算をします。
         */
        public void setContinueMode(bool i_is_continue)
        {
            this._is_continue = i_is_continue;
        }
    }
}
