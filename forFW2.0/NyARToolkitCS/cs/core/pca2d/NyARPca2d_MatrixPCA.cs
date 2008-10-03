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

namespace jp.nyatla.nyartoolkit.cs.core
{

    /**
     * NyARMatrixを利用した主成分分析
     * ARToolKitと同じ処理をします。
     */
    public class NyARPca2d_MatrixPCA : INyARPca2d
    {
        private NyARMat __pca_input = new NyARMat(1, 2);
        private NyARMat __pca_evec = new NyARMat(2, 2);
        private NyARVec __pca_ev = new NyARVec(2);
        private NyARVec __pca_mean = new NyARVec(2);

        public void pca(double[] i_x, double[] i_y, int i_start, int i_number_of_point, NyARDoubleMatrix22 o_evec, NyARDoublePoint2d o_ev, NyARDoublePoint2d o_mean)
        {
            NyARMat input = this.__pca_input;// 次処理で初期化される。
            double[][] input_array = input.getArray();
            // pcaの準備
            input.realloc(i_number_of_point, 2);
            Array.Copy(i_x, 0, input_array[0], i_start, i_number_of_point);
            Array.Copy(i_y, 0, input_array[1], i_start, i_number_of_point);
            // 主成分分析
            input.matrixPCA(this.__pca_evec, this.__pca_ev, this.__pca_mean);
            double[] mean_array = this.__pca_mean.getArray();
            double[][] evec_array = this.__pca_evec.getArray();
            double[] ev_array = this.__pca_ev.getArray();
            o_evec.m00 = evec_array[0][0];
            o_evec.m01 = evec_array[0][1];
            o_evec.m10 = evec_array[1][0];
            o_evec.m11 = evec_array[1][1];
            o_ev.x = ev_array[0];
            o_ev.x = ev_array[1];
            o_mean.x = mean_array[0];
            o_mean.y = mean_array[1];
            return;

        }

        public void pcaWithDistortionFactor(int[] i_x, int[] i_y, int i_start, int i_number_of_point, NyARCameraDistortionFactor i_factor, NyARDoubleMatrix22 o_evec, NyARDoublePoint2d o_ev, NyARDoublePoint2d o_mean)
        {
            NyARMat input = this.__pca_input;// 次処理で初期化される。
            double[][] input_array = input.getArray();
            // pcaの準備
            input.realloc(i_number_of_point, 2);
            i_factor.observ2IdealBatch(i_x, i_y, i_start, i_number_of_point, input_array[0], input_array[1]);
            // 主成分分析
            input.matrixPCA(this.__pca_evec, this.__pca_ev, this.__pca_mean);
            double[] mean_array = this.__pca_mean.getArray();
            double[][] evec_array = this.__pca_evec.getArray();
            double[] ev_array = this.__pca_ev.getArray();
            o_evec.m00 = evec_array[0][0];
            o_evec.m01 = evec_array[0][1];
            o_evec.m10 = evec_array[1][0];
            o_evec.m11 = evec_array[1][1];
            o_ev.x = ev_array[0];
            o_ev.x = ev_array[1];
            o_mean.x = mean_array[0];
            o_mean.y = mean_array[1];
            return;
        }
    }
}
