/* 
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
using jp.nyatla.nyartoolkit.cs.core;


namespace jp.nyatla.nyartoolkit.cs.detector
{

    /**
     * 画像からARCodeに最も一致するマーカーを1個検出し、その変換行列を計算するクラスです。
     * 
     */
    public class NyARSingleDetectMarker
    {
        private const int AR_SQUARE_MAX = 100;

        private bool _is_continue = false;
        private NyARMatchPatt_Color_WITHOUT_PCA _match_patt;
        private INyARSquareDetector _square_detect;

        private NyARSquareStack _square_list = new NyARSquareStack(AR_SQUARE_MAX);

        private NyARCode _code;

        protected INyARTransMat _transmat;

        private double _marker_width;

        // 検出結果の保存用
        private int _detected_direction;

        private double _detected_confidence;

        private NyARSquare _detected_square;

        private INyARColorPatt _patt;

        /**
         * 検出するARCodeとカメラパラメータから、1個のARCodeを検出するNyARSingleDetectMarkerインスタンスを作ります。
         * 
         * @param i_param
         * カメラパラメータを指定します。
         * @param i_code
         * 検出するARCodeを指定します。
         * @param i_marker_width
         * ARコードの物理サイズを、ミリメートルで指定します。
         * @throws NyARException
         */
        public NyARSingleDetectMarker(NyARParam i_param, NyARCode i_code, double i_marker_width)
        {
            NyARIntSize scr_size = i_param.getScreenSize();
            // 解析オブジェクトを作る
            this._square_detect = new NyARSquareDetector(i_param.getDistortionFactor(), scr_size);
            this._transmat = new NyARTransMat(i_param);
            // 比較コードを保存
            this._code = i_code;
            this._marker_width = i_marker_width;
            // 評価パターンのホルダを作る
            this._patt = new NyARColorPatt_O3(_code.getWidth(), _code.getHeight());
            // 評価器を作る。
            this._match_patt = new NyARMatchPatt_Color_WITHOUT_PCA();
            //２値画像バッファを作る
            this._bin_raster = new NyARBinRaster(scr_size.w, scr_size.h);
        }

        NyARBinRaster _bin_raster;
        NyARRasterFilter_ARToolkitThreshold _tobin_filter = new NyARRasterFilter_ARToolkitThreshold(100);

        /**
         * i_imageにマーカー検出処理を実行し、結果を記録します。
         * 
         * @param i_raster
         * マーカーを検出するイメージを指定します。イメージサイズは、カメラパラメータ
         * と一致していなければなりません。
         * @return マーカーが検出できたかを真偽値で返します。
         * @throws NyARException
         */
        public bool detectMarkerLite(INyARRgbRaster i_raster, int i_threshold)
        {
            //サイズチェック
            if (!this._bin_raster.getSize().isEqualSize(i_raster.getSize()))
            {
                throw new NyARException();
            }

            //ラスタを２値イメージに変換する.
            this._tobin_filter.setThreshold(i_threshold);
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

            // 評価基準になるパターンをイメージから切り出す
            if (!this._patt.pickFromRaster(i_raster, l_square_list.getItem(0)))
            {
                // パターンの切り出しに失敗
                return false;
            }
            // パターンを評価器にセット
            if (!this._match_patt.setPatt(this._patt))
            {
                // 計算に失敗した。
                throw new NyARException();
            }
            // コードと比較する
            this._match_patt.evaluate(this._code);
            int square_index = 0;
            int direction = this._match_patt.getDirection();
            double confidence = this._match_patt.getConfidence();
            for (int i = 1; i < number_of_square; i++)
            {
                // 次のパターンを取得
                this._patt.pickFromRaster(i_raster, l_square_list.getItem(i));
                // 評価器にセットする。
                this._match_patt.setPatt(this._patt);
                // コードと比較する
                this._match_patt.evaluate(this._code);
                double c2 = this._match_patt.getConfidence();
                if (confidence > c2)
                {
                    continue;
                }
                // もっと一致するマーカーがあったぽい
                square_index = i;
                direction = this._match_patt.getDirection();
                confidence = c2;
            }
            // マーカー情報を保存
            this._detected_square = l_square_list.getItem(square_index);
            this._detected_direction = direction;
            this._detected_confidence = confidence;
            return true;
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